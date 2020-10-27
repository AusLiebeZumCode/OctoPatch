using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using OctoPatch.Descriptions;
using OctoPatch.Server;
using OctoPatch.Setup;

namespace OctoPatch.DesktopClient.Views
{
    /// <summary>
    /// Interaction logic for PatchView.xaml
    /// </summary>
    public partial class PatchView : Canvas
    {
        private IRuntime _runtime;

        private NodeDescription[] _nodeDescriptions;

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

            var node = _nodes.Values.FirstOrDefault(n => 
                GetLeft(n.View) <= position.X && 
                GetLeft(n.View) + n.View.ActualWidth >= position.X &&
                GetTop(n.View) <= position.Y && 
                GetTop(n.View) + n.View.ActualHeight >= position.Y);

            // Find node
            //var widthHalf = (double) NodeWidth / 2;
            //var heightHalf = (double) NodeHeight / 2;
            //var node = _nodes.Values.FirstOrDefault(n =>
            //    n.View.ActualWidth / 2 Model.Setup.PositionX - widthHalf <= position.X &&
            //    n.Model.Setup.PositionX + widthHalf >= position.X &&
            //    n.Model.Setup.PositionY - heightHalf <= position.Y &&
            //    n.Model.Setup.PositionY + heightHalf >= position.Y);

            //var node = _nodes.Values.FirstOrDefault(n =>
            //    n.Model.Setup.PositionX - widthHalf <= position.X &&
            //    n.Model.Setup.PositionX + widthHalf >= position.X &&
            //    n.Model.Setup.PositionY - heightHalf <= position.Y &&
            //    n.Model.Setup.PositionY + heightHalf >= position.Y);

            SetSelectedNode(node);

            _draggingNode = node;
            _draggingStart = position;
        }

        private void SetSelectedNode(NodeModel nodeModel)
        {
            // Deselect current selection
            if (_selectedNode != null)
            {
                _selectedNode.View.BorderBrush = null;
            }

            _selectedNode = null;

            // Select new selection
            if (nodeModel != null)
            {
                _selectedNode = nodeModel;
                _selectedNode.View.BorderBrush = new SolidColorBrush(Colors.Black);
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
                var newPos = new Point(_draggingNode.Model.Setup.PositionX, _draggingNode.Model.Setup.PositionY) +
                             (position - _draggingStart);

                SetLeft(_draggingNode.View, newPos.X - _draggingNode.View.ActualWidth / 2);
                SetTop(_draggingNode.View, newPos.Y - _draggingNode.View.ActualHeight / 2);

                _runtime.SetNodePosition(_draggingNode.Model.Setup.NodeId, (int) newPos.X, (int) newPos.Y,
                    CancellationToken.None);

                _draggingNode = null;
            }
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            _runtime = DataContext as IRuntime;

            _nodeDescriptions = (await _runtime.GetNodeDescriptions(CancellationToken.None)).ToArray();

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
            var newPos = new Point(_draggingNode.Model.Setup.PositionX, _draggingNode.Model.Setup.PositionY) + (position - _draggingStart);

            SetLeft(_draggingNode.View, newPos.X - _draggingNode.View.ActualWidth / 2);
            SetTop(_draggingNode.View, newPos.Y - _draggingNode.View.ActualHeight / 2);
        }

        #region Nodes

        private void RuntimeOnOnNodeAdded(NodeSetup setup, NodeState state, string environment)
        {
            var description = _nodeDescriptions.First(d => d.Key == setup.Key);
            var model = new ModelsX.NodeModel
            {
                Setup = setup,
                Description = description,
            };


            var nodeView = new NodeView
            {
                DataContext = model,
                Background = new SolidColorBrush(Colors.Brown),
                BorderThickness = new Thickness(2)
            };

            SetLeft(nodeView, -(nodeView.ActualWidth / 2) + setup.PositionX);
            SetTop(nodeView, -(nodeView.ActualHeight / 2) + setup.PositionY);
            Children.Add(nodeView);

            var node = new NodeModel
            {
                Model = model,
                View = nodeView,
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

            node.Model.Setup = setup;

            // TODO: Update control

            SetLeft(node.View, setup.PositionX - node.View.ActualWidth / 2);
            SetTop(node.View, setup.PositionY - node.View.ActualHeight / 2);
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
            Children.Remove(node.View);
        }

        private sealed class NodeModel
        {
            public ModelsX.NodeModel Model { get; set; }

            public NodeView View { get; set; }

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
