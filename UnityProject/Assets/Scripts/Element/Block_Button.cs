using UnityEngine;
using System.Collections;
using UnityClientKSGT;
using PhotonNetwork;
using System;

namespace Element {
    public class Block_Button : PhotonBehaviour {


        [SerializeField]
        Transform transformButton;

        [Header( "Смещение перемещения" )]
        [SerializeField]
        Vector3 offsetDown = Vector3.zero;

        [Header( "Смещение вращения" )]
        [SerializeField]
        Vector3 offsetAngle = Vector3.zero;

        [Header( "Признак что фиксируемая" )]
        [SerializeField]
        bool lockable = false;

        public bool IsLockable {
            get {
                return lockable;
            }
        }

        [SerializeField]
        KeyCode pressedKey = KeyCode.RightControl;

        static public bool usePressedMode = false;

        private Vector3 startPosition;
        private Quaternion startRotation;
        private bool isPressed = false;                     // признак что кнопка была зажата

        private bool isDown = false;                        // признак что кнопку нажали
        public bool _isDown {
            get { return isDown; }
            set { ChangeButton( value ); }
        }

        public delegate void ButtonDown( bool val );
        public delegate void delEventButton<T>( object sender, T value );

        public event ButtonDown _ButtonDown;
        public event delEventButton<bool> EventButton = null;

        [PhotonNetwork.Photon]
        public EventValueType<bool> StatusButton = new EventValueType<bool>( false );


        void Awake( ) {
            if ( transformButton == null ) {
                transformButton = this.transform;
            }

            startPosition = transformButton.localPosition;
            startRotation = transformButton.localRotation;

            if ( transformButton.gameObject.GetComponent<Collider>( ) == null ) {
                MeshCollider mc = transformButton.gameObject.AddComponent<MeshCollider>( );
                mc.convex = true;
                mc.isTrigger = true;
            }

            this.StatusButton.AddingEvent( this, this.StatusButton_Change );

        }

       
        void Start( ) {

        }

        void OnMouseDown( ) {
            if ( lockable && isPressed ) {
                return;
            }
            ChangeButton( true );
        }

        void OnMouseUp( ) {
            if ( ( lockable || Input.GetKey( pressedKey ) || usePressedMode ) && !isPressed ) {
                isPressed = true;
                return;
            }
            ChangeButton( false );
        }


        void ChangeButton( bool val ) {
            if ( val == isDown ) {
                return;
            }

            isDown = val;
           
            // генерация события
            if ( _ButtonDown != null ) {
                _ButtonDown( isDown );
            }

            if ( this.EventButton != null ) {
                this.EventButton( this, isDown );
            }

            this.StatusButton.Value = isDown;
        }

        /// <summary>
        /// МЕТОД: Изменение состояния кнопки (АНИМАЦИЯ)
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="Value"></param>
        private void StatusButton_Change( object Sender, object Value ) {
            transformButton.localPosition = startPosition + ( ( isDown ) ? offsetDown : Vector3.zero );

            if ( offsetAngle != Vector3.zero ) {
                transformButton.localRotation = startRotation * ( ( isDown ) ? Quaternion.Euler( offsetAngle ) : Quaternion.identity );
            }

            if ( !isDown ) {
                isPressed = false;
            }
        }

    }
}
