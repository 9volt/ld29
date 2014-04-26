using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldLogic : MonoBehaviour {
	public WorldGen wg;
	private List<Vertex> food_targets;
	private List<Vertex> dig_targets;
	private List<Vertex> attack_targets;

	// Use this for initialization
	void Start () {
		wg = GameObject.FindGameObjectWithTag("world").GetComponent<WorldGen>();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown("Fire1")){

		}
		if(Input.GetButtonDown("Fire2")){
			
		}
	}
}
