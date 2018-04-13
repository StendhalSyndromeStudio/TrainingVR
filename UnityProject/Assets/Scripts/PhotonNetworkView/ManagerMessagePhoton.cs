using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace PhotonNetwork
{
    internal static class ManagerMessagePhoton
    {
        private const string nameComponent  = "NetworkInstantiate";
        private const string nameObject     = "ManagerPhotonNetworkView";

        /// <summary>
        /// Структура сообщения
        /// </summary>
        private struct strMessagePhoton
        {
            /// <summary>
            /// Направление
            /// </summary>
            public readonly Rmode sendDirection;
            /// <summary>
            /// Сообщение
            /// </summary>
            public readonly object[] message;
            /// <summary>
            /// Идентификатор игрока
            /// </summary>
            public readonly string playerID;

            public strMessagePhoton(Rmode sendDirection, object[] message, string playerID) : this()
            {
                this.sendDirection  = sendDirection;
                this.message        = message;
                this.playerID       = playerID;
            }
        }

        /// <summary>
        /// Интервал отправки сообщения (компоновка)
        /// </summary>
        private const double intervalSend = 5f;

        /// <summary>
        /// Таймер отправки и компоновки сообщений на Photon
        /// </summary>
        private static readonly Timer timerSendMessage;

        /// <summary>
        /// объект блокировки объекта buffer
        /// </summary>
        private static object lockerFor_buffer = new object();
        /// <summary>
        /// Список сообщений на отправку
        /// </summary>
        private static List<strMessagePhoton> buffer = new List<strMessagePhoton>();

        /// <summary>
        /// Объект блокировки bufferLinkageMessage
        /// </summary>
        private static object lockerFor_bufferLinkageMessage = new object();
        /// <summary>
        /// Буфер сообщений на отправку
        /// </summary>
        private static List<strLinkageMessage> bufferLinkageMessage = new List<strLinkageMessage>();

        static ManagerMessagePhoton()
        {
            ManagerMessagePhoton.timerSendMessage = new System.Timers.Timer(ManagerMessagePhoton.intervalSend);
            ManagerMessagePhoton.timerSendMessage.Elapsed += ManagerMessagePhoton.OnTimerSendMessage_Elapsed;
            ManagerMessagePhoton.timerSendMessage.Enabled = true;
        }

        /// <summary>
        /// Событие ManagerMessagePhoton.timerSendMessage.Elapsed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnTimerSendMessage_Elapsed(object sender, ElapsedEventArgs e)
        {
            ManagerMessagePhoton.timerSendMessage.Enabled = false;
            try
            {
                ManagerMessagePhoton.LinkageMessage();
            }
            catch { }
            ManagerMessagePhoton.timerSendMessage.Enabled = true;
        }

        /// <summary>
        /// Метод компоновки сообщений
        /// </summary>
        private static void LinkageMessage()
        {
            try
            {
                List<strMessagePhoton> _listBuffer = null;
                lock (ManagerMessagePhoton.lockerFor_buffer)
                {
                    if (ManagerMessagePhoton.buffer.Count == 0) return;
                    _listBuffer = new List<strMessagePhoton>(ManagerMessagePhoton.buffer);
                    ManagerMessagePhoton.buffer.Clear();
                }
                List<strLinkageMessage> _listOUT = new List<strLinkageMessage>();
                strLinkageMessage linkageMessage = new strLinkageMessage(Rmode.all, String.Empty);
                foreach (strMessagePhoton message in _listBuffer)
                {
                    if ((message.sendDirection != linkageMessage.sendDirection) || (message.playerID != linkageMessage.playerID))
                    {
                        if (linkageMessage.IsNotEmpty) _listOUT.Add(linkageMessage);
                        linkageMessage = new strLinkageMessage(message.sendDirection, message.playerID);
                    }
                    linkageMessage.Add(message.message);
                }
                if (linkageMessage.IsNotEmpty) _listOUT.Add(linkageMessage);
                if (_listOUT.Count == 0) return;
                lock(ManagerMessagePhoton.lockerFor_bufferLinkageMessage)
                {
                    ManagerMessagePhoton.bufferLinkageMessage.AddRange(_listOUT);
                }                
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogException(ex);
            }
        }

        /// <summary>
        /// Отправка сообщения по Photon!!!
        /// </summary>
        public static void SendBufferListOUT()
        {
            try
            {
                List<strLinkageMessage> list = null;
                lock (ManagerMessagePhoton.lockerFor_bufferLinkageMessage)
                {
                    list = new List<strLinkageMessage>(ManagerMessagePhoton.bufferLinkageMessage);
                    ManagerMessagePhoton.bufferLinkageMessage.Clear();
                }
                if (list.Count == 0) return;

                foreach (strLinkageMessage message in list)
                {
                    switch (message.sendDirection)
                    {
                        case Rmode.player:
                            {
                                PhotonServerTCP.Instance.NetWorkUnity(ManagerMessagePhoton.nameComponent, ManagerMessagePhoton.nameObject, (byte)message.sendDirection, message.playerID, message.ToArray());
                            }
                            break;
                        default:
                            {
                                PhotonServerTCP.Instance.NetWorkUnity(ManagerMessagePhoton.nameComponent, ManagerMessagePhoton.nameObject, (byte)message.sendDirection, message.ToArray());
                            }
                            break;
                    }
                }
            }
            catch (Exception ex) { UnityEngine.Debug.LogException(ex); }
        }

        /// <summary>
        /// Структура для запакованного сообщения
        /// </summary>
        private struct strLinkageMessage
        {
            public const string delimiter = "$//delimiter\\$";

            public readonly string playerID;
            public readonly Rmode sendDirection;
            private readonly List<object> message;
            private int count;

            public strLinkageMessage(Rmode sendDirection, string playerID)
            {
                this.sendDirection = sendDirection;
                this.playerID = playerID;
                this.message = new List<object>();
                this.count = 0;
            }

            /// <summary>
            /// Проверка на наличие сообщений 
            /// </summary>
            public bool IsNotEmpty
            {
                get { return this.count != 0; }
            }

            /// <summary>
            /// добавление в список сообщений
            /// </summary>
            /// <param name="message"></param>
            internal void Add(object[] message)
            {
                foreach(object item in message)
                {
                    this.message.AddRange(message);
                }
                this.message.Add(strLinkageMessage.delimiter);
                this.count++;
            }

            /// <summary>
            /// Преобразование в массив
            /// </summary>
            /// <returns></returns>
            internal object[] ToArray()
            {
                return this.message.ToArray();
            }

            /// <summary>
            /// Количество
            /// </summary>
            public int Count
            {
                get { return this.count; }
            }

        }

        /// <summary>
        /// Добавление сообщения в локальный буфер
        /// </summary>
        /// <param name="sendDirection"></param>
        /// <param name="message"></param>
        public static void Add(Rmode sendDirection, object[] message)
        {
            lock (ManagerMessagePhoton.lockerFor_buffer)
            {
                ManagerMessagePhoton.buffer.Add(new strMessagePhoton(sendDirection, message, String.Empty));
            }
        }

        /// <summary>
        /// Добавление сообщения в локальный буфер
        /// </summary>
        /// <param name="sendDirection"></param>
        /// <param name="playerID"></param>
        /// <param name="message"></param>
        public static void Add(Rmode sendDirection, string playerID, object[] message)
        {
            lock (ManagerMessagePhoton.lockerFor_buffer)
            {
                ManagerMessagePhoton.buffer.Add(new strMessagePhoton(sendDirection, message, playerID));
            }
        }

        /// <summary>
        /// Преобразование в лист
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        internal static List<object[]> ToList(object[] array)
        {
            List<object[]> result = new List<object[]>();
            object oldObject = null;
            foreach(object message in array)
            {
                if (message is string)
                {
                    if ((string)message == strLinkageMessage.delimiter)  result.Add((object[])oldObject);
                }
                else
                {
                    oldObject = message;
                }
            }
            return result;
        }
    }
}
