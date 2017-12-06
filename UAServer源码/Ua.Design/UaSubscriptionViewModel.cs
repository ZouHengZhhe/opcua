// Copyright (c) 2014 Converter Systems LLC

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ConverterSystems.Workstation.Services;
using Microsoft.Windows.Design.Model;
using Opc.Ua;

namespace ConverterSystems.Ua.Design
{
    public class UaSubscriptionViewModel : INotifyPropertyChanged
    {
        private readonly ModelItem _modelItem;
        private ModelEditingScope _scope;
        private UaSession _session;

        public UaSubscriptionViewModel(ModelItem modelItem)
        {
            _modelItem = modelItem;
            Properties = new ObservableModelItemCollection(_modelItem.Properties["Properties"].Collection);
            Commands = new ObservableModelItemCollection(_modelItem.Properties["Commands"].Collection);
            DataSources = new ObservableModelItemCollection(_modelItem.Properties["DataSources"].Collection);
            Methods = new ObservableModelItemCollection(_modelItem.Properties["Methods"].Collection);
            NamespaceItems = new ObservableCollection<ReferenceDescriptionViewModel>();
        }

        public string DisplayName
        {
            get { return (string) _modelItem.Properties["DisplayName"].ComputedValue; }
            set
            {
                _modelItem.Properties["DisplayName"].ComputedValue = value;
                NotifyPropertyChanged();
            }
        }

        public UaSession Session
        {
            get { return (UaSession) _modelItem.Properties["Session"].ComputedValue; }
        }

        public int PublishingInterval
        {
            get { return (int) _modelItem.Properties["PublishingInterval"].ComputedValue; }
            set
            {
                _modelItem.Properties["PublishingInterval"].ComputedValue = value;
                NotifyPropertyChanged();
            }
        }

        public ObservableModelItemCollection Properties { get; private set; }

        public ObservableModelItemCollection Commands { get; private set; }

        public ObservableModelItemCollection DataSources { get; private set; }

        public ObservableModelItemCollection Methods { get; private set; }

        public ObservableCollection<ReferenceDescriptionViewModel> NamespaceItems { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void BeginEdit()
        {
            try
            {
                _scope = _modelItem.BeginEdit();
                _session = new UaSession { EndpointUrl = Session.EndpointUrl };
                if (_session != null)
                {
                    try
                    {
                        NamespaceItems.Clear();
                        var root = new ReferenceDescriptionViewModel(new ReferenceDescription { DisplayName = "Objects", NodeId = new NodeId(Objects.ObjectsFolder, 0) }, null, LoadChildren);
                        NamespaceItems.Add(root);
                        root.IsExpanded = true;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public void EndEdit()
        {
            try
            {
                _scope.Complete();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                _scope.Dispose();
                _scope = null;
                if (_session != null)
                {
                    _session.Close();
                    _session = null;
                }
            }
        }

        public void CancelEdit()
        {
            try
            {
                _scope.Revert();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                _scope.Dispose();
                _scope = null;
                if (_session != null)
                {
                    _session.Close();
                    _session = null;
                }
            }
        }

        public async Task<ModelItem> AddProperty(ReferenceDescriptionViewModel vm)
        {
            if (_session != null && _session.Connected)
            {
                var nodeId = ExpandedNodeId.ToNodeId(vm.NodeId, _session.NamespaceUris);
                var type = await _session.GetDataTypeFromNodeId(nodeId);
                var name = string.Concat(vm.Parent.DisplayName, vm.DisplayName).Replace(" ", "");
                var regex = new Regex(@"[^\p{Ll}\p{Lu}\p{Lt}\p{Lo}\p{Nd}\p{Nl}\p{Mn}\p{Mc}\p{Cf}\p{Pc}\p{Lm}]");
                name = regex.Replace(name, "_");
                if (!char.IsLetter(name, 0))
                {
                    name = string.Concat("_", name);
                }
                var list2 = Properties.Select(mi => (string) mi.Properties["DisplayName"].ComputedValue).Concat(Commands.Select(mi => (string) mi.Properties["DisplayName"].ComputedValue)).Concat(DataSources.Select(mi => (string) mi.Properties["DisplayName"].ComputedValue)).ToList();
                while (list2.Contains(name))
                {
                    name = string.Concat(name, "_2");
                }
                var item = ModelFactory.CreateItem(_modelItem.Context, new UaItem { DisplayName = name, StartNodeId = nodeId, Type = type, NodeClass = vm.NodeClass });
                Properties.Add(item);
                return item;
            }
            return null;
        }

        public async Task<ModelItem> AddCommand(ReferenceDescriptionViewModel vm)
        {
            if (_session != null && _session.Connected)
            {
                var nodeId = ExpandedNodeId.ToNodeId(vm.NodeId, _session.NamespaceUris);
                var name = string.Concat(vm.Parent.DisplayName, vm.DisplayName, "Command").Replace(" ", "");
                var regex = new Regex(@"[^\p{Ll}\p{Lu}\p{Lt}\p{Lo}\p{Nd}\p{Nl}\p{Mn}\p{Mc}\p{Cf}\p{Pc}\p{Lm}]");
                name = regex.Replace(name, "_");
                if (!char.IsLetter(name, 0))
                {
                    name = string.Concat("_", name);
                }
                var list2 = Properties.Select(mi => (string) mi.Properties["DisplayName"].ComputedValue).Concat(Commands.Select(mi => (string) mi.Properties["DisplayName"].ComputedValue)).Concat(DataSources.Select(mi => (string) mi.Properties["DisplayName"].ComputedValue)).ToList();
                while (list2.Contains(name))
                {
                    name = string.Concat(name, "_2");
                }
                var type = await _session.GetDataTypeFromNodeId(nodeId);
                var item = ModelFactory.CreateItem(_modelItem.Context, new UaItem { DisplayName = name, StartNodeId = nodeId, Type = type, NodeClass = vm.NodeClass });
                Commands.Add(item);
                return item;
            }
            return null;
        }

        public async Task<ModelItem> AddDataSource(ReferenceDescriptionViewModel vm)
        {
            if (_session != null && _session.Connected)
            {
                var nodeId = ExpandedNodeId.ToNodeId(vm.NodeId, _session.NamespaceUris);
                var type = await _session.GetDataTypeFromNodeId(nodeId);
                var name = string.Concat(vm.Parent.DisplayName, vm.DisplayName, "Source").Replace(" ", "");
                var regex = new Regex(@"[^\p{Ll}\p{Lu}\p{Lt}\p{Lo}\p{Nd}\p{Nl}\p{Mn}\p{Mc}\p{Cf}\p{Pc}\p{Lm}]");
                name = regex.Replace(name, "_");
                if (!char.IsLetter(name, 0))
                {
                    name = string.Concat("_", name);
                }
                var list2 = Properties.Select(mi => (string) mi.Properties["DisplayName"].ComputedValue).Concat(Commands.Select(mi => (string) mi.Properties["DisplayName"].ComputedValue)).Concat(DataSources.Select(mi => (string) mi.Properties["DisplayName"].ComputedValue)).ToList();
                while (list2.Contains(name))
                {
                    name = string.Concat(name, "_2");
                }
                var item = ModelFactory.CreateItem(_modelItem.Context, new UaItem { DisplayName = name, StartNodeId = nodeId, Type = type, NodeClass = vm.NodeClass, CacheQueueSize = 240 });
                DataSources.Add(item);
                return item;
            }
            return null;
        }

        public ModelItem AddMethod(ReferenceDescriptionViewModel vm)
        {
            if (_session != null && _session.Connected)
            {
                var name = string.Concat(vm.Parent.DisplayName, vm.DisplayName, "Method").Replace(" ", "");
                var regex = new Regex(@"[^\p{Ll}\p{Lu}\p{Lt}\p{Lo}\p{Nd}\p{Nl}\p{Mn}\p{Mc}\p{Cf}\p{Pc}\p{Lm}]");
                name = regex.Replace(name, "_");
                if (!char.IsLetter(name, 0))
                {
                    name = string.Concat("_", name);
                }
                var list2 = Methods.Select(mi => (string) mi.Properties["DisplayName"].ComputedValue).ToList();
                while (list2.Contains(name))
                {
                    name = string.Concat(name, "_2");
                }
                var type = typeof (bool);
                var item = ModelFactory.CreateItem(_modelItem.Context, new UaItem { DisplayName = name, StartNodeId = ExpandedNodeId.ToNodeId(vm.Parent.NodeId, _session.NamespaceUris), Type = type, NodeClass = vm.NodeClass, RelativePath = vm.BrowseName });
                Methods.Add(item);
                return item;
            }
            return null;
        }

        private async Task LoadChildren(ReferenceDescriptionViewModel viewModel)
        {
            if (_session == null || _session.Disposed)
            {
                return;
            }
            try
            {
                if (!_session.Connected)
                {
                    await _session.ConnectAsync();
                }
                var browseRequest = new BrowseRequest { NodesToBrowse = { new BrowseDescription { NodeId = ExpandedNodeId.ToNodeId(viewModel.NodeId, _session.NamespaceUris), ReferenceTypeId = ReferenceTypeIds.HierarchicalReferences, ResultMask = (uint) BrowseResultMask.All, NodeClassMask = (uint) NodeClass.Variable | (uint) NodeClass.Object | (uint) NodeClass.Method, BrowseDirection = BrowseDirection.Forward } } };
                var browseResponse = await _session.BrowseAsync(browseRequest);
                foreach (var description in browseResponse.Results.SelectMany(result => result.References))
                {
                    viewModel.Children.Add(new ReferenceDescriptionViewModel(description, viewModel, LoadChildren));
                    await Task.Delay(50);
                }
                var continuationPoints = new ByteStringCollection(browseResponse.Results.Select(br => br.ContinuationPoint).Where(cp => null != cp));
                while (continuationPoints.Count > 0)
                {
                    var browseNextRequest = new BrowseNextRequest { ContinuationPoints = continuationPoints, ReleaseContinuationPoints = false };
                    var browseNextResponse = await _session.BrowseNextAsync(browseNextRequest);
                    foreach (var description in browseNextResponse.Results.SelectMany(result => result.References))
                    {
                        viewModel.Children.Add(new ReferenceDescriptionViewModel(description, viewModel, LoadChildren));
                        await Task.Delay(50);
                    }
                    continuationPoints = new ByteStringCollection(browseNextResponse.Results.Select(br => br.ContinuationPoint).Where(cp => null != cp));
                }
            }
            catch (OperationCanceledException)
            {
            }
        }
    }

    internal static class UaSessionExtensions
    {
        internal static async Task<Type> GetDataTypeFromNodeId(this UaSession session, NodeId nodeId)
        {
            var readRequest = new ReadRequest { NodesToRead = { new ReadValueId { NodeId = nodeId, AttributeId = Attributes.DataType }, new ReadValueId { NodeId = nodeId, AttributeId = Attributes.ValueRank } } };
            var readResponse = await session.ReadAsync(readRequest);
            var dataType = readResponse.Results[0].GetValue(NodeId.Null);
            var valueRank = readResponse.Results[1].GetValue(-2);
            var type = TypeInfo.GetSystemType(dataType, session.Factory);
            if (type != null)
            {
                if (valueRank > 0)
                {
                    return type.MakeArrayType(valueRank);
                }
                return type;
            }
            return typeof (object);
        }

        internal static async Task<Tuple<Argument[], Argument[]>> GetMethodArgumentsFromNodeId(this UaSession session, NodeId nodeId)
        {
            var inputArguments = new Argument[0];
            var outputArguments = new Argument[0];
            var translateRequest = new TranslateBrowsePathsToNodeIdsRequest { BrowsePaths = { new BrowsePath { StartingNode = nodeId, RelativePath = new RelativePath(ReferenceTypeIds.HasProperty, "InputArguments") }, new BrowsePath { StartingNode = nodeId, RelativePath = new RelativePath(ReferenceTypeIds.HasProperty, "OutputArguments") } } };
            var translateResponse = await session.TranslateBrowsePathsToNodeIdsAsync(translateRequest);
            if (StatusCode.IsGood(translateResponse.Results[0].StatusCode) && translateResponse.Results[0].Targets.Count > 0)
            {
                var argNodeId = ExpandedNodeId.ToNodeId(translateResponse.Results[0].Targets[0].TargetId, session.NamespaceUris);
                var readRequest = new ReadRequest { NodesToRead = { new ReadValueId { NodeId = argNodeId, AttributeId = Attributes.Value } } };
                var readResponse = await session.ReadAsync(readRequest);
                if (StatusCode.IsGood(readResponse.Results[0].StatusCode))
                {
                    var value = readResponse.Results[0].GetValue<ExtensionObject[]>(null);
                    if (value != null)
                    {
                        inputArguments = (Argument[]) ExtensionObject.ToArray(value, typeof (Argument));
                    }
                }
            }
            if (StatusCode.IsGood(translateResponse.Results[1].StatusCode) && translateResponse.Results[1].Targets.Count > 0)
            {
                var argNodeId = ExpandedNodeId.ToNodeId(translateResponse.Results[1].Targets[0].TargetId, session.NamespaceUris);
                var readRequest = new ReadRequest { NodesToRead = { new ReadValueId { NodeId = argNodeId, AttributeId = Attributes.Value } } };
                var readResponse = await session.ReadAsync(readRequest);
                if (StatusCode.IsGood(readResponse.Results[0].StatusCode))
                {
                    var value = readResponse.Results[0].GetValue<ExtensionObject[]>(null);
                    if (value != null)
                    {
                        outputArguments = (Argument[]) ExtensionObject.ToArray(value, typeof (Argument));
                    }
                }
            }
            return Tuple.Create(inputArguments, outputArguments);
        }
    }
}