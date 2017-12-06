// Copyright (c) 2014 Converter Systems LLC

using System;
using System.ComponentModel;
using ConverterSystems.Ua.Design;
using ConverterSystems.Workstation.Services;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.PropertyEditing;
using Opc.Ua;
using Opc.Ua.Client;

[assembly: ProvideMetadata(typeof (Metadata))]

namespace ConverterSystems.Ua.Design
{
    public class Metadata : IProvideAttributeTable
    {
        private static readonly Type UaSubscriptionType = typeof (UaSubscription);
        private static readonly Type SubscriptionType = typeof (Subscription);

        #region IProvideAttributeTable Members

        public AttributeTable AttributeTable
        {
            get
            {
                var builder = new AttributeTableBuilder();
                builder.AddCustomAttributes(UaSubscriptionType, CategoryEditor.CreateEditorAttribute(typeof (UaSubscriptionEditor)));
                builder.AddCustomAttributes(SubscriptionType, "DefaultItem", new EditorBrowsableAttribute(EditorBrowsableState.Advanced));
                builder.AddCustomAttributes(SubscriptionType, "DisableMonitoredItemCache", new EditorBrowsableAttribute(EditorBrowsableState.Advanced));
                builder.AddCustomAttributes(SubscriptionType, "FastDataChangeCallback", new EditorBrowsableAttribute(EditorBrowsableState.Advanced));
                builder.AddCustomAttributes(SubscriptionType, "FastEventCallback", new EditorBrowsableAttribute(EditorBrowsableState.Advanced));
                builder.AddCustomAttributes(SubscriptionType, "Handle", new EditorBrowsableAttribute(EditorBrowsableState.Advanced));
                builder.AddCustomAttributes(SubscriptionType, "KeepAliveCount", new EditorBrowsableAttribute(EditorBrowsableState.Advanced));
                builder.AddCustomAttributes(SubscriptionType, "LifetimeCount", new EditorBrowsableAttribute(EditorBrowsableState.Advanced));
                builder.AddCustomAttributes(SubscriptionType, "MaxMessageCount", new EditorBrowsableAttribute(EditorBrowsableState.Advanced));
                builder.AddCustomAttributes(SubscriptionType, "MaxNotificationsPerPublish", new EditorBrowsableAttribute(EditorBrowsableState.Advanced));
                builder.AddCustomAttributes(SubscriptionType, "MinLifetimeInterval", new EditorBrowsableAttribute(EditorBrowsableState.Advanced));
                builder.AddCustomAttributes(SubscriptionType, "Priority", new EditorBrowsableAttribute(EditorBrowsableState.Advanced));
                builder.AddCustomAttributes(SubscriptionType, "PublishingEnabled", new EditorBrowsableAttribute(EditorBrowsableState.Advanced));
                builder.AddCustomAttributes(SubscriptionType, "TimestampsToReturn", new DefaultValueAttribute(TimestampsToReturn.Both), new EditorBrowsableAttribute(EditorBrowsableState.Advanced));
                return builder.CreateTable();
            }
        }

        #endregion
    }
}