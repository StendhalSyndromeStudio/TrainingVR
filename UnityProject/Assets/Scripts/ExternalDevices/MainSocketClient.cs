using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System;

public class MainSocketClient  : MonoBehaviour
{
    public delegate void delOutcomingMessage(IExternalDevices sender, string message);
    
    private static GameObject Instance = null;    
    
    private static Dictionary<string, SocketClient> dictionary_SockerClient = new Dictionary<string, SocketClient>();
    private static Dictionary<string, List<IExternalDevices>> dic_Device = new Dictionary<string, List<IExternalDevices>>();
    private static Dictionary<IExternalDevices, SocketClient> dic_DeviceOfSocket = new Dictionary<IExternalDevices, SocketClient>();

    private struct ParametersMessage
    {
        public string id_Sender;
        public string message;
        public AsynchronousClientExample socketClient;
    }

    private static List<ParametersMessage> _list_OutcomingMessage = new List<ParametersMessage>();
    private static object lockerFor_list_OutcomingMessage = new object();
    private static List<ParametersMessage> list_OutcomingMessage
    {
        get { lock (MainSocketClient.lockerFor_list_OutcomingMessage) { return MainSocketClient._list_OutcomingMessage; } }
    }

    private static List<ParametersMessage> _list_IncomingMessage = new List<ParametersMessage>();
    private static object lockerFor_list_IncomingMessage = new object();
    private static List<ParametersMessage> list_IncomingMessage
    {
        get { lock (MainSocketClient.lockerFor_list_IncomingMessage) { return MainSocketClient._list_IncomingMessage; } }
    }

    /// <summary>
    /// Файл с настройками
    /// </summary>
    public string pathSettings = "ExternalDevices.xml";

    void Awake()
    {
        MainSocketClient.Instance = this.gameObject;
        this.pathSettings = System.IO.Path.Combine(Application.streamingAssetsPath, this.pathSettings);
        if (!File.Exists(this.pathSettings))
        {
            Debug.LogError("MainSocketClient: not File.Exists " + this.pathSettings);
            return;
        }
        XmlDocument document = new XmlDocument();
        document.Load(pathSettings);
        foreach (XmlNode node in document.ChildNodes)
        {
            if (node.Name == "settings") MainSocketClient.LoadingSettings(node);
        }        
    }

    void Update()
    {
        //полученное от сервера
        this.ReceivedFromServer();
        //отправляемое на сервера
        this.SentToServer();    
    }

    /// <summary>
    /// отправляемое на сервера
    /// </summary>
    private void SentToServer()
    {
        if (MainSocketClient.list_OutcomingMessage.Count != 0)
        {
            List<ParametersMessage> listTemp = null;
            lock (MainSocketClient.lockerFor_list_OutcomingMessage)
            {
                listTemp = new List<ParametersMessage>(MainSocketClient._list_OutcomingMessage);
                MainSocketClient._list_OutcomingMessage.Clear();
            }

            for (int i = 0; i < listTemp.Count; i++)
            {
                MainSocketClient.SendMessageToSocket(listTemp[i].id_Sender, listTemp[i].message);
            }            
            
        }
    }

    /// <summary>
    /// полученное от сервера
    /// </summary>
    private void ReceivedFromServer()
    {
        if (MainSocketClient.list_IncomingMessage.Count != 0)
        {
            List<ParametersMessage> listTemp = null;
            lock (MainSocketClient.lockerFor_list_IncomingMessage)
            {
                listTemp = new List<ParametersMessage>(MainSocketClient._list_IncomingMessage);
                MainSocketClient._list_IncomingMessage.Clear();
            }

            for (int i = 0; i < listTemp.Count; i++)
            {
                MainSocketClient.Handler_IncomingMessage(listTemp[i].socketClient.Parent_SocketClient, listTemp[i].message);
            }

        }
    }

    public static void LoadingSettings(XmlNode settings)
    {
        if (settings == null) return;
        foreach (XmlNode node in settings.ChildNodes)
        {
            switch (node.Name)
            {
                case "servers": { MainSocketClient.LoadingSettings_servers(node); } break;                
            }
        }
    }

    private static void LoadingSettings_servers(XmlNode xmlNode)
    {
        foreach (XmlNode node in xmlNode.ChildNodes)
        {
            switch (node.Name)
            {
                case "server": { MainSocketClient.Create_SockerClient(node); } break;
            }
        }
    }

    public static void Create_SockerClient(XmlNode node)
    {
        if (node == null) return;
        object[] parameter = MainSocketClient.Setup(node);

        MainSocketClient.Create_SockerClient((string)parameter[0], (int)parameter[1]);
    }

    public static void Create_SockerClient(string server_IP, int server_PORT)
    {   
        object[] parameter = MainSocketClient.Setup(server_IP, server_PORT);

        SocketClient client = MainSocketClient.GET_SockerClient((string)parameter[0], (int)parameter[1]);
        if (client == null)
        {
            int count = MainSocketClient.dictionary_SockerClient.Count + 1;
            string id = "Client_" + count.ToString();
            client = Instance.AddComponent<SocketClient>();
            client.Setup(id, (string)parameter[0], (int)parameter[1]);
            client.enabled = true;
            client.IncomingMessage += MainSocketClient.Adding_IncomingMessage;
            client.OnChangeStatusConnect += MainSocketClient.OnChangeStatusConnect;
            try
            {
                MainSocketClient.dictionary_SockerClient.Add(id, client);
            }
            catch { }
        }        
    }

    public static SocketClient GET_SockerClient(string ip, int port)
    {
        SocketClient result = null;

        foreach (KeyValuePair<string, SocketClient> kvp in MainSocketClient.dictionary_SockerClient)
        {
            if ((kvp.Value.server_IP == ip) && (kvp.Value.server_PORT == port))
            {
                result = kvp.Value;
                break;
            }
        }

        return result;
    }

    public static object[] Setup(XmlNode xmlNode)
    {
        object[] result = new object[] { "", 0, "" };
        foreach (XmlAttribute att in xmlNode.Attributes)
        {
            switch (att.Name)
            {
                case "ip": result[0] = att.Value; break;
                case "port":
                    {
                        try
                        {
                            result[1] = int.Parse(att.Value);
                        }
                        catch { }
                    } break;               
            }
        }

        return result;
    }

    public static object[] Setup(string server_IP, int server_PORT)
    {
        object[] result = new object[] { "", 0, "" };
        result[0] = server_IP;
        result[1] = server_PORT;
        return result;
    }
    
    internal static void SendMessageToSocket(string id, string message)
    {
        try
        {
            SocketClient client = null;
            if (MainSocketClient.dictionary_SockerClient.TryGetValue(id, out client))
            {
                client.SendMessageToServer(message);
            }
            else
            {
                foreach (KeyValuePair<string, SocketClient> kvp in MainSocketClient.dictionary_SockerClient)
                {
                    kvp.Value.SendMessageToServer(message);
                }
            }
        }
        catch { }
    }

    internal static void AddDevice(IExternalDevices device)
    {        
        try
        {
            MainSocketClient.dic_Device.Add(device.ID, new List<IExternalDevices>(){device});
            device.OutcomingMessage += MainSocketClient.Handler_OutcomingMessage;
            if (MainSocketClient.GET_isConnect())
            {
                device.ReStart();
            }
        }
        catch 
        {
            if (!MainSocketClient.dic_Device[device.ID].Contains(device))
            {
                MainSocketClient.dic_Device[device.ID].Add(device);
                device.OutcomingMessage += MainSocketClient.Handler_OutcomingMessage;
                if (MainSocketClient.GET_isConnect())
                {
                    device.ReStart();
                }
            }
        }
    }

    private static bool GET_isConnect()
    {
        bool flag = false;
        foreach (KeyValuePair<string, SocketClient> kvp in MainSocketClient.dictionary_SockerClient)
        {
            if (kvp.Value.AsynchronousClient.isConnect)
            {
                flag = true;
                break;
            }
        }
        return flag;
    }

    private static void Handler_OutcomingMessage(IExternalDevices sender, string message)
    {
        MainSocketClient.list_OutcomingMessage.Add(new ParametersMessage() { id_Sender = MainSocketClient.GET_ID_Socket(sender), message = sender.ID + "{" + message + "}" });
    }

    private static string GET_ID_Socket(IExternalDevices sender)
    {
        SocketClient client = null;
        MainSocketClient.dic_DeviceOfSocket.TryGetValue(sender, out client);
        if (client == null)
        {
            return "";
        }
        else
        {
            return client.identifier;
        }
        
    }

    
    private static void Adding_IncomingMessage(AsynchronousClientExample sender, string message)
    {
        string[] array = message.Split(new string[] { "<EOF>" }, 1024, System.StringSplitOptions.RemoveEmptyEntries);

        lock (MainSocketClient.lockerFor_list_IncomingMessage)
        {
            for (int i = 0; i < array.Length; i++)
            {
                MainSocketClient._list_IncomingMessage.Add(new ParametersMessage() { socketClient = sender, message = array[i]});
            }
        }
        
    }

    private static void Handler_IncomingMessage(SocketClient client, string message)
    {
        try
        {
            int index_Start = message.IndexOf("{");
            int index_End = message.IndexOf("}");
            string id = message.Substring(0, index_Start);
            string value = message.Substring(index_Start + 1, index_End - index_Start - 1);
            for (int i = 0; i < MainSocketClient.dic_Device[id].Count; i++)
            {
                MainSocketClient.dic_Device[id][i].IncomingMessage(value);
                try
                {
                    MainSocketClient.dic_DeviceOfSocket.Add(MainSocketClient.dic_Device[id][i], client);
                }
                catch
                {
                    MainSocketClient.dic_DeviceOfSocket[MainSocketClient.dic_Device[id][i]] = client;
                }
            }
        }
        catch { }
    }

    private static void OnChangeStatusConnect(AsynchronousClientExample sender, bool status)
    {
        if (status)
        {
            foreach (KeyValuePair<string, List<IExternalDevices>> kvp in MainSocketClient.dic_Device)
            {
                for (int i = 0; i < kvp.Value.Count; i++)
                {
                    kvp.Value[i].ReStart();
                }
            }
        }
        else
        {
            foreach (KeyValuePair<string, List<IExternalDevices>> kvp in MainSocketClient.dic_Device)
            {
                for (int i = 0; i < kvp.Value.Count; i++)
                {
                    kvp.Value[i].ReStart();
                }                
            }
        }
    }
    
}
