using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;

public struct SizePairs
{
    public float physicalSize;
    public float virtualSize;
    public char calType; //(m)atched, dp, dm
    public int Repetition;

    // constructor
    public SizePairs(float pSize, float vSize, char cType, int Rep)
    {
        physicalSize = pSize;
        virtualSize = vSize;
        calType = cType;
        Repetition = Rep;
    }
}

public class Controller : MonoBehaviour
{
    StreamWriter userData;
    StreamWriter usertrialInformation, randomTrialFile;
    System.DateTime sysTime;

    //UI Elements and Variables used to display in UI Elements
    public GameObject infoPanel, TrialType, StartTrialButton, EndTrialButton, RecordInfoButton;
    public Text pidText, trialphaselabel, trialdisplaylabel, physicalKnobSizeDisplay, virtualKnobSizeDisplay, MessageTextDislplay, TrialDescriptionDisplay;
    int trialnum = 1;

    //Variables for Recording/Logging Data
    int participantid;
    int trialtype_index;
    string trialtype;
    int trialnumforLogging = 1;
    int randomListSize = 1;


    List<SizePairs> sizePairList = new List<SizePairs>();
    List<SizePairs> sizePairListRandom = new List<SizePairs>();
    List<SizePairs> sizePairListRandomAux = new List<SizePairs>();
    List<SizePairs> sizePairListCal = new List<SizePairs>();
    SizePairs sizePairItem = new SizePairs();

    // Start is called before the first frame update
    void Start()
    {
        trialdisplaylabel.text = trialnum.ToString() + " of ..."; ;
        StartTrialButton.GetComponent<Button>().interactable = false;
        Globals.GlobalVar.StartTrialButtonActive = false;
        EndTrialButton.GetComponent<Button>().interactable = false;

    }

    void Update()
    {
        if (Globals.GlobalVar.EndTrialSyncEvent == true)
        {
            if (Globals.GlobalVar.EndTrialSyncValue == true && EndTrialButton.GetComponent<Button>().interactable == true)
            {
                EndTrialCallBack();
                Globals.GlobalVar.EndTrialSyncEvent = false;
                Globals.GlobalVar.EndTrialSyncValue = false;
            }
        }
    }

    void msgTextDisplay(string msg)
    {
        if (msg == "Clear")
        {
            MessageTextDislplay.text = "";
        }
        else
        {
            MessageTextDislplay.text = "\n" + System.DateTime.Now + "::" + msg + MessageTextDislplay.text;
        }
    }

    bool ParticipantIDValid(string pID)
    {
        bool check = true;

        if (pID.Length == 0)
        {
            msgTextDisplay("Enter a valid participant ID!");
            check = false;
        }

        if (IsNotInteger(pID) == false)
        {
            msgTextDisplay("Enter a valid participant ID!");
            check = false;
        }

        participantid = int.Parse(pID);
        if (participantid < 1)
        {
            msgTextDisplay("Enter a valid participant ID!");
            check = false;
        }

        return check;
    }

    public void setUp() //Callback from Record Information Button
    { 
        Globals.GlobalVar.TrialPhase = "WaitStart"; //WaitRecord, WaitStart, WaitEnd, Over
        //Check Participant Id field
        sizePairListRandom.Clear();

        string participantIDString = pidText.text;
        // Checks if Participant ID is valid
        if (ParticipantIDValid(participantIDString))
        {
            //set button states
            TrialType.GetComponent<Dropdown>().interactable = false;
            RecordInfoButton.GetComponent<Button>().interactable = false;
            StartTrialButton.GetComponent<Button>().interactable = true;
            Globals.GlobalVar.StartTrialButtonActive = true;

            //Set files
            CreateDirectory("MVParticipantData");
            CreateDirectory("MVParticipantData/UserDataFiles");

            usertrialInformation = CreateFile("MVParticipantData/UserDataFiles/user_" + participantid + "_TrialInformation");
            usertrialInformation.WriteLine("Trial_Number, TrialType_Index, TrialType, Virtual_Knob_Size, Physical_Knob_Size, Reported_Diameter, ");
            msgTextDisplay("Files created successfully!");

            //Get trial type from Dropdown menu
            trialtype_index = TrialType.GetComponent<Dropdown>().value;
            Globals.GlobalVar.ControllerTrialTypeIndex = trialtype_index;
            List<Dropdown.OptionData> TrialTypeOptions = TrialType.GetComponent<Dropdown>().options;
            trialtype = TrialTypeOptions[trialtype_index].text;

            //Create/randomize/treat list with sizes - is destroy by randomizePairsList
            createPairsList(); 
            randomizePairsList();
            createCallibrationList();
            treatPairListRandom();

            //Populate physical and virtual sizes
            virtualKnobSizeDisplay.text = sizePairListRandom[0].virtualSize.ToString();
            physicalKnobSizeDisplay.text = sizePairListRandom[0].physicalSize.ToString();

           
            populateTrialDescription();

            //Set report Button to Wait (0)
            setReportButtonState(0); // 0-Wait | 1-Turn Around | 2-Press to End

            //Wait user to start Trial
        }
    }

    void populateTrialDescription()
    {
        //randomListSize = sizePairListRandom.Count;
        trialdisplaylabel.text = trialnum.ToString() + " of " + randomListSize;
        switch (trialtype_index)
        {
            case 0:
                trialphaselabel.text = "Single phase";
                TrialDescriptionDisplay.text = "Single phase-"+ randomListSize + ". \nEstimate size after vision+haptics in VR. ";
                msgTextDisplay("Prepare first physical knob, and press Start Trial.");
                Globals.GlobalVar.TestPhase = "Single phase";
                break;
            case 1:
                trialphaselabel.text = "PreTest phase";
                TrialDescriptionDisplay.text = "Pre-" + randomListSize + "|Cal-" + randomListSize/2 + "|Post-" + randomListSize + ". Convergent. \n\nEstimate size after vision-only in RW. ";
                msgTextDisplay("Prepare first physical knob, and press Start Trial.");
                Globals.GlobalVar.TestPhase = "PreTest phase";
                break;
            case 2:
                trialphaselabel.text = "PreTest phase";
                TrialDescriptionDisplay.text = "Pre-" + randomListSize + "|Cal-" + randomListSize /2 + "|Post-" + randomListSize + ".\nDivergent+ (Physical > Virtual). \n\nEstimate size after vision-only in RW. ";
                msgTextDisplay("Prepare first physical knob, and press Start Trial.");
                Globals.GlobalVar.TestPhase = "PreTest phase";
                break;
            case 3:
                trialphaselabel.text = "PreTest phase";
                TrialDescriptionDisplay.text = "Pre -" + randomListSize + " | Cal - " + randomListSize / 2 + " | Post - " + randomListSize + ".\nDivergent - (Physical < Virtual). \n\nEstimate size after vision-only in RW. ";
                msgTextDisplay("Prepare first physical knob, and press Start Trial.");
                Globals.GlobalVar.TestPhase = "PreTest phase";
                break;
            case 4:
                trialphaselabel.text = "PreTest phase";
                TrialDescriptionDisplay.text = "Pre-" + randomListSize + "|Cal-" + randomListSize / 2 + "|Post-" + randomListSize + ". Convergent. \nEstimate size after vision-only in VR. ";
                msgTextDisplay("No physical knob needed. Press Start Trial to begin.");
                Globals.GlobalVar.TestPhase = "PreTest phase";
                break;
            case 5:
                trialphaselabel.text = "PreTest phase";
                TrialDescriptionDisplay.text = "Pre-" + randomListSize + "|Cal-" + randomListSize / 2 + "|Post-" + randomListSize + ".\nDivergent+ (Physical > Virtual). \n\nEstimate size after vision-only in VR. ";
                msgTextDisplay("No physical knob needed. Press Start Trial to begin.");
                Globals.GlobalVar.TestPhase = "PreTest phase";
                break;
            case 6:
                trialphaselabel.text = "PreTest phase";
                TrialDescriptionDisplay.text = "Pre - " + randomListSize + " | Cal - " + randomListSize / 2 + " | Post - " + randomListSize + ".\nDivergent - (Physical < Virtual). \n\nEstimate size after vision-only in VR. ";
                msgTextDisplay("No physical knob needed. Press Start Trial  to begin.");
                Globals.GlobalVar.TestPhase = "PreTest phase";
                break;
            case 7:
                trialphaselabel.text = "PreTest phase";
                TrialDescriptionDisplay.text = "Pre-" + randomListSize + "|Cal-" + randomListSize / 2 + "|Post-" + randomListSize + ". Convergent. \nEstimate size after haptics-only in VR. ";
                msgTextDisplay("Prepare first physical knob, and press Start Trial.");
                Globals.GlobalVar.TestPhase = "PreTest phase";
                break;
            case 8:
                trialphaselabel.text = "PreTest phase";
                TrialDescriptionDisplay.text = "Pre-" + randomListSize + "|Cal-" + randomListSize / 2 + "|Post-" + randomListSize + ".\nDivergent+ (Physical > Virtual). \n\nEstimate size after haptics-only in VR. ";
                msgTextDisplay("Prepare first physical knob, and press Start Trial.");
                Globals.GlobalVar.TestPhase = "PreTest phase";
                break;
            case 9:
                trialphaselabel.text = "PreTest phase";
                TrialDescriptionDisplay.text = "Pre - " + randomListSize + " | Cal - " + randomListSize /2 + " | Post - " + randomListSize + ".\nDivergent - (Physical < Virtual). \n\nEstimate size after haptics-only in VR. ";
                msgTextDisplay("Prepare first physical knob, and press Start Trial.");
                Globals.GlobalVar.TestPhase = "PreTest phase";
                break;
            default:
                print("PopulateTrialDescription::Something went wrong.");
                break;
        }
    }

    void setReportButtonState(int state) // 0-Wait | 1-Turn Around | 2-Press to End
    {
        Globals.GlobalVar.OscReportButtonValue = state;
        Globals.GlobalVar.OscReportButtonEvent = true;
        Globals.GlobalVar.LocalReportButtonEvent = true; //Read by BTMessageControl
    }

    public void StartTrialCallBack()
    {
        EndTrialButton.GetComponent<Button>().interactable = true;
        StartTrialButton.GetComponent<Button>().interactable = false;
        Globals.GlobalVar.StartTrialButtonActive = false;
        Globals.GlobalVar.TrialPhase = "WaitEnd"; //WaitRecord, WaitStart, WaitEnd, Over

        switch (trialtype_index)
        {
            case 0:
                RunOption0();
                break;
            case 1:
                RunOption1();
                break;
            case 2:
                RunOption2(); // VR Convergent
                break;
            case 3:
                RunOption3();
                break;
            case 4:
                RunOption4();
                break;
            case 5:
                RunOption5();
                break;
            case 6:
                RunOption6(); // VR Convergent
                break;
            case 7:
                RunOption7();
                break;
            case 8:
                RunOption8();
                break;
            case 9:
                RunOption8();
                break;
            default:
                print("Incorrect intelligence level.");
                break;
        }
        msgTextDisplay("Waiting for reported size for trial " + trialnum);
    }

    public void EndTrialCallBack()
    {
        //Set button states
        StartTrialButton.GetComponent<Button>().interactable = true;
        Globals.GlobalVar.StartTrialButtonActive = true;
        EndTrialButton.GetComponent<Button>().interactable = false;

        //Set report Button to Wait (0)
        setReportButtonState(0); // 0-Wait | 1-Turn Around | 2-Press to End
        Globals.GlobalVar.TrialPhase = "WaitStart"; //WaitRecord, WaitStart, WaitEnd, Over

        if (trialphaselabel.text != "Calibration phase")
        {
            //Log reported size to file
            usertrialInformation.WriteLine(trialnum + "," + trialtype_index + "," + trialtype + "," + virtualKnobSizeDisplay.text + "," + physicalKnobSizeDisplay.text + "," + Globals.GlobalVar.report_diameter + ", ");
        } else
        {
            //Log reported size to file
            usertrialInformation.WriteLine(trialnum + "," + trialtype_index + "," + trialtype + "," + virtualKnobSizeDisplay.text + "," + physicalKnobSizeDisplay.text + "," + "calibration" + ", ");
        }
        

        //Remove from random list, get new labels for knob sizes
        if (sizePairListRandom.Count > 0)
        {
            //Remove old trial
            sizePairListRandom.RemoveAt(0);

            //Check if it was the last one
            if (sizePairListRandom.Count > 0)
            {
                //There are more. Get new trial
                virtualKnobSizeDisplay.text = sizePairListRandom[0].virtualSize.ToString();
                physicalKnobSizeDisplay.text = sizePairListRandom[0].physicalSize.ToString();
                //Increment Trial number and write to screen
                if (trialtype_index != 4 && trialtype_index != 5 && trialtype_index != 6)
                {
                    msgTextDisplay("Prepare next physical knob and Press start trial to begin.");
                }
                else
                {
                    msgTextDisplay("No physical knob.Press start trial to begin.");
                }
                
            }
            else
            {
                if (trialtype_index == 0) // Single phase
                {
                    StartTrialButton.GetComponent<Button>().interactable = false;
                    Globals.GlobalVar.StartTrialButtonActive = false;
                    EndTrialButton.GetComponent<Button>().interactable = false;
                    msgTextDisplay("Clear");
                    msgTextDisplay("Trials are over. Thanks!!");
                    Globals.GlobalVar.TrialPhase = "Over";
                }
                else if (trialtype_index != 0) //All others, with pre, cal, and post phases
                {
                    //Debug.Log(trialphaselabel.text+"--hi-------------------------");
                    if (trialphaselabel.text == "PreTest phase")
                    {
                        msgTextDisplay("Clear");
                        if (trialtype_index != 4 && trialtype_index != 5 && trialtype_index != 6)
                        {
                            msgTextDisplay("PreTest done, start Calibration.\n\nNo physical knob needed. Press start trial to begin.");
                        }
                        else
                        {
                            msgTextDisplay("PreTest done, start Calibration.\n\nPrepare next physical knob and press start trial to begin.");
                        }
                        
                        // Setting calibration phase
                        trialphaselabel.text = "Calibration phase";
                        Globals.GlobalVar.TestPhase = "Calibration phase";
                        trialnum = 0;

                        for (int i = 0; i < sizePairListCal.Count; i++)
                        {
                            sizePairListRandom.Add(sizePairListCal[i]);
                        }
                        randomListSize = sizePairListRandom.Count;

                        virtualKnobSizeDisplay.text = sizePairListRandom[0].virtualSize.ToString();
                        physicalKnobSizeDisplay.text = sizePairListRandom[0].physicalSize.ToString();
                    }
                    else if (trialphaselabel.text == "Calibration phase")
                    {
                        msgTextDisplay("Clear");
                        if (trialtype_index != 4 && trialtype_index != 5 && trialtype_index != 6)
                        {
                            msgTextDisplay("Calibration done, start PostTest.\n\nPrepare next physical knob and press start trial to begin.");
                        }
                        else
                        {
                            msgTextDisplay("Calibration done, start PostTest.\n\nNo physical knob. Press start trial to begin.");
                        }

                        trialnum = 0;
                        // Setting calibration phase
                        trialphaselabel.text = "PostTest phase";
                        Globals.GlobalVar.TestPhase = "PostTest phase";

                        //Create new Randomized pair list
                        createPairsList();
                        randomizePairsList();
                        treatPairListRandom();

                        virtualKnobSizeDisplay.text = sizePairListRandom[0].virtualSize.ToString();
                        physicalKnobSizeDisplay.text = sizePairListRandom[0].physicalSize.ToString();
                    }
                    else if (trialphaselabel.text == "PostTest phase")
                    {
                        StartTrialButton.GetComponent<Button>().interactable = false;
                        Globals.GlobalVar.StartTrialButtonActive = false;
                        EndTrialButton.GetComponent<Button>().interactable = false;
                        msgTextDisplay("Clear");
                        msgTextDisplay("Trials are over. Thanks!!");
                        Globals.GlobalVar.TrialPhase = "Over";
                    }

                }

            }
            if (trialnum < randomListSize)
            {
                trialnum = trialnum + 1;
                trialdisplaylabel.text = trialnum.ToString() + " of " + randomListSize;
            }

        }
       

        
    }

    void RunOption0()
    {
        trialphaselabel.text = "Single phase";
        Globals.GlobalVar.TestPhase = "Single phase";
        Globals.GlobalVar.dialSizeRef= convertSizetoIndexSize(sizePairListRandom[0].virtualSize); 
        Globals.GlobalVar.dialSizeUpdate = true;

        //Set report Button to Turn Around (1)
        setReportButtonState(1); // 0-Wait | 1-Turn Around | 2-Press to End

    }

    void RunOption1() //Real Life-Convergent-No virtual knob.
    {
        Globals.GlobalVar.dialSizeRef = convertSizetoIndexSize(sizePairListRandom[0].virtualSize);
        Globals.GlobalVar.dialSizeUpdate = true;
        //Set report Button to Turn Around (1)
            setReportButtonState(1); // 0-Wait | 1-Turn Around | 2-Press to End

    }
    void RunOption2() //VR - Convergent
    {
        Globals.GlobalVar.dialSizeRef = convertSizetoIndexSize(sizePairListRandom[0].virtualSize);
        Globals.GlobalVar.dialSizeUpdate = true;
        //Set report Button to Turn Around (1)
        setReportButtonState(1); // 0-Wait | 1-Turn Around | 2-Press to End
    }

    void RunOption3()
    {
        Globals.GlobalVar.dialSizeRef = convertSizetoIndexSize(sizePairListRandom[0].virtualSize);
        Globals.GlobalVar.dialSizeUpdate = true;
        //Set report Button to Turn Around (1)
        setReportButtonState(1); // 0-Wait | 1-Turn Around | 2-Press to End

    }


    void RunOption4()
    {
        Globals.GlobalVar.dialSizeRef = convertSizetoIndexSize(sizePairListRandom[0].virtualSize);
        Globals.GlobalVar.dialSizeUpdate = true;
        //Set report Button to Turn Around (1)
        setReportButtonState(1); // 0-Wait | 1-Turn Around | 2-Press to End

    }

    void RunOption5()
    {
        Globals.GlobalVar.dialSizeRef = convertSizetoIndexSize(sizePairListRandom[0].virtualSize);
        Globals.GlobalVar.dialSizeUpdate = true;
        //Set report Button to Turn Around (1)
        setReportButtonState(1); // 0-Wait | 1-Turn Around | 2-Press to End

    }

    void RunOption6()
    {
        Globals.GlobalVar.dialSizeRef = convertSizetoIndexSize(sizePairListRandom[0].virtualSize);
        Globals.GlobalVar.dialSizeUpdate = true;
        //Set report Button to Turn Around (1)
        setReportButtonState(1); // 0-Wait | 1-Turn Around | 2-Press to End

    }

    void RunOption7()
    {
        Globals.GlobalVar.dialSizeRef = convertSizetoIndexSize(sizePairListRandom[0].virtualSize);
        Globals.GlobalVar.dialSizeUpdate = true;
        //Set report Button to Turn Around (1)
        setReportButtonState(1); // 0-Wait | 1-Turn Around | 2-Press to End

    }

    void RunOption8()
    {
        Globals.GlobalVar.dialSizeRef = convertSizetoIndexSize(sizePairListRandom[0].virtualSize);
        Globals.GlobalVar.dialSizeUpdate = true;
        //Set report Button to Turn Around (1)
        setReportButtonState(1); // 0-Wait | 1-Turn Around | 2-Press to End

    }

    void RunOption9()
    {
        Globals.GlobalVar.dialSizeRef = convertSizetoIndexSize(sizePairListRandom[0].virtualSize);
        Globals.GlobalVar.dialSizeUpdate = true;
        //Set report Button to Turn Around (1)
        setReportButtonState(1); // 0-Wait | 1-Turn Around | 2-Press to End

    }
    int convertSizetoIndexSize(float sizeinput)
    {
        int sizeInt = (int)sizeinput;
        int nSize = 0;
        switch (sizeinput)
        {
            case 0f: // No virtual knob.
                nSize = 99;
                break;
            case 35f:
                nSize = 1;
                break;
            case 40f:
                nSize = 2;
                break;
            case 45f:
                nSize = 3;
                break;
            case 50f:
                nSize = 4;
                break;
            case 55f:
                nSize = 5;
                break;
            case 60f:
                nSize = 6;
                break;
            case 65f:
                nSize = 7;
                break;
            case 70f:
                nSize = 8;
                break;
            case 75f:
                nSize = 9;
                break;
            case 80f:
                nSize = 0;
                break;


            case 38.5f:
                nSize = 11;
                break;
            case 44f:
                nSize = 12;
                break;
            case 49.5f:
                nSize = 13;
                break;
            case 60.5f:
                nSize = 14;
                break;
            case 66f:
                nSize = 15;
                break;
            case 71.5f:
                nSize = 16;
                break;
            case 77f:
                nSize = 17;
                break;
            case 82.5f:
                nSize = 18;
                break;
            case 88f:
                nSize = 19;
                break;


            default:
                nSize = 1;
                break;
        }
        return nSize;
    }


    void createCallibrationList()
    {
        sizePairListCal.Clear();
        char ctype;

        if (trialtype_index != 0) //no need to filter calibration table for index 0
        {
            switch (trialtype_index)
            {
                case 1:
                    ctype = 'm';
                    break;
                case 2:
                    ctype = '+';
                    break;
                case 3:
                    ctype = '-';
                    break;
                case 4:
                    ctype = 'm';
                    break;
                case 5:
                    ctype = '+';
                    break;
                case 6:
                    ctype = '-';
                    break;
                case 7:
                    ctype = 'm';
                    break;
                case 8:
                    ctype = '+';
                    break;
                case 9:
                    ctype = '-';
                    break;
                default:
                    ctype = ' ';
                    break;
            }

            int count = sizePairListRandom.Count;
            for (int i = 0; i < count; i++)
            {
                if (sizePairListRandom[i].calType == ctype)
                {
                        sizePairListCal.Add(sizePairListRandom[i]);
                }
            }
        }

    }


    void randomizePairsList()
    {

        int count = sizePairList.Count;
        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(0, sizePairList.Count);
            //Debug.Log(randomIndex);
            sizePairListRandom.Add(sizePairList[randomIndex]);
            sizePairList.RemoveAt(randomIndex);
            Debug.Log("index: " + randomIndex + " size randomized: " + sizePairListRandom.Count + " size original: " + sizePairList.Count);
        }
    }


    void treatPairListRandom() //executed after Ramdom list is populated for pre and post test - Set virtual size to 0 and remove matched lines (they are repeated!!)
    {
        if (trialtype_index != 0)
        {
            if (trialtype_index == 1 || trialtype_index == 2 || trialtype_index == 3 || trialtype_index == 7 || trialtype_index == 8 || trialtype_index == 9) //RW no virtual knob
            {
                //Treat sizePairListRandom - Set all virtual sizes to Zero
                for (int i = 0; i < sizePairListRandom.Count; i++)
                {
                    SizePairs sp;
                    sp = sizePairListRandom[i];
                    sp.virtualSize = 0;
                    sizePairListRandom[i] = sp;
                    //Debug.Log(sizePairListRandom[i].virtualSize);
                }
            }
            else //4,5,6
            {
                //Treat sizePairListRandom - Set all Physical sizes to Zero
                for (int i = 0; i < sizePairListRandom.Count; i++)
                {
                    SizePairs sp;
                    sp = sizePairListRandom[i];
                    sp.physicalSize = 0;
                    sizePairListRandom[i] = sp;
                }
            }
        }
            
            Debug.Log("Controller:treatPairLIstRandom::sizePairListRandom before removing m: " + sizePairListRandom.Count);
            //remove matched
            int count = sizePairListRandom.Count;
            for (int i = 0; i < count; i++)
            {
                if (sizePairListRandom[i].calType == 'm')
                {
                    sizePairListRandomAux.Add(sizePairListRandom[i]);
                }
            }
            sizePairListRandom.Clear();
            Debug.Log("Controller:treatPairLIstRandom::sizePairListRandom after clearing: "+sizePairListRandom.Count);
            Debug.Log("Controller:treatPairLIstRandom::sizePairListRandomAux without m:" + sizePairListRandomAux.Count);

            for (int i = 0; i < sizePairListRandomAux.Count; i++)
            {
                 sizePairListRandom.Add(sizePairListRandomAux[i]);
            }
            sizePairListRandomAux.Clear();
            //sizePairListRandom = sizePairListRandomAux;
            Debug.Log("Controller:treatPairLIstRandom::sizePairListRandom re-populated from Aux:" + sizePairListRandomAux.Count);
        
        randomListSize = sizePairListRandom.Count;
        Debug.Log("randomizePairsList::sizePairListRandom Count: " + randomListSize);
        
    }

    void createPairsList2()
    {
        // Matched
        sizePairItem = new SizePairs(45f, 45f, 'm', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(50f, 50f, 'm', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(55f, 55f, 'm', 0); sizePairList.Add(sizePairItem);
        

        ////

        // divergent physcial smaller
        sizePairItem = new SizePairs(35f, 55f, '-', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(40f, 60f, '-', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(45f, 65f, '-', 0); sizePairList.Add(sizePairItem);
        

        ////

        // divergent physcial larger
        sizePairItem = new SizePairs(55f, 35f, '+', 2); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(60f, 40f, '+', 2); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(65f, 45f, '+', 2); sizePairList.Add(sizePairItem);
        
        ////
        Debug.Log("createPairsList::sizePairList Count: " + sizePairList.Count);
    }


    void createPairsListFull() //All ten combinations
    {
        // Matched
        //sizePairItem = new SizePairs(35f, 35f, 'm', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(40f, 40f, 'm', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(44f, 44f, 'm', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(45f, 45f, 'm', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(49.5f, 49.5f, 'm', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(50f, 50f, 'm', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(55f, 55f, 'm', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(55f, 55f, 'm', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(60.5f, 60.5f, 'm', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(60f, 60f, 'm', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(66f, 66f, 'm', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(65f, 65f, 'm', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(71.5f, 71.5f, 'm', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(70f, 70f, 'm', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(77f, 77f, 'm', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(75f, 75f, 'm', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(82.5f, 82.5f, 'm', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(80f, 80f, 'm', 0); sizePairList.Add(sizePairItem);

        sizePairItem = new SizePairs(35f, 35f, 'm', 1); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(40f, 40f, 'm', 1); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(45f, 45f, 'm', 1); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(50f, 50f, 'm', 1); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(55f, 55f, 'm', 1); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(60f, 60f, 'm', 1); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(65f, 65f, 'm', 1); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(70f, 70f, 'm', 1); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(75f, 75f, 'm', 1); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(80f, 80f, 'm', 1); sizePairList.Add(sizePairItem);

        sizePairItem = new SizePairs(35f, 35f, 'm', 2); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(40f, 40f, 'm', 2); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(45f, 45f, 'm', 2); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(50f, 50f, 'm', 2); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(55f, 55f, 'm', 2); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(60f, 60f, 'm', 2); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(65f, 65f, 'm', 2); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(70f, 70f, 'm', 2); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(75f, 75f, 'm', 2); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(80f, 80f, 'm', 2); sizePairList.Add(sizePairItem);
        ////

        // divergent physcial smaller
        sizePairItem = new SizePairs(35f, 38.5f, '-', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(40f, 44f, '-', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(45f, 49.5f, '-', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(50f, 55f, '-', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(55f, 60.5f, '-', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(60f, 66f, '-', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(65f, 71.5f, '-', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(70f, 77f, '-', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(75f, 82.5f, '-', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(80f, 88f, '-', 0); sizePairList.Add(sizePairItem);

        sizePairItem = new SizePairs(35f, 38.5f, '-', 1); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(40f, 44f, '-', 1); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(45f, 49.5f, '-', 1); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(50f, 55f, '-', 1); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(55f, 60.5f, '-', 1); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(60f, 66f, '-', 1); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(65f, 71.5f, '-', 1); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(70f, 77f, '-', 1); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(75f, 82.5f, '-', 1); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(80f, 88f, '-', 1); sizePairList.Add(sizePairItem);

        sizePairItem = new SizePairs(35f, 38.5f, '-', 2); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(40f, 44f, '-', 2); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(45f, 49.5f, '-', 2); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(50f, 55f, '-', 2); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(55f, 60.5f, '-', 2); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(60f, 66f, '-', 2); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(65f, 71.5f, '-', 2); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(70f, 77f, '-', 2); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(75f, 82.5f, '-', 2); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(80f, 88f, '-', 2); sizePairList.Add(sizePairItem);
        ////

        // divergent physcial larger
        sizePairItem = new SizePairs(38.5f, 35f, '+', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(44f, 40f, '+', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(49.5f, 45f, '+', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(55f, 50f, '+', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(60.5f, 55f, '+', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(66f, 60f, '+', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(71.5f, 65f, '+', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(77f, 70f, '+', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(82.5f, 75f, '+', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(88f, 80f, '+', 0); sizePairList.Add(sizePairItem);

        sizePairItem = new SizePairs(38.5f, 35f, '+', 1); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(44f, 40f, '+', 1); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(49.5f, 45f, '+', 1); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(55f, 50f, '+', 1); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(60.5f, 55f, '+', 1); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(66f, 60f, '+', 1); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(71.5f, 65f, '+', 1); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(77f, 70f, '+', 1); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(82.5f, 75f, '+', 1); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(88f, 80f, '+', 1); sizePairList.Add(sizePairItem);

        sizePairItem = new SizePairs(38.5f, 35f, '+', 2); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(44f, 40f, '+', 2); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(49.5f, 45f, '+', 2); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(55f, 50f, '+', 2); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(60.5f, 55f, '+', 2); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(66f, 60f, '+', 2); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(71.5f, 65f, '+', 2); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(77f, 70f, '+', 2); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(82.5f, 75f, '+', 2); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(88f, 80f, '+', 2); sizePairList.Add(sizePairItem);
        ////
        Debug.Log("createPairsList::sizePairList Count: " + sizePairList.Count);
    }

    void createPairsList() //6 combinations
    {
        // Matched
        sizePairItem = new SizePairs(40f, 40f, 'm', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(44f, 44f, 'm', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(45f, 45f, 'm', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(49.5f, 49.5f, 'm', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(50f, 50f, 'm', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(55f, 55f, 'm', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(55f, 55f, 'm', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(60.5f, 60.5f, 'm', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(60f, 60f, 'm', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(66f, 66f, 'm', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(65f, 65f, 'm', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(71.5f, 71.5f, 'm', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(70f, 70f, 'm', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(77f, 77f, 'm', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(75f, 75f, 'm', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(82.5f, 82.5f, 'm', 0); sizePairList.Add(sizePairItem);


        sizePairItem = new SizePairs(40f, 40f, 'm', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(44f, 44f, 'm', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(45f, 45f, 'm', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(49.5f, 49.5f, 'm', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(50f, 50f, 'm', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(55f, 55f, 'm', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(55f, 55f, 'm', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(60.5f, 60.5f, 'm', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(60f, 60f, 'm', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(66f, 66f, 'm', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(65f, 65f, 'm', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(71.5f, 71.5f, 'm', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(70f, 70f, 'm', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(77f, 77f, 'm', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(75f, 75f, 'm', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(82.5f, 82.5f, 'm', 0); sizePairList.Add(sizePairItem);

        sizePairItem = new SizePairs(40f, 40f, 'm', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(44f, 44f, 'm', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(45f, 45f, 'm', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(49.5f, 49.5f, 'm', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(50f, 50f, 'm', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(55f, 55f, 'm', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(55f, 55f, 'm', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(60.5f, 60.5f, 'm', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(60f, 60f, 'm', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(66f, 66f, 'm', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(65f, 65f, 'm', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(71.5f, 71.5f, 'm', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(70f, 70f, 'm', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(77f, 77f, 'm', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(75f, 75f, 'm', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(82.5f, 82.5f, 'm', 0); sizePairList.Add(sizePairItem);
        ////

        // divergent physcial smaller
        //sizePairItem = new SizePairs(35f, 38.5f, '-', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(40f, 44f, '-', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(45f, 49.5f, '-', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(50f, 55f, '-', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(55f, 60.5f, '-', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(60f, 66f, '-', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(65f, 71.5f, '-', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(70f, 77f, '-', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(75f, 82.5f, '-', 0); sizePairList.Add(sizePairItem);
        //sizePairItem = new SizePairs(80f, 88f, '-', 0); sizePairList.Add(sizePairItem);

        //sizePairItem = new SizePairs(35f, 38.5f, '-', 1); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(40f, 44f, '-', 1); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(45f, 49.5f, '-', 1); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(50f, 55f, '-', 1); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(55f, 60.5f, '-', 1); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(60f, 66f, '-', 1); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(65f, 71.5f, '-', 1); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(70f, 77f, '-', 1); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(75f, 82.5f, '-', 1); sizePairList.Add(sizePairItem);
        //sizePairItem = new SizePairs(80f, 88f, '-', 1); sizePairList.Add(sizePairItem);

        //sizePairItem = new SizePairs(35f, 38.5f, '-', 2); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(40f, 44f, '-', 2); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(45f, 49.5f, '-', 2); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(50f, 55f, '-', 2); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(55f, 60.5f, '-', 2); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(60f, 66f, '-', 2); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(65f, 71.5f, '-', 2); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(70f, 77f, '-', 2); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(75f, 82.5f, '-', 2); sizePairList.Add(sizePairItem);
        //sizePairItem = new SizePairs(80f, 88f, '-', 2); sizePairList.Add(sizePairItem);
        ////

        // divergent physcial larger
        //sizePairItem = new SizePairs(38.5f, 35f, '+', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(44f, 40f, '+', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(49.5f, 45f, '+', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(55f, 50f, '+', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(60.5f, 55f, '+', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(66f, 60f, '+', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(71.5f, 65f, '+', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(77f, 70f, '+', 0); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(82.5f, 75f, '+', 0); sizePairList.Add(sizePairItem);
        //sizePairItem = new SizePairs(88f, 80f, '+', 0); sizePairList.Add(sizePairItem);

        //sizePairItem = new SizePairs(38.5f, 35f, '+', 1); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(44f, 40f, '+', 1); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(49.5f, 45f, '+', 1); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(55f, 50f, '+', 1); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(60.5f, 55f, '+', 1); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(66f, 60f, '+', 1); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(71.5f, 65f, '+', 1); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(77f, 70f, '+', 1); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(82.5f, 75f, '+', 1); sizePairList.Add(sizePairItem);
        //sizePairItem = new SizePairs(88f, 80f, '+', 1); sizePairList.Add(sizePairItem);

        //sizePairItem = new SizePairs(38.5f, 35f, '+', 2); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(44f, 40f, '+', 2); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(49.5f, 45f, '+', 2); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(55f, 50f, '+', 2); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(60.5f, 55f, '+', 2); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(66f, 60f, '+', 2); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(71.5f, 65f, '+', 2); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(77f, 70f, '+', 2); sizePairList.Add(sizePairItem);
        sizePairItem = new SizePairs(82.5f, 75f, '+', 2); sizePairList.Add(sizePairItem);
        //sizePairItem = new SizePairs(88f, 80f, '+', 2); sizePairList.Add(sizePairItem);
        ////
        Debug.Log("createPairsList::sizePairList Count: " + sizePairList.Count);
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


    bool IsNotInteger(string s)
    {
        int i;
        if (int.TryParse(s, out i))
        {

            return true;
        }
        return false;
    }

    private void OnApplicationQuit()
    {
        // Debug.Log(maxAngle);
        ////userData.Close();
        usertrialInformation.Close();
        randomTrialFile.Close();
       //// if (File.Exists("MVParticipantData/UserDataFiles/user_" + participantid + "_TrialInformation.txt"))
       ////     File.Delete("MVParticipantData/UserDataFiles/user_" + participantid + "_TrialInformation.txt");
    }


   // IEnumerator quitapp()
   // {
       //// if (File.Exists("MVParticipantData/UserDataFiles/user_" + participantid + "_TrialInformation.txt"))
       ////     File.Delete("MVParticipantData/UserDataFiles/user_" + participantid + "_TrialInformation.txt");
       // yield return new WaitForSeconds(10);
       // Application.Quit();
    //}
}
