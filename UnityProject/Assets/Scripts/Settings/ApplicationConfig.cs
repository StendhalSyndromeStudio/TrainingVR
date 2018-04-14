using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

[XmlType( "app" )]
public class ApplicationConfig {

    [XmlType("server")]
    public class ServerClass {
        /// <summary>
        /// ПЕРЕЧИСЛЕНИЕ: Тип
        /// </summary>
        public enum TypeCode {
            /// <summary>
            /// Сервер
            /// </summary>
            server, 
            /// <summary>
            /// Клиент
            /// </summary>
            client,
        }
        /// <summary>
        /// ПОЛЕ: Хост
        /// </summary>
        [XmlAttribute("host")]
        public string Host = "192.168.0.1";
        /// <summary>
        /// ПОЛЕ: Порт
        /// </summary>
        [XmlAttribute("port")]
        public string Port = "4530";
        /// <summary>
        /// ПОЛЕ: Тип
        /// </summary>
        [XmlAttribute("type")]
        public TypeCode Type = TypeCode.client;
    }

    [XmlType("scenario")]
    public class ScenarioClass {
        [XmlAttribute("path")]
        public string Path = string.Empty;
    }

    /// <summary>
    /// ПОЛЕ: Описание сервера
    /// </summary>
    [XmlElement("server", typeof(ServerClass))]
    public ServerClass Server = new ServerClass( );

    [XmlElement( "scenario", typeof(ScenarioClass) )]
    public ScenarioClass Scenario = new ScenarioClass( );
}
