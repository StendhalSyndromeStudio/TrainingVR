using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PhotonNetwork
{
    public class ManagerPhotonNetworkView : UnityEngine.MonoBehaviour
    {
        public static string constName = "ManagerPhotonNetworkView";

        /// <summary>
        /// Информация о объекте
        /// </summary>
        private class InfoGameObject
        {
            public string namePrefab;
            public GameObject gameObject;

            public InfoGameObject(string namePrefab, GameObject gameobject)
            {
                this.namePrefab = namePrefab;
                this.gameObject = gameobject;
            }
        }

        /// <summary>
        ///  БЛОКИРОВКА СЛОВАРЯ СЕТЕВЫХ ОБЪЕКТОВ
        /// </summary>
        private static object lockerFor_dicNetworkObject = new object();

        /// <summary>
        ///  ПОЛЕ: СЛОВАРЬ СЕТЕВЫХ ОБЪЕКТОВ
        /// </summary>
        private static Dictionary<string, InfoGameObject> _dicNetworkObject = new Dictionary<string, InfoGameObject>();
        /// <summary>
        ///  СВОЙСТВО: СЛОВАРЬ СЕТЕВЫХ ОБЪЕКТОВ
        /// </summary>
        private static Dictionary<string, InfoGameObject> dicNetworkObject
        {
            get
            {
                lock (lockerFor_dicNetworkObject)
                {
                    return _dicNetworkObject;
                }
            }
        }

        private static object lockerFor_dicPhotonNetworkView = new object();
        private static Dictionary<string, PhotonNetworkView> _dicPhotonNetworkView = new Dictionary<string, PhotonNetworkView>();
        private static Dictionary<string, PhotonNetworkView> dicPhotonNetworkView
        {
            get
            {
                lock (lockerFor_dicPhotonNetworkView)
                {
                    return _dicPhotonNetworkView;
                }
            }
        }

        /// <summary>
        /// Задает идентификатор скрипта
        /// </summary>
        private static int Count = 1;

        /// <summary>
        /// Объект блокировки
        /// </summary>
        private static object lockerFor_voidInitialization = new object();


        private static List<PhotonNetworkView> buffer_PhotonNetworkView = new List<PhotonNetworkView>();

        /// <summary>
        /// Добавление в буфер сетевых клонов
        /// </summary>
        public static Action<PhotonNetworkView> AddBufferByServer = ManagerPhotonNetworkView.AddBufferByServer_Start;

        /// <summary>
        /// Обработка сообщений Photon
        /// </summary>
        /// <param name="parameters"></param>
        internal static void NetWorkUnityHandler(Dictionary<byte, object> parameters)
        {
            List<object[]> list = PhotonNetwork.ManagerMessagePhoton.ToList((object[])parameters[(byte)TestPhotonCommon.ParameterCode.arrayArg]);
            foreach (object[] arrayArg in list)
            {
                try
                {
                    switch ((byte)arrayArg[0])
                    {
                        case (byte)CodeCommands.instantiate:
                            {
                                ManagerPhotonNetworkView.InstantiateFromPhoton(arrayArg);
                            }
                            break;
                        case (byte)CodeCommands.update:
                            {
                                ManagerPhotonNetworkView.UpdateFromPhoton(arrayArg);
                            }
                            break;
                        case (byte)CodeCommands.listObjects:
                            {
                                ManagerPhotonNetworkView.ListObjectsFromPhoton(arrayArg);
                            }
                            break;
                        case (byte)CodeCommands.RPCmethod:
                            {
                                ManagerPhotonNetworkView.RPCmethodFromPhoton(arrayArg);
                            }
                            break;
                        case (byte)CodeCommands.destroy:
                            {
                                ManagerPhotonNetworkView.DestroyFromPhoton(arrayArg);
                            }
                            break;
                        case (byte)CodeCommands.instantiateClone:
                            {
                                ManagerPhotonNetworkView.InstantiateCloneFromPhoton(arrayArg);
                            }
                            break;
                    }
                }
                catch(Exception ex){ UnityEngine.Debug.LogException(ex); }
            }
            
            /*
            object[] arrayArg = (object[])parameters[(byte)TestPhotonCommon.ParameterCode.arrayArg];
            arrayArg = (object[])arrayArg[0];
            switch ((byte)arrayArg[0])
            {
                case (byte)CodeCommands.instantiate:
                    {
                        ManagerPhotonNetworkView.InstantiateFromPhoton(arrayArg);
                    }
                    break;
                case (byte)CodeCommands.update:
                    {
                        ManagerPhotonNetworkView.UpdateFromPhoton(arrayArg);
                    }
                    break;
                case (byte)CodeCommands.listObjects:
                    {
                        ManagerPhotonNetworkView.ListObjectsFromPhoton(arrayArg);
                    }
                    break;
                case (byte)CodeCommands.RPCmethod:
                    {
                        ManagerPhotonNetworkView.RPCmethodFromPhoton(arrayArg);
                    }
                    break;
            }*/
        }

        /// <summary>
        /// Создание сетевых клонов
        /// </summary>
        /// <param name="dicPhotonNetworkViews"></param>
        internal static void PhotonInstantiate(object[] dicPhotonNetworkViews)
        {
            PhotonNetwork.ManagerMessagePhoton.Add(Rmode.others, new object[] { dicPhotonNetworkViews });
            /*if (PhotonServerTCP.Instance.isServer)
            {
                PhotonNetwork.ManagerMessagePhoton.Add(Rmode.others, new object[] { dicPhotonNetworkViews });
            }
            else
            {
                dicPhotonNetworkViews[(int)CodeDescriptionGameObject.command] = (byte)CodeCommands.instantiateClone;
                PhotonNetwork.ManagerMessagePhoton.Add(Rmode.server, new object[] { dicPhotonNetworkViews });
            }*/
            /*
            PhotonServerTCP.Instance.NetWorkUnity
                (
                    "NetworkInstantiate",
                    "ManagerPhotonNetworkView",
                    (byte)Rmode.others,
                    new object[] { dicPhotonNetworkViews }
                );*/
        }

        private static void InstantiateFromPhoton(object[] arrayArg)
        {
            try
            {
                string GUID = (string)arrayArg[(int)CodeDescriptionGameObject.GUID];

                //UnityEngine.Debug.Log("Instantiate: GUID - " + GUID);

                if (ManagerPhotonNetworkView.dicNetworkObject.ContainsKey(GUID))
                {
                    //UnityEngine.Debug.Log("Not ContainsKey Instantiate: GUID - " + GUID);
                    return;
                }
                string namePrefab = (string)arrayArg[(int)CodeDescriptionGameObject.namePrefab];
                Vector3 position = new Vector3
                    (
                        (float)arrayArg[(int)CodeDescriptionGameObject.position_X],
                        (float)arrayArg[(int)CodeDescriptionGameObject.position_Y],
                        (float)arrayArg[(int)CodeDescriptionGameObject.position_Z]
                    );
                Quaternion rotation = new Quaternion
                    (
                        (float)arrayArg[(int)CodeDescriptionGameObject.rotation_X],
                        (float)arrayArg[(int)CodeDescriptionGameObject.rotation_Y],
                        (float)arrayArg[(int)CodeDescriptionGameObject.rotation_Z],
                        (float)arrayArg[(int)CodeDescriptionGameObject.rotation_W]
                    );
                GameObject gameobject = CreateGameObject(namePrefab, position, rotation);
                //UnityEngine.Debug.Log(namePrefab + " " + position.ToString() + "  " + rotation.ToString());
                if (gameobject == null) return;
                LoadingSettingsGameObject(gameobject, (object[])arrayArg[(int)CodeDescriptionGameObject.photonNetworkView]);
                ManagerPhotonNetworkView.dicNetworkObject.Add(GUID, new InfoGameObject(namePrefab, gameobject));
            }
            catch
            {
                //UnityEngine.Debug.LogException(ex);
            }
        }

        private static void InstantiateCloneFromPhoton(object[] arrayArg)
        {
            try
            {
                string GUID = (string)arrayArg[(int)CodeDescriptionGameObject.GUID];

                //UnityEngine.Debug.Log("Instantiate: GUID - " + GUID);

                if (ManagerPhotonNetworkView.dicNetworkObject.ContainsKey(GUID))
                {
                    //UnityEngine.Debug.Log("Not ContainsKey Instantiate: GUID - " + GUID);
                    return;
                }
                string namePrefab = (string)arrayArg[(int)CodeDescriptionGameObject.namePrefab];
                Vector3 position = new Vector3
                    (
                        (float)arrayArg[(int)CodeDescriptionGameObject.position_X],
                        (float)arrayArg[(int)CodeDescriptionGameObject.position_Y],
                        (float)arrayArg[(int)CodeDescriptionGameObject.position_Z]
                    );
                Quaternion rotation = new Quaternion
                    (
                        (float)arrayArg[(int)CodeDescriptionGameObject.rotation_X],
                        (float)arrayArg[(int)CodeDescriptionGameObject.rotation_Y],
                        (float)arrayArg[(int)CodeDescriptionGameObject.rotation_Z],
                        (float)arrayArg[(int)CodeDescriptionGameObject.rotation_W]
                    );
                GameObject gameobject = CreateGameObject(namePrefab, position, rotation);
                //UnityEngine.Debug.Log(namePrefab + " " + position.ToString() + "  " + rotation.ToString());
                if (gameobject == null) return;
                LoadingSettingsGameObject(gameobject, (object[])arrayArg[(int)CodeDescriptionGameObject.photonNetworkView]);
                ManagerPhotonNetworkView.dicNetworkObject.Add(GUID, new InfoGameObject(namePrefab, gameobject));

                //ManagerPhotonNetworkView.PhotonInstantiate(arrayArg);
            }
            catch
            {
                //UnityEngine.Debug.LogException(ex);
            }
        }

        /// <summary>
        /// Описание команды Destroy
        /// </summary>
        private enum CodePhotonDestroy : int
        {
            /// <summary>
            /// Команда
            /// </summary>
            command = 0,
            /// <summary>
            /// идентификатор объекта
            /// </summary>
            GUID = 1,
        }

        /// <summary>
        /// Метод удаления сетевых клонов
        /// </summary>
        /// <param name="GUID"></param>
        private static void PhotonDestroy(string GUID)
        {
            object[] descriptionObject = new object[Enum.GetNames(typeof(CodePhotonDestroy)).Length];
            descriptionObject[(int)CodePhotonDestroy.command] = (byte)CodeCommands.destroy;
            descriptionObject[(int)CodePhotonDestroy.GUID] = (string)GUID;
            PhotonNetwork.ManagerMessagePhoton.Add(Rmode.others, new object[] { descriptionObject });
        }

        /// <summary>
        /// Метод обработки команды Photon на удаление объекта 
        /// </summary>
        /// <param name="arrayArg"></param>
        private static void DestroyFromPhoton(object[] arrayArg)
        {
            string GUID = (string)arrayArg[(int)CodePhotonDestroy.GUID];
            InfoGameObject infoGameObject = null;
            if (ManagerPhotonNetworkView.dicNetworkObject.TryGetValue(GUID, out infoGameObject))
            {
                UnityEngine.Object.Destroy(infoGameObject.gameObject);
                ManagerPhotonNetworkView.dicNetworkObject.Remove(GUID);
            }
        }

        /// <summary>
        /// Добавление PhotonNetworkView из словарь по global_id
        /// </summary>
        internal static void Add(PhotonNetworkView photonNetworkView, string global_id)
        {
            try
            {
                ManagerPhotonNetworkView.dicPhotonNetworkView.Add(global_id, photonNetworkView);
                
            }
            catch(ArgumentException)
            {
                ManagerPhotonNetworkView.dicPhotonNetworkView[global_id] = photonNetworkView;
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogException(ex);
            }
        }

        /// <summary>
        /// Удаление PhotonNetworkView из словарь по global_id
        /// </summary>
        internal static void Remove(PhotonNetworkView photonNetworkView, string global_id)
        {
            try
            {
                if ((global_id != "") || (global_id == String.Empty)) return;
                ManagerPhotonNetworkView.dicPhotonNetworkView.Remove(global_id);
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogException(ex);
            }
        }

        /// <summary>
        /// Структура пакета "Описание объекта"
        /// </summary>
        private enum CodeDescriptionGameObject : int
        {
            command = 0,
            namePrefab = 1,
            position_X = 2,
            position_Y = 3,
            position_Z = 4,
            rotation_X = 5,
            rotation_Y = 6,
            rotation_Z = 7,
            rotation_W = 8,
            GUID = 9,
            photonNetworkView = 10,
        }

        /// <summary>
        /// описание компонента PhotonNetworkView
        /// </summary>
        private enum CodeDescriptionPhotonNetworkView : int
        {
            /// <summary>
            /// путь до компонента в дереве объекта
            /// </summary>
            path = 0,
            /// <summary>
            /// глобальный идентификатор
            /// </summary>
            global_ID = 1,
            /// <summary>
            /// локальный идентификатор
            /// </summary>
            local_ID = 2,
            /// <summary>
            /// компоненты photonBehaviour
            /// </summary>
            photonBehaviour = 3,
            /// <summary>
            /// хозяин
            /// </summary>
            proprietor = 4,
        }

        /// <summary>
        /// описание компонента PhotonBehaviour
        /// </summary>
        private enum CodeDescriptionPhotonBehaviour : int
        {
            /// <summary>
            /// место компонента в дереве объекта
            /// </summary>
            path = 0,
            /// <summary>
            /// глобальный идентикатор
            /// </summary>
            global_ID = 1,
            /// <summary>
            /// локальный идентификатор
            /// </summary>
            local_ID = 2,
            /// <summary>
            /// переменные компонента
            /// </summary>
            values = 3,
            /// <summary>
            /// хозяин
            /// </summary>
            proprietor = 4,
        }

        /// <summary>
        /// Описание структуры "Обновление состояния объекта"
        /// </summary>
        private enum CodeCommandUpdate : int
        {
            command = 0,
            photonNetworkView = 1,
            photonBehaviour = 2,
            name_value = 3,
            value = 4
        }

        /// <summary>
        /// Отправка RPC метода
        /// </summary>
        private enum CodeCommandMethod : int
        {
            /// <summary>
            /// Команда
            /// </summary>
            command             = 0,
            /// <summary>
            /// ID photonNetworkView
            /// </summary>
            photonNetworkView = 1,
            /// <summary>
            /// ID photonBehaviour
            /// </summary>
            photonBehaviour = 2,
            /// <summary>
            /// Отправитель
            /// </summary>
            sender              = 3,
            /// <summary>
            /// Имя метода
            /// </summary>
            nameMethod          = 4,
            /// <summary>
            /// Параметры
            /// </summary>
            parameters          = 5,
            /// <summary>
            /// Наличие параметров
            /// </summary>
            isAvailabilityParameters = 6,
        }

        /// <summary>
        /// Описание структуры пакета "Запрос сетевых клонов"
        /// </summary>
        private enum CodeCommandListObjects : int
        {
            command = 0,
            identifier = 1,
        }

        /// <summary>
        /// Изменение состояния подключения к серверу (ДЛЯ СЕРВЕРА)
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="Value"></param>
        internal static void OnChange_StatusConnectBy_Server(object Sender, object Value)
        {
            if (PhotonServerTCP.Instance.characterName.Value == "") return;
            ExitGames.Client.Photon.StatusCode statusCode = PhotonServerTCP.Instance.StatusConnect.Value;
            switch (statusCode)
            {
                case ExitGames.Client.Photon.StatusCode.Connect:
                    {
                        ManagerPhotonNetworkView.ScanListObjects();
                    }
                    break;
            }
        }

        /// <summary>
        /// Установка локального идентификатора
        /// </summary>
        /// <param name="photonNetworkView"></param>
        /// <returns></returns>
        internal static int Initialization(PhotonNetworkView photonNetworkView)
        {
            lock (ManagerPhotonNetworkView.lockerFor_voidInitialization)
            {
                int guid_local = ManagerPhotonNetworkView.Count;
                ManagerPhotonNetworkView.Count++;
                return guid_local;
            }
        }

        /// <summary>
        /// Добавление в буфер новых PhotonNetworkView (Отсутствие соединения!!!)
        /// </summary>
        /// <param name="photonNetworkView"></param>
        private static void AddBufferByServer_Start(PhotonNetworkView photonNetworkView)
        {
            lock (ManagerPhotonNetworkView.lockerFor_voidInitialization)
            {
                ManagerPhotonNetworkView.buffer_PhotonNetworkView.Add(photonNetworkView);
            }
        }

        /// <summary>
        /// Добавление в буфер новых PhotonNetworkView (При дальнейших добавлениях)
        /// </summary>
        /// <param name="photonNetworkView"></param>
        private static void AddBufferByServer_Clone(PhotonNetworkView photonNetworkView)
        {
            GameObject obj = photonNetworkView.gameObject.mainParent();

            List<PhotonNetworkView> listPhotonNetworkViews = null;
            InfoGameObject infoGameObject = null;
            string GUID = "";
            try
            {
//UnityEngine.Debug.Log("Сканирование: gameObject - " + obj.name);
                if (ManagerPhotonNetworkView.TryGetPhotonNetworkViews(obj, out listPhotonNetworkViews))
                {
                    GUID = ((IPhotonComponent)listPhotonNetworkViews[0]).global_ID;
                    infoGameObject = new InfoGameObject("$SCENA$" + obj.name, obj);

                    if (!ManagerPhotonNetworkView.dicNetworkObject.ContainsKey(GUID))
                    {
                        ManagerPhotonNetworkView.dicNetworkObject.Add(GUID, infoGameObject);
                    }
                }
            }
            catch
            {
//UnityEngine.Debug.Log("An element with the same key already exists in the dictionary GUID: "+GUID);
            }
        }

        /// <summary>
        /// Добавление в буфер новых PhotonNetworkView (Уже было установленно соединение)
        /// </summary>
        /// <param name="photonNetworkView"></param>
        internal static void AddBufferByServer_NULL(PhotonNetworkView photonNetworkView)
        {

        }

        /// <summary>
        /// Сканирование Объектов с PhotonNetworkView
        /// </summary>
        private static void ScanListObjects()
        {
//UnityEngine.Debug.Log("-- Сканирование: gameObject --" + ManagerPhotonNetworkView.buffer_PhotonNetworkView.Count.ToString());
            //PhotonNetworkView[] listScan = GameObject.FindObjectsOfType<PhotonNetworkView>();

            PhotonNetworkView[] listScan = ManagerPhotonNetworkView.buffer_PhotonNetworkView.ToArray();
            ManagerPhotonNetworkView.buffer_PhotonNetworkView.Clear();
            ManagerPhotonNetworkView.AddBufferByServer = ManagerPhotonNetworkView.AddBufferByServer_Clone;

            List<GameObject> listGameObjectScan = new List<GameObject>();
            GameObject obj = null;
            foreach (PhotonNetworkView photonNetworkView in listScan)
            {
                try
                {
                    obj = photonNetworkView.gameObject.mainParent();
                    if (listGameObjectScan.Contains(obj)) continue;
                    listGameObjectScan.Add(obj);
                }
                catch { }
            }

            List<PhotonNetworkView> listPhotonNetworkViews = null;
            InfoGameObject infoGameObject = null;
            foreach (GameObject _obj in listGameObjectScan)
            {
                try
                {
//UnityEngine.Debug.Log("Сканирование: gameObject - " + _obj.name);
                    if (ManagerPhotonNetworkView.TryGetPhotonNetworkViews(_obj, out listPhotonNetworkViews))
                    {
                        string GUID = ((IPhotonComponent)listPhotonNetworkViews[0]).global_ID;
                        infoGameObject = new InfoGameObject("$SCENA$" + _obj.name, _obj);
                        ManagerPhotonNetworkView.dicNetworkObject.Add(GUID, infoGameObject);
//UnityEngine.Debug.Log("Сканирование: gameObject - " + _obj.name + " -> GUID: " + GUID);
                    }
                }
                catch { }
            }
        }


        /// <summary>
        /// Изменение состояния подключения к серверу (ДЛЯ КЛИЕНТА)
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="Value"></param>
        internal static void OnChange_StatusConnectBy_Client(object Sender, object Value)
        {
            if (PhotonServerTCP.Instance.characterName.Value == "") return;
            ExitGames.Client.Photon.StatusCode statusCode = PhotonServerTCP.Instance.StatusConnect.Value;
            switch (statusCode)
            {
                case ExitGames.Client.Photon.StatusCode.Connect:
                    {
                        ManagerPhotonNetworkView.AddBufferByServer = ManagerPhotonNetworkView.AddBufferByServer_NULL;
                        ManagerPhotonNetworkView.RequestListObjects();
                    }
                    break;
            }
        }

        #region К СЕРВЕРУ
        /// <summary>
        /// Список сетевых объектов
        /// </summary>
        /// <param name="arrayArg"></param>
        private static void ListObjectsFromPhoton(object[] arrayArg)
        {
            try
            {
                string identifier = (string)arrayArg[(int)CodeCommandListObjects.identifier];
//UnityEngine.Debug.Log("Запрос на сервер пришел от identifier: " + identifier);
                lock (ManagerPhotonNetworkView.lockerFor_dicNetworkObject)
                {
                    object[] descriptionObject = null;
                    foreach (InfoGameObject infoGameObject in ManagerPhotonNetworkView._dicNetworkObject.Values)
                    {
                        if (ManagerPhotonNetworkView.TryGetDescription(infoGameObject.gameObject, out descriptionObject))
                        {
                            descriptionObject[(int)CodeDescriptionGameObject.namePrefab] = (string)infoGameObject.namePrefab;
//UnityEngine.Debug.Log("Object: " + infoGameObject.gameObject.name + " namePrefab: " + infoGameObject.namePrefab);

                            PhotonNetwork.ManagerMessagePhoton.Add(Rmode.player, identifier, new object[] { descriptionObject });

                            /*PhotonServerTCP.Instance.NetWorkUnity
                            (
                                "NetworkInstantiate",
                                "ManagerPhotonNetworkView",
                                (byte)Rmode.player,
                                identifier,
                                new object[] { descriptionObject }
                            );*/
                        }
                    }
                }

            }
            catch(Exception ex) { UnityEngine.Debug.LogException(ex); }

        }


        #endregion

        #region ОТ СЕРВЕРА
        private static void UpdateFromPhoton(object[] arrayArg)
        {
            string photonNetworkView = "";
            try
            {
                photonNetworkView = (string)arrayArg[(int)CodeCommandUpdate.photonNetworkView];
                string photonBehaviour = (string)arrayArg[(int)CodeCommandUpdate.photonBehaviour];
                string name_value = (string)arrayArg[(int)CodeCommandUpdate.name_value];
                object value = (object)arrayArg[(int)CodeCommandUpdate.value];
                ManagerPhotonNetworkView.dicPhotonNetworkView[photonNetworkView].SET_Value(photonBehaviour, name_value, value);
            }
            catch { }
        }

        

        /// <summary>
        /// Получение события на организацию метода
        /// </summary>
        /// <param name="arrayArg"></param>
        private static void RPCmethodFromPhoton(object[] arrayArg)
        {
            try
            {
                string photonNetworkView = (string)arrayArg[(int)CodeCommandMethod.photonNetworkView];
                string photonBehaviour = (string)arrayArg[(int)CodeCommandMethod.photonBehaviour];
                string sender = (string)arrayArg[(int)CodeCommandMethod.sender];
                string nameMethod = (string)arrayArg[(int)CodeCommandMethod.nameMethod];
                bool isAvailabilityParameters = (bool)arrayArg[(int)CodeCommandMethod.isAvailabilityParameters];

                if (isAvailabilityParameters)
                {
                    object[] parameters = (object[])arrayArg[(int)CodeCommandMethod.parameters];
                    ManagerPhotonNetworkView.dicPhotonNetworkView[photonNetworkView].SET_RPCMethod(photonBehaviour, sender, nameMethod, parameters);
                }
                else
                {
                    ManagerPhotonNetworkView.dicPhotonNetworkView[photonNetworkView].SET_RPCMethod(photonBehaviour, sender, nameMethod);
                }
            }
            catch { }
        }

        #endregion

        #region ОТ ИГРОКА
        /// <summary>
        /// Запрос списка локальных копий
        /// </summary>
        internal static void RequestListObjects()
        {
            object[] result = new object[Enum.GetNames(typeof(CodeCommandListObjects)).Length];
            result[(int)CodeCommandListObjects.command] = (byte)CodeCommands.listObjects;
            result[(int)CodeCommandListObjects.identifier] = PhotonServerTCP.Instance.CharacterName;

            PhotonNetwork.ManagerMessagePhoton.Add(Rmode.server, new object[] { result });
        }



        internal static void UpdateValue(string photonNetworkView, string photonBehaviour, string name_value, object value)
        {
//UnityEngine.Debug.Log("<-- photonNetworkView: " + photonNetworkView + " photonBehaviour: " + photonBehaviour + " name_value: " + name_value + " value: " + value.ToString());
            object[] result = new object[Enum.GetNames(typeof(CodeCommandUpdate)).Length];
            result[(int)CodeCommandUpdate.command] = (byte)CodeCommands.update;
            result[(int)CodeCommandUpdate.photonNetworkView] = photonNetworkView;
            result[(int)CodeCommandUpdate.photonBehaviour] = photonBehaviour;
            result[(int)CodeCommandUpdate.name_value] = name_value;
            result[(int)CodeCommandUpdate.value] = value;

            PhotonNetwork.ManagerMessagePhoton.Add(Rmode.others, new object[] { result });
            /*
            PhotonServerTCP.Instance.NetWorkUnity
                (
                    "NetworkInstantiate",
                    "ManagerPhotonNetworkView",
                    (byte)Rmode.others,
                    new object[]
                    {
                        result
                    }
                );*/
        }

        /// <summary>
        /// Отправка метода
        /// </summary>
        /// <param name="global_id"></param>
        /// <param name="global_ID"></param>
        /// <param name="nameMethod"></param>
        /// <param name="optionReceive"></param>
        /// <param name="parameters"></param>
        internal static void OnRPC_Method(string photonNetworkView, string photonBehaviour, string nameMethod, Rmode optionReceive, object[] parameters)
        {
            object[] result = new object[Enum.GetNames(typeof(CodeCommandMethod)).Length];
            result[(int)CodeCommandMethod.command] = (byte)CodeCommands.RPCmethod;
            result[(int)CodeCommandMethod.photonNetworkView] = photonNetworkView;
            result[(int)CodeCommandMethod.photonBehaviour] = photonBehaviour;
            result[(int)CodeCommandMethod.sender] = PhotonServerTCP.Instance.CharacterName;
            result[(int)CodeCommandMethod.nameMethod] = nameMethod;
            if (parameters == null)
            {
                result[(int)CodeCommandMethod.isAvailabilityParameters] = false;
            }
            else
            {
                if (parameters.Length == 0)
                {
                    result[(int)CodeCommandMethod.isAvailabilityParameters] = false;
                }
                else
                {
                    result[(int)CodeCommandMethod.isAvailabilityParameters] = true;
                    result[(int)CodeCommandMethod.parameters] = parameters;
                }
            }

            PhotonNetwork.ManagerMessagePhoton.Add(optionReceive, new object[] { result });
            /*
            PhotonServerTCP.Instance.NetWorkUnity
                (
                    "NetworkInstantiate",
                    "ManagerPhotonNetworkView",
                    (byte)optionReceive,
                    new object[]
                    {
                        result
                    }
                );*/
        }

        internal static GameObject Instantiate(string namePrefab, Vector3 position, Quaternion rotation)
        {
            GameObject gameobject = CreateGameObject(namePrefab, position, rotation);
            if (gameobject != null)
            {
                object[] descriptionObject = null;
                if (ManagerPhotonNetworkView.TryGetDescription(gameobject, out descriptionObject))
                {
                    descriptionObject[(int)CodeDescriptionGameObject.namePrefab] = (string)namePrefab;
                    string key = (string)descriptionObject[(int)CodeDescriptionGameObject.GUID];
//UnityEngine.Debug.Log("Добавление!!! Instantiate GUID: " + key);
                    if (!ManagerPhotonNetworkView.dicNetworkObject.ContainsKey(key))
                    {
                        ManagerPhotonNetworkView.dicNetworkObject.Add(key, new InfoGameObject(namePrefab, gameobject));
                        //Отправляем команду остальным
                        ManagerPhotonNetworkView.PhotonInstantiate(descriptionObject);
                    }
                }
            }
            return gameobject;
        }

        #endregion

        private static GameObject CreateGameObject(string namePrefab, Vector3 position, Quaternion rotation)
        {
            GameObject gameobject = null;
            if (namePrefab.IndexOf("$SCENA$") == -1)
            {
                gameobject = Resources.Load(namePrefab) as GameObject;
                if (gameobject != null)
                {
                    try
                    {
                        gameobject = Instantiate(gameobject, position, rotation) as GameObject;
                    }
                    catch { }
                }
                return gameobject;
            }
            else
            {
                gameobject = GameObject.Find(namePrefab.Replace("$SCENA$", ""));
                return gameobject;
            }
        }

        private static void LoadingSettingsGameObject(GameObject gameobject, object[] photonNetworkViews)
        {
            List<PhotonNetworkView> listPhotonNetworkViews = null;
            if (ManagerPhotonNetworkView.TryGetPhotonNetworkViews(gameobject, out listPhotonNetworkViews))
            {
                object[] array = null;
                PhotonNetworkView photonNetworkView = null;
                foreach (object obj_photonNetworkView in photonNetworkViews)
                {
                    array = (object[])obj_photonNetworkView;

                    string path = (string)array[(int)CodeDescriptionPhotonNetworkView.path];

                    string global_ID = (string)array[(int)CodeDescriptionPhotonNetworkView.global_ID];

                    photonNetworkView = listPhotonNetworkViews.Find(x => ((IPhotonComponent)x).path == path);

                    if (photonNetworkView == null) continue;
//UnityEngine.Debug.Log("Присвоено значение: old: " + ((IPhotonComponent)photonNetworkView).global_ID + "  new: " + global_ID);

                    ((IPhotonComponent)photonNetworkView).global_ID = global_ID;
                    ((IPhotonComponent)photonNetworkView).proprietor = (string)array[(int)CodeDescriptionPhotonNetworkView.proprietor]; ;

                    LoadingSettingsPhotonBehaviour(photonNetworkView.photonBehaviours, (object[])array[(int)CodeDescriptionPhotonNetworkView.photonBehaviour]);
                }
            }
        }

        private static void LoadingSettingsPhotonBehaviour(List<PhotonBehaviour> listPhotonBehaviours, object[] photonBehaviours)
        {
            object[] array = null;
            PhotonBehaviour photonBehaviour = null;
            foreach (object obj_photonBehaviour in photonBehaviours)
            {
                array = (object[])obj_photonBehaviour;
                string path = (string)array[(int)CodeDescriptionPhotonBehaviour.path];
                string global_ID = (string)array[(int)CodeDescriptionPhotonBehaviour.global_ID];
                photonBehaviour = listPhotonBehaviours.Find(x => ((IPhotonComponent)x).path == path);
                if (photonBehaviour == null) continue;
                ((IPhotonComponent)photonBehaviour).global_ID   = global_ID;
                ((IPhotonComponent)photonBehaviour).proprietor  = (string)array[(int)CodeDescriptionPhotonBehaviour.proprietor];
                photonBehaviour.SET_Values((object[])array[(int)CodeDescriptionPhotonBehaviour.values]);
            }
        }

        /// <summary>
        /// Получает описание по объекту
        /// </summary>
        /// <param name="gameobject">объект</param>
        /// <param name="descriptionObject">описание</param>
        /// <returns></returns>
        private static bool TryGetDescription(GameObject gameobject, out object[] descriptionObject)
        {
            descriptionObject = null;
            List<PhotonNetworkView> listPhotonNetworkViews = null;
//UnityEngine.Debug.Log("Сканирование: gameObject - " + gameobject.name);
            if (ManagerPhotonNetworkView.TryGetPhotonNetworkViews(gameobject, out listPhotonNetworkViews))
            {
                //Описание объекта
                descriptionObject = new object[Enum.GetNames(typeof(CodeDescriptionGameObject)).Length];

                descriptionObject[(int)CodeDescriptionGameObject.command] = (byte)CodeCommands.instantiate;
                descriptionObject[(int)CodeDescriptionGameObject.position_X] = (float)gameobject.transform.position.x;
                descriptionObject[(int)CodeDescriptionGameObject.position_Y] = (float)gameobject.transform.position.y;
                descriptionObject[(int)CodeDescriptionGameObject.position_Z] = (float)gameobject.transform.position.z;
                descriptionObject[(int)CodeDescriptionGameObject.rotation_X] = (float)gameobject.transform.rotation.x;
                descriptionObject[(int)CodeDescriptionGameObject.rotation_Y] = (float)gameobject.transform.rotation.y;
                descriptionObject[(int)CodeDescriptionGameObject.rotation_Z] = (float)gameobject.transform.rotation.z;
                descriptionObject[(int)CodeDescriptionGameObject.rotation_W] = (float)gameobject.transform.rotation.w;
                descriptionObject[(int)CodeDescriptionGameObject.GUID] = ((IPhotonComponent)listPhotonNetworkViews[0]).global_ID;

                object[] descriptionPhotonNetworkViews = new object[listPhotonNetworkViews.Count];
                descriptionObject[(int)CodeDescriptionGameObject.photonNetworkView] = descriptionPhotonNetworkViews;
                object[] kvp_key = null;
                PhotonNetworkView photonNetworkView = null;
                //начинаем заполнение PhotonNetworkView
                for (int i = 0; i < listPhotonNetworkViews.Count; i++)
                {
                    photonNetworkView = listPhotonNetworkViews[i];

                    kvp_key = new object[Enum.GetNames(typeof(CodeDescriptionPhotonNetworkView)).Length];
                    kvp_key[(int)CodeDescriptionPhotonNetworkView.path] =       ((IPhotonComponent)photonNetworkView).path;          //ПУТЬ ДО СКРИПТА
                    kvp_key[(int)CodeDescriptionPhotonNetworkView.global_ID] =  ((IPhotonComponent)photonNetworkView).global_ID;     //глобальный идентификатор
                    kvp_key[(int)CodeDescriptionPhotonNetworkView.local_ID] =   ((IPhotonComponent)photonNetworkView).local_ID;      //локальный идентификатор
                    kvp_key[(int)CodeDescriptionPhotonNetworkView.proprietor] = ((IPhotonComponent)photonNetworkView).proprietor;    //хозяин
                    kvp_key[(int)CodeDescriptionPhotonNetworkView.photonBehaviour] = ManagerPhotonNetworkView.DescriptionPhotonBehaviour(photonNetworkView.photonBehaviours);

                    descriptionPhotonNetworkViews[i] = kvp_key;
                }
                return true;
            }
            return false;
        }

        private static object[] DescriptionPhotonBehaviour(List<PhotonBehaviour> photonBehaviours)
        {
            object[] dicPhotonBehaviours = new object[photonBehaviours.Count];
            //Описание PhotonBehaviour
            object[] kvp_key = null;
            PhotonBehaviour photonBehaviour = null;
            try
            {
                for (int i = 0; i < photonBehaviours.Count; i++)
                {
                    photonBehaviour = photonBehaviours[i];

                    kvp_key = new object[Enum.GetNames(typeof(CodeDescriptionPhotonBehaviour)).Length];
                    kvp_key[(int)CodeDescriptionPhotonBehaviour.path]       = ((IPhotonComponent)photonBehaviour).path;         //ПУТЬ ДО СКРИПТА
                    kvp_key[(int)CodeDescriptionPhotonBehaviour.global_ID]  = ((IPhotonComponent)photonBehaviour).global_ID;    //глобальный идентификатор
                    kvp_key[(int)CodeDescriptionPhotonBehaviour.local_ID]   = ((IPhotonComponent)photonBehaviour).local_ID;     //локальный идентификатор
                    kvp_key[(int)CodeDescriptionPhotonBehaviour.proprietor] = ((IPhotonComponent)photonBehaviour).proprietor;   //хозяин
                    kvp_key[(int)CodeDescriptionPhotonBehaviour.values]     = photonBehaviour.GET_Values();

                    dicPhotonBehaviours[i] = kvp_key;
                }
            }catch(Exception ex)
            {
                UnityEngine.Debug.LogException(ex);
            }
            return dicPhotonBehaviours;
        }

        private static bool TryGetPhotonNetworkViews(GameObject gameObject, out List<PhotonNetworkView> listPhotonNetworkViews)
        {
            listPhotonNetworkViews = new List<PhotonNetworkView>();
            try
            {
//UnityEngine.Debug.Log("!!! ПОИСК Дочерних компонентов типа PhotonBehaviour !!!");
                //ПОИСК Дочерних компонентов типа PhotonBehaviour
                Component[] components = gameObject.GetComponentsInChildren(typeof(PhotonBehaviour));

                PhotonBehaviour photonBehaviour = null;
                PhotonNetworkView photonNetworkView = null;
                foreach (Component component in components)
                {
                    photonBehaviour = component as PhotonBehaviour;
                    if (photonBehaviour == null) continue;
                    if (photonBehaviour.mainPhotonNetworkView(out photonNetworkView))
                    {
                        if (String.IsNullOrEmpty(((IPhotonComponent)photonBehaviour).proprietor))
                        {
                            ((IPhotonComponent)photonBehaviour).path        = photonBehaviour.transform.Path();
                            ((IPhotonComponent)photonBehaviour).proprietor  = PhotonNetworkView.global_PlayerID.ToString();
                            ((IPhotonComponent)photonBehaviour).global_ID   = ((IPhotonComponent)photonBehaviour).local_ID.ToString();
                            if (photonNetworkView.Add(photonBehaviour))
                            {
                                photonBehaviour.InitializationAttributtes();
                            }
                        }
                        if (!listPhotonNetworkViews.Contains(photonNetworkView)) listPhotonNetworkViews.Add(photonNetworkView);
                    }
                }
                foreach (PhotonNetworkView _photonNetworkView in listPhotonNetworkViews)
                {
                    if (String.IsNullOrEmpty(((IPhotonComponent)_photonNetworkView).proprietor))
                    {
                        ((IPhotonComponent)_photonNetworkView).path = _photonNetworkView.transform.Path();
                        ((IPhotonComponent)_photonNetworkView).global_ID    = PhotonNetworkView.global_PlayerID.ToString() + "-" + ((IPhotonComponent)_photonNetworkView).local_ID.ToString();
                        ((IPhotonComponent)_photonNetworkView).proprietor   = PhotonNetworkView.global_PlayerID.ToString();
                    }
                }
                return listPhotonNetworkViews.Count != 0;
            }
            catch(Exception ex)
            {
                UnityEngine.Debug.LogException(ex);
                return false;
            }
        }

        /// <summary>
        /// Метод удаления сетевых клонов
        /// </summary>
        /// <param name="gameObject">удаляемый объект</param>
        /// <returns>True - удачно, иначе False</returns>
        internal static bool Destroy(GameObject gameObject)
        {
            try
            {
                if (gameObject != null)
                {
                    object[] descriptionObject = null;
                    if (ManagerPhotonNetworkView.TryGetDescription(gameObject, out descriptionObject))
                    {
                        string key = (string)descriptionObject[(int)CodeDescriptionGameObject.GUID];
                        InfoGameObject infoGameObject = null;
                        if (ManagerPhotonNetworkView.dicNetworkObject.TryGetValue(key, out infoGameObject))
                        {
                            UnityEngine.Object.Destroy(infoGameObject.gameObject);
                            ManagerPhotonNetworkView.dicNetworkObject.Remove(key);
                            ManagerPhotonNetworkView.PhotonDestroy(key);
                            return true;
                        }
                    }
                }
            }
            catch { }
            return false;
        }

        void OnApplicationQuit()
        {
            try
            {
                ManagerMessagePhoton.SendBufferListOUT();
            }
            catch { }
            try
            {
                ManagerPhotonNetworkView.buffer_PhotonNetworkView.Clear();
            }
            catch { }

            try
            {
                ManagerPhotonNetworkView.dicPhotonNetworkView.Clear();
            }
            catch { }

            try
            {
                ManagerPhotonNetworkView.dicNetworkObject.Clear();
            }
            catch { }
        }

        void OnDestroy()
        {
            try
            {
                ManagerMessagePhoton.SendBufferListOUT();
            }
            catch { }
            try
            {
                ManagerPhotonNetworkView.buffer_PhotonNetworkView.Clear();
            }
            catch { }

            try
            {
                ManagerPhotonNetworkView.dicPhotonNetworkView.Clear();
            }
            catch { }

            try
            {
                ManagerPhotonNetworkView.dicNetworkObject.Clear();
            }
            catch { }
        }

        void FixedUpdate()
        {
            ManagerMessagePhoton.SendBufferListOUT();
        }
    }
}
