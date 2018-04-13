using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhotonNetwork.Synchronization
{
    /// <summary>
    /// КЛАСС: Синхронизация по условия
    /// </summary>
    internal class SynchrCondition : ISynchrCondition
    {

        /// <summary>
        /// Делегат условия
        /// </summary>
        /// <returns></returns>
        public delegate bool delCondition(ref object oldValue);

        /// <summary>
        /// Условие
        /// </summary>
        private delCondition Condition = null;

        /// <summary>
        /// Событие выполнения условия
        /// </summary>
        public event delOnFulfilled OnFulfilled = null;

        /// <summary>
        /// Свойство "Выполнение условия"
        /// </summary>
        private bool isCondition
        {
            get { return this.Condition(ref this.oldValue); }
        }

        /// <summary>
        /// Старое значение
        /// </summary>
        private object oldValue;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="condition"></param>
        public SynchrCondition(object oldValue, delCondition condition)
        {
            this.oldValue = oldValue;
            this.Condition = condition;
        }

        /// <summary>
        /// Обновление
        /// </summary>
        public void OnUpdate()
        {
            if ((this.OnFulfilled != null)&&(this.isCondition))
            {
                this.OnFulfilled(this);
            }
        }
    }
}
