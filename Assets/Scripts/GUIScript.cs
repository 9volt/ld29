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
	public Texture[] season_art;
	private WorldLogic wl;
	public Texture bunny;
	public Texture food;
	public Texture foodcap;
	public Texture houses;
	public string action = "doing something!";
	private static  RabbitLogic[] myPastRabbits;
	private static bool game_over = false;

	// Use this for initialization
	void Start () {
		if(!game_over){
			myPastRabbits = null;
			profession = Guard;
			wl = GameObject.FindGameObjectWithTag("world").GetComponent<WorldLogic>();
			currentRabbit = GameObject.FindGameObjectsWithTag("Rabbit")[0].GetComponent<RabbitLogic>();
		}else{

		}

	}
	
	// Update is called once per frame
	void Update () {
		if(currentRabbit){
			if (currentRabbit.profession == "Guard"){
				profession = Guard;
			}else if (currentRabbit.profession == "Forager"){
				profession = Forager;
			}else{
				profession = Burrower;
			}
		}
	}
	
	void OnGUI(){

		if(!game_over){
			// Draw season and Burrow Stats
			GUI.Box(new Rect(0, 0, 400,42), "");
			GUI.DrawTexture(new Rect(0,0,32,32), season_art[wl.season]);
			GUI.DrawTexture(new Rect(70,0,32,32), bunny);
			GameObject[] g = GameObject.FindGameObjectsWithTag("Rabbit");

			if (g.Length <= 0){
				gameOver();
			}

			GUI.Label(new Rect(100,10,100,32),"" + g.Length + " / " + wl.BurrowSleepCapacity() ); 
			GUI.DrawTexture(new Rect(150,0,32,32), houses);
			GUI.DrawTexture(new Rect(260,0,32,32), food);
			GUI.Label(new Rect(290,10,100,32),"" + wl.FoodCount() + " / " + wl.FoodCapacity() ); 
			GUI.DrawTexture(new Rect(340,2,32,32), foodcap);

	//		if (GUI.Button(new Rect(Screen.width - 200, Screen.height - 200, 200,100), "Change name")){
	//			name = gameObject.GetComponent<NameGen>().getName();
	//		}
			if(currentRabbit != null){

				//Targeted Rabbit InfoBox
				GUI.Box(new Rect(Screen.width - 220, Screen.height - 100, 220,100), currentRabbit.myname);
				if(currentRabbit.sex == RabbitLogic.FEMALE){
					GUI.DrawTexture(new Rect(Screen.width - 28, Screen.height - 100, 30,30), female);
				}else{
					GUI.DrawTexture(new Rect(Screen.width - 28, Screen.height - 100, 30,30), male);
				}

				//profession switching button
				if(GUI.Button(new Rect(Screen.width - 215, Screen.height - 93, 1.5f * profession.width, 1.5f * profession.height), profession)){
					if(currentRabbit.profession == "Burrower"){
						currentRabbit.profession = "Forager";
					}else if(currentRabbit.profession == "Forager"){
						currentRabbit.profession = "Guard";
					}else{
						currentRabbit.profession = "Burrower";
					}
				}

				//stats
				GUI.Label(new Rect(Screen.width - 220 + 1.8f * profession.width, Screen.height - 80, 180,30),"I'm " + currentRabbit.WhatAmIDoing());
				GUI.Label(new Rect(Screen.width - 220 + 1.8f * profession.width, Screen.height - 60, 120,20), "Str:" + currentRabbit.str + "     Spd:" + currentRabbit.spd);
				//hunger and hp bars	
				GUI.Label(new Rect(Screen.width - 195,Screen.height - 35, 100,20), "Hunger:");
				GUI.Label(new Rect(Screen.width - 195,Screen.height - 20, 100,20), "HP:");
				GUI.DrawTexture(new Rect(Screen.width - 145,Screen.height - 30, 100,10), emptytexture);
				GUI.DrawTexture(new Rect(Screen.width - 145, Screen.height - 30, (float)(currentRabbit.hunger/(float)currentRabbit.full) * 100,10), hungertexture);
				GUI.DrawTexture(new Rect(Screen.width - 145,Screen.height - 15, 100,10), emptytexture);
				GUI.DrawTexture(new Rect(Screen.width - 145, Screen.height - 15, (float)(currentRabbit.hp/(float)currentRabbit.maxhp) * 100,10), hptexture);


				//To indicate your rabbit
				currentRabbit.gameObject.GetComponent<SpriteRenderer>().color = Color.gray;
			}
		}else{
			if(GUI.Button(new Rect(Screen.width - 215, Screen.height - 93, 150, 150), "Play Again?")){
				game_over = false;
				Application.LoadLevel("SimWarren");
			}
		}
	}

	void gameOver(){
		game_over = true;
		RabbitLogic[] myPastRabbits = (RabbitLogic[])Resources.FindObjectsOfTypeAll(typeof(RabbitLogic));
		Application.LoadLevel("game_over_level");

	}
}
