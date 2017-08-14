using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestRP.Common.Models
{
    public class ActionMetadataListResponse
    {
        public ActionMetadata[] value { get; set; }
    }

    /// <summary>
    /// None defined yet.
    /// </summary>
    public class ActionMetadataProperties
    {
    }


    /// <summary>
    /// All the properties of this object are localizable text to display to humans
    /// </summary>
    public class ActionMetadataDisplay
    {
        public string provider { get; set; }
        public string resource { get; set; }
        public string operation { get; set; }
        public string description { get; set; }
    }


    /// <summary>
    /// An 'action' in the CSM/Sparta 'Exposing Available Actions' metadata API.
    /// See RedisActionMetadataController
    /// </summary>
    public class ActionMetadata
    {
        /// <summary>
        /// Relative 'URL' of the action - with Read/Write/Delete suffix
        /// {resourceProviderNamespace}/{resourceType}/{read|write|delete|action}
        /// </summary>
        public string name { get; set; }
        public ActionMetadataDisplay display { get; set; }
        public ActionMetadataProperties properties { get; set; }
    }
}
