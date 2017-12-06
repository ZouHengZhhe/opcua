// Copyright (c) 2014 Converter Systems LLC

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Model;

namespace ConverterSystems.Ua.Design
{
    /// <summary>
    /// Interaction logic for UaSubscriptionEditorDialog.xaml
    /// </summary>
    [ToolboxBrowsable(false)]
    public partial class UaSubscriptionEditorDialog
    {
        private TreeViewItem _startDragItem;

        public UaSubscriptionEditorDialog(ModelItem modelItem)
        {
            DataContext = new UaSubscriptionViewModel(modelItem);
            Loaded += OnLoaded;
            Closed += OnClosed;
            InitializeComponent();
        }

        public UaSubscriptionViewModel Subscription
        {
            get { return (UaSubscriptionViewModel) DataContext; }
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            Subscription.BeginEdit();
        }

        private void OnClosed(object sender, EventArgs e)
        {
            if (DialogResult != null && DialogResult.Value)
            {
                Subscription.EndEdit();
            }
            else
            {
                Subscription.CancelEdit();
            }
        }

        private void ButtonOkClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void ButtonCancelClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void OnTreeMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _startDragItem = FindAncestor<TreeViewItem>((DependencyObject) e.OriginalSource);
        }

        private void OnTreeMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var item = FindAncestor<TreeViewItem>((DependencyObject) e.OriginalSource);
                if (item != null && item.Equals(_startDragItem))
                {
                    var vm = item.DataContext as ReferenceDescriptionViewModel;
                    if (vm != null && vm.IsSelected && ((vm.IsVariable && SubscriptionTabControl.SelectedIndex < 3) || (vm.IsMethod && SubscriptionTabControl.SelectedIndex == 3)))
                    {
                        DragDrop.DoDragDrop(item, vm, DragDropEffects.Copy);
                        _startDragItem = null;
                        e.Handled = true;
                    }
                }
            }
        }

        private void AddCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (NamespaceTree != null)
            {
                var vm = NamespaceTree.SelectedItem as ReferenceDescriptionViewModel;
                e.CanExecute = vm != null && ((vm.IsVariable && SubscriptionTabControl.SelectedIndex < 3) || (vm.IsMethod && SubscriptionTabControl.SelectedIndex == 3));
            }
            e.Handled = true;
        }

        private async void AddExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (NamespaceTree != null)
            {
                var vm = NamespaceTree.SelectedItem as ReferenceDescriptionViewModel;
                if (vm != null && (vm.IsVariable || vm.IsMethod))
                {
                    ModelItem item;
                    switch (SubscriptionTabControl.SelectedIndex)
                    {
                        case 0:
                            item = await Subscription.AddProperty(vm);
                            if (item != null)
                            {
                                PropertiesGrid.Items.MoveCurrentTo(item);
                                PropertiesGrid.ScrollIntoView(item);
                                PropertiesGrid.Focus();
                            }
                            break;
                        case 1:
                            item = await Subscription.AddCommand(vm);
                            if (item != null)
                            {
                                CommandsGrid.Items.MoveCurrentTo(item);
                                CommandsGrid.ScrollIntoView(item);
                                CommandsGrid.Focus();
                            }
                            break;
                        case 2:
                            item = await Subscription.AddDataSource(vm);
                            if (item != null)
                            {
                                DataSourcesGrid.Items.MoveCurrentTo(item);
                                DataSourcesGrid.ScrollIntoView(item);
                                DataSourcesGrid.Focus();
                            }
                            break;
                        case 3:
                            item = Subscription.AddMethod(vm);
                            if (item != null)
                            {
                                MethodsGrid.Items.MoveCurrentTo(item);
                                MethodsGrid.ScrollIntoView(item);
                                MethodsGrid.Focus();
                            }
                            break;
                    }
                }
                e.Handled = true;
            }
        }

        private async void OnPropertiesGridDrop(object sender, DragEventArgs e)
        {
            var vm = e.Data.GetData(typeof (ReferenceDescriptionViewModel)) as ReferenceDescriptionViewModel;
            if (vm != null)
            {
                var item = await Subscription.AddProperty(vm);
                if (item != null)
                {
                    PropertiesGrid.Items.MoveCurrentTo(item);
                    PropertiesGrid.ScrollIntoView(item);
                    PropertiesGrid.Focus();
                }
            }
            e.Handled = true;
        }

        private async void OnCommandsGridDrop(object sender, DragEventArgs e)
        {
            var vm = e.Data.GetData(typeof (ReferenceDescriptionViewModel)) as ReferenceDescriptionViewModel;
            if (vm != null)
            {
                var item = await Subscription.AddCommand(vm);
                if (item != null)
                {
                    CommandsGrid.Items.MoveCurrentTo(item);
                    CommandsGrid.ScrollIntoView(item);
                    CommandsGrid.Focus();
                }
            }
            e.Handled = true;
        }

        private async void OnDataSourcesGridDrop(object sender, DragEventArgs e)
        {
            var vm = e.Data.GetData(typeof (ReferenceDescriptionViewModel)) as ReferenceDescriptionViewModel;
            if (vm != null)
            {
                var item = await Subscription.AddDataSource(vm);
                if (item != null)
                {
                    DataSourcesGrid.Items.MoveCurrentTo(item);
                    DataSourcesGrid.ScrollIntoView(item);
                    DataSourcesGrid.Focus();
                }
            }
            e.Handled = true;
        }

        private void OnMethodsGridDrop(object sender, DragEventArgs e)
        {
            var vm = e.Data.GetData(typeof (ReferenceDescriptionViewModel)) as ReferenceDescriptionViewModel;
            if (vm != null)
            {
                var item = Subscription.AddMethod(vm);
                if (item != null)
                {
                    MethodsGrid.Items.MoveCurrentTo(item);
                    MethodsGrid.ScrollIntoView(item);
                    MethodsGrid.Focus();
                }
            }
            e.Handled = true;
        }

        private static T FindAncestor<T>(DependencyObject dependencyObject) where T : class
        {
            var target = dependencyObject;
            do
            {
                target = VisualTreeHelper.GetParent(target);
            } while (target != null && !(target is T));
            return target as T;
        }
    }
}