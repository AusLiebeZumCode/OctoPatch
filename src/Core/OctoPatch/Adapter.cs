using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OctoPatch.ContentTypes;

namespace OctoPatch
{
    /// <summary>
    /// Base class for all kind of adapters
    /// </summary>
    /// <typeparam name="TConfiguration">type of configuration</typeparam>
    /// <typeparam name="TEnvironment">type of environment</typeparam>
    /// <typeparam name="T">transport format</typeparam>
    public abstract class Adapter<T, TConfiguration, TEnvironment> : IAdapter
        where TConfiguration : IConfiguration
        where TEnvironment : IEnvironment
    {
        /// <summary>
        /// Reference to the current subscription
        /// </summary>
        private readonly IDisposable _subscription;

        private TConfiguration _configuration;

        /// <summary>
        /// Internal reference to the current configuration
        /// </summary>
        protected TConfiguration Configuration
        {
            get => _configuration;
            private set
            {
                _configuration = value;
                ConfigurationChanged?.Invoke(this, GetEnvironment());
            }
        }

        private TEnvironment _environment;

        /// <summary>
        /// Internal reference to the current environment
        /// </summary>
        protected TEnvironment Environment
        {
            get => _environment;
            private set
            {
                _environment = value;
                EnvironmentChanged?.Invoke(this, GetEnvironment());
            }
        }

        /// <summary>
        /// Internal reference to the input connector
        /// </summary>
        protected IOutputConnector Input { get; }

        /// <summary>
        /// Internal reference to the output connector
        /// </summary>
        protected IInputConnector Output { get; }

        /// <inheritdoc />
        public IWire Wire { get; }

        protected Adapter(IWire wire)
        {
            Wire = wire ?? throw new ArgumentNullException(nameof(wire));
            Input = wire.Input ?? throw new ArgumentNullException(nameof(wire.Input));
            Output = wire.Output ?? throw new ArgumentNullException(nameof(wire.Output));

            _importers = new Dictionary<Type, IImporter>();
            _exporters = new Dictionary<Type, IExporter>();

            _subscription = Wire.Input.Subscribe(message =>
            {
                Wire.Output.OnNext(Handle(message));
            });
        }

        public void Dispose()
        {
            _subscription.Dispose();
        }

        /// <summary>
        /// Handles incoming messages
        /// </summary>
        /// <param name="message">input message</param>
        /// <returns>output message</returns>
        protected virtual Message Handle(Message message)
        {
            // Stop if incoming message does not fit to the import
            if (Input.ContentType.SupportedType != message.Type)
            {
                return message;
            }

            // Stop if there is no fitting import/export pair
            if (!_importers.TryGetValue(Input.ContentType.SupportedType, out var inputer) ||
                !_exporters.TryGetValue(Output.ContentType.SupportedType, out var exporter))
            {
                return message;
            }

            // Import, transform and export
            var value = inputer.Convert(message);
            value = Transform(value);
            return exporter.Convert(value);
        }

        /// <summary>
        /// Transforms the normalized message
        /// </summary>
        /// <param name="input">input value</param>
        /// <returns>output value</returns>
        protected virtual T Transform(T input)
        {
            return input;
        }

        /// <summary>
        /// Sets a new configuration to the adapter
        /// </summary>
        /// <param name="configuration">serialized configuration</param>
        /// <param name="cancellationToken">cancellation token</param>
        public async Task SetConfiguration(string configuration, CancellationToken cancellationToken)
        {
            // Deserialization of configuration
            TConfiguration config;
            try
            {
                config = JsonConvert.DeserializeObject<TConfiguration>(configuration);
            }
            catch (JsonSerializationException ex)
            {
                throw new ArgumentException("could not deserialize configuration", ex);
            }
            catch (JsonReaderException ex)
            {
                throw new ArgumentException("could not read configuration", ex);
            }

            // Initialize
            await OnSetConfiguration(config, cancellationToken);

            // Write back values
            Configuration = config;
        }

        /// <summary>
        /// Gets a call when node is in setup
        /// </summary>
        /// <param name="configuration">configuration</param>
        /// <param name="cancellationToken">cancellation token</param>
        protected virtual Task OnSetConfiguration(TConfiguration configuration, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public string GetEnvironment()
        {
            return JsonConvert.SerializeObject(Environment);
        }

        /// <inheritdoc />
        public string GetConfiguration()
        {
            return JsonConvert.SerializeObject(Configuration);
        }

        /// <inheritdoc />
        public event Action<IAdapter, string> ConfigurationChanged;
        
        /// <inheritdoc />
        public event Action<IAdapter, string> EnvironmentChanged;

        #region Handler Registration

        /// <summary>
        /// List of known importers.
        /// </summary>
        private readonly Dictionary<Type, IImporter> _importers;

        /// <summary>
        /// Registers the importer for the given type
        /// </summary>
        /// <typeparam name="TI">importer type</typeparam>
        /// <param name="importer">import delegate</param>
        protected void RegisterImporter<TI>(Func<TI, T> importer) where TI : struct
        {
            if (_importers.ContainsKey(typeof(TI)))
            {
                throw new NotSupportedException("Type importer is already registered");
            }

            _importers.Add(typeof(TI), new StructImporter<TI>(importer));
        }

        /// <summary>
        /// Registers the importer for a string
        /// </summary>
        /// <param name="importer">import delegate</param>
        protected void RegisterStringImporter(Func<string, T> importer)
        {
            if (_importers.ContainsKey(typeof(string)))
            {
                throw new NotSupportedException("Type importer is already registered");
            }

            _importers.Add(typeof(string), new StringImporter(importer));
        }

        /// <summary>
        /// Registers the importer for a byte array
        /// </summary>
        /// <param name="importer">import delegate</param>
        protected void RegisterBinaryImporter(Func<byte[], T> importer)
        {
            if (_importers.ContainsKey(typeof(byte[])))
            {
                throw new NotSupportedException("Type importer is already registered");
            }

            _importers.Add(typeof(string), new BinaryImporter(importer));
        }

        /// <summary>
        /// List of known exporters.
        /// </summary>
        private readonly Dictionary<Type, IExporter> _exporters;

        /// <summary>
        /// Registers the exporter for the given type
        /// </summary>
        /// <typeparam name="TO">export type</typeparam>
        /// <param name="exporter">importer delegate</param>
        protected void RegisterExporter<TO>(Func<T, TO> exporter) where TO : struct
        {
            if (_exporters.ContainsKey(typeof(TO)))
            {
                throw new NotSupportedException("Type exporter is already registered");
            }

            _exporters.Add(typeof(TO), new StructExporter<TO>(exporter));
        }

        /// <summary>
        /// Registers the exporter for strings
        /// </summary>
        /// <param name="exporter">exporter delegate</param>
        protected void RegisterStringExporter(Func<T, string> exporter)
        {
            if (_exporters.ContainsKey(typeof(string)))
            {
                throw new NotSupportedException("Type exporter is already registered");
            }

            _exporters.Add(typeof(string), new StringExporter(exporter));
        }

        /// <summary>
        /// Registers the exporter for byte array
        /// </summary>
        /// <param name="exporter">exporter delegate</param>
        protected void RegisterBinaryExporter(Func<T, byte[]> exporter)
        {
            if (_exporters.ContainsKey(typeof(byte[])))
            {
                throw new NotSupportedException("Type exporter is already registered");
            }

            _exporters.Add(typeof(byte[]), new BinaryExporter(exporter));
        }

        #endregion

        #region Handler Types

        /// <summary>
        /// Interface for all kind of importer 
        /// </summary>
        private interface IImporter
        {
            /// <summary>
            /// Converts the given message into transport format
            /// </summary>
            /// <param name="message">incoming message</param>
            /// <returns>transport format</returns>
            T Convert(Message message);
        }

        /// <summary>
        /// Importer for struct types
        /// </summary>
        /// <typeparam name="TI">type</typeparam>
        private sealed class StructImporter<TI> : IImporter where TI : struct
        {
            /// <summary>
            /// Reference to the handler
            /// </summary>
            private readonly Func<TI, T> _importer;

            /// <summary>
            /// ctor
            /// </summary>
            /// <param name="importer">importer reference</param>
            public StructImporter(Func<TI, T> importer)
            {
                _importer = importer ?? throw new ArgumentNullException(nameof(importer));
            }

            /// <inheritdoc />
            public T Convert(Message message)
            {
                if (message.Type != typeof(TI))
                {
                    throw new ArgumentException(nameof(message));
                }

                var container = (TI) message.Content;
                return _importer(container);
            }
        }

        /// <summary>
        /// Importer for string types
        /// </summary>
        private sealed class StringImporter : IImporter
        {
            /// <summary>
            /// Reference to the handler
            /// </summary>
            private readonly Func<string, T> _importer;

            /// <summary>
            /// ctor
            /// </summary>
            /// <param name="importer">importer reference</param>
            public StringImporter(Func<string, T> importer)
            {
                _importer = importer ?? throw new ArgumentNullException(nameof(importer));
            }

            /// <inheritdoc />
            public T Convert(Message message)
            {
                if (message.Type != typeof(string))
                {
                    throw new ArgumentException(nameof(message));
                }

                var container = (StringContentType.StringContainer) message.Content;
                return _importer(container.Content);
            }
        }

        /// <summary>
        /// Importer for binary types
        /// </summary>
        private sealed class BinaryImporter : IImporter
        {
            /// <summary>
            /// Reference to the handler
            /// </summary>
            private readonly Func<byte[], T> _importer;

            /// <summary>
            /// ctor
            /// </summary>
            /// <param name="importer">importer reference</param>
            public BinaryImporter(Func<byte[], T> importer)
            {
                _importer = importer ?? throw new ArgumentNullException(nameof(importer));
            }

            /// <inheritdoc />
            public T Convert(Message message)
            {
                if (message.Type != typeof(byte[]))
                {
                    throw new ArgumentException(nameof(message));
                }

                var container = (BinaryContentType.BinaryContainer) message.Content;
                return _importer(container.Content);
            }
        }

        /// <summary>
        /// Interface for all kind of exporter
        /// </summary>
        private interface IExporter
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="input">transport type</param>
            /// <returns>outgoing message</returns>
            Message Convert(T input);
        }

        /// <summary>
        /// Export handler for struct types
        /// </summary>
        /// <typeparam name="TO">export type</typeparam>
        private sealed class StructExporter<TO> : IExporter 
            where TO : struct
        {
            /// <summary>
            /// Reference to the exporter
            /// </summary>
            private readonly Func<T, TO> _exporter;

            /// <summary>
            /// ctor
            /// </summary>
            /// <param name="exporter">export delegate</param>
            public StructExporter(Func<T, TO> exporter)
            {
                _exporter = exporter;
            }

            /// <inheritdoc />
            public Message Convert(T input)
            {
                var output = _exporter(input);
                return Message.Create(output);
            }
        }

        /// <summary>
        /// Export handler for strings
        /// </summary>
        private sealed class StringExporter : IExporter 
        {
            /// <summary>
            /// Reference to the exporter
            /// </summary>
            private readonly Func<T, string> _exporter;

            /// <summary>
            /// ctor
            /// </summary>
            /// <param name="exporter">export delegate</param>
            public StringExporter(Func<T, string> exporter)
            {
                _exporter = exporter;
            }

            /// <inheritdoc />
            public Message Convert(T input)
            {
                var output = _exporter(input);
                return new Message(typeof(string), new StringContentType.StringContainer(output));
            }
        }

        /// <summary>
        /// Export handler for byte arrays
        /// </summary>
        private sealed class BinaryExporter : IExporter 
        {
            /// <summary>
            /// Reference to the exporter
            /// </summary>
            private readonly Func<T, byte[]> _exporter;

            /// <summary>
            /// ctor
            /// </summary>
            /// <param name="exporter">export delegate</param>
            public BinaryExporter(Func<T, byte[]> exporter)
            {
                _exporter = exporter;
            }

            /// <inheritdoc />
            public Message Convert(T input)
            {
                var output = _exporter(input);
                return new Message(typeof(byte[]), new BinaryContentType.BinaryContainer(output));
            }
        }

        #endregion
    }
}
