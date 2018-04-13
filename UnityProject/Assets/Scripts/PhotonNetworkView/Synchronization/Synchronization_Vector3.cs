using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityClientKSGT;
using UnityEngine;

namespace PhotonNetwork.Synchronization
{
    public class Synchronization_Vector3 : ISynchronization
    {
        /// <summary>
        /// Делегат обновления переменной
        /// </summary>
        private delegate void delOnUpdateValue();

        private delegate void delSET_Value(object value);

        IEventValue ISynchronization.EventValue
        {
            get { return null; }
            set { }
        }

        delGET_Value ISynchronization.GET_Value
        {
            get
            {
                return this.get_ValuePhoton;
            }
        }

        /// <summary>
        /// Вызов внешнего метода RPC
        /// </summary>
        public delMethodCall MethodCall { get; set; }

//private Vector3 oldPosition = Vector3.zero;

        /// <summary>
        /// Получение значения поля
        /// </summary>
        private delGET_Value get_ValuePhoton = null;

        /// <summary>
        /// Тип доступа
        /// </summary>
        public CodeInfo codeInfo { get; set; }

        /// <summary>
        /// Описание доступа
        /// </summary>
        private readonly MemberInfo memberInfo;

        /// <summary>
        /// Родитель объекта
        /// </summary>
        private readonly object parent;

        /// <summary>
        /// Метод обновления переменной 
        /// </summary>
        private event delOnUpdateValue OnUpdateValue = null;

        /// <summary>
        /// Сообщение пришло из сети
        /// </summary>
        private bool isMessageNetwork = false;

        /// <summary>
        /// Установка значения
        /// </summary>
        private delSET_Value set_ValueLocal = null;
        /// <summary>
        /// Получение значения локально
        /// </summary>
        private delGET_Value get_ValueLocal = null;
        /// <summary>
        /// Дельта изменения переменной
        /// </summary>
        private float deltaValue;

        /// Словарь установленных условий для переменной
        /// </summary>
        private Dictionary<ISynchrCondition, bool> dicCondition = new Dictionary<ISynchrCondition, bool>();

        /// <summary>
        /// Событие изменение переменной на сетевых клонах
        /// </summary>
        public event delOnChange_Value onChange_Value;

        /// <summary>
        /// Событие обновления сетевых клонов
        /// </summary>
        public event delOnUpdateValueNetwork onUpdateValueNetwork;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="codeInfo"></param>
        /// <param name="memberInfo"></param>
        /// <param name="parent"></param>
        /// <param name="att"></param>
        public Synchronization_Vector3(CodeInfo codeInfo, EventValueType<PhotonBehaviour.CodeOwner> isOwner, MemberInfo memberInfo, object parent, List<PhotonAttribute> listPhotonAttribute)
        {
            this.codeInfo = codeInfo;
            this.memberInfo = memberInfo;
            this.parent = parent;

            switch (codeInfo)
            {
                case CodeInfo.Field:
                    {
                        this.get_ValuePhoton = this.GET_ValuePhoton_FieldInfo;
                        this.get_ValueLocal = this.GET_ValueLocal_FieldInfo;
                        this.set_ValueLocal = this.SET_Value_FieldInfo;
                    } break;
                case CodeInfo.Property:
                    {
                        this.get_ValuePhoton = this.GET_ValuePhoton_PropertyInfo;
                        this.get_ValueLocal = this.GET_ValueLocal_PropertyInfo;
                        this.set_ValueLocal = this.SET_ValueLocal_PropertyInfo;
                    } break;
            }

            object oldValue = Vector3.zero;
            Vector3 oldPosition = Vector3.zero;
            if (this.get_ValueLocal(out oldValue))
            {
                oldPosition = (Vector3)oldValue;
            }

            foreach (PhotonAttribute att in listPhotonAttribute)
            {
                switch (att.сode)
                {
                    case PhotonAttribute.Code.none:
                        {
                            SynchrDefault сondition = new SynchrDefault(oldPosition, this.OnControl_Default);
                            this.OnUpdateValue += сondition.OnUpdate;
                            сondition.OnFulfilled += this.OnFulfilled;
                            this.dicCondition.Add(сondition, false);
                        }
                        break;
                    case PhotonAttribute.Code.delta:
                        {
                            this.deltaValue = att.delta;
                            SynchrCondition сondition = new SynchrCondition(oldPosition, this.OnControl_Delta);
                            this.OnUpdateValue += сondition.OnUpdate;
                            сondition.OnFulfilled += this.OnFulfilled;
                            this.dicCondition.Add(сondition, false);
                        }
                        break;
                    case PhotonAttribute.Code.time:
                        {
                            SynchrTime сondition = new SynchrTime(att.delta, oldPosition, this.OnControl_Default);
                            this.OnUpdateValue += сondition.OnUpdate;
                            сondition.OnFulfilled += this.OnFulfilled;
                            this.dicCondition.Add(сondition, false);
                        }
                        break;
                    case PhotonAttribute.Code.update:
                        {
                            SynchrMethod synchrMethod = new SynchrMethod(att.nameMetod, this);
                            this.onUpdateValueNetwork += synchrMethod.OnUpdateValueNetwork;
                        }
                        break;
                }
            }
            isOwner.AddingEvent(this, this.OnChange_isOwner);
            isOwner.ReStart(this);
        }

        /// <summary>
        /// Изменение хозяина компонента
        /// </summary>
        private void OnChange_isOwner(object Sender, object Value)
        {
            switch ((PhotonBehaviour.CodeOwner)Value)
            {
                case PhotonBehaviour.CodeOwner.@true:
                    {
                        this.SendStatus();
                    }
                    break;
            }
        }
        /// <summary>
        /// Выполнение установленного условия
        /// </summary>
        /// <param name="Sender"></param>
        private void OnFulfilled(ISynchrCondition Sender)
        {
            this.dicCondition[Sender] = true;
            if (this.dicCondition.Values.All(x => x == true))
            {
                try
                {
                    //Устанавливаем в начальное условие False
                    List<ISynchrCondition> keys = new List<ISynchrCondition>(this.dicCondition.Keys);
                    foreach (ISynchrCondition key in keys)
                    {
                        this.dicCondition[key] = false;
                    }
                }
                catch(Exception ex) { UnityEngine.Debug.LogException(ex); }
                this.SendStatus();
            }
        }

        /// <summary>
        /// Отправка статуса
        /// </summary>
        private void SendStatus()
        {
            object oldValue = null;
            if (this.get_ValueLocal(out oldValue))
            {
                Vector3 oldPosition = (Vector3)oldValue;
                //Отправляем сообщений
                if ((!this.isMessageNetwork) && (this.onChange_Value != null))
                {
                    this.onChange_Value(this.memberInfo.Name, new object[] { oldPosition.x, oldPosition.y, oldPosition.z });
                }
            }
        }

        /// <summary>
        /// Проверка изменения переменной на порог this.deltaValue
        /// </summary>
        /// <returns></returns>
        private bool OnControl_Delta(ref object oldPosition)
        {
            object oldValue = null;
            if (this.get_ValueLocal(out oldValue))
            {
                if (Vector3.Distance((Vector3)oldPosition, (Vector3)oldValue) >= this.deltaValue)
                {
                    oldPosition = (Vector3)oldValue;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Выполнения условия по умолчанию
        /// </summary>
        /// <returns></returns>
        private bool OnControl_Default(ref object oldPosition)
        {
            object oldValue = null;
            if (this.get_ValueLocal(out oldValue))
            {
                if (Vector3.Distance((Vector3)oldPosition, (Vector3)oldValue) >= this.deltaValue)
                {
                    oldPosition = (Vector3)oldValue;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Получить значение для передачи по сети Photon
        /// </summary>
        /// <param name="value">результат</param>
        /// <returns>True - возможно передать</returns>
        private bool GET_ValuePhoton_PropertyInfo(out object value)
        {
            value = null;
            try
            {
                Vector3 vec = (Vector3)((PropertyInfo)this.memberInfo).GetValue(parent, null);
                value = new object[] { vec.x, vec.y, vec.z };
                return true;
            }
            catch { }
            return false;
        }

        /// <summary>
        /// Получить значение для передачи по сети Photon
        /// </summary>
        /// <param name="value">результат</param>
        /// <returns>True - возможно передать</returns>
        private bool GET_ValuePhoton_FieldInfo(out object value)
        {
            value = null;
            try
            {
                Vector3 vec = (Vector3)((FieldInfo)this.memberInfo).GetValue(parent);
                value = new object[] { vec.x, vec.y, vec.z };
                return true;
            }
            catch { }
            return false;
        }

        /// <summary>
        /// Значение из свойство
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool GET_ValueLocal_PropertyInfo(out object value)
        {
            value = Vector3.zero;
            try
            {
                value = ((PropertyInfo)this.memberInfo).GetValue(parent, null);
                return true;
            }
            catch { }
            return false;
        }

        /// <summary>
        /// Значение из поля
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool GET_ValueLocal_FieldInfo(out object value)
        {
            value = Vector3.zero;
            try
            {
                value = ((FieldInfo)this.memberInfo).GetValue(parent);
                return true;
            }
            catch { }
            return false;
        }

        /// <summary>
        /// Установка значения свойства
        /// </summary>
        /// <param name="value"></param>
        private void SET_ValueLocal_PropertyInfo(object value)
        {
            PropertyInfo fieldInfo = (PropertyInfo)this.memberInfo;
            fieldInfo.SetValue(this.parent, value, null);
        }

        /// <summary>
        /// Установка значения поля
        /// </summary>
        /// <param name="value"></param>
        private void SET_Value_FieldInfo(object value)
        {
            FieldInfo fieldInfo = (FieldInfo)this.memberInfo;
            fieldInfo.SetValue(this.parent, value);
        }

        /// <summary>
        /// Проверка обновления переменной на клонах
        /// </summary>
        void ISynchronization.OnUpdateValue()
        {
            this.OnUpdateValue();
        }

        /// <summary>
        /// Установка значения полученного из сети
        /// </summary>
        /// <param name="value"></param>
        void ISynchronization.SET_ValueNetwork(object value)
        {
            this.isMessageNetwork = true;
            try
            {
                object[] array = value as object[];
                Vector3 vec = new Vector3((float)array[0], (float)array[1], (float)array[2]);
                this.set_ValueLocal(vec);
                if (this.onUpdateValueNetwork != null) this.onUpdateValueNetwork(value);
            }
            catch { }
            this.isMessageNetwork = false;
        }

    }

}
