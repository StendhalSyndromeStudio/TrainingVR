using UnityEngine;
using System.Collections;

namespace PhotonNetwork
{
    public class PhotonTransform : PhotonBehaviour
    {
        #region Позиция
        [Header("Синхронизация позиции")]
        [SerializeField]
        Transform transformPosition;

        [Header("Использовать сглаживание")]
        [SerializeField]
        bool lerpPosition = false;

        //[PhotonNetwork.Photon]
        [PhotonNetwork.PhotonDelta(0.01f)]
        [PhotonNetwork.PhotonTime(30f)]
        [PhotonNetwork.PhotonUpdate("UpdatePosition")]      // вызов метода после изменения значения
        Vector3 positionNew = Vector3.zero;                 // позиция на нововм шаге

        Vector3 positionOld = Vector3.zero;                 // позиция на предыдущем шаге
        float positionTimeNew;                              // время прихода нового сообщения
        float positionTimeOld;                              // время прихода предыдущего сообщения

        #endregion

        #region Ориентация
        [Header("Синхронизация ориентации")]
        [SerializeField]
        Transform transformRotation;

        [Header("Использовать сглаживание")]
        [SerializeField]
        bool lerpRotation = false;

        [PhotonNetwork.PhotonDelta(0.01f)]
        [PhotonNetwork.PhotonTime(30f)]
        [PhotonNetwork.PhotonUpdate("UpdateRotation")]      // вызов метода после изменения значения
        Quaternion rotationNew = Quaternion.identity;       // ориентация на нововм шаге

        Quaternion rotationOld = Quaternion.identity;       // ориентация на предыдущем шаге
        float rotationTimeNew;                              // время прихода нового сообщения
        float rotationTimeOld;                              // время прихода предыдущего сообщения

        #endregion

        [PhotonNetwork.PhotonDelta( 0.01f )]
        [PhotonNetwork.PhotonTime( 30f )]
        public Vector3 scale {
            get { return this.transformPosition.localScale; }
            set { this.transformPosition.localScale = value; }
        }


        float lerpDelta;

        void Start()
        {
            //enabled = this.isOwner.Value != CodeOwner.@true;
        }

        void Update()
        {
            if (this.isOwner.Value == CodeOwner.@true)
            {
                return;
            }
            if (lerpPosition)
            {
                LerpPosition();
            }
            if (lerpRotation)
            {
                LerpRotation();
            }
        }

        void LateUpdate()
        {
            if (this.isOwner.Value == CodeOwner.@true)
            {
                positionNew = transformPosition.localPosition;
                rotationNew = transformRotation.localRotation;
            }
        }

        /// <summary>
        /// синхронизация положения
        /// </summary>
        /// <param name="value"></param>
        [PhotonNetwork.Photon]
        void UpdatePosition(object value)
        {
            if (lerpPosition)
            {
                positionTimeOld = positionTimeNew;
                positionTimeNew = Time.realtimeSinceStartup;
                positionOld = transformPosition.localPosition;
            }
            else
            {
                transformPosition.localPosition = positionNew;
            }
        }

        void LerpPosition()
        {
            lerpDelta = Mathf.Clamp((Time.realtimeSinceStartup - positionTimeNew) / (positionTimeNew - positionTimeOld), 0f, 1f);
            transformPosition.localPosition = Vector3.Lerp(positionOld, positionNew, lerpDelta);
            //this.rigidbody.MovePosition(tpos);
        }



        /// <summary>
        /// синхронизация ориентации
        /// </summary>
        /// <param name="value"></param>
        [PhotonNetwork.Photon]
        void UpdateRotation(object value)
        {
            if (lerpRotation)
            {
                rotationTimeOld = rotationTimeNew;
                rotationTimeNew = Time.realtimeSinceStartup;
                rotationOld = transformRotation.localRotation;
            }
            else
            {
                transformRotation.localRotation = rotationNew;
            }
        }

        void LerpRotation()
        {
            lerpDelta = Mathf.Clamp((Time.realtimeSinceStartup - rotationTimeNew) / (rotationTimeNew - rotationTimeOld), 0f, 1f);
            transformRotation.localRotation = Quaternion.Slerp(rotationOld, rotationNew, lerpDelta);
            /*
            Quaternion stepRotation = Quaternion.Slerp(rotationOld, rotationNew, lerpDelta);
            Vector3 deltaRotation = stepRotation.eulerAngles - transformRotation.localEulerAngles;
            this.rigidbody.MoveRotation(this.transform.rotation * Quaternion.Euler(deltaRot));
            */
        }
    }
}