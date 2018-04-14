using PhotonNetwork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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

        //private readonly Dictionary<>
        #endregion

        // Use this for initialization
        void Start( ) {
            this.mode.AddingEvent( this, this.Mode_Change );
            this.mode.Value = PhotonServerTCP.Instance.isServer ? ModeType.lecturer : ModeType.trainee; ;
        }

        /// <summary>
        /// МЕТОД: Смена режима
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="Value"></param>
        private void Mode_Change( object Sender, object Value ) {
            switch ( this.mode.Value ) {
                case ModeType.lecturer: {

                    }break;
                case ModeType.trainee: {

                    }break;
            }
        }

    }
}
