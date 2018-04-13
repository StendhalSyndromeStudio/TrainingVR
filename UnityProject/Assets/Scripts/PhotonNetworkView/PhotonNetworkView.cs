using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PhotonNetwork
{
    public class PhotonNetworkView : UnityEngine.MonoBehaviour, IPhotonComponent
    {
        #region --IPhotonComponent--
        int IPhotonComponent.local_ID
        {
            get
            {
                return this.GET_local_id();
            }
        }



        string IPhotonComponent.global_ID
        {
            get
            {
                return this.global_id;
            }
            set
            {
                this.SET_global_id(value);
            }
        }



        string IPhotonComponent.path
        {
            get
            {
                return this.path;
            }
            set
            {
                this.path = value;
            }
        }

        string IPhotonComponent.proprietor
        {
            get { return this.proprietor; }
            set { this.proprietor = value; }
        }
        #endregion

        [SerializeField]
        private int local_id = 0;
        private string global_id = "";
        private string path = "";
        private string proprietor = String.Empty;

        [UnityEngine.SerializeField]
        internal List<PhotonBehaviour> photonBehaviours = new List<PhotonBehaviour>();

        public static double global_PlayerID = 0;

        private event Action onUpdate = null;

        public PhotonNetworkView()
        {
            if (PhotonNetworkView.global_PlayerID == 0)
            {
                global_PlayerID = System.DateTime.UtcNow.ToOADate();
            }
        }

        private int GET_local_id()
        {
            if (this.local_id == 0)
            {
                this.local_id = ManagerPhotonNetworkView.Initialization(this);
            }
            return this.local_id;
        }

        private void SET_global_id(string value)
        {
            if ((String.IsNullOrEmpty(this.proprietor)) || (this.proprietor == PhotonNetworkView.global_PlayerID.ToString()))
            {
                if (this.global_id != value)
                {
//UnityEngine.Debug.Log("SET_global_id: " + this.global_id + "   " + value);
                    ManagerPhotonNetworkView.Remove(this, this.global_id);
                    this.global_id = value;
                    ManagerPhotonNetworkView.Add(this, this.global_id);
                }
            }
        }

        internal bool Add(PhotonBehaviour photonBehaviour)
        {
            if (this.photonBehaviours.Contains(photonBehaviour)) return false;
            this.photonBehaviours.Add(photonBehaviour);
            this.onUpdate += photonBehaviour.OnUpdate;
            photonBehaviour.onChange_Value += this.OnChange_Value;
            photonBehaviour.onRPC_Method += this.OnRPC_Method;
            return true;
        }


        void Start()
        {
            ManagerPhotonNetworkView.AddBufferByServer(this);
        }

        void Update()
        {
            if (this.onUpdate == null) return;
            this.onUpdate();
        }

        /// <summary>
        /// Отправка метода
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="nameMethod"></param>
        /// <param name="optionReceive"></param>
        /// <param name="parameters"></param>
        private void OnRPC_Method(IPhotonComponent sender, string nameMethod, Rmode optionReceive, params object[] parameters)
        {
            ManagerPhotonNetworkView.OnRPC_Method(this.global_id, sender.global_ID, nameMethod, optionReceive, parameters);
        }

        private void OnChange_Value(IPhotonComponent sender, string name_value, object value)
        {
            ManagerPhotonNetworkView.UpdateValue(this.global_id, sender.global_ID, name_value, value);
        }

        internal void SET_Value(string id_photonBehaviour, string name_value, object value)
        {
            try
            {
                PhotonBehaviour photonBehaviour = this.photonBehaviours.Find(x => ((IPhotonComponent)x).global_ID == id_photonBehaviour);
                photonBehaviour.SET_Value(name_value, value);
            }
            catch (Exception ex) { UnityEngine.Debug.LogException(ex); }
        }

        internal void SET_RPCMethod(string id_photonBehaviour, string player, string nameMethod, object[] parameters = null)
        {
            PhotonBehaviour photonBehaviour = this.photonBehaviours.Find(x => ((IPhotonComponent)x).global_ID == id_photonBehaviour);
            photonBehaviour.SET_RPCMethod(nameMethod, parameters);

        }

        internal static GameObject Instantiate(string namePrefab, Vector3 position, Quaternion rotation)
        {
            return ManagerPhotonNetworkView.Instantiate(namePrefab, position, rotation);
        }

        internal static bool Destroy(GameObject gameObject)
        {
            return ManagerPhotonNetworkView.Destroy(gameObject);
        }

    }
}
