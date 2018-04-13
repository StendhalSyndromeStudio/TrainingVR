using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhotonNetwork
{
    internal enum CodeCommands : byte
    {
        unknown     = 0,
        /// <summary>
        /// Создание сетевый клонов (команда от сервера)
        /// </summary>
        instantiate = 1,
        /// <summary>
        /// Обновление
        /// </summary>
        update      = 2,
        /// <summary>
        /// Список объектов
        /// </summary>
        listObjects = 3,
        /// <summary>
        /// RPC метод
        /// </summary>
        RPCmethod   = 4,
        /// <summary>
        /// Удаление сетевых клонов
        /// </summary>
        destroy = 5,
        /// <summary>
        /// Создание сетевых клонов (команда от игрока)
        /// </summary>
        instantiateClone = 6,
    }
}
