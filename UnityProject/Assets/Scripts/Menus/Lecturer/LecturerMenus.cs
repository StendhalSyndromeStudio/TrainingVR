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
        void IMenu.Show( ) { this.Show( ); }
        /// <summary>
        /// МЕТОД: Скрытия
        /// </summary>
        void IMenu.Hide( ) { }
        /// <summary>
        /// МЕТОД: Передача аргументов
        /// </summary>
        /// <param name="args"></param>
        void IMenu.SetArgs( params object[ ] args ) { }
        /// <summary>
        /// СВОЙСТВО: Переход между меню
        /// </summary>
        public GoToMenuDelegate GoToMenu { get; set; }
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

        /// <summary>
        /// ПОЛЕ: Текущие меню
        /// </summary>
        private readonly UnityClientKSGT.ObjectReference<IMenu> currentMenu = new UnityClientKSGT.ObjectReference<IMenu>( );

        void Awake( ) {
            this.currentMenu.OnDiscovered += this.CurrentMenu_OnDiscovered;
            this.currentMenu.OnLeave += this.CurrentMenu_OnLeave;
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

        /// <summary>
        /// МЕТОД: Отображение 
        /// </summary>
        private void Show( ) {
            this.gameObject.SetActive( true );
            this.currentMenu.Object = this.menus.ElementAt( 0 ).Value;
        }

        /// <summary>
        /// МЕТОД: Скрытие
        /// </summary>
        private void Hide( ) {
            this.currentMenu.Object = null;
            this.gameObject.SetActive( false );
        }

        private void CurrentMenu_OnDiscovered( IMenu value ) {
            value.GoToMenu = this.Handler_GoToMenu;
            value.Show( );
        }

        
        private void CurrentMenu_OnLeave( IMenu value ) {
            value.Hide( );
        }

        private bool TryGetMenu( string code, out IMenu menu ) {
            menu = null;
            this.menus.TryGetValue( code, out menu );
            return menu != null;
        }


        private void Handler_GoToMenu( string code, object[ ] args ) {
            IMenu menu = null;
            if ( this.TryGetMenu( code, out menu ) ) {
                this.currentMenu.Object = menu;
                if (this.currentMenu.Object != null ) {
                    this.currentMenu.Object.SetArgs( args );
                }
            }
        }

    }
}
