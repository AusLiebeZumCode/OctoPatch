using System.Runtime.Serialization;

namespace OctoPatch.Descriptions
{
    /// <summary>
    /// Basic description class for all kind of descriptions
    /// </summary>
    [DataContract]
    public abstract class Description
    {
        /// <summary>
        /// Gets or sets the (localized) display name
        /// </summary>
        [DataMember]
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the (localized) description
        /// </summary>
        [DataMember]
        public string DisplayDescription { get; set; }

        protected Description(string displayName, string displayDescription)
        {
            DisplayName = displayName;
            DisplayDescription = displayDescription;
        }
    }
}
