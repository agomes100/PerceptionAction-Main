using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Globals : MonoBehaviour
{

    public class GlobalVar
    {
        public static int dialRight = 0;
        public static int dialLeft = 0;
        public static int dialClick = 0;

        public static float dialAccum = 0f;

        public static bool dialShowHideUpdate = false;
        public static int dialShowHide = 1; //1 = Show

        public static bool dialSizeUpdate = false;
        public static int dialSizeRef = 1; //35 mm

        public static string buttonPress = "";
        public static string prevButtonPress = "";

        public static string oscMessage = "";
        public static string prevOscMessage = "";
        public static bool OscSendEvent = false;

        public static string dialSync = "";
        public static bool OscSendSyncEvent = false;

        public static string inputDialSync = "";
        public static bool OscReceiveSyncEvent = false;

        public static int neopixelMessage = 0;
        public static bool OscSendNeopixelEvent = false;
        public static int NeopixelEventCount = 0;

        public static float prev_radius = 0f;
        public static float report_diameter = 0f;
        public static float radiusSyncValue = 0f;
        public static bool OscSendRadiusSyncEvent = false;

        public static bool EndTrialSyncValue = false;
        public static bool EndTrialSyncEvent = false;

        public static bool StartTrialButtonActive = true;

        public static int OscReportButtonValue = 0; // 0-Wait | 1-Turn Around | 2-Press to End
        public static bool OscReportButtonEvent = false;
        public static bool LocalReportButtonEvent = false;

        public static int ControllerTrialTypeIndex = 0;
        public static string TrialPhase = "WaitRecord"; //WaitRecord, WaitStart, WaitEnd, Over
        public static string TestPhase = "PreTest phase";

        public static bool dataLogInitiated = false;

        public static bool knobIsVisible = false;
    }
}