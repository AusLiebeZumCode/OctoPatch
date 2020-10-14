using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using OctoPatch.ContentTypes;
using OctoPatch.Descriptions;
using Timer = System.Timers.Timer;

namespace OctoPatch.Core.Nodes
{
    public sealed class SinusGeneratorNode : Node<EmptyConfiguration, EmptyEnvironment>
    {
        #region Type description

        /// <summary>
        /// Description of the node
        /// </summary>
        public static NodeDescription NodeDescription => CommonNodeDescription.Create<SinusGeneratorNode>(
                Guid.Parse(CorePlugin.PluginId),
                "Sinus generator node",
                "Sinus generator node")
            .AddOutputDescription(IntegerOutputDescription)
            .AddOutputDescription(FloatOutputDescription);

        /// <summary>
        /// Description of the integer output
        /// </summary>
        public static ConnectorDescription IntegerOutputDescription => new ConnectorDescription(
            "IntegerOutput", "Integer Output", "Integer Output", 
            IntegerContentType.Create(0, 100));

        /// <summary>
        /// Description of the integer output
        /// </summary>
        public static ConnectorDescription FloatOutputDescription => new ConnectorDescription(
            "FloatOutput", "Float Output", "Float Output", 
            FloatContentType.Create(0, 10f));

        #endregion

        private readonly IOutput<int> _integerHandler;

        private readonly IOutput<float> _floatHandler;

        private readonly Timer _timer;

        private readonly Stopwatch _stopwatch;

        public SinusGeneratorNode(Guid id) : base(id)
        {
            _integerHandler = RegisterOutput<int>(IntegerOutputDescription);
            _floatHandler = RegisterOutput<float>(FloatOutputDescription);

            _timer = new Timer(100);
            _timer.Elapsed += OnElapsed;

            _stopwatch = new Stopwatch();
        }

        protected override Task OnStart(CancellationToken cancellationToken)
        {
            _stopwatch.Restart();
            _timer.Start();
            return base.OnStart(cancellationToken);
        }

        protected override Task OnStop(CancellationToken cancellationToken)
        {
            _timer.Stop();
            return base.OnStop(cancellationToken);
        }

        private void OnElapsed(object sender, ElapsedEventArgs e)
        {
            var value = Math.Sin(_stopwatch.Elapsed.TotalSeconds) + 1;
            _integerHandler.Send((int)(value * 50));
            _floatHandler.Send((float)(value * 5));
        }

        protected override EmptyConfiguration DefaultConfiguration => new EmptyConfiguration();
    }
}
