namespace OctoPatch.Midi
{
    /// <summary>
    /// Single message from a midi device
    /// </summary>
    public struct MidiMessage
    {
        public int MessageType { get; set; }

        public int Channel { get; set; }

        public int Key { get; set; }

        public int Value { get; set; }
    }
}
