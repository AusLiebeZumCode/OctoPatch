using System;
using OctoPatch.Plugin.Rest;

namespace OctoPatch.DesktopClient.Models
{
    public sealed class RestGetModel : NodeConfigurationModel<RestGetNode.RestGetConfiguration, EmptyEnvironment>
    {
        private string _uri;

        /// <summary>
        /// Uri to make a REST GET request to
        /// </summary>
        public string Uri
        {
            get => _uri;
            set
            {
                _uri = value;
                OnPropertyChanged();
            }
        }

        protected override void OnSetup(EmptyEnvironment environment)
        {

        }

        protected override RestGetNode.RestGetConfiguration OnGetConfiguration()
        {
            return new RestGetNode.RestGetConfiguration()
            {
                Uri = new Uri(Uri)
            };
        }

        protected override void OnSetConfiguration(RestGetNode.RestGetConfiguration configuration)
        {
            Uri = configuration.Uri.ToString();
        }
    }
}