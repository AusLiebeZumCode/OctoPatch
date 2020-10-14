using OctoPatch.Descriptions;
using System;
using OctoPatch.ContentTypes;

namespace OctoPatch.Core.Adapters
{
    public sealed class LinearTransformationAdapter : Adapter<double, LinearTransformationAdapter.Config, EmptyEnvironment>
    {
        #region definitions

        public static AdapterDescription Description =
            AdapterDescription.Create<LinearTransformationAdapter>(
                Guid.Parse(CorePlugin.PluginId), "Linear Adapter", "Linear Adapter")
                .AddSupportedTypeCombination<IntegerContentType, IntegerContentType>()
                .AddSupportedTypeCombination<IntegerContentType, FloatContentType>()
                .AddSupportedTypeCombination<FloatContentType, IntegerContentType>()
                .AddSupportedTypeCombination<FloatContentType, FloatContentType>();

        #endregion

        public LinearTransformationAdapter(IWire wire) : base(wire)
        {
            RegisterImporter<int>(ImportInteger);
            RegisterImporter<float>(ImportFloat);
            RegisterExporter<int>(ExportInteger);
            RegisterExporter<float>(ExportFloat);
        }

        private double ImportInteger(int input)
        {
            var integerInput = (IntegerContentType)Input.ContentType;
            var integerMin = integerInput.MinimumValue ?? int.MinValue;
            var integerMax = integerInput.MaximumValue ?? int.MaxValue;
            return (1.0 / (integerMax - integerMin)) * (input - integerMin);
        }

        private double ImportFloat(float input)
        {
            var floatInput = (FloatContentType)Input.ContentType;
            var floatMin = floatInput.MinimumValue ?? float.MinValue;
            var floatMax = floatInput.MaximumValue ?? float.MaxValue;
            return (1.0 / (floatMax - floatMin)) * (input - floatMin);
        }

        private int ExportInteger(double input)
        {
            var integerOutput = (IntegerContentType)Output.ContentType;
            var integerMin = integerOutput.MinimumValue ?? int.MinValue;
            var integerMax = integerOutput.MaximumValue ?? int.MaxValue;
            return (int)((1.0 / (integerMax - integerMin)) * input) + integerMin;
        }

        private float ExportFloat(double input)
        {
            var floatInput = (FloatContentType)Output.ContentType;
            var floatMin = floatInput.MinimumValue ?? float.MinValue;
            var floatMax = floatInput.MaximumValue ?? float.MaxValue;
            return (float)((1.0 / (floatMax - floatMin)) * input) + floatMin;
        }

        public sealed class Config : IConfiguration
        {
            public bool Inverted { get; set; }
        }
    }
}
