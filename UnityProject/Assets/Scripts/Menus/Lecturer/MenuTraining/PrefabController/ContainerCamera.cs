using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Menus.Lecturer.MenuTraining.PrefabController {
    /// <summary>
    /// КЛАСС: Контейнер для камеры
    /// </summary>
    public  class ContainerCamera : MonoBehaviour {
        /// <summary>
        /// СТРУКТУРА: Описания положения камеры
        /// </summary>
        [System.Serializable]
        public struct PointStr {
            [Header("Режим:")]
            public ModeVisual visual;
            [Header("Точка:")]
            public Transform PointCamera;
        }

        #region --[COMPONENT]--
        /// <summary>
        /// ПОЛЕ: Идентификатор
        /// </summary>
        public string ID = String.Empty;
        /// <summary>
        /// ПОЛЕ: Точка просмотра для точки
        /// </summary>
        [SerializeField]
        [Header("Точки просмотра")]
        public PointStr[] Points;
        #endregion

        /// <summary>
        /// МЕТОД: Установка точки просмотра
        /// </summary>
        /// <param name="camera"></param>
        public void SetCamera(GameObject camera ) {
            try {
                Transform point = this.Points.ToList( ).FirstOrDefault( ( element ) => { return element.visual == MainTrenag.Instance.Config.Visual.Mode; } ).PointCamera;
                camera.transform.SetParent( point );
            }catch(Exception ex ) { UnityEngine.Debug.LogException( ex ); }
        }
    }
}
