using System.Collections.Generic;

namespace OctoPatch.Descriptions
{
    /// <summary>
    /// Description for a single adapter
    /// </summary>
    public sealed class AdapterDescription : CommonDescription
    {
        /// <summary>
        /// Gets a list of supported input/output pairs
        /// </summary>
        public List<(string input, string output)> SupportedTypePairs { get; }

        public AdapterDescription(string name, string description = null) 
            : base(name, description)
        {
        }
    }
}
