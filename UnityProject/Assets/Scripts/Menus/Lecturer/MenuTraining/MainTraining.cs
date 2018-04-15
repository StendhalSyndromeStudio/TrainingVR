using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Menus.Lecturer.MenuTraining.Top;
using UnityEngine;
using PhotonNetwork;
using Assets.Scripts.Settings.Scenario;

namespace Assets.Scripts.Menus.Lecturer.MenuTraining {

    public class MainTraining : PhotonBehaviour, IMenu {
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
        void IMenu.SetArgs( params object[ ] args ) { this.SetArgs( args ); }
        /// <summary>
        /// СВОЙСТВО: Переход между меню
        /// </summary>
        public GoToMenuDelegate GoToMenu { get; set; }
        #endregion
        [Header("Top:")]
        [SerializeField]
        public MenuTraining.Top.MenuTop Top;

        [Header( "PrefabController:" )]
        [SerializeField]
        public MenuTraining.PrefabController.Controller PrefabController;

        #region --[PRIVATE]--
        /// <summary>
        /// ПОЛЕ: Текущие описание
        /// </summary>
        private readonly UnityClientKSGT.ObjectReference<Settings.Scenario.XmlDesc> currentDescription = new UnityClientKSGT.ObjectReference<Settings.Scenario.XmlDesc>( );
        #endregion

        void Awake( ) {
            this.Top.OnClick += this.Handler_MenuTop;
            this.currentDescription.OnDiscovered += this.CurrentDescription_OnDiscovered;
            this.currentDescription.OnLeave += this.CurrentDescription_OnLeave;
        }

        private void Show( ) {
            this.gameObject.SetActive( true );
        }

        private void Hide( ) {
            this.gameObject.SetActive( false );
        }

        private void Handler_MenuTop( MenuTop sender, MenuTopCode code ) {
            switch ( code ) {
                case MenuTopCode.arrow: {
                        AutoFigureController.Create( AutoFigureController.FigureType.arrow, String.Empty );
                    }break;
                case MenuTopCode.circle: {
                        AutoFigureController.Create( AutoFigureController.FigureType.circle, String.Empty );
                    }
                    break;
            }
        }

        private void SetArgs( object[ ] args ) {
            string path = System.IO.Path.Combine( Application.streamingAssetsPath, ( string ) args[ 0 ] );
            Settings.Scenario.XmlDesc desc = null;
            if (XmlDeserialization.TryPathToObject(path, out desc ) ) {
                this.currentDescription.Object = desc;
            }else {
                this.currentDescription.Object = null;
            }
        }

        private void CurrentDescription_OnDiscovered( XmlDesc value ) {
            PhotonNetwork.ManagerPhotonNetworkView.Instantiate( value.Prefab.Resource, Vector3.zero, Quaternion.identity );
        }

        private void CurrentDescription_OnLeave( XmlDesc value ) {
         
        }
    }
}
