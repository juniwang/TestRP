using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TestRP.Common
{
    public class DataStore
    {
        private static DataStore store = new DataStore();
        private string fileName = "demorp.json";

        private DataStore() { }

        public static DataStore Instance
        {
            get
            {
                return store;
            }
        }

        public void Save(NData data)
        {
            if (data == null) return;

            using (StreamWriter writer = new StreamWriter(FullFileName, false))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(writer, data);
            }
        }

        public NData Load()
        {
            if (!File.Exists(FullFileName))
                return new NData();

            using (StreamReader reader = new StreamReader(FullFileName))
            {
                JsonSerializer serializer = new JsonSerializer();
                return (NData)serializer.Deserialize(reader, typeof(NData));
            }
        }

        public void Clean()
        {
            if (File.Exists(FullFileName))
                File.Delete(FullFileName);
        }

        string FullFileName
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory + Path.AltDirectorySeparatorChar + fileName;
            }
        }
    }

    public class NData
    {
        public List<NDeployment> Deployments { get; set; }
        public List<NResource> Resources { get; set; }

        public NData()
        {
            Deployments = new List<NDeployment>();
            Resources = new List<NResource>();
        }

        public void SaveChanges()
        {
            DataStore.Instance.Save(this);
        }

        public static NData TryLoad()
        {
            try
            {
                return DataStore.Instance.Load();
            }
            catch (Exception)
            {
                DataStore.Instance.Clean();
                return new NData();
            }
        }
    }

    public class NDeployment
    {
        public string SubscriptionId { get; set; }
        public string DeploymentId { get; set; }
        public string DeploymentLable { get; set; }
        public DateTime CreateTime { get; set; }
    }

    public class NResource
    {
        public NResource()
        {
            Tags = new Dictionary<string, string>();
            properties = new NResourceProperties();
        }
        public string Id
        {
            get
            {
                return string.Format("/subscriptions/{0}/resourceGroups/{1}/providers/Microsoft.Nginx/Nginx/{2}"
                    , SubscriptionId, ResourceGroup, Name);
            }
        }

        public string SubscriptionId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string ResourceGroup { get; set; }
        public string Location { get; set; }
        public bool Enabled { get; set; }
        public string DeploymentId { get; set; }
        public IDictionary<string, string> Tags { get; set; }
        public NResourceProperties properties { get; set; }
    }

    public class NResourceProperties
    {
        public string NginxVersion { get; set; }
    }
}
