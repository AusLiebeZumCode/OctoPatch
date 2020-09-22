using System;
using OctoPatch.ContentTypes;
using OctoPatch.Descriptions;

namespace OctoPatch
{
    /// <summary>
    /// Base class for all kind of connectors
    /// </summary>
    public abstract class Connector : IConnector
    {
        /// <summary>
        /// Returns the supported type
        /// </summary>
        protected Type SupportedType { get; }

        /// <inheritdoc />
        public Guid NodeId { get; }

        /// <inheritdoc />
        public string Key { get; }

        /// <inheritdoc />
        public ConnectorDescription Description { get; }

        protected Connector(Guid nodeId, Type supportedType, ConnectorDescription description)
        {
            if (description is null)
            {
                throw new ArgumentNullException(nameof(description));
            }

            // Compare supported type with description type
            switch (description.ContentType)
            {
                case AllContentType _:
                    break;
                case BinaryContentType _:
                    if (supportedType != typeof(byte[])) throw new ArgumentException("types do not match", nameof(description));
                    break;
                case BoolContentType _:
                    if (supportedType != typeof(bool)) throw new ArgumentException("types do not match", nameof(description));
                    break;
                case ComplexContentType _:
                    break;
                case EmptyContentType _:
                    if (supportedType != typeof(void)) throw new ArgumentException("types do not match", nameof(description));
                    break;
                case EnumContentType _:
                    break;
                case FloatContentType _:
                    if (supportedType != typeof(float)) throw new ArgumentException("types do not match", nameof(description));
                    break;
                case IntegerContentType _:
                    if (supportedType != typeof(int)) throw new ArgumentException("types do not match", nameof(description));
                    break;
                case StringContentType _:
                    if (supportedType != typeof(string)) throw new ArgumentException("types do not match", nameof(description));
                    break;
            }

            SupportedType = supportedType;

            NodeId = nodeId;
            Key = description.Key;
            Description = description;
        }
    }
}
