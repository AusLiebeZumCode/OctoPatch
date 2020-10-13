using OctoPatch.Descriptions;
using System;
using OctoPatch.ContentTypes;

namespace OctoPatch.Core.Adapters
{
    public sealed class LinearTransformationAdapter : Adapter<LinearTransformationAdapter.Config, EmptyEnvironment>
    {
        #region definitions

        public static AdapterDescription Description =
            AdapterDescription.Create<LinearTransformationAdapter>(
                Guid.Parse(CorePlugin.PluginId), "Linear Adapter", "Linear Adapter")
                .AddSupportedTypeCombination<IntegerContentType, IntegerContentType>()
                .AddSupportedTypeCombination<FloatContentType, IntegerContentType>()
                .AddSupportedTypeCombination<IntegerContentType, FloatContentType>()
                .AddSupportedTypeCombination<FloatContentType, FloatContentType>();

        #endregion

        public LinearTransformationAdapter(IOutputConnector input, IInputConnector output) 
            : base(input, output)
        {
        }

        protected override Message Handle(Message message)
        {
            return message;
        }

        public sealed class Config : IConfiguration
        {
            public bool Inverted { get; set; }
        }
    }
}
