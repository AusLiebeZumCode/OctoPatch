namespace OctoPatch.DesktopClient.Models
{
    /// <summary>
    /// Basic type for all kind of configuration models
    /// </summary>
    public abstract class ConfigurationModel : Model
    {
        /// <summary>
        /// Method to apply environment to the configuration model
        /// </summary>
        /// <param name="environment">serialized environment</param>
        public abstract void Setup(string environment);

        /// <summary>
        /// Method to apply configuration to the configuration model
        /// </summary>
        /// <param name="configuration">serialized configuration</param>
        public abstract void SetConfiguration(string configuration);

        /// <summary>
        /// Method to grab the configuration back in serialized format
        /// </summary>
        /// <returns>serialized configuration</returns>
        public abstract string GetConfiguration();

    }
}
