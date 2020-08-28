using OctoPatch.ContentTypes;

namespace OctoPatch.Descriptions
{
    /// <summary>
    /// Description for a single property of a custom message
    /// </summary>
    public sealed class PropertyDescription : Description
    {
        /// <summary>
        /// Gets or sets the key for this property. Must be unique within the property type
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the type information
        /// </summary>
        public ContentType ContentType { get; set; }

        public PropertyDescription()
        {

        }

        public PropertyDescription(string displayName, string displayDescription, ContentType contentType) 
            : base(displayName, displayDescription)
        {
            ContentType = contentType;
        }
    }
}
