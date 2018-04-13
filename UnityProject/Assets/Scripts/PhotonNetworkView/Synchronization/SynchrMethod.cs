using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhotonNetwork.Synchronization
{
    /// <summary>
    /// Вызов метода обновления 
    /// </summary>
    internal class SynchrMethod
    {
        private string nameMethod;
        private ISynchronization synchronization;
        /// <summary>
        /// Конструктор 
        /// </summary>
        /// <param name="nameMethod"> название метода</param>
        /// <param name="synchronization">объект синхронизации</param>
        public SynchrMethod(string nameMethod, ISynchronization synchronization)
        {
            this.nameMethod = nameMethod;
            this.synchronization = synchronization;
        }

        internal void OnUpdateValueNetwork(object value)
        {
            try
            {
                if (this.synchronization.MethodCall != null)
                {
                    this.synchronization.MethodCall(this.nameMethod, value);
                }
            }
            catch(Exception ex) { UnityEngine.Debug.LogException(ex); }
        }
    }
}
