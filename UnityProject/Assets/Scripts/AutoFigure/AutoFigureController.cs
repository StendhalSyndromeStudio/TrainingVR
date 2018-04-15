using PhotonNetwork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public class AutoFigureController : MonoBehaviour {
    /// <summary>
    /// ПЕРЕЧИСЛЕНИЕ: Тип фигуры
    /// </summary>
    public enum FigureType {
        arrow,
        circle,
    }

    [System.Serializable]
    public struct AutoFigureElement {
        public FigureType Type;
        public string Resource;
    }
    
    public List<AutoFigureElement> _autoFigurePrefab;

    /// <summary>
    /// СВОЙСТВО: Родитель для автофигур
    /// </summary>
    public static Transform Canvas {
        get { return instance.transform; }
    }
    /// <summary>
    /// ПОЛЕ: Объект управления автофигурами
    /// </summary>
    private static AutoFigureController instance;

    void Awake( ) {
        AutoFigureController.instance = this;
    }

    /// <summary>
    /// МЕТОД: Создание объекта автофигуры
    /// </summary>
    /// <param name="type"></param>
    /// <param name="caption"></param>
    public static void Create(FigureType type, string caption ) {
        try {
            string resource = instance._autoFigurePrefab.FirstOrDefault( ( element ) => { return element.Type == type; } ).Resource;
            if ( String.IsNullOrEmpty( resource ) ) { return; }

            var autoFigure = PhotonNetwork.PhotonNetworkView.Instantiate( resource, new Vector3( 0, 0, 0 ), new Quaternion( ) );
            autoFigure.transform.localPosition = new Vector3( 0, 0, 0 );
            var interfaceFigure = autoFigure.GetComponent<IAutoFigure>( );
            if ( interfaceFigure != null ) {
                interfaceFigure.text = caption;
            }
        }catch(Exception ex ) {
            UnityEngine.Debug.LogException( ex );
        }
    }
}
