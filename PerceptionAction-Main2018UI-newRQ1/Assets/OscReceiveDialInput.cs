using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OSCsharp.Data;

namespace UniOSC
{

    public class OscReceiveDialInput : UniOSCEventTarget
    {
        public override void OnOSCMessageReceived(UniOSCEventArgs args)
        {
            OscMessage msg = (OscMessage)args.Packet;
            if ((char)msg.Data[0] == 'L')
            {
                Globals.GlobalVar.dialLeft = 1;
                Debug.Log("OscReceiveDialInput::DialRotate:Left");
            } else if ((char)msg.Data[0] == 'R')
            {
                Globals.GlobalVar.dialRight = 1;
                Debug.Log("OscReceiveDialInput::DialRotate:Right");
            }

        }


    }
}