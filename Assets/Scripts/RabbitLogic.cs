using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class RabbitLogic : MonoBehaviour {
	public Vertex mySquare;
	private Vertex currentDestination;
	private RabbitFinder rf;
	private RabbitMover rm;
	private WorldGen wg;
	private WorldLogic wl;
	private Vertex nextNode;
	private float last_action;
	public float speed = .5f;
	public float hunger_tick = 10f;
	private float last_hunger;
	public float sleep_interval = 100f;
	public float sleep_length = 30f;
	private float last_sleep;
	private Animator anim;
	private int food_hold;

	private bool need_sleep = false;
	private bool need_food = false;

	private bool digging = false;
	private bool filling = false;
	private bool sleeping = false;

	// used for getting random vertex in burrow
	private int rrand;

	private NameGen ng;
	public int str;
	public int spd;
	public string profession;
	public string myname;
	public int hp;
	public int maxhp;
	public int hunger;
	public int full;
	public int sex;

	public const int MALE = 0;
	public const int FEMALE = 1;

	// Use this for initialization
	void Start () {
		rrand = Random.Range(0, 100);
		food_hold = 0;
		last_hunger = Time.time;
		last_action = Time.time;
		last_sleep = Time.time + Random.Range(0, sleep_interval);
		nextNode = mySquare;
		currentDestination = mySquare;
		wg = GameObject.FindGameObjectWithTag("world").GetComponent<WorldGen>();
		wl = GameObject.FindGameObjectWithTag("world").GetComponent<WorldLogic>();
		rf = gameObject.GetComponent<RabbitFinder>();
		rm = gameObject.GetComponent<RabbitMover>();
		ng = gameObject.GetComponent<NameGen>();
		rm.SetPosition(wg.VertexToVector3(mySquare));
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
		if(sex == null){
			sex = Random.Range(0,2);
		}
	}

	public string WhatAmIDoing(){
		if(sleeping){
			return "sleeping";
		} else if(need_sleep){
			return "looking for sleep";
		} else if(mySquare == currentDestination){
		switch(profession){
			case "Burrower":
				return "burrowing";
			case "Forager":
				return "foraging";
			case "Guard":
				return "defending";
			default:
				return "bumming";
			}
		} else if(currentDestination == null){
			return "idling";
		} else {
			return "running";
		}
	}

	void Die(){
		anim.SetTrigger("Dead");
		StartCoroutine(DeathWait());
	}

	bool CanGetFood(){
		return wl.GetClosestBurrowFood(mySquare) != null;
	}

	bool CanGetSleep(){
		return wl.GetClosestBurrowSleep(mySquare) != null;
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
			if(sleeping){
				if(Time.time > last_sleep + sleep_length){
					sleeping = false;
					need_sleep = false;
					last_sleep = Time.time;
					wl.EndSleep(mySquare);
					anim.SetBool("Sleep", false);
				}
			// First check to make sure don't need sleep
			} else if(CanGetSleep() && need_sleep){
				// Find closest burrow with sleeping room
				Burrow b = wl.GetClosestBurrowSleep(mySquare);
				if(b != null){
					currentDestination = b.GetRandomBlock(rrand);
					if(mySquare == currentDestination && Time.time > last_action + speed){
						anim.SetBool("Sleep", true);
						if(wl.StartSleep(mySquare)){
							sleeping = true;
							last_sleep = Time.time;
						}
					}
				}
			} else if(CanGetFood() && need_food){
				// Find closest burrow with food
				Burrow b = wl.GetClosestBurrowFood(mySquare);
				if(b != null){
					currentDestination = b.GetRandomBlock(rrand);
					if(mySquare == currentDestination && Time.time > last_action + speed){
						anim.SetTrigger("Dig");
						if(wl.EatFood(mySquare)){
							hunger = full;
							need_food = false;
						}
					}
				}
				
				//else pick new Destination or work
			} else {
				if(profession == "Burrower"){
					// First try to find a square to dig
					currentDestination = wl.GetClosestDig(mySquare);
					if(currentDestination == null){
						// If that fails find a square to fill in
						currentDestination = wl.GetClosestFill(mySquare);
						if(currentDestination != null){
							filling = true;
						}
					} else {
						digging = true;
					}
					if(mySquare == currentDestination && Time.time > last_action + speed){
						anim.SetTrigger("Dig");
						if(digging){
							wl.Dig(mySquare, str);
							digging = false;
						}else if(filling){
							wl.Fill(mySquare, str);
							filling = false;
						}
					}
				} else if(profession == "Forager"){
					if(food_hold == 0){
						currentDestination = wl.GetClosestFood(mySquare);
						if(mySquare == currentDestination && Time.time > last_action + speed){
							anim.SetTrigger("Dig");
							food_hold = wl.TakeFood(mySquare, str);
						}
					} else {
						//return to closest burrow with space
						Burrow b = wl.GetClosestBurrowDepositFood(mySquare);
						if(b != null){
							currentDestination = b.main_block;
							if(mySquare == currentDestination && Time.time > last_action + speed){
								anim.SetTrigger("Dig");
								food_hold = wl.DepositFood(mySquare, food_hold);
							}
						}
					}
				} else if(profession == "Guard"){
					currentDestination = wl.GetClosestEnemy(mySquare);
					if(mySquare == currentDestination && Time.time > last_action + speed){
						anim.SetTrigger("Dig");
						// Attempt to damage enemy for str
						wl.DamageEnemy(mySquare, str);
					}
				}
			}
		}
		if(Time.time > last_hunger + hunger_tick){
			hunger--;
			last_hunger = Time.time;
			if(hunger < (full * .25f)){
				need_food = true;
			}
			if(hunger < (full * .50f)){
				if(hp < maxhp) hp++;
			}
			if(need_sleep && !sleeping){
				hp -= 5;
			}
			if(hunger <= 0 && !sleeping){
				hp--;
				hunger = 0;
			}
		}
		if(hp <= 0){
			Die();
		}
		if(Time.time > last_sleep + sleep_interval){
			need_sleep = true;
		}
	}

	public bool Damage(int d){
		hp -= d;
		profession = "Guard";
		return hp <= 0;
	}

	IEnumerator DeathWait(){
		yield return new WaitForSeconds(.6f);
		gameObject.SetActive(false);
	}
}
