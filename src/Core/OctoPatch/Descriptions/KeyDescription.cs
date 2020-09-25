using System.Runtime.Serialization;

namespace OctoPatch.Descriptions
{
    /// <summary>
    /// Description base if a key is included
    /// </summary>
    [DataContract]
    public abstract class KeyDescription : Description
    {
        /// <summary>
        /// Gets or sets the key for this item
        /// </summary>
        [DataMember]
        public string Key { get; set; }

        protected KeyDescription(string key, string displayName, string displayDescription) 
            : base(displayName, displayDescription)
        {
            Key = key;
        }
    }
}
