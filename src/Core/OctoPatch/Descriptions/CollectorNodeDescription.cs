using System.Runtime.Serialization;

namespace OctoPatch.Descriptions
{
    /// <summary>
    /// Node to combine different inputs to get a complex type message
    /// </summary>
    [DataContract]
    public sealed class CollectorNodeDescription : NodeDescription
    {
        /// <summary>
        /// Type to collector inputs together for
        /// </summary>
        [DataMember]
        public string TypeKey { get; set; }

        private CollectorNodeDescription(string key, string typeKey, string displayName, string displayDescription) 
            : base(key, displayName, displayDescription)
        {
            TypeKey = typeKey;
        }

        /// <summary>
        /// Creates a collector description for the given type
        /// </summary>
        /// <param name="typeDescription">type description</param>
        /// <returns>collector description</returns>
        public static CollectorNodeDescription CreateFromComplexType(TypeDescription typeDescription)
        {
            var result = new CollectorNodeDescription($"{typeDescription.Key}:Collector", typeDescription.Key, $"{typeDescription.DisplayName} Collector", null);

            // Add all type properties as inputs
            foreach (var propertyDescription in typeDescription.PropertyDescriptions)
            {
                result.AddInputDescription(new ConnectorDescription(
                    propertyDescription.Key, propertyDescription.DisplayName,
                    propertyDescription.DisplayDescription, propertyDescription.ContentType));
            }

            return result;
        }
    }
}
