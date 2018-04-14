using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Assets.Scripts.Settings.Scenario {
    /// <summary>
    /// КЛАСС: Пункт
    /// </summary>
    [XmlType("item")]
    public class XmlItem {
        #region --[PUBLIC]--
        /// <summary>
        /// ПОЛЕ: Текст
        /// </summary>
        [XmlAttribute("text")]
        public string Text = String.Empty;
        /// <summary>
        /// ПОЛЕ: Ресурсы
        /// </summary>
        [XmlAttribute("resource")]
        public string Resource = String.Empty;
        #endregion
    }
}
