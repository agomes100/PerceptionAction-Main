using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OSCsharp.Data;

namespace UniOSC
{

    public class OscReceiveEndTrial : UniOSCEventTarget
    {
        public override void OnOSCMessageReceived(UniOSCEventArgs args)
        {
            if (Globals.GlobalVar.StartTrialButtonActive == false)
            {
                OscMessage msg = (OscMessage)args.Packet;
                Globals.GlobalVar.EndTrialSyncValue = (bool)msg.Data[0];
                Globals.GlobalVar.EndTrialSyncEvent = true;
            }

        }


    }
}