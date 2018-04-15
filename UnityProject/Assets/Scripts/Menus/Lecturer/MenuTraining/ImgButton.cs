using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityClientKSGT.ExtensionMethods;
using UnityEngine;

namespace Assets.Scripts.Menus.Lecturer.MenuTraining {
    public interface IImgButton {
        UnityEngine.UI.Image Image { get; }
        event ImgButton.ClickDelegate OnClick;
    }

    public class ImgButton : MonoBehaviour, IImgButton {
        #region --[DELEGATES]--
        /// <summary>
        /// ДЕЛЕГАТ: Сигнатура метода обработки клика
        /// </summary>
        public delegate void ClickDelegate( IImgButton sender );
        #endregion

        #region --[COMPONENT]--
        [SerializeField]
        public UnityEngine.UI.Button Button;
        [SerializeField]
        public UnityEngine.UI.Image Image;
        #endregion

        #region --[PUBLIC]--
        /// <summary>
        /// СВОЙСТВО: Изображение
        /// </summary>
        UnityEngine.UI.Image IImgButton.Image { get { return this.Image; } }

        /// <summary>
        /// СОБЫТИЕ: Клик
        /// </summary>
        event ClickDelegate IImgButton.OnClick {
            add { this.onClick.Add( value ); }
            remove { this.onClick.Remove( value ); }
        }
        #endregion

        #region --[PRIVATE]--
        /// <summary>
        /// СВОЙСТВО: Организатор "OnClick"
        /// </summary>
        private UnityClientKSGT.ExtensionMethods.ExEvent<ClickDelegate> onClick { get { return this.GetOnClick( ); } }
        /// <summary>
        /// ПОЛЕ: Организатор "OnClick"
        /// </summary>
        private UnityClientKSGT.ExtensionMethods.ExEvent<ClickDelegate> _onClick = null;
        #endregion

        void Awake( ) {
            this.Button.onClick.AddListener( ( ) => { this.onClick.Create( ); } );
        }

        /// <summary>
        /// МЕТОД: Организация события "OnClick"
        /// </summary>
        /// <param name="method"></param>
        private void Call_Click( ClickDelegate method ) {
            if ( method == null ) { return; }
            method.Invoke( this );
        }

        /// <summary>
        /// МЕТОД: Получение организатора "OnClick"
        /// </summary>
        /// <returns></returns>
        private ExEvent<ClickDelegate> GetOnClick( ) {
            if ( this._onClick == null ) {
                this._onClick = new UnityClientKSGT.ExtensionMethods.ExEvent<ClickDelegate>( this.Call_Click );
            }
            return this._onClick;
        }
    }
}
