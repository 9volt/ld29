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
	private RabbitLogic[] myPastRabbits;
	private bool game_over = false;
	private bool reload = false;
	private float scroll;
	public GameObject bg;
	public GameObject bg2;
	public Texture black;
	public bool won = false;
	public Texture mute;
	public Texture unmute;

	// Use this for initialization
	void Start () {
		if(reload){
			game_over = false;
			reload = false;
		}
		if(!game_over){
			scroll = Screen.height - 300;
			myPastRabbits = null;
			profession = Guard;
			wl = GameObject.FindGameObjectWithTag("world").GetComponent<WorldLogic>();
			currentRabbit = GameObject.FindGameObjectsWithTag("Rabbit")[0].GetComponent<RabbitLogic>();
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
		 if(PlayerPrefs.GetInt("Mute", 0) == 0){	
			 if(GUI.Button(new Rect(Screen.width - 42, 0, 42, 42), mute)){
				PlayerPrefs.SetInt("Mute", 1);	
				PlayerPrefs.Save();
			 }
		}else{
			if(GUI.Button(new Rect(Screen.width - 42, 0, 42, 42), unmute)){
				PlayerPrefs.SetInt("Mute", 0);	
				PlayerPrefs.Save();
			}
		}
		 if(!game_over){
			// Draw season and Burrow Stats
			GUI.Box(new Rect(0, 0, 400,42), "");
			GUI.DrawTexture(new Rect(5,5,32,32), season_art[wl.season]);
			GUI.DrawTexture(new Rect(70,0,32,32), bunny);
			GameObject[] g = GameObject.FindGameObjectsWithTag("Rabbit");

			if (g.Length <= 0){
				gameOver();
			}
			/*else if(wl.year == 4){
				won = true;
				gameOver();
			}*/

			GUI.Label(new Rect(100,10,100,32),"" + g.Length + " / " + wl.BurrowSleepCapacity() ); 
			GUI.DrawTexture(new Rect(150,0,32,32), houses);
			GUI.DrawTexture(new Rect(260,0,32,32), food);
			GUI.Label(new Rect(290,10,100,32),"" + wl.FoodCount() + " / " + wl.FoodCapacity() ); 
			GUI.DrawTexture(new Rect(340,2,32,32), foodcap);

			if(currentRabbit != null && currentRabbit.gameObject.activeSelf){

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

			//play again button
			if(GUI.Button(new Rect(Screen.width - 215, Screen.height - 160, 150, 150), "Play Again?")){
				reload = true;
				Application.LoadLevel("opening");//change to start scene
			}

			//credits
			int i;
 
			GUI.Label(new Rect(Screen.width/2 - 160, scroll + 50, 500, 40), "Your warren survived " + (((wl.year - 1) * 4) + (wl.season)) + " seasons and had a total of " + (myPastRabbits.Length -1) + " rabbits.");


			GUI.Label(new Rect(Screen.width/2 - 120, scroll +  120, 300, 40 ), "Gone, but not forgotten:");
			for(i = 0; i < myPastRabbits.Length-1; i++){ // -1 becuase the bunnyhop prefab is also caught by FindObjectsOfTypeAll
				currentRabbit = myPastRabbits[i];
				//if(!(currentRabbit.hp > 0)){
					
				//}else{
				//	currentRabbit.gameObject.SetActive(false);
				//	GUI.Label(new Rect(Screen.width/2 - 120, scroll + ((i+1)* 130), 300,40), "The Great Survivor");
				//}
				GUI.Label(new Rect(Screen.width/2 - 120, scroll + 20 + ((i+1)* 130), 300,40), currentRabbit.myname);
				GUI.DrawTexture(new Rect(Screen.width/2 - 120,  scroll + 40 + ((i+1)* 130), bunny.width, bunny.height), bunny);
				GUI.Label(new Rect(Screen.width/2 - 120 + bunny.width + 10,  scroll + 40 + ((i+1)* 130), 300, 40), currentRabbit.cause_of_death);

			}

			GUI.Label(new Rect(Screen.width/2 - 120, scroll + 40 + ((i+1)* 130), 500, 40), "Game created by Joseph and Sonya Utecht.");
			GUI.Label(new Rect(Screen.width/2 - 120, scroll + 60 + ((i+1)* 130), 300, 40), "Music by Jeff Craft.");
			GUI.Label(new Rect(Screen.width/2 - 120, scroll + 100 + ((i+1)* 130), 300, 40), "Thank you for playing!");

			scroll= scroll -.5f;

		}						
	}

	void gameOver(){
		game_over = true;
		myPastRabbits = (RabbitLogic[])Resources.FindObjectsOfTypeAll(typeof(RabbitLogic));
		gameObject.GetComponent<Camera>().cullingMask = 0;
		//Destroy(gameObject.GetComponent<WorldGen>());
		Destroy(wl);
		Destroy(gameObject.GetComponent<CameraMove>());
		bg.renderer.material.mainTexture = black;
		bg2.renderer.material.mainTexture = black;

		if (PlayerPrefs.GetInt("Most Rabbits", 0) < (myPastRabbits.Length-1)){
			PlayerPrefs.SetInt("Most Rabbits", myPastRabbits.Length-1);
		}
		if (PlayerPrefs.GetInt("Most Seasons", 0) < ((((wl.year - 1) * 4) + (wl.season)) )){
			PlayerPrefs.SetInt("Most Seasons", ((wl.year - 1) * 4) + (wl.season));
		}
		PlayerPrefs.Save();
	}
}
