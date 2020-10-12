using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Win32;
using Newtonsoft.Json;
using OctoPatch.Descriptions;
using OctoPatch.DesktopClient.Models;
using OctoPatch.Server;
using OctoPatch.Setup;

namespace OctoPatch.DesktopClient.ViewModels
{
    public sealed class RuntimeViewModel : IRuntimeViewModel
    {
        private readonly IRuntime _runtime;

        private readonly List<NodeItem> _nodes;

        private readonly List<WireItem> _wires;

        private readonly List<NodeDescription> _nodeDescriptions;

        private readonly List<AdapterDescription> _adapterDescriptions;

        #region Application

        private readonly ActionCommand _newCommand;

        /// <inheritdoc />
        public ICommand NewCommand => _newCommand;

        private readonly ActionCommand _loadCommand;

        /// <inheritdoc />
        public ICommand LoadCommand => _loadCommand;

        private readonly ActionCommand _saveCommand;

        /// <inheritdoc />
        public ICommand SaveCommand => _saveCommand;

        #endregion

        #region Toolbox

        /// <inheritdoc />
        public ObservableCollection<NodeDescription> NodeDescriptions { get; }

        private NodeDescription _selectedNodeDescription;

        /// <inheritdoc />
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

        /// <inheritdoc />
        public ICommand AddSelectedNodeDescription => _addSelectedNodeDescription;

        #endregion

        #region Patch

        public ObservableCollection<NodeModel> NodeTree { get; }

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

                        foreach (var description in _nodeDescriptions.OfType<CollectorNodeDescription>().Where(d => d.TypeKey == input.TypeKey))
                        {
                            ContextNodeDescriptions.Add(description);
                        }

                        break;
                    case OutputNodeModel output:

                        foreach (var description in _nodeDescriptions.OfType<SplitterNodeDescription>().Where(d => d.TypeKey == output.TypeKey))
                        {
                            ContextNodeDescriptions.Add(description);
                        }

                        break;

                    case CommonNodeModel common:

                        foreach (var description in _nodeDescriptions.OfType<AttachedNodeDescription>().Where(d => d.ParentKey == common.Key))
                        {
                            ContextNodeDescriptions.Add(description);
                        }

                        break;
                    case AttachedNodeModel attached:

                        foreach (var description in _nodeDescriptions.OfType<AttachedNodeDescription>().Where(d => d.ParentKey == attached.Key))
                        {
                            ContextNodeDescriptions.Add(description);
                        }

                        break;

                    case SplitterNodeModel splitter:

                        foreach (var description in _nodeDescriptions.OfType<AttachedNodeDescription>().Where(d => d.ParentKey == splitter.Key))
                        {
                            ContextNodeDescriptions.Add(description);
                        }

                        break;

                    case CollectorNodeModel collector:

                        foreach (var description in _nodeDescriptions.OfType<AttachedNodeDescription>().Where(d => d.ParentKey == collector.Key))
                        {
                            ContextNodeDescriptions.Add(description);
                        }

                        break;
                }

                #region node management

                var item = _nodes.FirstOrDefault(n => n.Model == value);
                if (item == null)
                {
                    NodeDescription = null;
                    NodeConfiguration = null;
                }
                else
                {
                    NodeDescription = new NodeDescriptionModel
                    {
                        Name = item.Setup.Name,
                        Description = item.Setup.Description
                    };

                    NodeConfiguration = ConfigurationMap.GetConfigurationModel(item.Setup.Key);
                    if (NodeConfiguration != null)
                    {
                        NodeConfiguration.Setup(item.Environment);
                        NodeConfiguration.SetConfiguration(item.Setup.Configuration);
                    }
                }

                _removeSelectedNode.Enabled = item != null;
                _startSelectedNode.Enabled = item != null;
                _stopSelectedNode.Enabled = item != null;
                _saveNodeDescription.Enabled = item != null;
                _takeConnector.Enabled = SelectedWireConnector != null && value is InputNodeModel || value is OutputNodeModel;
                _saveNodeConfiguration.Enabled = NodeConfiguration != null;

                #endregion

                #region wire management

                var wire = _wires.FirstOrDefault(w => w.InputWire == value || w.OutputWire == value);
                if (wire == null)
                {
                    SelectedAdapterDescription = null;
                    AdapterConfiguration = null;
                }
                else
                {
                    SelectedAdapterDescription =
                        AdapterDescriptions.FirstOrDefault(d => d.Key == wire.Setup.AdapterKey);

                    AdapterConfiguration = ConfigurationMap.GetConfigurationModel(wire.Setup.AdapterKey);
                    if (AdapterConfiguration != null)
                    {

                    }

                }

                _removeSelectedWire.Enabled = wire != null;
                _saveAdapter.Enabled = wire != null;
                _saveAdapterConfiguration.Enabled = AdapterConfiguration != null;

                #endregion
            }
        }

        private readonly ActionCommand _removeSelectedNode;

        public ICommand RemoveSelectedNode => _removeSelectedNode;

        private readonly ActionCommand _removeSelectedWire;

        public ICommand RemoveSelectedWire => _removeSelectedWire;

        #endregion

        #region Context Toolbox

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

        #endregion

        #region Property bar

        #region Node lifecycle management

        private readonly ActionCommand _startSelectedNode;

        public ICommand StartSelectedNode => _startSelectedNode;

        private readonly ActionCommand _stopSelectedNode;

        public ICommand StopSelectedNode => _stopSelectedNode;

        #endregion

        #region Common node configuration

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

        #endregion

        #region Node specific configuration

        private ConfigurationModel _nodeConfiguration;

        public ConfigurationModel NodeConfiguration
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

        #endregion

        #region Wire wizard

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

        #endregion

        #region Wire configuration

        public ObservableCollection<AdapterDescription> AdapterDescriptions { get; }

        private AdapterDescription _selectedAdapterDescription;

        public AdapterDescription SelectedAdapterDescription
        {
            get => _selectedAdapterDescription;
            set
            {
                _selectedAdapterDescription = value;
                OnPropertyChanged();
            }
        }

        private readonly ActionCommand _saveAdapter;

        public ICommand SaveAdapter => _saveAdapter;

        #endregion

        #region Adapter configuration

        private ConfigurationModel _adapterConfiguration;

        public ConfigurationModel AdapterConfiguration
        {
            get => _adapterConfiguration;
            private set
            {
                _adapterConfiguration = value;
                OnPropertyChanged();
            }
        }

        private readonly ActionCommand _saveAdapterConfiguration;

        public ICommand SaveAdapterConfiguration => _saveAdapterConfiguration;

        #endregion

        #endregion

        public RuntimeViewModel()
        {
            _nodes = new List<NodeItem>();
            _wires = new List<WireItem>();
            _nodeDescriptions = new List<NodeDescription>();
            _adapterDescriptions = new List<AdapterDescription>();

            NodeDescriptions = new ObservableCollection<NodeDescription>();
            ContextNodeDescriptions = new ObservableCollection<NodeDescription>();
            NodeTree = new ObservableCollection<NodeModel>();
            AdapterDescriptions = new ObservableCollection<AdapterDescription>();

            _addSelectedNodeDescription = new ActionCommand(AddNodeDescriptionCallback, false);
            _addSelectedContextNodeDescription = new ActionCommand(AddContextNodeDescriptionCallback, false);
            _removeSelectedNode = new ActionCommand(RemoveSelectedNodeCallback, false);
            _removeSelectedWire = new ActionCommand(RemoveSelectedWireCallback, false);
            _startSelectedNode = new ActionCommand(StartSelectedNodeCallback, false);
            _stopSelectedNode = new ActionCommand(StopSelectedNodeCallback, false);
            _saveNodeDescription = new ActionCommand(SaveNodeDescriptionCallback, false);
            _saveNodeConfiguration = new ActionCommand(SaveNodeConfigurationCallback, false);
            _takeConnector = new ActionCommand(TakeConnectorCallback, false);
            _saveAdapter = new ActionCommand(SaveAdapterCallback, false);
            _saveAdapterConfiguration = new ActionCommand(SaveAdapterConfigurationCallback, false);

            _newCommand = new ActionCommand(NewCommandCallback);
            _loadCommand = new ActionCommand(LoadCommandCallback);
            _saveCommand = new ActionCommand(SaveCommandCallback);

            var repository = new Repository();
            _runtime = new Runtime(repository);

            _runtime.NodeAdded += RuntimeOnOnNodeAdded;
            _runtime.NodeRemoved += RuntimeOnOnNodeRemoved;
            _runtime.NodeUpdated += RuntimeOnOnNodeUpdated;
            _runtime.NodeStateChanged += RuntimeOnOnNodeStateChanged;
            _runtime.NodeEnvironmentChanged += RuntimeOnOnNodeEnvironmentChanged;
            _runtime.WireAdded += RuntimeOnOnWireAdded;
            _runtime.WireRemoved += RuntimeOnOnWireRemoved;
            _runtime.WireUpdated += RuntimeOnWireUpdated;

            Task.Run(() => Setup(CancellationToken.None));
        }

        private async void SaveCommandCallback(object obj)
        {
            var dialog = new SaveFileDialog
            {
                Filter = "OctoPatch Grid|*.grid"
            };

            if (dialog.ShowDialog() == true)
            {
                var grid = await _runtime.GetConfiguration(CancellationToken.None);
                var output = JsonConvert.SerializeObject(grid);
                await File.WriteAllTextAsync(dialog.FileName, output);
            }
        }

        private async void LoadCommandCallback(object obj)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "OctoPatch Grid|*.grid"
            };

            if (dialog.ShowDialog() == true)
            {
                var input = await File.ReadAllTextAsync(dialog.FileName, CancellationToken.None);
                var grid = JsonConvert.DeserializeObject<GridSetup>(input);
                await _runtime.SetConfiguration(grid, CancellationToken.None);
            }
        }

        private async void NewCommandCallback(object obj)
        {
            await _runtime.SetConfiguration(null, CancellationToken.None);
        }

        private void RuntimeOnWireUpdated(WireSetup obj)
        {
            // Nothing to do yet
        }

        private void SaveAdapterConfigurationCallback(object obj)
        {
            throw new NotImplementedException();
        }

        private void SaveAdapterCallback(object obj)
        {
            throw new NotImplementedException();
        }

        private async void RemoveSelectedWireCallback(object obj)
        {
            var node = SelectedNode;
            var item = _wires.FirstOrDefault(n => n.InputWire == node || n.OutputWire == node);
            if (item == null)
            {
                return;
            }

            await _runtime.RemoveWire(item.Setup.WireId, CancellationToken.None);
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
                await _runtime.AddNode(contextNode.Key, parentId, connectorKey, 0, 0, CancellationToken.None);
            }
            else if (contextNode is CollectorNodeDescription)
            {
                await _runtime.AddNode(contextNode.Key, parentId, connectorKey, 0, 0, CancellationToken.None);
            }
            else if (contextNode is AttachedNodeDescription)
            {
                await _runtime.AddNode(contextNode.Key, parentId, null, 0, 0, CancellationToken.None);
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
            node.Model.Name = setup.Name;
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
                await _runtime.AddNode(description.Key, null, null, 0, 0, CancellationToken.None);
            }
        }

        private void RuntimeOnOnWireRemoved(Guid wireId)
        {
            var wire = _wires.FirstOrDefault(n => n.Setup.WireId == wireId);
            if (wire == null)
            {
                return;
            }

            // Remove node in tree
            RemoveRecursive(wire.InputWire, NodeTree);
            RemoveRecursive(wire.OutputWire, NodeTree);

            _wires.Remove(wire);
        }

        private void RuntimeOnOnWireAdded(WireSetup wire)
        {
            var inputNode = _nodes.First(n => n.Setup.NodeId == wire.InputNodeId);
            var inputConnector = inputNode.Model.Items.OfType<InputNodeModel>()
                .First(m => m.Key == wire.InputConnectorKey);

            var outputNode = _nodes.First(n => n.Setup.NodeId == wire.OutputNodeId);
            var outputConnector = outputNode.Model.Items.OfType<OutputNodeModel>()
                .First(m => m.Key == wire.OutputConnectorKey);

            var inputWire = new WireNodeModel(wire.WireId, $"Wire to {outputNode.Model.Name} ({outputConnector.Name})");
            inputConnector.Items.Add(inputWire);

            var outputWire = new WireNodeModel(wire.WireId, $"Wire to {inputNode.Model.Name} ({inputConnector.Name})");
            outputConnector.Items.Add(outputWire);

            _wires.Add(new WireItem
            {
                InputConnector = inputConnector,
                InputWire = inputWire,
                OutputConnector = outputConnector,
                OutputWire = outputWire,
                Setup = wire
            });
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
            var description = _nodeDescriptions.FirstOrDefault(d => d.Key == setup.Key);

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

                    attachedParent.Model.Items.Add(nodeModel);
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

                    input.Items.Add(nodeModel);
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

                    output.Items.Add(nodeModel);

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
            var nodeDescriptions = await _runtime.GetNodeDescriptions(cancellationToken);
            var adapterDescriptions = await _runtime.GetAdapterDescriptions(cancellationToken);

            _nodeDescriptions.Clear();
            _nodeDescriptions.AddRange(nodeDescriptions);

            foreach (var description in _nodeDescriptions.OfType<CommonNodeDescription>())
            {
                NodeDescriptions.Add(description);
            }

            _adapterDescriptions.Clear();
            _adapterDescriptions.AddRange(adapterDescriptions);
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

        private sealed class WireItem
        {
            public InputNodeModel InputConnector { get; set; }

            public OutputNodeModel OutputConnector { get; set; }

            public WireNodeModel InputWire { get; set; }

            public WireNodeModel OutputWire { get; set; }

            public WireSetup Setup { get; set; }
        }
    }
}
