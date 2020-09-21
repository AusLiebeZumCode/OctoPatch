using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using OctoPatch.Descriptions;
using OctoPatch.DesktopClient.Models;
using OctoPatch.Plugin.Rest;
using OctoPatch.Server;
using OctoPatch.Setup;

namespace OctoPatch.DesktopClient.ViewModels
{
    public sealed class RuntimeViewModel : IRuntimeViewModel
    {
        private readonly IRuntime _runtime;

        private List<NodeItem> _nodes;

        private List<NodeDescription> _descriptions;

        public ObservableCollection<NodeDescription> NodeDescriptions { get; }

        private NodeDescription _selectedNodeDescription;

        public NodeDescription SelectedNodeDescription
        {
            get => _selectedNodeDescription;
            set
            {
                _selectedNodeDescription = value;
                OnPropertyChanged();

                _addSelectedNodeDescription.Enabled = value != null;
            }
        }

        private readonly ActionCommand _addSelectedNodeDescription;

        public ICommand AddSelectedNodeDescription => _addSelectedNodeDescription;

        public ObservableCollection<NodeDescription> ContextNodeDescriptions { get; }

        private NodeDescription _selectedContextNodeDescription;

        public NodeDescription SelectedContextNodeDescription
        {
            get => _selectedContextNodeDescription;
            set
            {
                _selectedContextNodeDescription = value;
                OnPropertyChanged();

                _addSelectedContextNodeDescription.Enabled = value != null;
            }
        }

        private readonly ActionCommand _addSelectedContextNodeDescription;

        public ICommand AddSelectedContextNodeDescription => _addSelectedContextNodeDescription;


        public ObservableCollection<NodeModel> NodeTree { get; }

        private readonly ActionCommand _removeSelectedNode;

        public ICommand RemoveSelectedNode => _removeSelectedNode;

        private readonly ActionCommand _startSelectedNode;

        public ICommand StartSelectedNode => _startSelectedNode;

        private readonly ActionCommand _stopSelectedNode;

        public ICommand StopSelectedNode => _stopSelectedNode;

        private NodeModel _selectedNode;

        public NodeModel SelectedNode
        {
            get => _selectedNode;
            set
            {
                _selectedNode = value;
                OnPropertyChanged();

                // Fill context toolbox
                ContextNodeDescriptions.Clear();
                switch (value)
                {
                    case InputNodeModel input:

                        foreach (var description in _descriptions.OfType<CollectorNodeDescription>().Where(d => d.TypeKey == input.TypeKey))
                        {
                            ContextNodeDescriptions.Add(description);
                        }

                        break;
                    case OutputNodeModel output:

                        foreach (var description in _descriptions.OfType<SplitterNodeDescription>().Where(d => d.TypeKey == output.TypeKey))
                        {
                            ContextNodeDescriptions.Add(description);
                        }

                        break;

                    case CommonNodeModel common:

                        foreach (var description in _descriptions.OfType<AttachedNodeDescription>().Where(d => d.ParentKey == common.Key))
                        {
                            ContextNodeDescriptions.Add(description);
                        }

                        break;
                    case AttachedNodeModel attached:

                        foreach (var description in _descriptions.OfType<AttachedNodeDescription>().Where(d => d.ParentKey == attached.Key))
                        {
                            ContextNodeDescriptions.Add(description);
                        }

                        break;

                    case SplitterNodeModel splitter:

                        foreach (var description in _descriptions.OfType<AttachedNodeDescription>().Where(d => d.ParentKey == splitter.Key))
                        {
                            ContextNodeDescriptions.Add(description);
                        }

                        break;

                    case CollectorNodeModel collector:

                        foreach (var description in _descriptions.OfType<AttachedNodeDescription>().Where(d => d.ParentKey == collector.Key))
                        {
                            ContextNodeDescriptions.Add(description);
                        }

                        break;

                }

                var item = _nodes.FirstOrDefault(n => n.Model == value);

                if (item == null)
                {
                    NodeDescription = null;
                }
                else
                {
                    NodeDescription = new NodeDescriptionModel
                    {
                        Name = item.Setup.Name,
                        Description = item.Setup.Description
                    };
                }

                _removeSelectedNode.Enabled = item != null;
                _startSelectedNode.Enabled = item != null;
                _stopSelectedNode.Enabled = item != null;
                _saveNodeDescription.Enabled = item != null;
                _takeConnector.Enabled = (SelectedWireConnector != null && value is InputNodeModel) || value is OutputNodeModel;

                //// TODO: Lookup model by Attribute
                if (item?.Setup.Key == "12ea0035-45af-4da8-8b5d-e1b9d9484ba4:MidiDeviceNode")
                {
                    var model = new MidiDeviceModel();
                    model.Setup(item.Environment);
                    model.SetConfiguration(item.Setup.Configuration);
                    NodeConfiguration = model;
                    _saveNodeConfiguration.Enabled = true;
                }
                else if (item?.Setup.Key == "a6fe76d7-5f0e-4763-a3a5-fcaf43c71464:KeyboardNode")
                {
                    NodeConfiguration = null;
                    _saveNodeConfiguration.Enabled = true;
                }
                else if(item?.Setup.Key == $"{RestPlugin.PluginId[1..^1].ToLower()}:{nameof(RestGetNode)}")
                {
                    var model = new RestGetModel();
                    model.Setup(item.Environment);
                    model.SetConfiguration(item.Setup.Configuration);
                    NodeConfiguration = model;
                    _saveNodeConfiguration.Enabled = true;
                }
                else
                {
                    NodeConfiguration = null;
                    _saveNodeConfiguration.Enabled = false;
                }
            }
        }

        private NodeDescriptionModel _nodeDescription;

        public NodeDescriptionModel NodeDescription
        {
            get => _nodeDescription;
            private set
            {
                _nodeDescription = value;
                OnPropertyChanged();
            }
        }

        private readonly ActionCommand _saveNodeDescription;

        public ICommand SaveNodeDescription => _saveNodeDescription;

        private NodeConfigurationModel _nodeConfiguration;

        public NodeConfigurationModel NodeConfiguration
        {
            get => _nodeConfiguration;
            private set
            {
                _nodeConfiguration = value;
                OnPropertyChanged();
            }
        }

        private readonly ActionCommand _saveNodeConfiguration;

        public ICommand SaveNodeConfiguration => _saveNodeConfiguration;

        private OutputNodeModel _selectedWireConnector;

        public OutputNodeModel SelectedWireConnector
        {
            get => _selectedWireConnector;
            private set
            {
                _selectedWireConnector = value;
                OnPropertyChanged();
            }
        }

        private readonly ActionCommand _takeConnector;

        public ICommand TakeConnector => _takeConnector;

        public ObservableCollection<WireSetup> Wires { get; }

        public RuntimeViewModel()
        {
            _nodes = new List<NodeItem>();
            _descriptions = new List<NodeDescription>();

            NodeDescriptions = new ObservableCollection<NodeDescription>();
            ContextNodeDescriptions = new ObservableCollection<NodeDescription>();
            NodeTree = new ObservableCollection<NodeModel>();
            Wires = new ObservableCollection<WireSetup>();

            _addSelectedNodeDescription = new ActionCommand(AddNodeDescriptionCallback, false);
            _addSelectedContextNodeDescription = new ActionCommand(AddContextNodeDescriptionCallback, false);
            _removeSelectedNode = new ActionCommand(RemoveSelectedNodeCallback, false);
            _startSelectedNode = new ActionCommand(StartSelectedNodeCallback, false);
            _stopSelectedNode = new ActionCommand(StopSelectedNodeCallback, false);
            _saveNodeDescription = new ActionCommand(SaveNodeDescriptionCallback, false);
            _saveNodeConfiguration = new ActionCommand(SaveNodeConfigurationCallback, false);
            _takeConnector = new ActionCommand(TakeConnectorCallback, false);

            var repository = new Repository();
            _runtime = new Runtime(repository);

            _runtime.NodeAdded += RuntimeOnOnNodeAdded;
            _runtime.NodeRemoved += RuntimeOnOnNodeRemoved;
            _runtime.NodeUpdated += RuntimeOnOnNodeUpdated;
            _runtime.NodeStateChanged += RuntimeOnOnNodeStateChanged;
            _runtime.NodeEnvironmentChanged += RuntimeOnOnNodeEnvironmentChanged;
            _runtime.WireAdded += RuntimeOnOnWireAdded;
            _runtime.WireRemoved += RuntimeOnOnWireRemoved;

            Task.Run(() => Setup(CancellationToken.None));
        }

        private async void TakeConnectorCallback(object obj)
        {
            var node = SelectedNode;

            // Select first connector
            if (node is OutputNodeModel outputNode)
            {
                SelectedWireConnector = outputNode;
                return;
            }

            // Wire up
            if (SelectedWireConnector != null && node is InputNodeModel inputNode)
            {
                await _runtime.AddWire(
                    SelectedWireConnector.ParentId, SelectedWireConnector.Key, 
                    inputNode.ParentId, inputNode.Key, CancellationToken.None);
                SelectedWireConnector = null;
                _takeConnector.Enabled = false;
            }
        }

        private async void AddContextNodeDescriptionCallback(object obj)
        {
            var node = SelectedNode;
            if (node == null)
            {
                return;
            }

            Guid parentId;
            string connectorKey = null;
            if (node is CommonNodeModel commonParent)
            {
                parentId = commonParent.Id;
            }
            else if (node is AttachedNodeModel attachedParent)
            {
                parentId = attachedParent.Id;
            }
            else if (node is SplitterNodeModel splitterParent)
            {
                parentId = splitterParent.Id;
            }
            else if (node is CollectorNodeModel collectorParent)
            {
                parentId = collectorParent.Id;
            }
            else if (node is OutputNodeModel output)
            {
                parentId = output.ParentId;
                connectorKey = output.Key;
            }
            else if (node is InputNodeModel input)
            {
                parentId = input.ParentId;
                connectorKey = input.Key;
            }
            else
            {
                // Not supported type
                return;
            }

            var contextNode = SelectedContextNodeDescription;
            if (contextNode == null)
            {
                return;
            }

            if (contextNode is SplitterNodeDescription)
            {
                await _runtime.AddNode(contextNode.Key, parentId, connectorKey, CancellationToken.None);
            }
            else if (contextNode is CollectorNodeDescription)
            {
                await _runtime.AddNode(contextNode.Key, parentId, connectorKey, CancellationToken.None);
            }
            else if (contextNode is AttachedNodeDescription)
            {
                await _runtime.AddNode(contextNode.Key, parentId, null, CancellationToken.None);
            }
        }

        private async void SaveNodeConfigurationCallback(object obj)
        {
            var node = SelectedNode;
            var item = _nodes.FirstOrDefault(n => n.Model == node);
            if (item == null)
            {
                return;
            }

            try
            {
                await _runtime.SetNodeConfiguration(item.Setup.NodeId, NodeConfiguration?.GetConfiguration() ?? "{}",
                    CancellationToken.None);
            }
            catch (Exception)
            {
                // This is just to see what happens
            }
        }

        private async void SaveNodeDescriptionCallback(object parameter)
        {
            var node = SelectedNode;
            var item = _nodes.FirstOrDefault(n => n.Model == node);
            if (item == null)
            {
                return;
            }

            await _runtime.SetNodeDescription(item.Setup.NodeId, NodeDescription.Name, NodeDescription.Description,
                CancellationToken.None);
        }

        private void RuntimeOnOnNodeUpdated(NodeSetup setup)
        {
            var node = _nodes.FirstOrDefault(n => n.Setup.NodeId == setup.NodeId);
            if (node == null)
            {
                return;
            }

            node.Setup = setup;
        }

        private void RuntimeOnOnNodeStateChanged(Guid nodeId, NodeState state)
        {
            var node = _nodes.FirstOrDefault(n => n.Setup.NodeId == nodeId);
            if (node == null)
            {
                return;
            }

            node.Model.State = state;
        }

        private void RuntimeOnOnNodeEnvironmentChanged(Guid nodeId, string environment)
        {
            var node = _nodes.FirstOrDefault(n => n.Setup.NodeId == nodeId);
            if (node == null)
            {
                return;
            }

            node.Environment = environment;
        }

        private async void StopSelectedNodeCallback(object obj)
        {
            var node = SelectedNode;
            var item = _nodes.FirstOrDefault(n => n.Model == node);
            if (item == null)
            {
                return;
            }

            await _runtime.StopNode(item.Setup.NodeId, CancellationToken.None);
        }

        private async void StartSelectedNodeCallback(object obj)
        {
            var node = SelectedNode;
            var item = _nodes.FirstOrDefault(n => n.Model == node);
            if (item == null)
            {
                return;
            }

            await _runtime.StartNode(item.Setup.NodeId, CancellationToken.None);
        }

        private async void RemoveSelectedNodeCallback(object obj)
        {
            var node = SelectedNode;
            var item = _nodes.FirstOrDefault(n => n.Model == node);
            if (item == null)
            {
                return;
            }

            await _runtime.RemoveNode(item.Setup.NodeId, CancellationToken.None);
        }

        private async void AddNodeDescriptionCallback(object obj)
        {
            var description = SelectedNodeDescription;
            if (description != null)
            {
                await _runtime.AddNode(description.Key, null, null, CancellationToken.None);
            }
        }

        private void RuntimeOnOnWireRemoved(Guid obj)
        {
        }

        private void RuntimeOnOnWireAdded(WireSetup obj)
        {
        }

        private void RuntimeOnOnNodeRemoved(Guid obj)
        {
            var node = _nodes.FirstOrDefault(n => n.Setup.NodeId == obj);
            if (node == null)
            {
                return;
            }

            // Remove node in tree
            RemoveRecursive(node.Model, NodeTree);

            _nodes.Remove(node);
        }

        private void RemoveRecursive(NodeModel model, ObservableCollection<NodeModel> list)
        {
            NodeModel hit = null;

            foreach (var item in list)
            {
                if (item == model)
                {
                    hit = item;
                }

                RemoveRecursive(model, item.Items);
            }

            if (hit != null)
            {
                list.Remove(hit);
            }
        }

        private void RuntimeOnOnNodeAdded(NodeSetup setup, NodeState state, string environment)
        {
            // identify description
            var description = _descriptions.FirstOrDefault(d => d.Key == setup.Key);

            NodeModel nodeModel = null;
            switch (description)
            {
                case AttachedNodeDescription attached:
                    nodeModel = new AttachedNodeModel(setup.NodeId, attached);

                    var attachedParent = _nodes.FirstOrDefault(n => n.Setup.NodeId == setup.ParentNodeId);
                    if (attachedParent == null)
                    {
                        return;
                    }

                    attachedParent.Model.Items.Add(new AttachedNodeModel(setup.NodeId, attached));
                    break;
                case CollectorNodeDescription collector:
                    nodeModel = new CollectorNodeModel(setup.NodeId, collector);

                    var collectorParent = _nodes.FirstOrDefault(n => n.Setup.NodeId == setup.ParentNodeId);
                    if (collectorParent == null)
                    {
                        return;
                    }

                    var input = collectorParent.Model.Items.OfType<InputNodeModel>()
                        .FirstOrDefault(i => i.Key == setup.ParentConnector);
                    if (input == null)
                    {
                        return;
                    }

                    input.Items.Add(new CollectorNodeModel(setup.NodeId, collector));
                    break;
                case SplitterNodeDescription splitter:
                    nodeModel = new SplitterNodeModel(setup.NodeId, splitter);

                    var splitterParent = _nodes.FirstOrDefault(n => n.Setup.NodeId == setup.ParentNodeId);
                    if (splitterParent == null)
                    {
                        return;
                    }

                    var output = splitterParent.Model.Items.OfType<OutputNodeModel>()
                        .FirstOrDefault(i => i.Key == setup.ParentConnector);
                    if (output == null)
                    {
                        return;
                    }

                    output.Items.Add(new SplitterNodeModel(setup.NodeId, splitter));

                    break;
                case CommonNodeDescription common:
                    nodeModel = new CommonNodeModel(setup.NodeId, common);
                    NodeTree.Add(nodeModel);
                    break;
            }

            var node = new NodeItem
            {
                Setup = setup,
                Environment = environment,
                Model = nodeModel
            };

            _nodes.Add(node);
        }

        public async Task Setup(CancellationToken cancellationToken)
        {
            var descriptions = await _runtime.GetNodeDescriptions(cancellationToken);

            _descriptions.Clear();
            _descriptions.AddRange(descriptions);

            foreach (var description in _descriptions.OfType<CommonNodeDescription>())
            {
                NodeDescriptions.Add(description);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private sealed class NodeItem
        {
            public NodeSetup Setup { get; set; }

            public string Environment { get; set; }

            public NodeModel Model { get; set; }
        }
    }
}
