using Newtonsoft.Json;

namespace OctoPatch.DesktopClient.Models
{
    /// <summary>
    /// Basic configuration type for node configuration
    /// </summary>
    /// <typeparam name="TConfiguration">model type for configuration</typeparam>
    /// <typeparam name="TEnvironment">model type for environment</typeparam>
    public abstract class AdapterConfigurationModel<TConfiguration> : ConfigurationModel 
        where TConfiguration : IConfiguration
    {
        public override void Setup(string environment)
        {
            // Do nothing since adapter do not have any environment yet
        }

        public override void SetConfiguration(string configuration)
        {
            if (configuration == null)
            {
                return;
            }

            OnSetConfiguration(JsonConvert.DeserializeObject<TConfiguration>(configuration));
        }

        protected abstract void OnSetConfiguration(TConfiguration configuration);

        public override string GetConfiguration()
        {
            return JsonConvert.SerializeObject(OnGetConfiguration());
        }

        protected abstract TConfiguration OnGetConfiguration();
    }
}
