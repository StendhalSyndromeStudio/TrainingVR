using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class MainTrenag : MonoBehaviour {
    /// <summary>
    /// КЛАСС: Конфигурация сервера
    /// </summary>
    [System.Serializable]
    public class ServerConfig {
        public string Host = "192.168.0.1";
        public string Port = "4530";
        public bool isServer = false;
    }

    [Header( "Конфигурация сервера" )]
    [SerializeField]
    public ApplicationConfig config;

    public string PathSetting = "Application/config.xml";

    void Awake( ) {
        string path = System.IO.Path.Combine( Application.streamingAssetsPath, this.PathSetting );
        ApplicationConfig _config = null;
        if ( XmlDeserialization.TryPathToObject<ApplicationConfig>( path , out _config ) ) {
            this.config = _config;
        }else {
            this.config = new ApplicationConfig( );
            UnityEngine.Debug.Log( "path: " + path );
        }
    }

    public void Start( ) {
        PhotonServerTCP.Instance.StartConnection( this.config.Server.Host, this.config.Server.Port, this.config.Server.Type == ApplicationConfig.ServerClass.TypeCode.server );
    }
}


