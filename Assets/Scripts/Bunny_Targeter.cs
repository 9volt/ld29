﻿using UnityEngine;
using System.Collections;


public class Bunny_Targeter : MonoBehaviour {
	public GameObject[] bunnies;
	private int pos;
	private GUIScript g;
	private CameraMove cm;

	// Use this for initialization
	void Start () {
		bunnies = GameObject.FindGameObjectsWithTag("Rabbit");
		cm = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraMove>();
		pos = 0;
		g = gameObject.GetComponent<GUIScript>();
	}
	
	// Update is called once per frame
	void Update () {
		bunnies = GameObject.FindGameObjectsWithTag("Rabbit"); //cause rabbits can die whenever
		if (bunnies.Length <= 0){
			g.currentRabbit = null;
		}else{
			if(Input.GetKeyDown(KeyCode.Tab)){
				g.currentRabbit.gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Rabbits";
				g.currentRabbit.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
				if (pos + 1 <= bunnies.Length - 1){
					pos++;
				}else{
					pos = 0;
				}
				cm.SetPosition(bunnies[pos].GetComponent<RabbitLogic>().mySquare);
				g.currentRabbit = bunnies[pos].GetComponent<RabbitLogic>();
				g.currentRabbit.gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "RabbitSelected";
			}

		}

	}
	
}
