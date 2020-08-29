namespace OctoPatch
{
    /// <summary>
    /// Base class for all kind of adapters
    /// </summary>
    /// <typeparam name="TConfiguration">type of configuration</typeparam>
    /// <typeparam name="TEnvironment">type of environment</typeparam>
    public abstract class Adapter<TConfiguration, TEnvironment> : IAdapter
        where TConfiguration : IConfiguration
        where TEnvironment : IEnvironment
    {
        protected Adapter(IInputConnector input, IOutputConnector output)
        {
        }

        public void Dispose()
        {

        }
    }
}
