// Copyright (c) 2014 Converter Systems LLC

using System.Windows;
using Microsoft.Windows.Design.PropertyEditing;

namespace ConverterSystems.Ua.Design
{
    internal class UaSubscriptionEditor : CategoryEditor
    {
        private DataTemplate _dataTemplate;

        public override string TargetCategory
        {
            get { return "Common"; }
        }

        public override DataTemplate EditorTemplate
        {
            get { return _dataTemplate ?? (_dataTemplate = new DataTemplate { VisualTree = new FrameworkElementFactory(typeof (UaSubscriptionEditorButton)) }); }
        }

        public override bool ConsumesProperty(PropertyEntry property)
        {
            return property.PropertyName == "Properties" || property.PropertyName == "Commands" || property.PropertyName == "DataSources" || property.PropertyName == "Methods";
        }

        public override object GetImage(Size desiredSize)
        {
            return null;
        }
    }
}