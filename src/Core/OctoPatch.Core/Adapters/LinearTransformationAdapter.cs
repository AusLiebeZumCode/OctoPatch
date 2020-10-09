using OctoPatch.Descriptions;
using System;
using OctoPatch.ContentTypes;

namespace OctoPatch.Core.Adapters
{
    public sealed class LinearTransformationAdapter : Adapter<IConfiguration, IEnvironment>
    {
        #region definitions

        public static AdapterDescription Description =
            AdapterDescription.Create<LinearTransformationAdapter>(
                Guid.Parse(CorePlugin.PluginId), "Linear Adapter", "Linear Adapter")
                .AddSupportedTypeCombination<IntegerContentType, IntegerContentType>();

        #endregion

        public LinearTransformationAdapter(IInputConnector input, IOutputConnector output) 
            : base(input, output)
        {
        }
    }
}
