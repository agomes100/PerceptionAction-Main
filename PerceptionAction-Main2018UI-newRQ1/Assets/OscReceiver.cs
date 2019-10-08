using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OSCsharp.Data;

namespace UniOSC
{

    public class OscReceiver : UniOSCEventTarget
    {

        public override void OnOSCMessageReceived(UniOSCEventArgs args)
        {
            OscMessage msg = (OscMessage)args.Packet;
            Globals.GlobalVar.radiusSyncValue = (float)msg.Data[0]; 
            Globals.GlobalVar.OscSendRadiusSyncEvent = true;
        }


    }
}