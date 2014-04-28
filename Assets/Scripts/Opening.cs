using UnityEngine;
using System.Collections;

public class Opening : MonoBehaviour {
	public Texture logo;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI(){

		if(GUI.Button(new Rect(Screen.width/2 - 50, 3 * Screen.height/4, 100, 50), "Start")){
			Application.LoadLevel("SimWarren");
		}
		GUI.DrawTexture(new Rect(Screen.width/2 - logo.width/2, Screen.height/2 - logo.height/2 , logo.width , logo.height), logo);
		//if(GUI.Button(new Rect(Screen.width - 215, 3 * Screen.height/4, 150, 150), "Endless")){
		//	Application.LoadLevel("SimWarren");
		//}
	}
}
