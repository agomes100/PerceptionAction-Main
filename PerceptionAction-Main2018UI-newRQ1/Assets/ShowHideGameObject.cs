using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowHideGameObject : MonoBehaviour
{

	public GameObject showHideObject;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    	if (Globals.GlobalVar.dialShowHideUpdate == true)
        {
            Globals.GlobalVar.dialShowHideUpdate = false;
            ShowHide(Globals.GlobalVar.dialShowHide);
        }

    	
        
        void ShowHide(int sh){
        	if (sh == 1){
        		//showHideObject.SetActive(true);	
        		Globals.GlobalVar.dialSizeUpdate = true;
        		Debug.Log("ShowHideGO::Show GameObject");

        		
        	} else {
        		//showHideObject.SetActive(false);
        		Debug.Log("ShowHideGO::Hide GameObject");
        		showHideObject.transform.localScale = new Vector3(0.2f, 0.01f, 0.2f);
        	}
        }
    }
}
