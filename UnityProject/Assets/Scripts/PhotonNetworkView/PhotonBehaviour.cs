using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityClientKSGT;

namespace PhotonNetwork
{
    public class PhotonBehaviour : MonoBehaviour, IPhotonComponent
    {
        #region --IPhotonComponent--
        int     IPhotonComponent.local_ID
        {
            get
            {
                return this.local_id;
            }
        }
        string  IPhotonComponent.global_ID
        {
            get
            {
                return this.global_id;
            }
            set
            {
                this.SET_globalID(value);
            }
        }


        string  IPhotonComponent.path
        {
            get
            {
                return path;
            }
            set
            {
                this.path = value;
            }
        }

        string IPhotonComponent.proprietor
        {
            get { return this.proprietor; }
            set { this.proprietor = value; }
        }
        #endregion

        public enum CodeOwner : int
        {
            not_defined = 0,
            @true       = 1,
            @false      = 2,
        }

        /// <summary>
        /// Хозяин объекта String
        /// </summary>
        [PhotonNetwork.Photon]
        protected EventValueType<string> Owner = new EventValueType<string>("");

        /// <summary>
        /// Хозяин объекта CodeOwner
        /// </summary>
        protected EventValueType<CodeOwner> isOwner = new EventValueType<CodeOwner>(CodeOwner.not_defined);
        private static readonly string name_isOwner = "isOwner";

        
        private int         local_id;
        private string      global_id;
        private string      path;
        private string      proprietor = String.Empty;

        #region ПОЛУЧЕНИЕ
        private Dictionary<string, strMemberInfo> Fields;
        private Dictionary<string, strMemberInfo> Propertys;
        private List<MethodInfo> Metods;
        #endregion

        private Dictionary<string, Synchronization.ISynchronization> dicInfoParameter = new Dictionary<string, Synchronization.ISynchronization>();
        /// <summary>
        /// СЛОВАРЬ имя - MethodInfo
        /// </summary>
        private Dictionary<string, MethodInfo> dicMethodInfo = new Dictionary<string, MethodInfo>();

        private event Action onUpdateValues = null;

        internal event Action<IPhotonComponent, string, object> onChange_Value = null;
        internal event Action<IPhotonComponent, string, Rmode, object[]> onRPC_Method = null;

        /// <summary>
        /// Блокировка смены хозяина (чтобы this.Owner не менял this.isOwner)
        /// </summary>
        private bool isLock_Owner = false;

        /// <summary>
        /// Объект управления событиями внутри данного класса
        /// </summary>
        private object controlObject = new object();

        internal void OnUpdate()
        {
            try
            {
                if (this.isOwner.Value == CodeOwner.@true)
                {
                    if (this.onUpdateValues == null) return;
                    this.onUpdateValues();
                }
            }
            catch { }
        }

        public PhotonBehaviour()
        {
            this.local_id = ManagerPhotonBehaviour.Initialization(this);
        }

        /// <summary>
        /// Установка глобального идентификатора
        /// </summary>
        /// <param name="value"></param>
        private void SET_globalID(string value)
        {
            if ((String.IsNullOrEmpty(this.proprietor)) || (this.proprietor == PhotonNetworkView.global_PlayerID.ToString()))
            {
                this.global_id = value;
            }
        }

        /// <summary>
        /// Инциализация полей, свойств и методов
        /// </summary>
        internal void InitializationAttributtes()
        {
            try
            {
                this.Fields =       this.ParseField();
                this.Propertys =    this.ParseProperty();
                this.Metods =       this.ParseMetod();
                this.Filling_FieldInfo(this.dicInfoParameter, this.Fields);
                this.Filling_Property(this.dicInfoParameter, this.Propertys);

                this.isOwner.AddingEvent(this.controlObject, this.OnChange_isOwner);
                if (PhotonServerTCP.Instance.isServer)
                {
                    this.Owner.AddingEvent(this.controlObject, this.OnChange_Owner_isServer);
                }
                else
                {
                    this.Owner.AddingEvent(this.controlObject, this.OnChange_Owner_isClient);
                }
                this.Owner.ReStart(this.controlObject);
            }
            catch(Exception ex)
            {
                UnityEngine.Debug.LogException(ex);
            }
        }

        #region УПРАВЛЕНИЕ ХОЗЯИНОМ СКРИПТА
        /// <summary>
        /// Смена хозяина блока
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="Value"></param>
        private void OnChange_isOwner(object Sender, object Value)
        {
            //Если стоит блокировка выходим
            if (this.isLock_Owner) return;
            //this.Owner.Value = PhotonServerTCP.Instance.CharacterName;
            switch (this.isOwner.Value)
            {
                case CodeOwner.@true: this.Owner.Value = PhotonServerTCP.Instance.CharacterName; break;
                case CodeOwner.@false: this.Owner.Value = ""; break;
            }
        }

        /// <summary>
        /// Установка хозяина для логики сервера
        /// </summary>
        private void OnChange_Owner_isServer(object Sender, object Value)
        {
            this.isLock_Owner = true;
            if ((this.Owner.Value == "") || (this.Owner.Value == PhotonServerTCP.Instance.CharacterName))
            {
                this.isOwner.Value = CodeOwner.@true;
            }
            else
            {
                this.isOwner.Value = CodeOwner.@false;
            }
            this.isLock_Owner = false;
        }

        /// <summary>
        /// Установка хозяина для логики клиента
        /// </summary>
        private void OnChange_Owner_isClient(object Sender, object Value)
        {
            this.isLock_Owner = true;
            if (this.Owner.Value == PhotonServerTCP.Instance.CharacterName)
            {
                this.isOwner.Value = CodeOwner.@true;
            }
            else
            {
                this.isOwner.Value = CodeOwner.@false;
            }
            this.isLock_Owner = false;
        }
        #endregion

        private void Filling_FieldInfo(Dictionary<string, Synchronization.ISynchronization> dicInfoParameter, Dictionary<string, strMemberInfo> fields)
        {
            FieldInfo field;
            foreach (strMemberInfo info in fields.Values)
            {
                try
                {
                    field = (FieldInfo)info.memberInfo;
                    Synchronization.ISynchronization parameterSynchr = null;
                    if (Synchronization.ManagerSynchronization.GET_ObjSynchronization(this.isOwner, info, this, out parameterSynchr))
                    {
                        parameterSynchr.onChange_Value += this.OnChange_Value;
                        this.onUpdateValues += parameterSynchr.OnUpdateValue;
                        parameterSynchr.MethodCall = this.OnMethodCall;
                        this.dicInfoParameter.Add(info.name, parameterSynchr);
                    }
                    else
                    {
                        Synchronization.clsInfoParameter parameter = null;
                        parameter = new Synchronization.clsInfoParameter(this);
                        parameter.codeInfo = Synchronization.CodeInfo.Field;
                        parameter.memberInfo = field;
                        if (field.GetValue(this) is IEventValue)
                        {
                            parameter.EventValue = field.GetValue(this) as IEventValue;
                            parameter.EventTypeValueSubscribe(this.isOwner);
                            parameter.onChange_Value += this.OnChange_Value;
                        }
                        else
                        {
                            parameter.EventValue = CreateEventValue(field.FieldType, field.GetValue(this));
                            parameter.SharpSubscribe(((FieldInfo)info.memberInfo).FieldType);
                            parameter.onChange_Value += this.OnChange_Value;
                            this.onUpdateValues += parameter.OnUpdateValue;
                        }
                        parameter.MethodCall = this.OnMethodCall;
                        this.dicInfoParameter.Add(info.name, parameter);
                    }
                    
                }
                catch { }
            }
        }


        private void Filling_Property(Dictionary<string, Synchronization.ISynchronization> dicInfoParameter, Dictionary<string, strMemberInfo> propertys)
        {
            Synchronization.clsInfoParameter parameter = null;
            PropertyInfo prop;
            foreach (strMemberInfo info in propertys.Values)
            {
                try
                {
                    prop = (PropertyInfo)info.memberInfo;
                    Synchronization.ISynchronization parameterSynchr = null;
                    if (Synchronization.ManagerSynchronization.GET_ObjSynchronization(this.isOwner, info, this, out parameterSynchr))
                    {
                        parameterSynchr.onChange_Value += this.OnChange_Value;
                        this.onUpdateValues += parameterSynchr.OnUpdateValue;
                        parameterSynchr.MethodCall = this.OnMethodCall;
                        this.dicInfoParameter.Add(info.name, parameterSynchr);
                    }
                    else
                    {
                        parameter = new Synchronization.clsInfoParameter(this);
                        parameter.codeInfo = Synchronization.CodeInfo.Property;
                        parameter.memberInfo = info.memberInfo;
                        parameter.EventValue = CreateEventValue(prop.PropertyType, prop.GetValue(this, null));
                        parameter.SharpSubscribe(prop.PropertyType);
                        this.onUpdateValues += parameter.OnUpdateValue;
                        parameter.MethodCall = this.OnMethodCall;
                        parameter.onChange_Value += this.OnChange_Value;
                        //UnityEngine.Debug.Log("Filling: name: " + info.name);
                        this.dicInfoParameter.Add(info.name, parameter);
                    }
                }
                catch { }
            }
        }



        private IEventValue CreateEventValue(Type declaringType, object defaultValue)
        {
            switch(declaringType.ToString())
            {
                case "System.Boolean":  { return new EventValueType<bool>((bool)defaultValue); }
                case "System.Byte":     { return new EventValueType<byte>((byte)defaultValue); }
                case "System.SByte":    { return new EventValueType<sbyte>((sbyte)defaultValue); }
                case "System.Char":     { return new EventValueType<char>((char)defaultValue); }
                case "System.Decimal":  { return new EventValueType<decimal>((decimal)defaultValue); }
                case "System.Double":   { return new EventValueType<double>((double)defaultValue); }
                case "System.Single":   { return new EventValueType<float>((float)defaultValue); }
                case "System.Int32":    { return new EventValueType<int>((int)defaultValue); }
                case "System.UInt32":   { return new EventValueType<uint>((uint)defaultValue); }
                case "System.Int64":    { return new EventValueType<long>((long)defaultValue); }
                case "System.UInt64":   { return new EventValueType<ulong>((ulong)defaultValue); }
                case "System.Int16":    { return new EventValueType<short>((short)defaultValue); }
                case "System.UInt16":   { return new EventValueType<ushort>((ushort)defaultValue); }
                case "System.String":   { return new EventValueType<string>((string)defaultValue); }
                default:
                    {                        
                        if (defaultValue is IEventValue)
                        {
                            return defaultValue as IEventValue;
                        }
                        else
                        {
                            return null;
                        }
                    }
            }
        }

        internal object[] GET_Values()
        {
            List<object> result = new List<object>();
            object value = null;
            foreach (KeyValuePair<string, Synchronization.ISynchronization> kvp in this.dicInfoParameter)
            {
                if (kvp.Value.GET_Value(out value))
                {
                    result.Add(kvp.Key);
                    result.Add(value);
                }
            }
            return result.ToArray();
        }

        internal void SET_Values(object[] array)
        {
            for (int i = 0; i < array.Length; i+=2)
            {
                try
                {
                    this.dicInfoParameter[(string)array[i]].SET_ValueNetwork(array[i + 1]);
                }
                catch { }
            }
        }

        /// <summary>
        /// Вызов метода при обновлении сетевых параметров "PhotonUpdateAttribute"
        /// </summary>
        /// <param name="nameMethod">название метода</param>
        /// <param name="value">значение переменной</param>
        private void OnMethodCall(string nameMethod, object value)
        {
            try
            {
                this.SET_RPCMethod(nameMethod, new object[] { value });
            }
            catch (Exception ex) { UnityEngine.Debug.LogException(ex); }
        }

        internal void SET_Value(string name_value, object value)
        {
            try
            {
                this.dicInfoParameter[name_value].SET_ValueNetwork(value);
            }
            catch { }
        }

        internal void SET_RPCMethod(string nameMethod, object[] parameters)
        {
            try
            {
                MethodInfo method = this.Metods.Find(x => x.Name == nameMethod);
                if (method == null) return;
                method.Invoke(this, parameters);
            }
            catch(Exception ex) { UnityEngine.Debug.LogException(ex); }
        }

        private void OnChange_Value(string sender, object value)
        {
            if (this.onChange_Value == null) return;
            this.onChange_Value(this, sender,  value);
        }

        public void RPC(string nameMethod, Rmode optionReceive, params object[] parameters)
        {
            if (this.onRPC_Method == null) return;
            this.onRPC_Method(this, nameMethod, optionReceive, parameters);
        }
    }
}
