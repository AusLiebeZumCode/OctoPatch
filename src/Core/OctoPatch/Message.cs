using System;

namespace OctoPatch
{
    /// <summary>
    /// Represents a single message
    /// </summary>
    public readonly struct Message
    {
        /// <summary>
        /// Gets or sets the type of the serialized message
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Gets or sets the content
        /// </summary>
        public ValueType Content { get; }

        public Message(Type type, ValueType content)
        {
            Type = type;
            Content = content;
        }

        /// <summary>
        /// Creates a new message based on the content
        /// </summary>
        /// <typeparam name="T">message content type</typeparam>
        /// <param name="value">content</param>
        /// <returns>new message</returns>
        public static Message Create<T>(T value) where T : struct
        {
            return new Message(typeof(T), value);
        }

        /// <summary>
        /// Creates an empty message
        /// </summary>
        /// <returns>new message</returns>
        public static Message Create()
        {
            return new Message(typeof(void), default);
        }
    }
}
