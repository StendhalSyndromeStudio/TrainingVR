using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Assets.Scripts.Settings.Scenario {
    /// <summary>
    /// КЛАСС: Доступные сценарии
    /// </summary>
    [XmlType( "scenario" )]
    public class XmlScenario {

        /// <summary>
        /// ПОЛЕ: Сценарий
        /// </summary>
        [XmlElement("item")]
        public List<XmlItem> Items = new List<XmlItem>( );
    }
}
