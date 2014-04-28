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
		GUI.Label(new Rect(Screen.width/2 - 240, 3 * Screen.height/4, 150, 100), "Seasons High Score: " + PlayerPrefs.GetInt("Most Seasons", 0));
		GUI.Label(new Rect(Screen.width/2 + 120, 3 * Screen.height/4, 150, 100), "Rabbit High Score: " + PlayerPrefs.GetInt("Most Rabbits", 0));
	}
}
