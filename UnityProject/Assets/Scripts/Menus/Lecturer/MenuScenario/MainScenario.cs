using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Menus.Lecturer.MenuScenario {
    internal class MainScenario : MonoBehaviour, IMenu {
        #region --{IMenu}--
        bool IMenu.IsActive { get { return this.gameObject.activeSelf; } }
        /// <summary>
        /// МЕТОД: Отображение
        /// </summary>
        void IMenu.Show( ) { this.Show( ); }
        /// <summary>
        /// МЕТОД: Скрытия
        /// </summary>
        void IMenu.Hide( ) { this.Hide( ); }
        /// <summary>
        /// МЕТОД: Передача аргументов
        /// </summary>
        /// <param name="args"></param>
        void IMenu.SetArgs( params object[ ] args ) { }

        #endregion

        /// <summary>
        /// ПОЛЕ: Пункты
        /// </summary>
        private readonly List<IItem> items = new List<IItem>( );


        /// <summary>
        /// МЕТОД: Отображение
        /// </summary>
        private void Show( ) {
            this.gameObject.SetActive( true );
            this.Load( );
        }

        /// <summary>
        /// МЕТОД: Скрытите
        /// </summary>
        private void Hide( ) {
            this.gameObject.SetActive( false );
            this.RemoveAll( );
        }

        /// <summary>
        /// МЕТОД: Удаление все пунктов
        /// </summary>
        private void RemoveAll( ) {
            this.items.ForEach( ( element ) => { GameObject.Destroy( element.GameObject ); } );
        }

        /// <summary>
        /// МЕТОД: Загрузка
        /// </summary>
        private void Load( ) {
            IItem _item = null;
            foreach ( var item in MainTrenag.Instance.Scenarios.Items ) {
                _item = ButtonItem.Create( item );
                _item.OnClick += this.Item_Click;
            }
        }

        /// <summary>
        /// МЕТОД: Обработка выбора сценария
        /// </summary>
        /// <param name="item"></param>
        private void Item_Click( IItem item ) {

        }
    }
}
