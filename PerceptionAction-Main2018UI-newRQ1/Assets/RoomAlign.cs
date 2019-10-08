using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomAlign : MonoBehaviour
{
    public Transform rig;
    public Transform tableCorner;
    public Transform controllerTip;

    // Update is called once per frame
    void Update()
    {   if(Input.GetKeyUp(KeyCode.C))
        {
            Vector3 offset = tableCorner.position - controllerTip.position;
            rig.position += offset;
        }
    }
}
