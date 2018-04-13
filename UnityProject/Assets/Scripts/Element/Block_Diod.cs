using UnityEngine;
using UnityClientKSGT;
using System;
using UnityEngine.UI;
using PhotonNetwork;

namespace Element {
    public class Block_Diod : PhotonBehaviour {

        [System.Serializable]
        public struct structStatusToColor {
            public string _Status;
            public Color _Color;
        }

        [SerializeField]
        public structStatusToColor[ ] StatusToColor;

        [PhotonNetwork.Photon]
        public EventValueType<string> State = new EventValueType<string>( String.Empty );

        private Image scriptImage;

        public GameObject parent;

        public bool isEmission = false;
        public float minEmission = 0.01f;
        public float maxEmission = 0.9f;
        private const string EmissiveValue = "_EmissionScaleUI";

        // Use this for initialization
        void Start( ) {


            if ( this.parent == null ) {
                this.parent = this.gameObject;
            }


            this.scriptImage = this.parent.GetComponent<Image>( );
            if ( this.scriptImage == null ) {
                this.State.AddingEvent( this, this.OnChange_State );
                this.State.ReStart( this );
            }
            else {
                this.State.AddingEvent( this, this.OnChange_Image );
                this.State.ReStart( this );
            }

            if ( this.isEmission ) {
                this.State.AddingEvent( this, this.OnChange_Emission );
                this.State.ReStart( this );
            }
        }


        // Update is called once per frame
        void Update( ) {

        }

        private void OnChange_State( object sender, object value ) {
            int index = Array.FindIndex( this.StatusToColor, x => x._Status == value.ToString( ) );
            if ( index == -1 )
                index = 0;
            structStatusToColor statusToColor = this.StatusToColor[ index ];
            this.parent.GetComponent<MeshRenderer>( ).materials[ 0 ].color = statusToColor._Color;
        }

        private void OnChange_Image( object sender, object value ) {
            int index = Array.FindIndex( this.StatusToColor, x => x._Status == value.ToString( ) );
            if ( index == -1 )
                index = 0;
            structStatusToColor statusToColor = this.StatusToColor[ index ];
            //this.gameObject.GetComponent<MeshRenderer>().materials[0].color = statusToColor._Color;
            this.scriptImage.color = statusToColor._Color;
        }

        private void OnChange_Emission( object Sender, object Value ) {
            try {
                int index = Array.FindIndex( this.StatusToColor, x => x._Status == Value.ToString( ) );
                if ( index == -1 )
                    index = 0;

                this.parent.GetComponent<Renderer>( ).material.EnableKeyword( "_EMISSION" );
                if ( index == 0 ) {
                    this.parent.GetComponent<Renderer>( ).material.SetColor( "_EmissionColor", this.StatusToColor[ index ]._Color * Mathf.LinearToGammaSpace( this.minEmission ) );
                }
                else {
                    this.parent.GetComponent<Renderer>( ).material.SetColor( "_EmissionColor", this.StatusToColor[ index ]._Color * Mathf.LinearToGammaSpace( this.maxEmission ) );
                }

            }
            catch ( Exception ex ) { Debug.LogError( parent.name + "->" + this.name + ": " + ex.ToString( ) ); }
        }

    }
}
