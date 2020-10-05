using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class btnLinks : MonoBehaviour {

    private Button yourButton;

    // Use this for initialization
    void Start () {
        Button yourButton = this.GetComponent<Button>();
        yourButton.onClick.AddListener(() => { TaskOnClick(); });
     //   Debug.Log(this.name);
    }

    void TaskOnClick()
    {

        Debug.Log(this.name+" You have clicked the button!");
    }

    // Update is called once per frame
    void Update () {
		
	}

}
