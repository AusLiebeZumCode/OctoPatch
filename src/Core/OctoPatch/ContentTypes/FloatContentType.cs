using System;
using System.Runtime.Serialization;

namespace OctoPatch.ContentTypes
{
    /// <summary>
    /// Represents a integer based message
    /// </summary>
    [DataContract]
    public sealed class FloatContentType : ContentType
    {
        /// <summary>
        /// Optional lowest value for this message
        /// </summary>
        [DataMember]
        public float? MinimumValue { get; set; }

        /// <summary>
        /// Optional highest value for this message
        /// </summary>
        [DataMember]
        public float? MaximumValue { get; set; }

        /// <inheritdoc />
        public override Type SupportedType => typeof(float);

        /// <inheritdoc />
        public override ValueType NormalizeValue(ValueType value)
        {
            var input = (float)value;

            // Cap on minimum
            if (MinimumValue.HasValue)
            {
                input = Math.Max(input, MinimumValue.Value);
            }

            // Cap on maximum
            if (MaximumValue.HasValue)
            {
                input = Math.Min(input, MaximumValue.Value);
            }

            return input;
        }
    }
}
