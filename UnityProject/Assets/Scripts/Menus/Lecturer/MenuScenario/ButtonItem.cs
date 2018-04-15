using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Menus.Lecturer.MenuScenario {
    /// <summary>
    /// ИНТЕРФЕЙС: Пункт
    /// </summary>
    internal interface IItem {
        /// <summary>
        /// СВОЙСТВО: GameObject
        /// </summary>
        UnityEngine.Object GameObject { get; }

        /// <summary>
        /// СВОЙСТВО: Текст
        /// </summary>
        string Text { get; }

        /// <summary>
        /// СОБЫТИЕ: Клик по объекту
        /// </summary>
        event ButtonItem.ButtonDelegate OnClick;

        /// <summary>
        /// СВОЙСТВО: XmlItem
        /// </summary>
        Settings.Scenario.XmlItem XmlItem { get; }
    }

    /// <summary>
    /// КЛАСС: Пункт меню
    /// </summary>
    internal class ButtonItem : MonoBehaviour, IItem {
        #region --[DELEGATES]--
        /// <summary>
        /// ДЕЛЕГАТ: Сигнатура метода выбора пункта
        /// </summary>
        /// <param name="item"></param>
        public delegate void ButtonDelegate( IItem item );
        #endregion

        #region --{IItem}--
        /// <summary>
        /// СВОЙСТВО: GameObject
        /// </summary>
        UnityEngine.Object IItem.GameObject { get { return this.gameObject; } }
        /// <summary>
        /// СВОЙСТВО: Текст
        /// </summary>
        string IItem.Text {
            get {
                return this.Text.text;
            }
        }
        /// <summary>
        /// СОБЫТИЕ: Клик по объекту
        /// </summary>
        event ButtonItem.ButtonDelegate IItem.OnClick {
            add { this.onClick.Add( value ); }
            remove { this.onClick.Remove( value ); }
        }

        /// <summary>
        /// СВОЙСТВО: XmlItem
        /// </summary>
        Settings.Scenario.XmlItem IItem.XmlItem { get { return this.XmlItem; } }
        #endregion

        #region --[COMPONENT]--
        /// <summary>
        /// ПОЛЕ: Текст
        /// </summary>
        [ UnityEngine.Header("Текст")]
        public UnityEngine.UI.Text Text;
        /// <summary>
        /// ПОЛЕ: Кнопка
        /// </summary>
        [UnityEngine.Header("Кнопка")]
        public UnityEngine.UI.Button Button;
        #endregion

        #region --[PRIVATE]--
        /// <summary>
        /// ПОЛЕ: Объект
        /// </summary>
        private static GameObject prefab;
        /// <summary>
        /// ПОЛЕ: Родитель
        /// </summary>
        private static Transform parentTransform;
        /// <summary>
        /// ПОЛЕ: XmlItem
        /// </summary>
        private Settings.Scenario.XmlItem XmlItem;
        /// <summary>
        /// ПОЛЕ: Организатор "OnClick"
        /// </summary>
        private UnityClientKSGT.ExtensionMethods.ExEvent<ButtonDelegate> onClick;
        #endregion

        void Awake( ) {
            if ( ButtonItem.prefab == null ) {
                ButtonItem.parentTransform = this.gameObject.transform.parent;
                ButtonItem.prefab = this.gameObject;
                ButtonItem.prefab.SetActive( false );
                return;
            }
            this.onClick = new UnityClientKSGT.ExtensionMethods.ExEvent<ButtonDelegate>( this.Click );
            this.Button.onClick.AddListener( ( ) => { this.onClick.Create( ); } );
        }


        /// <summary>
        /// МЕТОД: Организация события "OnClick"
        /// </summary>
        /// <param name="method"></param>
        private void Click( ButtonDelegate method ) {
            if ( method == null ) { return; }
            method.Invoke( this );
        }

        /// <summary>
        /// ПОЛЕ: Создание пункта
        /// </summary>
        /// <param name="xmlItem">описание пункта</param>
        /// <returns></returns>
        public static IItem Create( Settings.Scenario.XmlItem xmlItem ) {
            GameObject gameObject = GameObject.Instantiate( ButtonItem.prefab );
            gameObject.transform.SetParent( ButtonItem.parentTransform );
            gameObject.SetActive( true );
            ButtonItem buttonItem = gameObject.GetComponent<ButtonItem>( );
            buttonItem.Text.text = xmlItem.Text;
            buttonItem.XmlItem = xmlItem;
            return buttonItem;
        }
    }
}
