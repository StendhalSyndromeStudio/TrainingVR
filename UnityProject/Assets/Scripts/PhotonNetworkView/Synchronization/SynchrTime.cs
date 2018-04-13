using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhotonNetwork.Synchronization
{
    /// <summary>
    /// КЛАСС: Синхронизация по времени
    /// </summary>
    internal class SynchrTime : ISynchrCondition
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
        /// менеджер таймера
        /// </summary>
        private readonly ManagerTimers managerTimers = new ManagerTimers();

        /// <summary>
        /// таймер проверка
        /// </summary>
        private readonly TimerLogic timerUpdate = new TimerLogic(30) { Enabled = false };

        /// <summary>
        /// Выполнение условия
        /// </summary>
        public event delOnFulfilled OnFulfilled;

        private object oldValue;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="interval">интервал в миллисекундах</param>
        public SynchrTime(float interval, object oldValue, delCondition condition)
        {
            this.oldValue = oldValue;
            this.Condition = condition;
            this.managerTimers.Add(this.timerUpdate);
            this.timerUpdate.Interval = (int)Math.Truncate(interval);
            this.timerUpdate.Tick.AddingEvent(this, this.OnTimerUpdate_Tick);
            this.timerUpdate.Enabled = true;
        }

        internal void OnUpdate()
        {
            this.managerTimers.CurrentTime = UnityEngine.Time.deltaTime;
        }


        private void OnTimerUpdate_Tick(object Sender, object Value)
        {
            if (this.Condition(ref this.oldValue))
            {
                this.OnFulfilled(this);
            }
        }
    }
}
