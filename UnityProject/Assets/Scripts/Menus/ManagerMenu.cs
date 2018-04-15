using PhotonNetwork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace Assets.Scripts.Menus {
    /// <summary>
    /// ПЕРЕЧИСЛЕНИЕ: Режим работы
    /// </summary>
    public enum ModeType {
        none,
        /// <summary>
        /// Лектор
        /// </summary>
        lecturer,
        /// <summary>
        /// Обучаемый
        /// </summary>
        trainee,
    }
    /// <summary>
    /// КЛАСС: Менеджер меню
    /// </summary>
    internal class ManagerMenu : PhotonBehaviour {

        #region --[PUBLIC]--
        /// <summary>
        /// ПОЛЕ: Состав
        /// </summary>
        [UnityEngine.Header("Состав:")]
        public List<CollectionDescription> Collection = new List<CollectionDescription>( ); 
        #endregion

        #region --[PRIVATE]--
        /// <summary>
        /// ПОЛЕ: Режим работы
        /// </summary>
        private readonly UnityClientKSGT.EventValueType<ModeType> mode = new UnityClientKSGT.EventValueType<ModeType>( ModeType.none );

        /// <summary>
        /// ПОЛЕ: Коллекция меню
        /// </summary>
        private readonly UnityClientKSGT.ObjectReference<IMenusCollection> menusCollection = new UnityClientKSGT.ObjectReference<IMenusCollection>( );
        #endregion

        void Awake( ) {
            this.menusCollection.OnDiscovered += this.MenusCollection_OnDiscovered;
            this.menusCollection.OnLeave += this.MenusCollection_OnLeave;
        }

       

        // Use this for initialization
        void Start( ) {
            this.mode.AddingEvent( this, this.Mode_Change );
            PhotonServerTCP.Instance.StatusConnect.AddingEvent( this, this.StatusConnect_Change );
            this.StatusConnect_Change( null, null );
        }

       
        /// <summary>
        /// МЕТОД: Смена режима
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="Value"></param>
        private void Mode_Change( object Sender, object Value ) {
            switch ( this.mode.Value ) {
                case ModeType.lecturer: {
                        this.SelectMode( this.mode.Value );
                    }break;
                case ModeType.trainee: {
                        this.SelectMode( this.mode.Value );
                    }
                    break;
            }
        }

        private void StatusConnect_Change( object Sender, object Value ) {
            if (PhotonServerTCP.Instance.StatusConnect.Value != ExitGames.Client.Photon.StatusCode.Connect ) { return; }
            PhotonServerTCP.Instance.StatusConnect.Remove( this, this.StatusConnect_Change );
            this.mode.Value = PhotonServerTCP.Instance.isServer ? ModeType.lecturer : ModeType.trainee;
        }


        private void SelectMode( ModeType mode ) {
           GameObject gameObject = this.Collection.FirstOrDefault( ( element ) => { return element.Mode == mode; } ).GameObject;
            if (gameObject == null ) {
                this.menusCollection.Object = null;
                return;
            }
            this.menusCollection.Object = gameObject.GetComponent<IMenusCollection>( );
        }

        private void MenusCollection_OnDiscovered( IMenusCollection value ) {
            value.Show( );
        }

        private void MenusCollection_OnLeave( IMenusCollection value ) {
            value.Hide( );
        }
    }
}
