namespace OctoPatch.Plugin.Midi
{
    /// <summary>
    /// Common configuration for all kind of attached notes
    /// </summary>
    public sealed class AttachedNodeConfiguration : IConfiguration
    {
        /// <summary>
        /// Selected channel
        /// </summary>
        public int Channel { get; set; }

        /// <summary>
        /// Selected key
        /// </summary>
        public int Key { get; set; }
    }
}
