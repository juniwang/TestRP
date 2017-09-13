using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AzNginx.Common;

namespace AzNginx.DAL
{
    public class NginxDbContext : NginxDBEntities
    {
        private NginxDbContext()
            : base(AzNginxConfiguration.AzNginxDBConnectionString)
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
