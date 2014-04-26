using UnityEngine;
using System.Collections;

public class GUIScript : MonoBehaviour {
	public Texture profession;
	public int hp;
	public int maxhp;
	public int hunger;
	public int full;
	public int spd;
	public int atk;
	public string name;
	public Texture hptexture;
	public Texture hungertexture;
	public Texture emptytexture;
	
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void OnGUI(){

//		if (GUI.Button(new Rect(Screen.width - 200, Screen.height - 200, 200,100), "Change name")){
//			name = gameObject.GetComponent<NameGen>().getName();
//		}

		GUI.Box(new Rect(Screen.width - 200, Screen.height - 100, 200,100), name);
		//profession icon
		GUI.DrawTexture(new Rect(Screen.width - 195, Screen.height - 93, 30,30), profession);
		//stats
		GUI.Label(new Rect(Screen.width - 200 + profession.width + 10, Screen.height - 80, 50,20), "Spd:" + spd);
		GUI.Label(new Rect(Screen.width - 200 + profession.width + 10, Screen.height - 60, 50,20), "Atk:" + atk);
		//hunger and hp bars	
		GUI.Label(new Rect(Screen.width - 195,Screen.height - 35, 100,20), "Hunger:");
		GUI.Label(new Rect(Screen.width - 195,Screen.height - 20, 100,20), "HP:");
		GUI.DrawTexture(new Rect(Screen.width - 145,Screen.height - 30, 100,10), emptytexture);
		GUI.DrawTexture(new Rect(Screen.width - 145, Screen.height - 30, (float)(hunger/(float)full) * 100,10), hungertexture);
		GUI.DrawTexture(new Rect(Screen.width - 145,Screen.height - 15, 100,10), emptytexture);
		GUI.DrawTexture(new Rect(Screen.width - 145, Screen.height - 15, (float)(hp/(float)maxhp) * 100,10), hptexture);
	}
}
