using System;

namespace OctoPatch
{
    /// <summary>
    /// Represents a single message
    /// </summary>
    public readonly struct Message : IEquatable<Message>
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

        public override string ToString()
        {
            return $"{Content}";
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

        public bool Equals(Message other)
        {
            return Equals(Type, other.Type) && 
                   Equals(Content, other.Content);
        }

        public override bool Equals(object obj)
        {
            return obj is Message other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Type != null ? Type.GetHashCode() : 0) * 397) ^ (Content != null ? Content.GetHashCode() : 0);
            }
        }
    }
}
