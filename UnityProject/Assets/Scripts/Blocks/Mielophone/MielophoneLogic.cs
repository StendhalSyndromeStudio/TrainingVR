using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MielophoneLogic : MonoBehaviour {

    public Transform antennAzimut;
    public Transform antennAnglePlace;

    bool isGameStopped = false;
    private int timer = 0;
    int blinkOk = 0;
    int blinkNo = 1;
    IEnumerator BlinkOkNo( ) {
        while ( true ) {
            if ( !isGameStopped ) {
                blinkOk = ( blinkOk == 0 ) ? 1 : 0;
                blinkNo = ( blinkNo == 0 ) ? 1 : 0;
                diods [ ( int ) Diods.DiodOk ].onChangeStatus( blinkOk );
                diods [ ( int ) Diods.DiodNo ].onChangeStatus( blinkNo );
                yield return new WaitForSeconds( 0.5f );
            }

            yield return null;
        }
    }
    bool rotate = false;
    IEnumerator toLeft( ) {
        while ( rotate ) {
            if ( antennAzimut != null )
                antennAzimut.Rotate( new Vector3( 0, 1, 0 ) );
            makeData( _azimut--, _anglePlace );
            yield return new WaitForSeconds( 0.05f );
        }
        yield return null;
    }

    IEnumerator toRight( ) {
        while ( rotate ) {
            if ( antennAzimut != null )
                antennAzimut.Rotate( new Vector3( 0, -1, 0 ) );
            makeData( _azimut++, _anglePlace );
            yield return new WaitForSeconds( 0.05f );
        }
        yield return null;
    }

    IEnumerator toUp( ) {
        while ( rotate ) {
            if ( antennAnglePlace != null )
                antennAnglePlace.Rotate( new Vector3( -1, 0, 0 ) );
            makeData( _azimut, _anglePlace++ );
            yield return new WaitForSeconds( 0.05f );
        }
        yield return null;
    }

    IEnumerator toDown( ) {
        while ( rotate ) {
            if ( antennAnglePlace != null )
                antennAnglePlace.Rotate( new Vector3( 1, 0, 0 ) );
            makeData( _azimut, _anglePlace-- );
            yield return new WaitForSeconds( 0.05f );
        }
        yield return null;
    }

    public List<Diod> diods = new List<Diod>( );

    public List<Btn> btns = new List<Btn>( );

    public Switch switchVkl;

    public Text display;

    private const string MIELOPHONE = "МИЕЛОФЕН-01";

    enum Btns {
        BtnDown,
        BtnLeft,
        BtnRight,
        BtnStop,
        BtnUp
    }

    enum Diods {
        DiodDown,
        DiodNo,
        DiodOk,
        DiodRight,
        DiodTop,
        DiodWait,
        DiodLeft,
        DiodCrash
    }

    int _azimut = 0;
    int _anglePlace = 0;

	// Use this for initialization
	void Start () {
        onChangePower( false );
        switchVkl.onChangeSwitch += this.onVkl;
        btns [ ( int ) Btns.BtnDown ].onPress += onBtnDownPressed;
        btns [ ( int ) Btns.BtnLeft ].onPress += onBtnLeftPressed;
        btns [ ( int ) Btns.BtnRight ].onPress += onBtnRightPressed;
        btns [ ( int ) Btns.BtnStop ].onPress += onBtnStopPressed;
        btns [ ( int ) Btns.BtnUp ].onPress += onBtnUpPressed;

    }

    void makeData( int azimut, int anglePlace ) {
        string strAzimut = "Азимут:" + azimut.ToString( );
        string strAnglePlace = "Угол места:" + anglePlace.ToString( );
        display.text = strAzimut + "\n" + strAnglePlace;
    }

    bool power = false;
    void onVkl( int value ) {
        power = value == 1;
        onChangePower( power );
    }

    void onBtnLeftPressed( bool value  ) {
        if ( !power )
            return;

        if ( value ) {
            rotate = true;
            StartCoroutine( toLeft( ) );
            diods [ ( int ) Diods.DiodLeft ].onChangeStatus( 1 );
        }
        else {
            rotate = false;
            
            diods [ ( int ) Diods.DiodLeft ].onChangeStatus( 0 );
        }
    }

    void onBtnRightPressed( bool value ) {
        if ( !power )
            return;
        if ( value ) {
            rotate = true;
            StartCoroutine( toRight( ) );
            diods [ ( int ) Diods.DiodRight ].onChangeStatus( 1 );
        } else {
            rotate = false;
            diods [ ( int ) Diods.DiodRight ].onChangeStatus( 0 );
        }
    }

    void onBtnDownPressed( bool value ) {
        if ( !power )
            return;
        if ( value ) {
            rotate = true;
            StartCoroutine( toDown( ) );
            diods [ ( int ) Diods.DiodDown ].onChangeStatus( 1 );
        } else {
            rotate = false;
            diods [ ( int ) Diods.DiodDown ].onChangeStatus( 0 );
        }
    }

    void onBtnUpPressed( bool value ) {
        if ( !power )
            return;
        if ( value ) {
            rotate = true;
            StartCoroutine( toUp( ) );
            diods [ ( int ) Diods.DiodTop ].onChangeStatus( 1 );
        } else {
            rotate = false;
            diods [ ( int ) Diods.DiodTop ].onChangeStatus( 0 );
        }
    }

    void onBtnStopPressed( bool value ) {
        if ( !power )
            return;
        if ( value ) {
            
        } else {

        }
    }

    void onChangePower( bool value ) {
        if ( value ) {
            display.text = MIELOPHONE;
            makeData( _azimut, _anglePlace );
            StartCoroutine( BlinkOkNo( ) );
        }
        else {
            StopAllCoroutines( );
            display.text = string.Empty;
            diods [ ( int ) Diods.DiodOk ].onChangeStatus( 0 );
            diods [ ( int ) Diods.DiodNo ].onChangeStatus( 0 );
        }
    }
}
