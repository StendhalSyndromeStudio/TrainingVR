using System.Collections;
using System.Collections.Generic;
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
    public ServerConfig config;

    public void Start( ) {
        PhotonServerTCP.Instance.StartConnection( this.config.Host, this.config.Port, this.config.isServer );
    }
}
