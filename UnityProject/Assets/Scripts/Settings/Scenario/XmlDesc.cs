using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Assets.Scripts.Settings.Scenario {
    [XmlType( "description" )]
    public class XmlDesc {
        [XmlType("item")]
        public class XmlItem {
            [XmlAttribute("id")]
            public string ID = String.Empty;
            [XmlAttribute( "text" )]
            public string Text = String.Empty;
        }

        [XmlType( "prefab" )]
        public class XmlPrefab {
            [XmlAttribute( "resource" )]
            public string Resource = String.Empty;

        }

        [XmlElement("item", typeof(XmlItem))]
        public List<XmlItem> Items = new List<XmlItem>( );

        [XmlElement("prefab", typeof(XmlPrefab))]
        public XmlPrefab Prefab = new XmlPrefab( );
    }
}
