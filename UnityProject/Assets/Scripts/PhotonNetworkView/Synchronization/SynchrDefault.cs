using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhotonNetwork.Synchronization
{
    /// <summary>
    /// Условие по умолчанию
    /// </summary>
    internal class SynchrDefault : ISynchrCondition
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
        public event delOnFulfilled OnFulfilled;

        private object oldValue;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="condition"></param>
        public SynchrDefault(object oldValue, delCondition condition)
        {
            this.oldValue = oldValue;
            this.Condition = condition;
        }

        internal void OnUpdate()
        {
            if (this.Condition(ref this.oldValue))
            {
                this.OnFulfilled(this);
            }
        }
    }
}
