using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Menus.Lecturer.MenuTraining.PrefabController {
    /// <summary>
    /// КЛАСС: Контроллер управления
    /// </summary>
    public class Controller : MonoBehaviour {

        #region --[PUBLIC]--
        /// <summary>
        /// СВОЙСТВО: Текущий объект
        /// </summary>
        public static GameObject Current {
            get { return instance.current.Object; }
            set { instance.current.Object = value; }
        }
        #endregion

        #region --[PRIVATE]--
        private static Controller instance;
        /// <summary>
        /// ПОЛЕ: Текущий объект
        /// </summary>
        private readonly UnityClientKSGT.ObjectReference<GameObject> current = new UnityClientKSGT.ObjectReference<GameObject>( );
        /// <summary>
        /// ПОЛЕ: Доступные контейнеры для камеры
        /// </summary>
        [SerializeField]
        private readonly List<ContainerCamera> containers = new List<ContainerCamera>( );
        #endregion

        void Awake( ) {
            instance = this;
            this.current.OnDiscovered += this.Current_OnDiscovered;
            this.current.OnLeave += this.Current_OnLeave;
        }

        /// <summary>
        /// МЕТОД: Установка объекта
        /// </summary>
        /// <param name="value"></param>
        private void Current_OnDiscovered( GameObject value ) {
            //value.transform.SetParent( this.transform, false );
            ContainerCamera[ ] containers = value.GetComponentsInChildren<ContainerCamera>( );
            this.containers.AddRange( containers );
        }

        /// <summary>
        /// МЕТОД: Отключение объекта
        /// </summary>
        /// <param name="value"></param>
        private void Current_OnLeave( GameObject value ) {
            GameObject.Destroy( value, 1f );
        }
    }
}
