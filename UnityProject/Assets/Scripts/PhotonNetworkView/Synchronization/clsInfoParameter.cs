using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityClientKSGT;

namespace PhotonNetwork.Synchronization
{
    internal class clsInfoParameter : Synchronization.ISynchronization
    {
        /// <summary>
        /// Событие обновления сетевых клонов
        /// </summary>
        public event delOnUpdateValueNetwork onUpdateValueNetwork;

        Synchronization.delGET_Value Synchronization.ISynchronization.GET_Value { get { return this.get_Value; } }

        public clsInfoParameter(object parent)
        {
            this.parent = parent;
            this.get_Value = this.defaultGET_Value;
        }

        public Synchronization.CodeInfo codeInfo { get; set; }
        public MemberInfo memberInfo;
        public IEventValue EventValue { get; set; }
        public object parent;

        private Synchronization.delGET_Value get_Value = null;

        private bool isMessageNetwork = false;

        private Action<object> delSET_Value = null;

        /// <summary>
        /// объект для обновления сетевых клонов
        /// </summary>
        private object controlObj_OnUpdate_NetworkClone = new object();

        public event Synchronization.delOnChange_Value onChange_Value = null;

        /// <summary>
        /// Вызов внешнего метода RPC
        /// </summary>
        public delMethodCall MethodCall { get; set; }

        internal void EventTypeValueSubscribe(EventValueType<PhotonBehaviour.CodeOwner> isOwner)
        {
            /*switch (codeInfo)
            {
                case Synchronization.CodeInfo.Field: { this.EventValue.AddingEvent(this, this.OnChange_FieldValue); } break;
                case Synchronization.CodeInfo.Property: { this.EventValue.AddingEvent(this, this.OnChange_PropertyValue); } break;
            }*/

            switch (this.EventValue.GetType().ToString())
            {
                case "System.Boolean":
                    {
                        this.delSET_Value = this.SET_ValueType<bool>;
                        this.get_Value = this.eventTypeValueGET_Value<bool>;
                        isOwner.AddingEvent(this, this.OnChange_isOwner<bool>);
                    }
                    break;
                case "System.Byte":
                    {
                        this.delSET_Value = this.SET_ValueType<byte>;
                        this.get_Value = this.eventTypeValueGET_Value<byte>;
                        isOwner.AddingEvent(this, this.OnChange_isOwner<byte>);
                    }
                    break;
                case "System.SByte":
                    {
                        this.delSET_Value = this.SET_ValueType<sbyte>;
                        this.get_Value = this.eventTypeValueGET_Value<sbyte>;
                        isOwner.AddingEvent(this, this.OnChange_isOwner<sbyte>);
                    }
                    break;
                case "System.Char":
                    {
                        this.delSET_Value = this.SET_ValueType<char>;
                        this.get_Value = this.eventTypeValueGET_Value<char>;
                        isOwner.AddingEvent(this, this.OnChange_isOwner<char>);
                    }
                    break;
                case "System.Decimal":
                    {
                        this.delSET_Value = this.SET_ValueType<decimal>;
                        this.get_Value = this.eventTypeValueGET_Value<decimal>;
                        isOwner.AddingEvent(this, this.OnChange_isOwner<decimal>);
                    }
                    break;
                case "System.Double":
                    {
                        this.delSET_Value = this.SET_ValueType<double>;
                        this.get_Value = this.eventTypeValueGET_Value<double>;
                        isOwner.AddingEvent(this, this.OnChange_isOwner<double>);
                    }
                    break;
                case "System.Single":
                    {
                        this.delSET_Value = this.SET_ValueType<Single>;
                        this.get_Value = this.eventTypeValueGET_Value<Single>;
                        isOwner.AddingEvent(this, this.OnChange_isOwner<Single>);
                    }
                    break;
                case "System.Int32":
                    {
                        this.delSET_Value = this.SET_ValueType<Int32>;
                        this.get_Value = this.eventTypeValueGET_Value<Int32>;
                        isOwner.AddingEvent(this, this.OnChange_isOwner<Int32>);
                    }
                    break;
                case "System.UInt32":
                    {
                        this.delSET_Value = this.SET_ValueType<UInt32>;
                        this.get_Value = this.eventTypeValueGET_Value<UInt32>;
                        isOwner.AddingEvent(this, this.OnChange_isOwner<UInt32>);
                    }
                    break;
                case "System.Int64":
                    {
                        this.delSET_Value = this.SET_ValueType<Int64>;
                        this.get_Value = this.eventTypeValueGET_Value<Int64>;
                        isOwner.AddingEvent(this, this.OnChange_isOwner<Int64>);
                    }
                    break;
                case "System.UInt64":
                    {
                        this.delSET_Value = this.SET_ValueType<UInt64>;
                        this.get_Value = this.eventTypeValueGET_Value<UInt64>;
                        isOwner.AddingEvent(this, this.OnChange_isOwner<UInt64>);
                    }
                    break;
                case "System.Int16":
                    {
                        this.delSET_Value = this.SET_ValueType<Int16>;
                        this.get_Value = this.eventTypeValueGET_Value<Int16>;
                        isOwner.AddingEvent(this, this.OnChange_isOwner<Int16>);
                    }
                    break;
                case "System.UInt16":
                    {
                        this.delSET_Value = this.SET_ValueType<UInt16>;
                        this.get_Value = this.eventTypeValueGET_Value<UInt16>;
                        isOwner.AddingEvent(this, this.OnChange_isOwner<UInt16>);
                    }
                    break;
                case "System.String":
                    {
                        this.delSET_Value = this.SET_ValueType<string>;
                        this.get_Value = this.eventTypeValueGET_Value<string>;
                        isOwner.AddingEvent(this, this.OnChange_isOwner<string>);
                    }
                    break;
            }
            isOwner.ReStart(this);
        }

        private void OnChange_isOwner<T>(object Sender, object Value) where T : IComparable
        {
            UnityClientKSGT.EventValueType<T> obj = this.EventValue as UnityClientKSGT.EventValueType<T>;
            PhotonBehaviour.CodeOwner code = (PhotonBehaviour.CodeOwner)Value;
            switch(code)
            {
                case PhotonBehaviour.CodeOwner.@true:
                    {
                        obj.AddingEvent(this.controlObj_OnUpdate_NetworkClone, this.OnUpdate_NetworkClone);
                        obj.ReStart(this.controlObj_OnUpdate_NetworkClone);
                    }
                    break;
                case PhotonBehaviour.CodeOwner.@false:
                    {
                        obj.Remove(this.controlObj_OnUpdate_NetworkClone, this.OnUpdate_NetworkClone);
                    }
                    break;
            }
        }

        internal void SharpSubscribe(Type typeValue)
        {
            this.get_Value = this.defaultGET_Value;
            switch (codeInfo)
            {
                case Synchronization.CodeInfo.Field: { this.EventValue.AddingEvent(this, this.OnChange_FieldValue); } break;
                case Synchronization.CodeInfo.Property: { this.EventValue.AddingEvent(this, this.OnChange_PropertyValue); } break;
            }
            this.EventValue.AddingEvent(this, this.OnUpdate_NetworkClone);
            //UnityEngine.Debug.Log("typeValue: " + typeValue.ToString());
            switch (typeValue.ToString())
            {
                case "System.Boolean": { this.delSET_Value = this.SET_ValueType<bool>; } break;
                case "System.Byte": { this.delSET_Value = this.SET_ValueType<byte>; } break;
                case "System.SByte": { this.delSET_Value = this.SET_ValueType<sbyte>; } break;
                case "System.Char": { this.delSET_Value = this.SET_ValueType<char>; } break;
                case "System.Decimal": { this.delSET_Value = this.SET_ValueType<decimal>; } break;
                case "System.Double": { this.delSET_Value = this.SET_ValueType<double>; } break;
                case "System.Single": { this.delSET_Value = this.SET_ValueType<Single>; } break;
                case "System.Int32": { this.delSET_Value = this.SET_ValueType<Int32>; } break;
                case "System.UInt32": { this.delSET_Value = this.SET_ValueType<UInt32>; } break;
                case "System.Int64": { this.delSET_Value = this.SET_ValueType<Int64>; } break;
                case "System.UInt64": { this.delSET_Value = this.SET_ValueType<UInt64>; } break;
                case "System.Int16": { this.delSET_Value = this.SET_ValueType<Int16>; } break;
                case "System.UInt16": { this.delSET_Value = this.SET_ValueType<UInt16>; } break;
                case "System.String": { this.delSET_Value = this.SET_ValueType<string>; } break;
            }
        }

        private void OnUpdate_NetworkClone(object Sender, object Value)
        {
            if ((!this.isMessageNetwork) && (this.onChange_Value != null))
            {
                //UnityEngine.Debug.Log("this.memberInfo.Name: " + this.memberInfo.Name + " Value: " + Value);
                this.onChange_Value(this.memberInfo.Name, Value);
            }
        }

        private void OnChange_PropertyValue(object Sender, object Value)
        {
            try
            {
                PropertyInfo fieldInfo = (PropertyInfo)this.memberInfo;
                fieldInfo.SetValue(this.parent, Value, null);
                if (this.onUpdateValueNetwork != null) this.onUpdateValueNetwork(Value);
            }
            catch(Exception ex) { UnityEngine.Debug.Log(ex.ToString()); }
        }

        private void OnChange_FieldValue(object Sender, object Value)
        {
            try
            {
                FieldInfo fieldInfo = (FieldInfo)this.memberInfo;
                fieldInfo.SetValue(parent, Value);
                if (this.onUpdateValueNetwork != null) this.onUpdateValueNetwork(Value);
            }
            catch (Exception ex) { UnityEngine.Debug.Log(ex.ToString()); }
        }

        private bool eventTypeValueGET_Value<T>(out object value) where T : IComparable
        {
            value = null;
            try
            {
                EventValueType<T> eventValueType = this.EventValue as EventValueType<T>;
                value = eventValueType.Value;
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool defaultGET_Value(out object value)
        {
            value = null;
            try
            {
                switch (codeInfo)
                {
                    case Synchronization.CodeInfo.Field:
                        {
                            value = ((FieldInfo)this.memberInfo).GetValue(this.parent);
                        }
                        break;
                    case Synchronization.CodeInfo.Property:
                        {
                            value = ((PropertyInfo)this.memberInfo).GetValue(this.parent, null);
                        }
                        break;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Установка переменной по сети
        /// </summary>
        /// <param name="value"></param>
        public void SET_ValueNetwork(object value)
        {
            this.isMessageNetwork = true;
            try
            {
                //UnityEngine.Debug.Log("SET_ValueNetwork: " + value.ToString());
                this.delSET_Value(value);
            }
            catch { }
            this.isMessageNetwork = false;
        }

        private void SET_ValueType<T>(object value) where T : IComparable
        {
            EventValueType<T> eventValueType = this.EventValue as EventValueType<T>;
            eventValueType.Value = (T)value;
        }

        public void OnUpdateValue()
        {
            try
            {
                object value = null;
                if (this.get_Value(out value))
                {
                    try
                    {
                        this.delSET_Value(value);
                    }
                    catch { }
                }
            }
            catch { }
        }
    }

}
