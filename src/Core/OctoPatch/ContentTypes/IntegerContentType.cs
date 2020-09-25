using System;
using System.Runtime.Serialization;

namespace OctoPatch.ContentTypes
{
    /// <summary>
    /// Represents a integer based message
    /// </summary>
    [DataContract]
    public sealed class IntegerContentType : ContentType
    {
        /// <summary>
        /// Optional lowest value for this message
        /// </summary>
        [DataMember]
        public int? MinimumValue { get; set; }

        /// <summary>
        /// Optional highest value for this message
        /// </summary>
        [DataMember]
        public int? MaximumValue { get; set; }

        /// <inheritdoc />
        public override bool IsSupportedType(Type type)
        {
            return type == typeof(int);
        }

        /// <inheritdoc />
        public override ValueType NormalizeValue(ValueType value)
        {
            var input = (int)value;

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

        public static IntegerContentType Create(int? minimumValue = null, int? maximumValue = null)
        {
            return new IntegerContentType
            {
                MinimumValue = minimumValue,
                MaximumValue = maximumValue
            };
        }
    }
}
