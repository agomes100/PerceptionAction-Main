using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class knobResize : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
    	if (Globals.GlobalVar.dialSizeUpdate == true)
        {
            Globals.GlobalVar.dialSizeUpdate = false;
            Resize(Globals.GlobalVar.dialSizeRef);
        }
        
    }

    void Resize(int sizeRef){
    	float nSize = 0f;
    	float standard_height = 1f;
    	switch (sizeRef)
      {
            case 99:  // No virtual knob.
                nSize = (1 * 1f) / 100f; ;
                Debug.Log("knobResize::dialSizeRef=99");
                break;
            case 1:
                nSize = (1 * 35f) / 100f;
              //Debug.Log("knobResize::dialSizeRef=1");
              break;
          case 2:
              nSize = (1 * 40f) / 100f;
              //Debug.Log("knobResize::dialSizeRef=2");
              break;
          case 3:
              nSize = (1 * 45f) / 100f;
              //Debug.Log("knobResize::dialSizeRef=3");
              break;
          case 4:
              nSize = (1 * 50f) / 100f;
              //Debug.Log("knobResize::dialSizeRef=4");
              break;
          case 5:
              nSize = (1 * 55f) / 100f;
              //Debug.Log("knobResize::dialSizeRef=5");
              break;
          case 6:
              nSize = (1 * 60f) / 100f;
              //Debug.Log("knobResize::dialSizeRef=6");
              break;
          case 7:
              nSize = (1 * 65f) / 100f;
              //Debug.Log("knobResize::dialSizeRef=7");
              break;
          case 8:
              nSize = (1 * 70f) / 100f;
              //Debug.Log("knobResize::dialSizeRef=8");
              break;
          case 9:
              nSize = (1 * 75f) / 100f;
              //Debug.Log("knobResize::dialSizeRef=9");
              break;
          case 0:
              nSize = (1 * 80f) / 100f;
              //Debug.Log("knobResize::dialSizeRef=0");
              break;


            case 11:
                nSize = (1 * 38.5f) / 100f;
                //Debug.Log("knobResize::dialSizeRef=1");
                break;
            case 12:
                nSize = (1 * 44f) / 100f;
                //Debug.Log("knobResize::dialSizeRef=2");
                break;
            case 13:
                nSize = (1 * 49.5f) / 100f;
                //Debug.Log("knobResize::dialSizeRef=3");
                break;
            case 14:
                nSize = (1 * 60.5f) / 100f;
                //Debug.Log("knobResize::dialSizeRef=4");
                break;
            case 15:
                nSize = (1 * 66f) / 100f;
                //Debug.Log("knobResize::dialSizeRef=5");
                break;
            case 16:
                nSize = (1 * 71.5f) / 100f;
                //Debug.Log("knobResize::dialSizeRef=6");
                break;
            case 17:
                nSize = (1 * 77f) / 100f;
                //Debug.Log("knobResize::dialSizeRef=7");
                break;
            case 18:
                nSize = (1 * 82.5f) / 100f;
                //Debug.Log("knobResize::dialSizeRef=8");
                break;
            case 19:
                nSize = (1 * 88f) / 100f;
                //Debug.Log("knobResize::dialSizeRef=9");
                break;

            default:
                nSize = (1 * 35f);///1000f;
              //Debug.Log("knobResize::dialSizeRef=default !!! 1");
              break;
      }
      //Debug.Log("knobResize::Calculated Size: "+nSize);
      transform.localScale = new Vector3(nSize, nSize, standard_height);
    }
}
