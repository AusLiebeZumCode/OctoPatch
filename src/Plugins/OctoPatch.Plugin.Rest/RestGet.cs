using OctoPatch.ContentTypes;
using OctoPatch.Descriptions;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace OctoPatch.Plugin.Rest
{
    /// <summary>
    /// A node to make REST GET calls
    /// </summary>
    public sealed class RestGetNode : Node<RestGetNode.RestGetConfiguration, EmptyEnvironment>
    {
        /// <summary>
        /// The configuration for <see cref="RestGetNode"/>
        /// </summary>
        public sealed class RestGetConfiguration : IConfiguration, IEquatable<RestGetConfiguration>
        {
            /// <summary>
            /// Uri to make a REST GET request to
            /// </summary>
            public Uri Uri { get; set; }

            #region IEquatable

            public override bool Equals(object obj)
            {
                return Equals(obj as RestGetConfiguration);
            }


            public bool Equals(RestGetConfiguration other)
            {
                return other != null &&
                    Uri == other.Uri;
            }

            public override int GetHashCode()
            {
                return -1249714907 + EqualityComparer<Uri>.Default.GetHashCode(Uri);
            }

            public static bool operator ==(RestGetConfiguration left, RestGetConfiguration right)
            {
                return EqualityComparer<RestGetConfiguration>.Default.Equals(left, right);
            }

            public static bool operator !=(RestGetConfiguration left, RestGetConfiguration right)
            {
                return !(left == right);
            }

            #endregion
        }

        /// <summary>
        /// The result for <see cref="RestGetNode"/>
        /// </summary>
        public struct RestGetResult : IEquatable<RestGetResult>
        {
            /// <summary>
            /// The response from the REST GET call
            /// </summary>
            public string Response { get; set; }

            public RestGetResult(string response)
            {
                Response = response;
            }

            public override string ToString() => Response;

            #region IEquatable

            public override bool Equals(object obj)
            {
                return Equals((RestGetResult)obj);
            }


            public bool Equals(RestGetResult other)
            {
                return Response == other.Response;
            }

            public override int GetHashCode()
            {
                return 888819886 + EqualityComparer<string>.Default.GetHashCode(Response);
            }

            public static bool operator ==(RestGetResult left, RestGetResult right)
            {
                return EqualityComparer<RestGetResult>.Default.Equals(left, right);
            }

            public static bool operator !=(RestGetResult left, RestGetResult right)
            {
                return !(left == right);
            }

            #endregion
        }

        /// <summary>
        /// Description of the node
        /// </summary>
        public static NodeDescription Description => CommonNodeDescription.Create<RestGetNode>(
            Guid.Parse(RestPlugin.PluginId),
            "REST GET",
            "Makes a REST GET call to the specified uri")
            .AddInputDescription(RestGetInputDescription)
            .AddOutputDescription(RestGetOutputDescription);

        /// <summary>
        /// Description of the rest get input connector
        /// </summary>
        public static ConnectorDescription RestGetInputDescription => new ConnectorDescription(
            "RestGetInput", "Any input value to trigger the GET call", "When input received the node will make the GET call", new AllContentType());

        /// <summary>
        /// Description of the rest get output connector
        /// </summary>
        public static ConnectorDescription RestGetOutputDescription => new ConnectorDescription(
            "RestGetOutput", "GET Response", "Gets the response of the GET call",
            ComplexContentType.Create<RestGetResult>(Guid.Parse(RestPlugin.PluginId)));

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override RestGetConfiguration DefaultConfiguration => new RestGetConfiguration();

        private readonly IOutputConnectorHandler _outputConnector;
        private readonly HttpClient _http;

        public RestGetNode(Guid id) : base(id)
        {
            _http = new HttpClient();

            RegisterInputConnector(RestGetInputDescription).HandleRaw(HandleMessage);
            _outputConnector = RegisterOutputConnector(RestGetOutputDescription);
        }

        private async void HandleMessage(Message message)
        {
            if (State != NodeState.Running)
                return;

            if (Configuration?.Uri is null)
                return;

            var value = await _http.GetStringAsync(Configuration.Uri);
            _outputConnector.Send(new RestGetResult(value));
        }

        protected override void OnDispose()
        {
            _http.Dispose();
            base.OnDispose();
        }
    }
}