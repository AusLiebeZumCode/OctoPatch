using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using OctoPatch.Server;
using OctoPatch.Setup;

namespace OctoPatch.DesktopClient.Views
{
    /// <summary>
    /// Interaction logic for PatchView.xaml
    /// </summary>
    public partial class PatchView : Canvas
    {
        private const int NodeWidth = 50;

        private const int NodeHeight = 25;

        private IRuntime _runtime;

        private readonly ConcurrentDictionary<Guid, NodeModel> _nodes;

        private NodeModel _selectedNode;

        private NodeModel _draggingNode;

        private Point _draggingStart;

        public PatchView()
        {
            InitializeComponent();

            _nodes = new ConcurrentDictionary<Guid, NodeModel>();

            Loaded += OnLoaded;
            MouseDown += OnMouseDown;
            MouseUp += OnMouseUp;
            MouseMove += OnMouseMove;
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left)
            {
                return;
            }

            var position = e.GetPosition(this);

            // Find node
            var widthHalf = (double) NodeWidth / 2;
            var heightHalf = (double) NodeHeight / 2;
            var node = _nodes.Values.FirstOrDefault(n =>
                n.Setup.PositionX - widthHalf <= position.X &&
                n.Setup.PositionX + widthHalf >= position.X &&
                n.Setup.PositionY - heightHalf <= position.Y &&
                n.Setup.PositionY + heightHalf >= position.Y);

            SetSelectedNode(node);

            _draggingNode = node;
            _draggingStart = position;
        }

        private void SetSelectedNode(NodeModel nodeModel)
        {
            // Deselect current selection
            if (_selectedNode != null)
            {
                _selectedNode.Rectangle.Stroke = null;
            }

            _selectedNode = null;

            // Select new selection
            if (nodeModel != null)
            {
                _selectedNode = nodeModel;
                _selectedNode.Rectangle.Stroke = new SolidColorBrush(Colors.Black);
            }
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left)
            {
                return;
            }

            // Handle drag/drop
            if (_draggingNode != null)
            {
                var position = e.GetPosition(this);
                var newPos = new Point(_draggingNode.Setup.PositionX, _draggingNode.Setup.PositionY) +
                             (position - _draggingStart);

                SetLeft(_draggingNode.Rectangle, newPos.X - (double) NodeWidth / 2);
                SetTop(_draggingNode.Rectangle, newPos.Y - (double) NodeHeight / 2);

                _runtime.SetNodePosition(_draggingNode.Setup.NodeId, (int) newPos.X, (int) newPos.Y,
                    CancellationToken.None);

                _draggingNode = null;
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _runtime = DataContext as IRuntime;

            _runtime.NodeAdded += RuntimeOnOnNodeAdded;
            _runtime.NodeRemoved += RuntimeOnOnNodeRemoved;
            _runtime.NodeUpdated += RuntimeOnOnNodeUpdated;
            _runtime.NodeStateChanged += RuntimeOnOnNodeStateChanged;
            _runtime.NodeEnvironmentChanged += RuntimeOnOnNodeEnvironmentChanged;
            _runtime.WireAdded += RuntimeOnOnWireAdded;
            _runtime.WireRemoved += RuntimeOnOnWireRemoved;
            _runtime.WireUpdated += RuntimeOnWireUpdated;
            _runtime.AdapterEnvironmentChanged += RuntimeOnAdapterEnvironmentChanged;
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_draggingNode == null)
            {
                return;
            }

            var position = e.GetPosition(this);
            var newPos = new Point(_draggingNode.Setup.PositionX, _draggingNode.Setup.PositionY) + (position - _draggingStart);

            SetLeft(_draggingNode.Rectangle, newPos.X - (double) NodeWidth / 2);
            SetTop(_draggingNode.Rectangle, newPos.Y - (double) NodeHeight / 2);
        }

        #region Nodes

        private void RuntimeOnOnNodeAdded(NodeSetup setup, NodeState state, string environment)
        {
            // TODO: Create control

            var rectangle = new Rectangle
            {
                Width = NodeWidth,
                Height = NodeHeight,
                Fill = new SolidColorBrush(Colors.Brown),
                StrokeThickness = 2
            };

            SetLeft(rectangle, -((double)NodeWidth / 2) + setup.PositionX);
            SetTop(rectangle, -((double)NodeHeight / 2) + setup.PositionY);
            Children.Add(rectangle);

            var node = new NodeModel
            {
                Setup = setup,
                Rectangle = rectangle,
                State = state,
                Environment = environment
            };

            _nodes.TryAdd(setup.NodeId, node);
        }

        private void RuntimeOnOnNodeUpdated(NodeSetup setup)
        {
            if (!_nodes.TryGetValue(setup.NodeId, out var node))
            {
                return;
            }

            node.Setup = setup;

            // TODO: Update control

            SetLeft(node.Rectangle, setup.PositionX - (double) NodeWidth / 2);
            SetTop(node.Rectangle, setup.PositionY - (double) NodeHeight / 2);
        }

        private void RuntimeOnOnNodeEnvironmentChanged(Guid nodeId, string environment)
        {
            if (!_nodes.TryGetValue(nodeId, out var node))
            {
                return;
            }

            node.Environment = environment;
        }

        private void RuntimeOnOnNodeStateChanged(Guid nodeId, NodeState state)
        {
            if (!_nodes.TryGetValue(nodeId, out var node))
            {
                return;
            }

            node.State = state;

            // TODO: Change 
        }

        private void RuntimeOnOnNodeRemoved(Guid nodeId)
        {
            if (!_nodes.TryRemove(nodeId, out var node))
            {
                return;
            }

            // Remove control
            Children.Remove(node.Rectangle);
        }

        private sealed class NodeModel
        {
            public NodeSetup Setup { get; set; }

            public Rectangle Rectangle { get; set; }

            public NodeState State { get; set; }

            public string Environment { get; set; }
        }

        #endregion

        #region Wires

        private void RuntimeOnOnWireAdded(WireSetup setup)
        {
        }

        private void RuntimeOnWireUpdated(WireSetup setup)
        {
        }

        private void RuntimeOnAdapterEnvironmentChanged(Guid wireId, string enviroment)
        {
        }

        private void RuntimeOnOnWireRemoved(Guid wireId)
        {
        }

        #endregion

    }
}
