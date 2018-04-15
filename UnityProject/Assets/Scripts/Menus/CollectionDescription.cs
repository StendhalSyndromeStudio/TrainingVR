using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Menus {
    /// <summary>
    /// КЛАСС: Описание коллекции
    /// </summary>
    [System.Serializable]
    internal struct CollectionDescription {

        /// <summary>
        /// ПОЛЕ: Имя
        /// </summary>
        [UnityEngine.Header( "Тип:" )]
        public ModeType Mode;
        /// <summary>
        /// ПОЛЕ: Объект
        /// </summary>
        [ UnityEngine.Header("Объект:")]
        public UnityEngine.GameObject GameObject;
    }
}
