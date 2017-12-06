// Copyright (c) 2014 Converter Systems LLC

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Opc.Ua;
using Opc.Ua.Export;
using Opc.Ua.Server;

namespace ConverterSystems.Test
{
    public class TestNodeManager : CustomNodeManager2
    {
        private BaseDataVariableState _axis1;
        private BaseDataVariableState _axis2;
        private BaseDataVariableState _axis3;
        private BaseDataVariableState _axis4;
        private BaseDataVariableState _laser;
        private double _masterAxis;
        private BaseDataVariableState _mode;
        private NodeState _robot1;
        private BaseDataVariableState _speed;
        private Timer _timer;
        private DateTime _timestamp;

        public TestNodeManager(IServerInternal server, ApplicationConfiguration configuration) : base(server, configuration)
        {
            SetNamespaces("http://opcuaservicesforwpf.codeplex.com/test/");
        }

        public override void CreateAddressSpace(IDictionary<NodeId, IList<IReference>> externalReferences)
        {
            lock (Lock)
            {
                base.CreateAddressSpace(externalReferences);
                _robot1 = FindPredefinedNode(new NodeId("Robot1", NamespaceIndex), typeof (NodeState));
                if (_robot1 != null)
                {
                    _axis1 = FindPredefinedNode(new NodeId("Robot1_Axis1", NamespaceIndex), typeof (BaseDataVariableState)) as BaseDataVariableState;
                    _axis2 = FindPredefinedNode(new NodeId("Robot1_Axis2", NamespaceIndex), typeof (BaseDataVariableState)) as BaseDataVariableState;
                    _axis3 = FindPredefinedNode(new NodeId("Robot1_Axis3", NamespaceIndex), typeof (BaseDataVariableState)) as BaseDataVariableState;
                    _axis4 = FindPredefinedNode(new NodeId("Robot1_Axis4", NamespaceIndex), typeof (BaseDataVariableState)) as BaseDataVariableState;
                    _mode = FindPredefinedNode(new NodeId("Robot1_Mode", NamespaceIndex), typeof (BaseDataVariableState)) as BaseDataVariableState;
                    _speed = FindPredefinedNode(new NodeId("Robot1_Speed", NamespaceIndex), typeof (BaseDataVariableState)) as BaseDataVariableState;
                    _laser = FindPredefinedNode(new NodeId("Robot1_Laser", NamespaceIndex), typeof (BaseDataVariableState)) as BaseDataVariableState;
                    _timestamp = DateTime.UtcNow;
                    _timer = new Timer(OnScan, null, 250, 250);
                }
            }
        }

        protected override NodeStateCollection LoadPredefinedNodes(ISystemContext context)
        {
            var nodeStateCollection = new NodeStateCollection();
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ConverterSystems.Test.PredefinedNodes.xml"))
            {
                var nodeset = UANodeSet.Read(stream);
                nodeset.Import(context, nodeStateCollection);
            }
            return nodeStateCollection;
        }

        public override void DeleteAddressSpace()
        {
            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
            }
        }

        private void OnScan(object state)
        {
            var dt = DateTime.UtcNow - _timestamp;
            _timestamp = DateTime.UtcNow;

            if ((short) _mode.Value != 2) // if not in man mode
            {
                double period;
                switch ((short) _speed.Value)
                {
                    case 1:
                        period = 20.0;
                        break;
                    case 2:
                        period = 10.0;
                        break;
                    case 3:
                        period = 5.0;
                        break;
                    default:
                        period = 30.0;
                        break;
                }
                if ((short) _mode.Value == 0) // auto mode
                {
                    _masterAxis = (_masterAxis + dt.TotalSeconds/period)%1.0; // 0.0 to 1.0
                }
                _axis1.Value = (float) (Math.Sin(_masterAxis*2.0*Math.PI)*45.0);
                _axis2.Value = (float) (Math.Cos(_masterAxis*2.0*Math.PI)*45.0);
                _axis3.Value = (float) (Math.Sin(((_masterAxis*2.0)%1.0)*2.0*Math.PI)*45.0);
                _axis4.Value = (float) (Math.Cos(_masterAxis*2.0*Math.PI)*-180.0);
            }
            _axis1.Timestamp = _timestamp;
            _axis1.ClearChangeMasks(SystemContext, false);
            _axis2.Timestamp = _timestamp;
            _axis2.ClearChangeMasks(SystemContext, false);
            _axis3.Timestamp = _timestamp;
            _axis3.ClearChangeMasks(SystemContext, false);
            _axis4.Timestamp = _timestamp;
            _axis4.ClearChangeMasks(SystemContext, false);
        }

        protected override ServiceResult Call(ISystemContext context, CallMethodRequest methodToCall, MethodState method, CallMethodResult result)
        {
            if (methodToCall.MethodId == new NodeId("Robot1_Stop", NamespaceIndex))
            {
                _mode.Value = (short) 1;
                _mode.Timestamp = DateTime.UtcNow;
                _mode.ClearChangeMasks(SystemContext, false);
                _laser.Value = false;
                _laser.Timestamp = DateTime.UtcNow;
                _laser.ClearChangeMasks(SystemContext, false);
                result.StatusCode = StatusCodes.Good;
                return StatusCodes.Good;
            }
            if (methodToCall.MethodId == new NodeId("Robot1_Multiply", NamespaceIndex))
            {
                try
                {
                    var a = Convert.ToDouble(methodToCall.InputArguments[0].Value);
                    var b = Convert.ToDouble(methodToCall.InputArguments[1].Value);
                    result.OutputArguments.Add(new Variant(a*b));
                    result.StatusCode = StatusCodes.Good;
                    return StatusCodes.Good;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    result.StatusCode = StatusCodes.BadInvalidArgument;
                    return StatusCodes.BadInvalidArgument;
                }
            }
            return base.Call(context, methodToCall, method, result);
        }
    }
}