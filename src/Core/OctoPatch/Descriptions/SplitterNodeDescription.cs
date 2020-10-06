using System.Runtime.Serialization;

namespace OctoPatch.Descriptions
{
    /// <summary>
    /// Node to split up complex types into its properties
    /// </summary>
    [DataContract]
    public sealed class SplitterNodeDescription : NodeDescription
    {
        /// <summary>
        /// Gets the type key to split
        /// </summary>
        [DataMember]
        public string TypeKey { get; set; }

        private SplitterNodeDescription(string key, string typeKey, string displayName, string displayDescription) 
            : base(key, displayName, displayDescription)
        {
            TypeKey = typeKey;
        }

        /// <summary>
        /// Creates a splitter description for the given type
        /// </summary>
        /// <param name="typeDescription">type description</param>
        /// <returns>splitter description</returns>
        public static SplitterNodeDescription CreateFromComplexType(TypeDescription typeDescription)
        {
            var result = new SplitterNodeDescription($"{typeDescription.Key}:Splitter", typeDescription.Key, $"{typeDescription.DisplayName} Splitter", null);
            
            // Add all type properties as outputs
            foreach (var propertyDescription in typeDescription.PropertyDescriptions)
            {
                result.AddOutputDescription(new ConnectorDescription(
                    propertyDescription.Key, propertyDescription.DisplayName,
                    propertyDescription.DisplayDescription, propertyDescription.ContentType));
            }

            return result;
        }
    }
}
