﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RabbitLogic : MonoBehaviour {
	private Vertex mySquare;
	private Vertex currentDestination;
	private RabbitFinder rf;
	private RabbitMover rm;
	private WorldGen wg;
	private WorldLogic wl;
	private Vertex nextNode;
	private float last_action;
	public float speed = .5f;
	private Animator anim;

	public NameGen ng;
	public int str;
	public int spd;
	public string profession;
	public string myname;
	public int hp;
	public int maxhp;
	public int hunger;
	public int full;
	public int sex;

	// Use this for initialization
	void Start () {
		last_action = Time.time;
		mySquare = new Vertex(5,8);
		nextNode = mySquare;
		wg = GameObject.FindGameObjectWithTag("world").GetComponent<WorldGen>();
		wl = GameObject.FindGameObjectWithTag("world").GetComponent<WorldLogic>();
		rf = gameObject.GetComponent<RabbitFinder>();
		rm = gameObject.GetComponent<RabbitMover>();
		rm.SetPosition(wg.VertexToVector3(mySquare));
		currentDestination = new Vertex(15, 8);
		anim = gameObject.GetComponent<Animator>();
		ng = gameObject.GetComponent<NameGen>();

		//initiate rabbit stats
		myname = ng.getName();
		hp = 20;
		maxhp = 20;
		hunger = 7;
		full = 10;
		str = 1;
		spd = 1;
		profession = "Burrower";
		sex = Random.Range(0,2);

	}
	
	// Update is called once per frame
	void Update () {
		//Checks for a path to the destination (In case it has changed), and moves to the next node if not currently transitioning between nodes
		if(currentDestination != null && mySquare != currentDestination){
			if(!rm.Moving){
				mySquare = nextNode;
				List<Vertex> p = rf.FindPath(mySquare, currentDestination, wg.GetPathfindingCosts());
				if (p != null && p.Count > 1){
					nextNode = p[1];
					rm.Move(wg.VertexToVector3(nextNode));
				}else{
					currentDestination = mySquare;
				}
			}
		} else {
			//else pick new Destination or work
			currentDestination = wl.GetClosestDig(mySquare);
			if(mySquare == currentDestination && Time.time > last_action + speed){
				anim.SetTrigger("Dig");
				wl.Dig(mySquare, 1);
			}
		}
	}
}
