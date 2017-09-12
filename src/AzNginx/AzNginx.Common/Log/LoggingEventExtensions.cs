using log4net.Appender;
using log4net.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzNginx.Common
{
    public static class LoggingEventExtensions
    {
        public static string MakeRowKey(this LoggingEvent loggingEvent)
        {
            return string.Format(
                "{0:D19}.{1}",
                 DateTime.MaxValue.Ticks - loggingEvent.TimeStamp.Ticks,
                Guid.NewGuid().ToString().ToLower());
        }

        public static string MakePartitionKey(this LoggingEvent loggingEvent, PartitionKeyTypeEnum partitionKeyType)
        {
            switch (partitionKeyType)
            {
                case PartitionKeyTypeEnum.LoggerName:
                    return loggingEvent.LoggerName;
                case PartitionKeyTypeEnum.DateReverse:
                    // substract from DateMaxValue the Tick Count of the current hour
                    // so a Table Storage Partition spans an hour
                    return string.Format("{0:D19}",
                        (DateTime.MaxValue.Ticks -
                         loggingEvent.TimeStamp.Date.AddHours(loggingEvent.TimeStamp.Hour).Ticks + 1));
                default:
                    // ReSharper disable once NotResolvedInText
                    throw new ArgumentOutOfRangeException("PartitionKeyType", partitionKeyType, null);
            }
        }

        public static IEnumerable<IEnumerable<TSource>> Batch<TSource>(this IEnumerable<TSource> source, int size)
        {
            TSource[] bucket = null;
            var count = 0;

            foreach (var item in source)
            {
                if (bucket == null)
                    bucket = new TSource[size];

                bucket[count++] = item;
                if (count != size)
                    continue;

                yield return bucket;

                bucket = null;
                count = 0;
            }

            if (bucket != null && count > 0)
                yield return bucket.Take(count);
        }
    }
}
