using Newtonsoft.Json;

namespace OctoPatch.DesktopClient.Models
{
    /// <summary>
    /// Basic configuration type for node configuration
    /// </summary>
    /// <typeparam name="TConfiguration">model type for configuration</typeparam>
    /// <typeparam name="TEnvironment">model type for environment</typeparam>
    public abstract class NodeConfigurationModel<TConfiguration, TEnvironment> : ConfigurationModel
        where TConfiguration : IConfiguration
        where TEnvironment : IEnvironment
    {
        public override void Setup(string environment)
        {
            if (environment == null)
            {
                return;
            }

            OnSetup(JsonConvert.DeserializeObject<TEnvironment>(environment));
        }

        protected abstract void OnSetup(TEnvironment environment);

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
