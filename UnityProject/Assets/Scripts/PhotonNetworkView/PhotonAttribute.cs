using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace PhotonNetwork
{
    /// <summary>
    /// Отправка изменения через указанный интервал времени
    /// </summary>
    [Serializable]
    public class PhotonTimeAttribute : PhotonAttribute
    {
        /// <summary>
        /// Отправка изменения через указанный интервал времени
        /// </summary>
        /// <param name="interval">интервал проверки</param>
        public PhotonTimeAttribute(float interval)
        {
            this.сode = Code.time;
            this.delta = interval;
        }
    }

    /// <summary>
    /// Отправка изменения при изменении переменной на указанную величину
    /// </summary>
    [Serializable]
    public class PhotonDeltaAttribute : PhotonAttribute
    {
        /// <summary>
        ///  Отправка изменения при изменении переменной на указанную величину
        /// </summary>
        /// <param name="interval">указанная величина</param>
        public PhotonDeltaAttribute(float interval)
        {
            this.сode = Code.delta;
            this.delta = interval;
        }
    }

    /// <summary>
    /// Атрибут контролируемого параметра
    /// </summary>
    [Serializable]
    public class PhotonAttribute : Attribute
    {
        /// <summary>
        /// Код управления
        /// </summary>
        public enum Code : int
        {
            /// <summary>
            /// по умолчанию
            /// </summary>
            none = 0,
            /// <summary>
            /// Дистанция
            /// </summary>
            delta = 1,
            /// <summary>
            /// Время
            /// </summary>
            time = 2,
            /// <summary>
            /// Обновление переменной
            /// </summary>
            update = 3,
        }

        public float delta;

        public Code сode;

        public string nameMetod;

        /// <summary>
        /// Атрибут контролируемого параметра
        /// </summary>
        public PhotonAttribute()
        {
            this.сode = Code.none;
        }
    }

    /// <summary>
    /// Атрибут события обновления переменной
    /// </summary>
    [Serializable]
    public class PhotonUpdateAttribute : PhotonAttribute
    {
        /// <summary>
        /// Атрибут события обновления переменной
        /// </summary>
        /// <param name="nameMetod">название метода помеченный RPC</param>
        public PhotonUpdateAttribute(string nameMetod)
        {
            this.сode = Code.update;
            this.nameMetod = nameMetod;
        }
    }
}
