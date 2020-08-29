namespace OctoPatch.Core.Adapters
{
    public sealed class LinearTransformationAdapter : Adapter<IConfiguration, IEnvironment>
    {
        public LinearTransformationAdapter(IInputConnector input, IOutputConnector output) 
            : base(input, output)
        {
        }
    }
}
