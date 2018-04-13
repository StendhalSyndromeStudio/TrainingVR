using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhotonNetwork
{
    public interface IPhotonComponent
    {
        /// <summary>
        /// Локальный идентификатор компонента
        /// </summary>
        int local_ID { get; }
        /// <summary>
        /// Глобальный идентификатор компонента
        /// </summary>
        string global_ID { get; set; }
        /// <summary>
        /// Место компонента в дереве объекта
        /// </summary>
        string path { get; set; }
        /// <summary>
        /// Хозяин компонента
        /// </summary>
        string proprietor { get; set; }
    }
}
