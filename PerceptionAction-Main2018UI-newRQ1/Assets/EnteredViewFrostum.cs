using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnteredViewFrostum : MonoBehaviour
{
    void OnBecameVisible()
    {
        Globals.GlobalVar.knobIsVisible = true;
        Debug.Log("Knob became visible!");

        if (Globals.GlobalVar.ControllerTrialTypeIndex == 4 || Globals.GlobalVar.ControllerTrialTypeIndex == 5 || Globals.GlobalVar.ControllerTrialTypeIndex == 6)
        {
            Debug.Log("EnteredViewFrostum::Entered");
            if (Globals.GlobalVar.TestPhase != "Calibration phase")
            {
                if (Globals.GlobalVar.TrialPhase == "WaitEnd") //WaitRecord, WaitStart, WaitEnd, Over
                {
                    //Set report Button to Press to End Trial (2)
                    Debug.Log("EnteredViewFrostum::set Report button to 2-Press to End");
                    setReportButtonState(2); // 0-Wait | 1-Turn Around | 2-Press to End
                }
            }

        }


    }

    void OnBecameInvisible()
    {
        Globals.GlobalVar.knobIsVisible = false;
        Debug.Log("Knob became INvisible!");
    }
    // Update is called once per frame
    void Update()
    {
        

        

        

    }

    void setReportButtonState(int state) // 0-Wait | 1-Turn Around | 2-Press to End
    {
        Globals.GlobalVar.OscReportButtonValue = state;
        Globals.GlobalVar.OscReportButtonEvent = true;
        Globals.GlobalVar.LocalReportButtonEvent = true; //Read by BTMessageControl
        Debug.Log("Rotate::setReportButtonStat: set state: " + state);
    }
}
