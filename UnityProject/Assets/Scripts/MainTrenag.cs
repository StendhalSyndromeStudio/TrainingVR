﻿using System;
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

    #region --[STATIC]--
    /// <summary>
    /// СВОЙСТВО: Главный объект
    /// </summary>
    public static MainTrenag Instance { get { return MainTrenag.instance; } }
    #endregion

    #region --[PUBLIC]--
    [Header( "Конфигурация сервера" )]
    [SerializeField]
    public ApplicationConfig Config;
    [Header("Достпуные сценарии:")]
    [SerializeField]
    public Assets.Scripts.Settings.Scenario.XmlScenario Scenarios;
    /// <summary>
    /// СВОЙСТВО: Путь до файла конфигурации
    /// </summary>
    public string PathSetting = "Application/config.xml";

    [Header( "EyePair:" )]
    public EyePair EyePair;

    [Header( "Tracking:" )]
    public CVEM.Tracking.TrackingSocketClient Tracking;

    [Header( "PhotonTransform:" )]
    public PhotonNetwork.PhotonTransform PhotonTransform;

    [Header( "FreeCamera:" )]
    public FreeCamera FreeCamera;
    #endregion

    #region --[PRIVATE]--
    /// <summary>
    /// ПОЛЕ: Главный объект
    /// </summary>
    private static MainTrenag instance = null;
    #endregion

    void Awake( ) {
        MainTrenag.instance = this;

        string path = this.PathSetting;
        ApplicationConfig _config = null;
        if ( XmlDeserialization.TryPathToObject<ApplicationConfig>( path , out _config ) ) {
            this.Config = _config;
        }else {
            this.Config = new ApplicationConfig( );
            UnityEngine.Debug.Log( "ApplicationConfig path: " + path );
        }
        path = System.IO.Path.Combine( Application.streamingAssetsPath, this.Config.Scenario.Path);
        Assets.Scripts.Settings.Scenario.XmlScenario _xmlScenario = null;
        if ( XmlDeserialization.TryPathToObject<Assets.Scripts.Settings.Scenario.XmlScenario>( path, out _xmlScenario ) ) {
            this.Scenarios = _xmlScenario;
        }
        else {
            this.Scenarios = new Assets.Scripts.Settings.Scenario.XmlScenario();
            UnityEngine.Debug.Log( "XmlScenario path: " + path );
        }
    }

    public void Start( ) {
        PhotonServerTCP.Instance.StartConnection( this.Config.Server.Host, this.Config.Server.Port, this.Config.Server.Type == ApplicationConfig.ServerClass.TypeCode.server );
        PhotonServerTCP.Instance.StatusConnect.AddingEvent( this, this.StatusConnect_Change );
        this.StatusConnect_Change( null, null );
    }

    /// <summary>
    /// МЕТОД: Изменение состояния подключения к серверу
    /// </summary>
    private void StatusConnect_Change( object sender, object value ) {
        if (PhotonServerTCP.Instance.StatusConnect.Value != ExitGames.Client.Photon.StatusCode.Connect ) { return; }

        bool isActive = ( ( this.Config.Visual.Mode == ModeVisual.vr ) && ( Config.Server.Type != ApplicationConfig.ServerClass.TypeCode.server ) );

        this.EyePair.Vr = isActive;
        this.Tracking.enabled = isActive;
        this.PhotonTransform.enabled = Config.Server.Type == ApplicationConfig.ServerClass.TypeCode.server;
        this.FreeCamera.enabled = Config.Server.Type == ApplicationConfig.ServerClass.TypeCode.server;
    }
}


