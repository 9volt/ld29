using UnityEngine;
using System.Collections;

public class GUIScript : MonoBehaviour {
	private Texture profession;
	public Texture Burrower;
	public Texture Forager;
	public Texture Guard;
	public RabbitLogic currentRabbit;
	public Texture hptexture;
	public Texture hungertexture;
	public Texture emptytexture;
	public Texture male;
	public Texture female;
	
	// Use this for initialization
	void Start () {
		profession = Guard; 
	}
	
	// Update is called once per frame
	void Update () {
		if (currentRabbit.profession == "Guard"){
			profession = Guard;
		}else if (currentRabbit.profession == "Forager"){
			profession = Forager;
		}else{
			profession = Burrower;
		}
	}
	
	void OnGUI(){

//		if (GUI.Button(new Rect(Screen.width - 200, Screen.height - 200, 200,100), "Change name")){
//			name = gameObject.GetComponent<NameGen>().getName();
//		}

		GUI.Box(new Rect(Screen.width - 200, Screen.height - 100, 200,100), currentRabbit.myname);
		if(currentRabbit.sex == RabbitLogic.FEMALE){
			GUI.DrawTexture(new Rect(Screen.width - 55, Screen.height - 100, 30,30), female);
		}else{
			GUI.DrawTexture(new Rect(Screen.width - 55, Screen.height - 100, 30,30), male);
		}
		//profession icon
		GUI.DrawTexture(new Rect(Screen.width - 195, Screen.height - 93, 30,30), profession);
		//stats
		GUI.Label(new Rect(Screen.width - 200 + profession.width + 10, Screen.height - 80, 50,20), "Spd:" + currentRabbit.spd);
		GUI.Label(new Rect(Screen.width - 200 + profession.width + 10, Screen.height - 60, 50,20), "Str:" + currentRabbit.str);
		//hunger and hp bars	
		GUI.Label(new Rect(Screen.width - 195,Screen.height - 35, 100,20), "Hunger:");
		GUI.Label(new Rect(Screen.width - 195,Screen.height - 20, 100,20), "HP:");
		GUI.DrawTexture(new Rect(Screen.width - 145,Screen.height - 30, 100,10), emptytexture);
		GUI.DrawTexture(new Rect(Screen.width - 145, Screen.height - 30, (float)(currentRabbit.hunger/(float)currentRabbit.full) * 100,10), hungertexture);
		GUI.DrawTexture(new Rect(Screen.width - 145,Screen.height - 15, 100,10), emptytexture);
		GUI.DrawTexture(new Rect(Screen.width - 145, Screen.height - 15, (float)(currentRabbit.hp/(float)currentRabbit.maxhp) * 100,10), hptexture);
	}
}
