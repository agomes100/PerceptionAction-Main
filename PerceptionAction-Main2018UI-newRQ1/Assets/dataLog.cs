using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;

public class dataLog : MonoBehaviour
{
    public Text trialType, participantid, trialNumber, trialPhase, virtualKnobSize, physicalKnobSize;
    public Transform headset;
    StreamWriter userTrialData;
    System.DateTime sysTime;

    // Update is called once per frame
    void Update()
    {
        if (Globals.GlobalVar.dataLogInitiated == false)
        {
            if (Globals.GlobalVar.TrialPhase == "WaitStart")
            {
                //Set files
                CreateDirectory("MVParticipantData");
                CreateDirectory("MVParticipantData/UserDataFiles");

                userTrialData = CreateFile("MVParticipantData/UserDataFiles/user_" + int.Parse(participantid.text) + "_DataLog");
                userTrialData.WriteLine("Trial_Type, Participant_Id, Trial_Number, Trial_Phase, Test_Phase, Report_Button_Value, TimeOfDay, Time, Frame_Count, Virtual_Knob, Physical_Knob, Knob_inFOV, Reported_Diameter(mm), Reported_yVal_Diameter, Dial_Direction, Headset_lpx, Headset_lpy, Headset_lpz, Headset_lrw, Headset_lrx, Headset_lry, Headset_lrz, Headset_gpx, Headset_gpy, Headset_gpz, Headset_grw, Headset_grx, Headset_gry, Headset_grz,");
                Globals.GlobalVar.dataLogInitiated = true;
            }
        }
        sysTime = System.DateTime.Now;
        string line = trialType.text + "," + participantid.text + "," + trialNumber.text + "," + Globals.GlobalVar.TrialPhase + "," + Globals.GlobalVar.TestPhase + "," + GetReportButtonValue(Globals.GlobalVar.OscReportButtonValue) + "," + sysTime.TimeOfDay + "," + Time.time + "," + Time.frameCount + "," + virtualKnobSize.text + "," + physicalKnobSize.text + "," + Globals.GlobalVar.knobIsVisible +"," + Globals.GlobalVar.report_diameter + "," + Globals.GlobalVar.radiusSyncValue + "," + Globals.GlobalVar.dialAccum + "," + GetLocalTransformInfo(headset) + "," + GetGlobalTransformInfo(headset) + "," ; 
        if (userTrialData != null) userTrialData.WriteLine(line);

    }


    void CreateDirectory(string path)    //creating a file directory for participant data to be stored
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    StreamWriter CreateFile(string path)  //creating files for participant data
    {
        StreamWriter sw;

        if (!File.Exists(path + ".txt"))
        {
            sw = new StreamWriter(path + ".txt");
            return sw;
        }
        else
        {
            int i = 1;

            while (true)
            {
                if (!File.Exists(path + "_" + i + ".txt"))
                {
                    sw = new StreamWriter(path + "_" + i + ".txt");
                    return sw;
                }

                i++;
            }
        }
    }

    string GetGlobalTransformInfo(Transform t) // Tracked Object Global Info
    {
        string str = t.position.x + "," + t.position.y + "," + t.position.z + ",";
        str += t.rotation.w + "," + t.rotation.x + "," + t.rotation.y + "," + t.rotation.z;

        return str;
    }

    string GetLocalTransformInfo(Transform t) // Tracked Object Local Info
    {
        string str = t.localPosition.x + "," + t.localPosition.y + "," + t.localPosition.z + ",";
        str += t.localRotation.w + "," + t.localRotation.x + "," + t.localRotation.y + "," + t.localRotation.z;

        return str;
    }

    string GetReportButtonValue(int btState)
    {
        if (Globals.GlobalVar.OscReportButtonValue == 0)
        {
            return "0 - Wait";
        }
        else if (Globals.GlobalVar.OscReportButtonValue == 1)
        {
            return "1 - Turn Around"; // | 2 - Press to End
        }
        else if (Globals.GlobalVar.OscReportButtonValue == 2)
        {
            return "2 - Press to End";
        }
        else return "unknown";



    }
}
