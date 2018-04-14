using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Menus.Lecturer {
    /// <summary>
    /// КЛАСС: Коллекция меню
    /// </summary>
    internal class LecturerMenus : MonoBehaviour, IMenusCollection {

        #region --{IMenu}--
        bool IMenu.IsActive { get { return this.gameObject.activeSelf; } }
        /// <summary>
        /// МЕТОД: Отображение
        /// </summary>
        void IMenu.Show( ) { }
        /// <summary>
        /// МЕТОД: Скрытия
        /// </summary>
        void IMenu.Hide( ) { }
        /// <summary>
        /// МЕТОД: Передача аргументов
        /// </summary>
        /// <param name="args"></param>
        void IMenu.SetArgs( params object[ ] args ) { }
        #endregion


        #region --{IMenusCollection}--

        #endregion

        /// <summary>
        /// СВОЙСТВО: Состав
        /// </summary>
        [UnityEngine.Header("Состав:")]
        public List<MenuDescription> Collection = new List<MenuDescription>( );

        /// <summary>
        /// ПОЛЕ: Словарь меню
        /// </summary>
        private Dictionary<string, IMenu> menus = new Dictionary<string, IMenu>( );

        void Awake( ) {
            this.Collection.ForEach( this.MenusAdd );
        }

        /// <summary>
        /// МЕТОД: Добавление меню
        /// </summary>
        /// <param name="menuDescription">описание меню</param>
        private void MenusAdd( MenuDescription menuDescription ) {
            IMenu value = menuDescription.GameObject.GetComponent<IMenu>( );
            this.menus.Add( menuDescription.Name, value );
            value.Hide( );
        }
    }
}
