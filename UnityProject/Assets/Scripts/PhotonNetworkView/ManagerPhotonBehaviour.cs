using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhotonNetwork
{
    /// <summary>
    /// Менеджер синхронизации
    /// </summary>
    public class ManagerPhotonBehaviour : UnityEngine.MonoBehaviour
    {
        /// <summary>
        /// Задает идентификатор скрипта
        /// </summary>
        private static int _count = 0;
        private static object lockerFor_Count = new object();
        private static int Count
        {
            get { lock(lockerFor_Count) { return _count; } }
            set { lock (lockerFor_Count) { _count = value; } }
        }

        /// <summary>
        /// Объект блокировки
        /// </summary>
        private static object lockerFor_voidInitialization = new object();

        /// <summary>
        /// Инциализация объекта PhotonBehaviour
        /// </summary>
        /// <param name="photonBehaviour"></param>
        /// <returns></returns>
        internal static int Initialization(PhotonBehaviour photonBehaviour)
        {
            lock (ManagerPhotonBehaviour.lockerFor_voidInitialization)
            {                
                int local_id = ManagerPhotonBehaviour.Count;
                ManagerPhotonBehaviour.Count++;
                return local_id;
            }
        }
    }
}
