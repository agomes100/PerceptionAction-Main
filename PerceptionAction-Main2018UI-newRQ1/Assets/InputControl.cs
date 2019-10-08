using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputControl : MonoBehaviour {
    public bool activate_keyboard = false;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (activate_keyboard) { 
        if (Input.GetKey("left"))
        {
            Globals.GlobalVar.dialLeft = 1;
            Debug.Log("InputControl::DialRotate:Left");
        }
        else if (Input.GetKey("right"))
        {
            Globals.GlobalVar.dialRight = 1;
            Debug.Log("InputControl::DialRotate:Right");
        }
        else if (Input.GetKey("space"))
        {
            Globals.GlobalVar.dialClick = 1;
            Debug.Log("InputControl::DialClick");
        }
        else if (Input.GetKey("1"))
        {
            Globals.GlobalVar.dialSizeUpdate = true;
            Globals.GlobalVar.dialSizeRef = 1;
            Debug.Log("InputControl::DialSizeRef=1");
        }
        else if (Input.GetKey("2"))
        {
            Globals.GlobalVar.dialSizeUpdate = true;
            Globals.GlobalVar.dialSizeRef = 2;
            Debug.Log("InputControl::DialSizeRef=2");
        }
        else if (Input.GetKey("3"))
        {
            Globals.GlobalVar.dialSizeUpdate = true;
            Globals.GlobalVar.dialSizeRef = 3;
            Debug.Log("InputControl::DialSizeRef=3");
        }
        else if (Input.GetKey("4"))
        {
            Globals.GlobalVar.dialSizeUpdate = true;
            Globals.GlobalVar.dialSizeRef = 4;
            Debug.Log("InputControl::DialSizeRef=4");
        }
        else if (Input.GetKey("5"))
        {
            Globals.GlobalVar.dialSizeUpdate = true;
            Globals.GlobalVar.dialSizeRef = 5;
            Debug.Log("InputControl::DialSizeRef=5");
        }
        else if (Input.GetKey("6"))
        {
            Globals.GlobalVar.dialSizeUpdate = true;
            Globals.GlobalVar.dialSizeRef = 6;
            Debug.Log("InputControl::DialSizeRef=6");
        }
        else if (Input.GetKey("7"))
        {
            Globals.GlobalVar.dialSizeUpdate = true;
            Globals.GlobalVar.dialSizeRef = 7;
            Debug.Log("InputControl::DialSizeRef=7");
        }
        else if (Input.GetKey("8"))
        {
            Globals.GlobalVar.dialSizeUpdate = true;
            Globals.GlobalVar.dialSizeRef = 8;
            Debug.Log("InputControl::DialSizeRef=8");
        }
        else if (Input.GetKey("9"))
        {
            Globals.GlobalVar.dialSizeUpdate = true;
            Globals.GlobalVar.dialSizeRef = 9;
            Debug.Log("InputControl::DialSizeRef=9");
        }
        else if (Input.GetKey("0"))
        {
            Globals.GlobalVar.dialSizeUpdate = true;
            Globals.GlobalVar.dialSizeRef = 0;
            Debug.Log("InputControl::DialSizeRef=0");
        }
        else if (Input.GetKey("s"))
        {
            Globals.GlobalVar.dialShowHideUpdate = true;
            Globals.GlobalVar.dialShowHide = 1;
            Debug.Log("InputControl::dialShowHide=1 - show");
        }
        else if (Input.GetKey("h"))
        {
            Globals.GlobalVar.dialShowHideUpdate = true;
            Globals.GlobalVar.dialShowHide = 0;
            Debug.Log("InputControl::dialShowHide=0 - hide");
        }
        }

    }
}

