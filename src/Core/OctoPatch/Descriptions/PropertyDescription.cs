﻿using System.Runtime.Serialization;
using OctoPatch.ContentTypes;

namespace OctoPatch.Descriptions
{
    /// <summary>
    /// Description for a single property of a custom message
    /// </summary>
    [DataContract]
    public sealed class PropertyDescription : KeyDescription
    {
        /// <summary>
        /// Gets or sets the type information
        /// </summary>
        [DataMember]
        public ContentType ContentType { get; set; }

        public PropertyDescription(string key, string displayName, string displayDescription, ContentType contentType) 
            : base(key, displayName, displayDescription)
        {
            ContentType = contentType;
        }
    }
}
