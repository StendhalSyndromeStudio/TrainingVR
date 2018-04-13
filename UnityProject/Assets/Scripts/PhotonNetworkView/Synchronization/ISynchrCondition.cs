using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhotonNetwork.Synchronization
{
    internal delegate void delOnFulfilled(ISynchrCondition Sender);
    interface ISynchrCondition
    {
        /// <summary>
        /// Событие выполнения условия
        /// </summary>
        event delOnFulfilled OnFulfilled;
    }
}
