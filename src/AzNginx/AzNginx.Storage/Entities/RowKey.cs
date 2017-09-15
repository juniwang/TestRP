using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzNginx.Storage.Entities
{
    public class RowKey
    {
        public string Key { get; set; }
        public Guid Guid { get; set; }
    }

    public static class Entities
    {
        public static RowKey GetNewRowKey()
        {
            Guid guid = Guid.NewGuid();
            return new RowKey
            {
                Key = string.Format("{0}-{1}", DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks, guid),
                Guid = guid
            };
        }
    }
}
