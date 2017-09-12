using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestRP.Common;

namespace AzNginx.DAL
{
    public class NginxDbContext : NginxDBEntities
    {
        private NginxDbContext()
            : base(TestRPConfiguration.NginxRPDbConnectionString)
        {

        }

        // better for unit tests
        public static NginxDbContext Create()
        {
            return new NginxDbContext();
        }
    }

    public partial class NginxDBEntities
    {
        public NginxDBEntities(string connectionString)
            : base(connectionString)
        {

        }
    }
}
