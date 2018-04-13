using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

/// <summary>
/// КЛАСС: Обработка xml
/// </summary>
public static class XmlDeserialization  {

    /// <summary>
    /// МЕТОД: Десерелизации объекта по xml описанию
    /// </summary>
    /// <typeparam name="TObject">тип возращаемого объекта</typeparam>
    /// <param name="xmlText">xml описание</param>
    /// <returns>объект</returns>
    public static TObject Deserialization<TObject>(string xmlText) where TObject : class, new() {
        try {
            XmlDocument doc = new XmlDocument( );
            doc.LoadXml( xmlText );
            return Deserialization<TObject>( doc );
        }catch(Exception ex ) { UnityEngine.Debug.LogException( ex ); }
        return null;
    }

    /// <summary>
    /// МЕТОД: Десерелизации объекта по xml описанию
    /// </summary>
    /// <typeparam name="TObject">тип возращаемого объекта</typeparam>
    /// <param name="path">путь</param>
    /// <returns>объект</returns>
    internal static TObject PathToDeserialization<TObject>( string path ) where TObject : class, new() {
        try {
            XmlDocument doc = new XmlDocument( );
            doc.Load( path );
            return Deserialization<TObject>( doc );
        }
        catch ( Exception ex ) { UnityEngine.Debug.LogException( ex ); }
        return null;
    }

    /// <summary>
    /// МЕТОД: Десерелизации объекта по xml описанию
    /// </summary>
    /// <typeparam name="TObject">тип возращаемого объекта</typeparam>
    /// <param name="doc">xml описание</param>
    /// <returns>объект</returns>
    public static TObject Deserialization<TObject>( XmlDocument doc ) where TObject : class, new() {
        try {
            XmlSerializer ser = new XmlSerializer( typeof( TObject ) );
            TObject result = null;
            using ( XmlReader reader = new XmlNodeReader( doc ) ) {
                result = ser.Deserialize( reader ) as TObject;
            }
            return result;
        }
        catch ( Exception ex ) { UnityEngine.Debug.LogException( ex ); }
        return null;
    }

    /// <summary>
    /// МЕТОД: Получение объекта по xml описанию
    /// </summary>
    /// <typeparam name="TObject">тип возращаемого объекта</typeparam>
    /// <param name="xmlText">xml описание</param>
    /// <param name="result">объект возвращения</param>
    /// <returns></returns>
    public static bool TryGetObject<TObject>( string xmlText, out TObject result ) where TObject : class, new() {
        result = Deserialization<TObject>( xmlText );
        return result != null;
    }

    /// <summary>
    /// МЕТОД: Получение объекта по пути
    /// </summary>
    /// <typeparam name="TObject">тип возращаемого объекта</typeparam>
    /// <param name="path">путь до файла</param>
    /// <param name="result">объект возвращения</param>
    /// <returns></returns>
    public static bool TryPathToObject<TObject>( string path, out TObject result ) where TObject : class, new() {
        result = PathToDeserialization<TObject>( path );
        return result != null;
    }
}
