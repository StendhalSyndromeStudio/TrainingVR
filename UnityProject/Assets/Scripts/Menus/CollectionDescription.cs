using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Menus {
    /// <summary>
    /// КЛАСС: Описание коллекции
    /// </summary>
    [System.Serializable]
    internal class CollectionDescription {

        /// <summary>
        /// ПОЛЕ: Имя
        /// </summary>
        [UnityEngine.Header("Имя:")]
        public string Name = String.Empty;
        /// <summary>
        /// ПОЛЕ: Объект
        /// </summary>
        [UnityEngine.Header("Объект:")]
        public UnityEngine.GameObject GameObject;
    }
}
