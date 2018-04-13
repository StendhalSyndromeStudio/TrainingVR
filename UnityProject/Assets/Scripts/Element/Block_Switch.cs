using UnityEngine;
using System.Collections;
using UnityClientKSGT;
using System;
using PhotonNetwork;

namespace Element {
    public class Block_Switch : PhotonBehaviour {
        [System.Serializable]
        public class BlokSwitchParam {
            public Vector3 rot;
            public Vector3 pos;
            public string Value;
        }

        [SerializeField]
        Transform transformSwitch;
        [SerializeField]
        BlokSwitchParam[ ] offsetSwitch;
        //[SerializeField]
        public bool isLoop = false;

        public int indexStart = 0;
        private Vector3 posStart;
        private Vector3 mousePose = Vector3.zero;
        private int offset = 0;

        private int indexCurr = -1;

        private EventValueType<int> Position = new EventValueType<int>( -1 );

        [PhotonNetwork.Photon]
        public EventValueType<string> State = new EventValueType<string>( String.Empty );

        [ Header( "Для перемещения снять" )]
        public bool isMove = true;

        
        void Awake( ) {
            try {
                if ( transformSwitch == null ) {
                    transformSwitch = this.transform;
                }

                posStart = transformSwitch.localPosition;

                if ( transformSwitch.gameObject.GetComponent<Collider>( ) == null ) {
                    MeshCollider mc = transformSwitch.gameObject.AddComponent<MeshCollider>( );
                    mc.convex = true;
                    mc.isTrigger = true;
                }
            }
            catch ( Exception ex ) { Debug.LogError( "name: " + this.gameObject.name + "  Message: " + ex.ToString( ) ); }
        }

        void Start( ) {
            // задаём стартовое значение        
            this.Position.AddingEvent( this, this.OnChange_Switch );
            this.State.AddingEvent( this, this.OnChange_State );
            this.Position.Value = this.indexStart;
            this.State.ReStart( this );
        }
        /*
        void OnMouseOver() {
            if (Input.GetMouseButtonDown(1))
            {
                if (isLoop && indexCurr == 0)
                {
                    this.Position.Value = offsetSwitch.Length - 1;
                }
                else
                {
                    this.Position.Value = indexCurr - 1;
                }
            }
        }
        */
        void eventMouseDown( ) {
            mousePose = Input.mousePosition;
        }

        void eventMouseUp( ) {
            offset = ( int ) Mathf.Sign( Input.mousePosition.x - mousePose.x );
            //Debug.Log( string.Format( "offset: {0}, index: {0}", offset, indexCurr ) );

            if ( isLoop ) {
                if ( indexCurr == offsetSwitch.Length - 1 && offset == 1 ) {
                    offset = -( offsetSwitch.Length - 1 );
                }
                if ( indexCurr == 0 && offset <= 0 ) {
                    offset = offsetSwitch.Length - 1;
                }
            }
            this.Position.Value = indexCurr + offset;
        }
        /*
        /// <summary>
        /// событие при клике на объекте
        /// </summary>
        /// <param name="button"></param>
        void eventMouseClick( EventMouse.CodeKey button ) {
            offset = button == EventMouse.CodeKey.left ? 1 : -1;
            //Debug.Log( string.Format( "offset: {0}, index: {0}", offset, indexCurr ) );

            if ( isLoop ) {
                if ( indexCurr == offsetSwitch.Length - 1 && offset == 1 ) {
                    offset = -( offsetSwitch.Length - 1 );
                }
                if ( indexCurr == 0 && offset <= 0 ) {
                    offset = offsetSwitch.Length - 1;
                }
            }
            this.Position.Value = indexCurr + offset;
        }
        */
        void Rendering_Switch( int val ) {
            try {
                // для того чтобы не было рекурсии по событию
                if ( val == indexCurr ) {
                    return;
                }

                indexCurr = Mathf.Clamp( val, 0, offsetSwitch.Length - 1 );
                transformSwitch.localEulerAngles = offsetSwitch[ indexCurr ].rot;
                if ( this.isMove ) {
                    transformSwitch.localPosition = posStart + offsetSwitch[ indexCurr ].pos;
                }
                else {
                    transformSwitch.localPosition = offsetSwitch[ indexCurr ].pos;
                }

                this.Position.Value = indexCurr;//val;
                this.State.Value = offsetSwitch[ indexCurr ].Value;
            }
            catch ( Exception ex ) { Debug.LogError( "name: " + this.gameObject.name + "  Message: " + ex.ToString( ) ); }
        }

        private void OnChange_Switch( object sender, object value ) {
            this.Rendering_Switch( ( int ) value );
            //Debug.Log( string.Format( "{0}: OnChange_Switch -> {1}", this.name, value ) );
        }

        private void OnChange_State( object sender, object value ) {
            int index = Array.FindIndex( this.offsetSwitch, x => x.Value == value.ToString( ) );
            //Debug.Log( string.Format( "{0}: OnChange_State -> {1}", this.name, index ) );
            if ( index == -1 ) {
                return;
            }
            this.Position.Value = index;
        }
    }
}
