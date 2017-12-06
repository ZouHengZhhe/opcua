// Copyright (c) 2014 Converter Systems LLC

using System;
using System.Threading.Tasks;
using Opc.Ua;

namespace ConverterSystems.Ua.Design
{
    public class ReferenceDescriptionViewModel : TreeViewItemViewModel
    {
        private readonly ReferenceDescription _description;
        private readonly Func<ReferenceDescriptionViewModel, Task> _loadChildren;

        public ReferenceDescriptionViewModel(ReferenceDescription description, TreeViewItemViewModel parentViewModel, Func<ReferenceDescriptionViewModel, Task> loadChildren) : base(parentViewModel, true)
        {
            _description = description;
            _loadChildren = loadChildren;
        }

        public new ReferenceDescriptionViewModel Parent
        {
            get { return base.Parent as ReferenceDescriptionViewModel; }
        }

        public string BrowseName
        {
            get { return _description.BrowseName.ToString(); }
        }

        public string DisplayName
        {
            get { return _description.DisplayName.ToString(); }
        }

        public ExpandedNodeId NodeId
        {
            get { return _description.NodeId; }
        }

        public NodeClass NodeClass
        {
            get { return _description.NodeClass; }
        }

        public bool IsVariable
        {
            get { return _description.NodeClass.HasFlag(NodeClass.Variable); }
        }

        public bool IsMethod
        {
            get { return _description.NodeClass.HasFlag(NodeClass.Method); }
        }

        public bool CanAdd
        {
            get { return IsSelected && (IsMethod || IsVariable); }
        }

        /// <summary>
        /// Invoked when the child items need to be loaded on demand.
        /// </summary>
        protected override async Task LoadChildrenAsync()
        {
            await _loadChildren(this);
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == "IsSelected")
            {
                base.OnPropertyChanged("CanAdd");
            }
        }
    }
}