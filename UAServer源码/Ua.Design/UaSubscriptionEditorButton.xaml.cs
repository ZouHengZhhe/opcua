// Copyright (c) 2014 Converter Systems LLC

using System;
using System.Linq;
using System.Windows;
using ConverterSystems.Workstation.Services;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.PropertyEditing;

namespace ConverterSystems.Ua.Design
{
    [ToolboxBrowsable(false)]
    public partial class UaSubscriptionEditorButton
    {
        public UaSubscriptionEditorButton()
        {
            InitializeComponent();
        }

        private void BrowseClick(object sender, RoutedEventArgs e)
        {
            var categoryEntry = DataContext as CategoryEntry;
            if (categoryEntry != null)
            {
                try
                {
                    var modelItem = categoryEntry.Properties.First().ModelProperties.First().Parent;
                    if (modelItem != null)
                    {
                        var session = modelItem.Properties["Session"].ComputedValue as UaSession;
                        if (session != null)
                        {
                            using (var scope = modelItem.BeginEdit())
                            {
                                var dialog = new UaSubscriptionEditorDialog(modelItem);
                                if (dialog.ShowDialog() == true)
                                {
                                    scope.Complete();
                                }
                                else
                                {
                                    scope.Revert();
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Set Session before action.");
                        }
                    }
                }
                catch (ArgumentNullException)
                {
                }
                catch (InvalidOperationException)
                {
                }
            }
        }
    }
}