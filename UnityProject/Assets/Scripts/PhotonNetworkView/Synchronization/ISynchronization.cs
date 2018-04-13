using UnityEngine;
using System.Collections;
using System;
using UnityClientKSGT;
using System.Reflection;

namespace PhotonNetwork.Synchronization
{
    public delegate bool delGET_Value(out object value);
    public delegate void delOnChange_Value(string name, object value);
    public delegate void delMethodCall(string nameMethod, object value);
    public delegate void delOnUpdateValueNetwork(object value);

    public enum CodeInfo
    {
        Field,
        Property,
    }

    /// <summary>
    /// Интерфейс синхронизации
    /// </summary>
    internal interface ISynchronization
    {
        /// <summary>
        /// Установка значения пришедшего по сети
        /// </summary>
        /// <param name="value"></param>
        void SET_ValueNetwork(object value);
        /// <summary>
        /// Событие обновления сетевых клонов
        /// </summary>
        event delOnUpdateValueNetwork onUpdateValueNetwork;

        /// <summary>
        /// Получить текущие значение
        /// </summary>
        delGET_Value GET_Value { get; }
        /// <summary>
        /// Тип доступа поле/свойство
        /// </summary>
        Synchronization.CodeInfo codeInfo { get; set; }
        /// <summary>
        /// Переменная синхронизации
        /// </summary>
        IEventValue EventValue { get; set; }
        /// <summary>
        /// Проверка переменной (событий OnUpdate())
        /// </summary>
        void OnUpdateValue();
        /// <summary>
        /// Изменение переменной
        /// </summary>
        event delOnChange_Value onChange_Value;
        /// <summary>
        /// Вызов метода RPC
        /// </summary>
        delMethodCall MethodCall { get; set; }
    }
}
