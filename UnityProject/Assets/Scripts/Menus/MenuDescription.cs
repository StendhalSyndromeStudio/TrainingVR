using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Menus {
    /// <summary>
    /// КЛАСС: Описание меню
    /// </summary>
    [System.Serializable]
    internal class MenuDescription {

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

    /// <summary>
    /// ИНТЕРФЕЙС: Меню
    /// </summary>
    internal interface IMenu {
        #region --[СВОЙСТВО]--
        /// <summary>
        /// СВОЙСТВО: Активность
        /// </summary>
        bool IsActive { get; }
        #endregion

        #region --[МЕТОД]--
        /// <summary>
        /// МЕТОД: Отображение
        /// </summary>
        void Show( );
        /// <summary>
        /// МЕТОД: Скрытия
        /// </summary>
        void Hide( );
        /// <summary>
        /// МЕТОД: Передача аргументов
        /// </summary>
        /// <param name="args"></param>
        void SetArgs( params object[ ] args );
        #endregion
    }
}
