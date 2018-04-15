using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Menus.Training.MenuTraining {
    public class MainTraining : MonoBehaviour, IMenu {
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


        private void Show( ) {
            this.gameObject.SetActive( true );
        }

        private void Hide( ) {
            this.gameObject.SetActive( false );
        }

        private void SetArgs(params object[] args ) {

        }
    }
}
