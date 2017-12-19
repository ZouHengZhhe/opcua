using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using UnityEngine;

public class MQTTClientRecv : MonoBehaviour
{
    private MqttClient client;

    private void Start()
    {
        client = new MqttClient(IPAddress.Parse("127.0.0.1"));
        string clientId = Guid.NewGuid().ToString();
        try
        {
            client.Connect(clientId);
        }
        catch
        {
            print("客户端与服务器端连接失败！");
        }
        client.MqttMsgPublishReceived += OnReceive;
        client.Subscribe(new string[] { "opcua" }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
    }

    private void OnReceive(object sender, MqttMsgPublishEventArgs e)
    {
        print(Encoding.UTF8.GetString(e.Message));
    }

    private void OnDestroy()
    {
        client.Disconnect();
    }
}