using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OctoPatch.Descriptions
{
    /// <summary>
    /// Description for a single adapter
    /// </summary>
    [DataContract]
    public sealed class AdapterDescription : KeyDescription
    {
        /// <summary>
        /// Gets a list of supported input/output combinations
        /// </summary>
        [DataMember]
        public List<(string input, string output)> SupportedTypeCombinations { get; }

        public AdapterDescription(string key, string displayName, string displayDescription) 
            : base(key, displayName, displayDescription)
        {
            SupportedTypeCombinations = new List<(string input, string output)>();
        }
    }
}
