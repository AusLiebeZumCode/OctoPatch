using System;
using System.Collections.Generic;

namespace OctoPatch.Descriptions
{
    /// <summary>
    /// Description for a single adapter
    /// </summary>
    public sealed class AdapterDescription : KeyDescription
    {
        /// <summary>
        /// Gets a list of supported input/output combinations
        /// </summary>
        public List<(string input, string output)> SupportedTypeCombinations { get; }

        public AdapterDescription(string key, string displayName, string displayDescription) 
            : base(key, displayName, displayDescription)
        {
            SupportedTypeCombinations = new List<(string input, string output)>();
        }
    }
}
