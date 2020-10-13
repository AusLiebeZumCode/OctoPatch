namespace OctoPatch.Plugin.Keyboard
{
    public sealed class KeyboardStringConfiguration : IConfiguration
    {
        /// <summary>
        /// Ignores not printable character like whitespace and newline
        /// </summary>
        public bool IgnoreNotPrintable { get; set; }
    }
}
