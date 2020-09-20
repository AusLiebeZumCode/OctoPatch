using Newtonsoft.Json;

namespace OctoPatch.DesktopClient.Models
{
    public abstract class NodeConfigurationModel : Model
    {
        public abstract void Setup(string environment);

        public abstract void SetConfiguration(string configuration);

        public abstract string GetConfiguration();
    }

    public abstract class NodeConfigurationModel<TConfiguration, TEnvironment> : NodeConfigurationModel
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
