using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour {

    float smooth = 100.0f;
    float tiltAngle = 0.0f;
    private float step = 10f;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Globals.GlobalVar.dialLeft == 1)
        {
           if (Globals.GlobalVar.ControllerTrialTypeIndex != 4 && Globals.GlobalVar.ControllerTrialTypeIndex != 5 && Globals.GlobalVar.ControllerTrialTypeIndex != 6)
            {
                if (Globals.GlobalVar.TrialPhase == "WaitEnd") //WaitRecord, WaitStart, WaitEnd, Over
                {
                    //Set report Button to Press to End Trial (2)
                    setReportButtonState(2); // 0-Wait | 1-Turn Around | 2-Press to End
                }
            }
            
            Globals.GlobalVar.dialLeft = 0;
            tiltAngle = tiltAngle + step;
            RotateObject(tiltAngle);
            Globals.GlobalVar.dialAccum += 1f;
        }
        else if (Globals.GlobalVar.dialRight == 1)
        {
            if (Globals.GlobalVar.ControllerTrialTypeIndex != 4 && Globals.GlobalVar.ControllerTrialTypeIndex != 5 && Globals.GlobalVar.ControllerTrialTypeIndex != 6)
            {
                if (Globals.GlobalVar.TrialPhase == "WaitEnd") //WaitRecord, WaitStart, WaitEnd, Over
                {
                    //Set report Button to Press to End Trial (2)
                    setReportButtonState(2); // 0-Wait | 1-Turn Around | 2-Press to End
                }
            }

            Globals.GlobalVar.dialRight = 0;
            tiltAngle = tiltAngle - step;
            RotateObject(tiltAngle);
            Globals.GlobalVar.dialAccum -= 1f;
        }

    }

    void RotateObject (float tiltAroundZ)
    {
        // Smoothly tilts a transform towards a target rotation tiltAroundZ.

        // Rotate the quad by converting the angles into a quaternion.
        Quaternion target = Quaternion.Euler(90,0, tiltAroundZ);

        // Dampen towards the target rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * smooth);

        //transform.Rotate(0, 0, tiltAngle, Space.Self);
       // Debug.Log("tilt angle: " + tiltAngle);
    }

    void setReportButtonState(int state) // 0-Wait | 1-Turn Around | 2-Press to End
    {
        Globals.GlobalVar.OscReportButtonValue = state;
        Globals.GlobalVar.OscReportButtonEvent = true;
        Globals.GlobalVar.LocalReportButtonEvent = true; //Read by BTMessageControl
        Debug.Log("Rotate::setReportButtonStat: set state: " + state);
    }
}
