using UnityEngine;
using System.Collections;
using TestPhotonCommon;
using ExitGames.Client.Photon;
using System.Collections.Generic;
using System.Threading;
using System.Timers;
using System.Linq;
using System.IO;
using System.Xml;
using System;
using UnityClientKSGT;

public enum Rmode : byte {server = 0, all = 1, others = 2, player = 3}

public class PhotonServerTCP : MonoBehaviour, IPhotonPeerListener
{
   
    // ПЕРЕМЕННЫЕ PHOTONSERVERa
    private string TCP_IP = "192.168.0.1";
	private	string TCP_PORT = "4530";
    private const string APP_NAME = "MyServerPhoton";
    public PhotonPeer PhotonPeer { get; set; }

   
    private bool _Bool_ConnectPhotonServer;
    private object lockerFor_Bool_ConnectPhotonServer = new object();
    public bool Bool_ConnectPhotonServer
    {
        get
        {
            lock (lockerFor_Bool_ConnectPhotonServer)
            {
                return _Bool_ConnectPhotonServer;
            }
        }

        set
        {
            lock (lockerFor_Bool_ConnectPhotonServer)
            {
                _Bool_ConnectPhotonServer = value;
            }
        }
    }

    #region Параметры игрока
    public EventValueType<string> characterName = new EventValueType<string>("");
    public string CharacterName
    {
        get { return this.characterName.Value; }
        set { this.characterName.Value = value; }
    }
    public bool isServer = false;
    public bool isClient = true;
    bool onlinePlayer = false;
    #endregion


    private static PhotonServerTCP _instance;
    public static PhotonServerTCP Instance
    {
        get { return _instance; }
    }
                
	

	
    /// <summary>
    /// Статус соединения
    /// </summary>
    public UnityClientKSGT.EventValueType<StatusCode> StatusConnect = new UnityClientKSGT.EventValueType<StatusCode>(StatusCode.Disconnect);

    public ThreadPhotonServer threadPhoton = null;

    private List<StatusCode> _List_OnStatusChanged = new List<StatusCode>();
    private object lockerFor_List_OnStatusChanged = new object();
    private List<StatusCode> List_OnStatusChanged
    {
        get 
        {
            lock (lockerFor_List_OnStatusChanged)
            {
                return _List_OnStatusChanged;
            }
        }
        set
        {
            lock (lockerFor_List_OnStatusChanged)
            {
                _List_OnStatusChanged = value;
            }
        }
    }

    private List<OperationResponse> _List_OnOperationResponse = new List<OperationResponse>();
    private object lockerFor_List_OnOperationResponse = new object();
    private List<OperationResponse> List_OnOperationResponse
    {
        get
        {
            lock (lockerFor_List_OnOperationResponse)
            {
                return _List_OnOperationResponse;
            }
        }
        set
        {
            lock (lockerFor_List_OnOperationResponse)
            {
                _List_OnOperationResponse = value;
            }
        }
    }

    private List<EventData> _List_OnEvent = new List<EventData>();
    private object lockerFor_List_OnEvent = new object();
    private List<EventData> List_OnEvent
    {
        get
        {
            lock (lockerFor_List_OnEvent)
            {
                return _List_OnEvent;
            }
        }
        set
        {
            lock (lockerFor_List_OnEvent)
            {
                _List_OnEvent = value;
            }
        }
    }

    # region PHOTONSERVER ИНТЕРФЕЙС
    public void DebugReturn(DebugLevel level, string message)
    {

    }

    public void OnEvent( EventData eventData ) {
        switch ( eventData.Code ) {
            case ( byte ) EventCode.NetWorkUnity:
                NetWorkUnityHandler( eventData );
                break;
            default:
                break;
        }
    }

  
    public void OnOperationResponse(OperationResponse operationResponse)
    {
        switch (operationResponse.OperationCode)
        {
            case (byte)OperationCode.Login:
                {
                    LoginResponseHandler(operationResponse);
                }
                break;
            default:
                break;
        }
    }

   



    public void OnStatusChanged(StatusCode statusCode)
    {
        this.StatusConnect.Value = statusCode;
        switch (statusCode)
        {
            case StatusCode.Connect: {
                    Debug.Log( "Server to Protocol TCP Connect " );
                    // Производит отправку индентификатора клиента
                    if ( !onlinePlayer ) {
                        LoginResponse( );
                        onlinePlayer = true;
                    }
                }
                break;
			
            case StatusCode.Disconnect:
                Debug.Log("Server to Protocol TCP Disconect");
                if (Bool_ConnectPhotonServer)
                {
                    // Производит повторное подключение к серверу TCP
                    Connect();
                    onlinePlayer = false;
                }
                break;
            case StatusCode.SendError:
                {
                    Connect();
                }
                break;
            default:
                Debug.Log("Unknown status:" + statusCode.ToString());
                break;
        }
    }


    # endregion


    # region UNITY ИНТЕРФЕЙС

    void Awake()
    {
        if (Instance != null)
        {
            DestroyObject(gameObject);
            return;

        }
        DontDestroyOnLoad(gameObject);
        Application.runInBackground = true;
        _instance = this;
	
    }


    void Update()
    {

        #region OnStatusChanged
        if (List_OnStatusChanged.Count != 0)
        {
            List<StatusCode> list_StatusChanged = new List<StatusCode>();
            lock (lockerFor_List_OnStatusChanged)
            {
                for (int i = 0; i < _List_OnStatusChanged.Count; i++)
                {
                    list_StatusChanged.Add(_List_OnStatusChanged[i]);
                }
                _List_OnStatusChanged.Clear();
            }
            HandlerUpdate(CodeHandler.OnStatusChanged, (object)list_StatusChanged);
        }
        #endregion
        #region OnOperationResponse
        if (List_OnOperationResponse.Count != 0)
        {
            List<OperationResponse> list_OperationResponse = new List<OperationResponse>();
            lock (lockerFor_List_OnOperationResponse)
            {
                for (int i = 0; i < _List_OnOperationResponse.Count; i++)
                {
                    list_OperationResponse.Add(_List_OnOperationResponse[i]);
                }
                _List_OnOperationResponse.Clear();
            }
            HandlerUpdate(CodeHandler.OnOperationResponse, (object)list_OperationResponse);
        }
        #endregion
        #region OnEvent
        if (List_OnEvent.Count != 0)
        {
            List<EventData> list_Event = new List<EventData>();
            lock (lockerFor_List_OnEvent)
            {
                for (int i = 0; i < _List_OnEvent.Count; i++)
                {
                    list_Event.Add(_List_OnEvent[i]);
                }
                _List_OnEvent.Clear();
            }
            HandlerUpdate(CodeHandler.OnEvent, (object)list_Event);
        }
        #endregion
    }
    #endregion

    # region UNITY ВСПОМОГАТЕЛЬНЫЕ ПРОЦЕДУРЫ
	// запуск коннекта из MainTrenag
	public void StartConnection(string ip, string port, bool serv) 
    {
        if (!Bool_ConnectPhotonServer) Bool_ConnectPhotonServer = true;
        if (serv)
        {
            CharacterName = "Server";
            isServer = true;
            isClient = false;
            this.characterName.AddingEvent(this, PhotonNetwork.ManagerPhotonNetworkView.OnChange_StatusConnectBy_Server);
            this.StatusConnect.AddingEvent(this, PhotonNetwork.ManagerPhotonNetworkView.OnChange_StatusConnectBy_Server);
            PhotonNetwork.ManagerPhotonNetworkView.OnChange_StatusConnectBy_Server(this, null);
        }
        else
        {
            this.characterName.AddingEvent(this, PhotonNetwork.ManagerPhotonNetworkView.OnChange_StatusConnectBy_Client);
            this.StatusConnect.AddingEvent(this, PhotonNetwork.ManagerPhotonNetworkView.OnChange_StatusConnectBy_Client);
            PhotonNetwork.ManagerPhotonNetworkView.OnChange_StatusConnectBy_Client(this, null);
        }
        TCP_IP = ip;
        
//		TCP_PORT = port;
        Debug.Log("***StartConnection***"+Bool_ConnectPhotonServer.ToString());

        if (threadPhoton == null)
        {
            threadPhoton = new ThreadPhotonServer(this, TCP_IP, TCP_PORT);
            Thread newThread = new Thread(new ThreadStart(threadPhoton.ServerUpdate));
            newThread.Start();
            Connect();
        }

	}

    public void Connect()
    {

	}

    public void DisConnectServer()
    {
        Bool_ConnectPhotonServer = false;
        onlinePlayer = false;
        DisConnect();
    }
	
    

    private void DisConnect()
    {
        /*
        if (PhotonPeer != null)
            PhotonPeer.Disconnect();        
         */
        OnDestroy();
    }



    #endregion

    #region PHOTONSERVER ОПЕРАЦИИ НА СЕРВЕР

    // Запрос индентификатора
    private void LoginResponse()
    {
        threadPhoton.OpCustom(this, (byte)OperationCode.Login, new Dictionary<byte, object> { 
        { (byte)ParameterCode.CharacterName, CharacterName } 
        }, 
        true);
    }

    //
    public void NetWorkUnity(string nameScript, string nameObject, byte sendDirection, object[] arrayArg)
    {
		NetWorkUnity(nameScript, nameObject, sendDirection, this.CharacterName, arrayArg);
    }

	public void NetWorkUnity(string nameScript, string nameObject, byte sendDirection, string playerID, object[] arrayArg)
    {
        try
        {
            threadPhoton.OpCustom(this, (byte)OperationCode.NetWorkUnity, new Dictionary<byte, object>

            {
                {(byte)ParameterCode.CharacterName, playerID },
                {(byte)ParameterCode.nameScript, nameScript },
                {(byte)ParameterCode.nameObject, nameObject },
                {(byte)ParameterCode.sendDirection, sendDirection },
                {(byte)ParameterCode.arrayArg, arrayArg }
            }, false);
        }
        catch { }
    }

    public void NetWorkUnityInstantiate(string namePrefabl, string directoryPrefabl, Vector3 _position, Quaternion _rotation, byte _group)
    {
        Debug.Log("*******************NetWorkUnityInstantiate*******************");
        threadPhoton.OpCustom(this, (byte)OperationCode.NetWorkUnityInstantiate, new Dictionary<byte, object>
        {
            {(byte)ParameterCode.parameter_1, directoryPrefabl},
            {(byte)ParameterCode.PosX, _position.x},
            {(byte)ParameterCode.PosY, _position.y},
            {(byte)ParameterCode.PosZ, _position.z},
            {(byte)ParameterCode.sz_X, _rotation.x},
            {(byte)ParameterCode.sz_Y, _rotation.y},
            {(byte)ParameterCode.sz_Z, _rotation.z},
            {(byte)ParameterCode.heights, _rotation.w},
            {(byte)ParameterCode.CharacterName, namePrefabl},
            {(byte)ParameterCode.group, _group}
        }, false);
    }
	
    #endregion

    #region PHOTONSERVER ОБРАБОТЧИК ОТВЕТОВ ОТ СЕРВЕРА

    //Обощенный обработчик ответов от TCP и UDP
    public void LoginResponseHandler(OperationResponse operationResponse)
    {
        string characterName = (string)operationResponse.Parameters[(byte)ParameterCode.CharacterName];
        CharacterName = characterName;
    }

    
    #endregion

    #region PHOTONSERVER ОБРАБОТЧИК СОБЫТИЙ ОТ СЕРВЕРА

    private void NetWorkUnityHandler(EventData eventData) {
		
        string nameObject = (string)eventData.Parameters[(byte)ParameterCode.nameObject];

        GameObject obj = null;
        if (nameObject == PhotonNetwork.ManagerPhotonNetworkView.constName)
        {
            PhotonNetwork.ManagerPhotonNetworkView.NetWorkUnityHandler(eventData.Parameters);
            return;
        }

        string PlayerID = "";
		try {
			PlayerID = (string)eventData.Parameters[(byte)ParameterCode.CharacterName];
		} catch {			

		}
        if (obj == null)
		{
			return;
        }

        string nameScript = (string)eventData.Parameters[(byte)ParameterCode.nameScript];
        object[] arrayArg = (object[])eventData.Parameters[(byte)ParameterCode.arrayArg];

      

        switch ( nameScript ) {
            case "GetIstances": {
                    PhotonManager.Instance.GetInstances( ( string ) arrayArg[ 0 ] );
                }
                break;

            case "InstantiateResources": {
                    PhotonManager.Instance.InstantiateResources( ( string ) arrayArg[ 0 ], ( string ) arrayArg[ 1 ],
                        new Vector3( ( float ) arrayArg[ 2 ], ( float ) arrayArg[ 3 ], ( float ) arrayArg[ 4 ] ),
                        new Quaternion( ( float ) arrayArg[ 5 ], ( float ) arrayArg[ 6 ], ( float ) arrayArg[ 7 ], ( float ) arrayArg[ 8 ] ) );
                }
                break;
        }

   
    }


  
    #endregion

	internal void SendMessage(byte operation, byte typeMessage, Dictionary<byte, object> dictionary)
	{
		dictionary.Add((byte)1, typeMessage);

        threadPhoton.OpCustom(this,operation, dictionary, true);
	}

	
	internal void SendRequest(Dictionary<byte, object> dictionary) {
        threadPhoton.OpCustom(this, (byte)OperationCode.ClientStatistic, dictionary, true);
	}

    void OnDestroy()
    {
        System.Timers.Timer timer = new System.Timers.Timer( 100 ) { Enabled = false };
        timer.Elapsed += ( s, e ) => {
            timer.Enabled = false;
            if ( threadPhoton != null ) {
                threadPhoton.Disconnect( );
            }
        };
        timer.Enabled = true;
    }

    
    internal void Dic_CodeHandler(CodeHandler codeHandler, object value)
    {
        try
        {
            switch (codeHandler)
            {
                case CodeHandler.OnStatusChanged:
                    {
                        List_OnStatusChanged.Add((StatusCode)value);
                    }
                    break;
                case CodeHandler.OnOperationResponse:
                    {
                        List_OnOperationResponse.Add((OperationResponse)value);
                    }
                    break;
                case CodeHandler.OnEvent:
                    {
                        List_OnEvent.Add((EventData)value);
                    }
                    break;
                case CodeHandler.DebugReturn:
                    {
                    }
                    break;
            }
        }
        catch (Exception ex)
        {
            Debug.Log("EROOR PHOTONSERVER_TCP _ : " + ex);    
        }

    }

    private void HandlerUpdate(CodeHandler codeHandler, object value)
    {
        try
        {
            switch (codeHandler)
            {
                case CodeHandler.OnStatusChanged:
                    {
                        List<StatusCode> list = (List<StatusCode>)value;
                        for (int i = 0; i < list.Count; i++)
                        {
                            OnStatusChanged(list[i]);
                        }
                    }
                    break;
                case CodeHandler.OnOperationResponse:
                    {
                        List<OperationResponse> list = (List<OperationResponse>)value;
                        for (int i = 0; i < list.Count; i++)
                        {
                            OnOperationResponse(list[i]);
                        }
                    }
                    break;
                case CodeHandler.OnEvent:
                    {
                        List<EventData> list = (List<EventData>)value;
                        for (int i = 0; i < list.Count; i++)
                        {
                            OnEvent(list[i]);
                        }
                    }
                    break;
                default:
                    {

                    }
                    break;
            }
        }
        catch (Exception ex )
        {
			Debug.Log("EROOR HandlerUpdate _ : " + ex + ", CodeHandler: " + codeHandler);
        }
    }
}
