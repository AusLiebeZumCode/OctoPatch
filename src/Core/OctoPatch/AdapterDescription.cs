using System.Collections.Generic;

namespace OctoPatch
{
    /// <summary>
    /// Description for a single adapter
    /// </summary>
    public sealed class AdapterDescription
    {
        public string Name { get; set; }

        public string Description { get; set; }

        /// <summary>
        /// Gets a list of supported input/output pairs
        /// </summary>
        public List<(string input, string output)> SupportedTypePairs { get; }
    }
}
