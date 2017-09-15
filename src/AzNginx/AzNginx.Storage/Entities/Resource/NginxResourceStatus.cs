using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzNginx.Storage.Entities.Resource
{
    /// <summary>
    /// A class represents the status of NginxResource.
    /// 
    /// int is the best option for type if we want to use it in LINQ.
    /// 
    /// We cannot use Enum type here because Enum type cannot be (de)serialized by default. That means it will be always empty
    /// in Azure table. We have another option by using ConvertableEntityPropertyAttribute which will help us (de)serialize Enum type correctly,
    /// however there will another critical issue: it cannot work well with Linq. That's because the underlying data type will be
    /// different so LINQ fail to evaluate the Expression.
    /// </summary>
    public class NginxResourceStatus
    {
        public static readonly int Active = 0;
        public static readonly int Creating = 1;
        public static readonly int CreateFailed = 2;
        public static readonly int Enabling = 3;
        public static readonly int Enabled = 4;
        public static readonly int EnableFailed = 5;
        public static readonly int Disabling = 6;
        public static readonly int Disabled = 7;
        public static readonly int DisableFailed = 8;
        public static readonly int Deleting = 9;
        public static readonly int Deleted = 10;
        public static readonly int DeleteFailed = 11;
        public static readonly int Moving = 12;
        public static readonly int Moved = 13;
    }
}
