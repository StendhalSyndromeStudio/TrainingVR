using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Menus.Lecturer.MenuTraining.Top {
    /// <summary>
    /// ПЕРЕЧИСЛЕНИЕ: Идентификатор кнопки
    /// </summary>
    public enum MenuTopCode {
        arrow,
        circle,
        remote,
        speaker,
        microphone,
        abonents,
    }

    /// <summary>
    /// КЛАСС: Агрегатор верхнего меню
    /// </summary>
    public class MenuTop : MonoBehaviour {

        public delegate void ClickDelegate( MenuTop sender, MenuTopCode code );

        #region --[OBJECT]--
        [System.Serializable]
        public class LeftClass {
            public ImgButton Arrow;
            public ImgButton Circle;
            public ImgButton Remote;
        }

        [System.Serializable]
        public class RightClass {
            public ImgButton Speaker;
            public ImgButton Microphone;
            public ImgButton Abonents;
        }
        #endregion

        #region --[COMPONENT]--
        /// <summary>
        /// ПОЛЕ: Левая
        /// </summary>
        [SerializeField]
        public LeftClass Left;
        /// <summary>
        /// ПОЛЕ: Правая
        /// </summary>
        [SerializeField]
        public RightClass Right;
        #endregion

        #region --[PUBLIC]--
        /// <summary>
        /// СОБЫТИЕ: Клик
        /// </summary>
        public event ClickDelegate OnClick {
            add { this.onClick.Add( value ); }
            remove { this.onClick.Remove( value ); }
        }
        #endregion

        #region --[PRIVATE]--
        /// <summary>
        /// ПОЛЕ: Организатор "OnClick"
        /// </summary>
        private readonly UnityClientKSGT.ExtensionMethods.ExEvent<ClickDelegate> onClick = new UnityClientKSGT.ExtensionMethods.ExEvent<ClickDelegate>( );
        #endregion

        void Awake( ) {
            this.Connect( this.Left.Arrow, MenuTopCode.arrow );
            this.Connect( this.Left.Circle, MenuTopCode.circle );
            this.Connect( this.Left.Remote, MenuTopCode.remote );
            this.Connect( this.Right.Abonents, MenuTopCode.abonents );
            this.Connect( this.Right.Microphone, MenuTopCode.microphone );
            this.Connect( this.Right.Speaker, MenuTopCode.speaker );
        }

        /// <summary>
        /// МЕТОД: Подключение событий
        /// </summary>
        /// <param name="button"></param>
        /// <param name="menuTopCode"></param>
        private void Connect(IImgButton button, MenuTopCode menuTopCode ) {
            button.OnClick += ( sender ) => { this.Call_Click( menuTopCode ); };
        }

        /// <summary>
        /// МЕТОД: Организация "OnClick"
        /// </summary>
        /// <param name="menuTopCode">идентификатор клавиши</param>
        private void Call_Click( MenuTopCode menuTopCode ) {
            this.onClick.Create( ( method ) => {
                if(method == null ) { return; }
                method.Invoke( this, menuTopCode );
            } );
        }
    }
}
