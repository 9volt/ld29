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
	public float sex_length = 1f;
	private float sex_start;
	private bool sexing = false;

	public const int MALE = 0;
	public const int FEMALE = 1;

	private bool horny = false;
	public float mating_cooldown = 30f;
	private float last_mating;
	public bool ready_for_mate = false;
	private RabbitLogic father;

	public string cause_of_death;
	private string current_action = "idle";
	public bool starting_rabbit = false;

	// Use this for initialization
	void Start () {
		rrand = Random.Range(0, 100);
		food_hold = 0;
		last_hunger = Time.time;
		last_action = Time.time;
		last_sleep = Time.time + Random.Range(0, sleep_interval);
		last_mating = Time.time + Random.Range(mating_cooldown/2, mating_cooldown);
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
		str = Random.Range(1,4);
		spd = Random.Range(1,4);
		speed = 1 / (float)spd;
		if(!starting_rabbit){
			profession = "Guard";
			sex = Random.Range(0,2);
		}
	}

	public string WhatAmIDoing(){
		return current_action;
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

	bool AreThereMales(){
		foreach(GameObject go in GameObject.FindGameObjectsWithTag("Rabbit")){
			RabbitLogic rl = go.GetComponent<RabbitLogic>();
			if(rl.sex == MALE){
					return true;
			}
		}
		return false;
	}

	RabbitLogic CanGetFemale(){
		Dictionary<RabbitLogic, int> ladies = new Dictionary<RabbitLogic, int>();
		foreach(GameObject go in GameObject.FindGameObjectsWithTag("Rabbit")){
			RabbitLogic rl = go.GetComponent<RabbitLogic>();
			if(rl.sex == FEMALE && rl.ready_for_mate){
				List<Vertex> d = rf.FindPath(mySquare, currentDestination, wg.GetPathfindingCosts());
				if(d != null){
					ladies[rl] = d.Count;
				}
			}
		}
		int lowest = -1;
		RabbitLogic lucky_lady = null;
		foreach(RabbitLogic rl in ladies.Keys){
			if(lowest == -1 || ladies[rl] < lowest){
				lucky_lady = rl;
				lowest = ladies[rl];
			}
		}
        return lucky_lady;
	}

	void Mate(RabbitLogic male){
		sexing = true;
		sex_start = Time.time;
		horny = false;
		last_mating = Time.time;
		ready_for_mate = false;
		father = male;
		anim.SetBool("Mating", false);
	}

	void Birth(){
		wl.BirthRabbit(father, this);
	}

	void PickDestination(){
		if(need_sleep && CanGetSleep()){
			ready_for_mate = false;
			// Find closest burrow with sleeping room
			Burrow b = wl.GetClosestBurrowSleep(mySquare);
			if(b != null){
				currentDestination = b.main_block;
				if(mySquare == currentDestination && Time.time > last_action + speed){
					anim.SetBool("Sleep", true);
					if(wl.StartSleep(mySquare)){
						current_action = "sleeping";
						sleeping = true;
						last_sleep = Time.time;
					}
				} else {
					current_action = "finding sleep";
				}
			}
		} else if(need_food && CanGetFood()){
			ready_for_mate = false;
			// Find closest burrow with food
			Burrow b = wl.GetClosestBurrowFood(mySquare);
			if(b != null){
				currentDestination = b.GetRandomBlock(rrand);
				if(mySquare == currentDestination && Time.time > last_action + speed){
					anim.SetTrigger("Dig");
					if(wl.EatFood(mySquare)){
						current_action = "eating";
						hunger = full;
						need_food = false;
					}
				} else {
					current_action = "finding food";
				}
			}
		} else if(ready_for_mate && AreThereMales()){
			// animation???
			anim.SetBool("Mating", true);
			current_action = "waiting for mate";
		} else if(horny && sex == FEMALE && CanGetSleep() && AreThereMales()){
			Burrow b = wl.GetClosestBurrowSleep(mySquare);
			if(b != null){
				currentDestination = b.main_block;
				if(mySquare == currentDestination && Time.time > last_action + speed){
					ready_for_mate = true;
				} else {
					current_action = "finding mating burrow";
				}
			}
		} else if(horny && sex == MALE && CanGetFemale() != null){
			RabbitLogic female = CanGetFemale();
			if(female.mySquare == mySquare){
				anim.SetBool("Hump", true);
				current_action = "mating";
				sexing = true;
				sex_start = Time.time;
				female.Mate(this);
				last_mating = Time.time;
			} else {
				currentDestination = female.mySquare;
				current_action = "moving to female";
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
						current_action = "moving to fill";
						filling = true;
					} else {
						current_action = "idle";
					}
				} else {
					current_action = "moving to dig";
					digging = true;
				}
				if(mySquare == currentDestination && Time.time > last_action + speed){
					anim.SetTrigger("Dig");
					if(digging){
						current_action = "digging";
						wl.Dig(mySquare, str);
						digging = false;
					}else if(filling){
						current_action = "filling";
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
					} else if(currentDestination != null){
						current_action = "moving to forage";
					} else {
						current_action = "idle";
					}
				} else {
					//return to closest burrow with space
					Burrow b = wl.GetClosestBurrowDepositFood(mySquare);
					if(b != null){
						current_action = "returning with food";
						currentDestination = b.main_block;
						if(mySquare == currentDestination && Time.time > last_action + speed){
							anim.SetTrigger("Dig");
							food_hold = wl.DepositFood(mySquare, food_hold);
						}
					} else {
						current_action = "looking for burrow to store food";
					}
				}
			} else if(profession == "Guard"){
				Burrow b = wl.GetClosestBurrowSleep(mySquare);
				if(b != null){
					currentDestination = b.GetRandomBlock(rrand);
					if(mySquare == currentDestination){
						current_action = "cowering";
					} else {
						current_action = "finding cover";
					}
				}
			}
		}
	}

	// Update is called once per frame
	void Update () {
		if(hp > 0){
			if(sexing && Time.time < sex_length + sex_start){
				return;
			}
			if(sexing && Time.time > sex_length + sex_start){
				sexing = false;
				if(sex == FEMALE){
					Birth();
				} else {
					anim.SetBool("Hump", false);
				}
			}
			//Checks for a path to the destination (In case it has changed), and moves to the next node if not currently transitioning between nodes
			if(currentDestination != null && mySquare != currentDestination){
				if(!rm.Moving){
					mySquare = nextNode;
					PickDestination();
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
				} else {
					PickDestination();
				}
			}
			if(Time.time > last_hunger + hunger_tick){
				hunger--;
				last_hunger = Time.time;
				if(hunger < (full * .25f)){
					need_food = true;
				}
				if(hunger > (full * .5f)){
					if(hp < maxhp) hp++;
				}
				if(need_sleep && !sleeping){
					hp -= 5;
					if(hp <= 0){
						cause_of_death = "Lack of sleep";
					}
				}
				if(hunger <= 0 && !sleeping){
					hp -= 4;
					hunger = 0;
					if(hp <= 0){
						cause_of_death = "Starved";
					}
				}
				if(hunger < 0) hunger = 0;
				if(wl.season == WorldLogic.WINTER && wg.VertexToType(mySquare) == WorldGen.AIR){
					hp -= 5;
					if(hp <= 0){
						cause_of_death = "Froze";
					}
				}
			}
		} else {
			current_action = "dieing";
			Die();
		}
		if(Time.time > last_sleep + sleep_interval){
			need_sleep = true;
		}
		if(Time.time > last_mating + mating_cooldown) {
			horny = true;
		}
		if(sex == MALE){
			horny = true;
		}
	}

	public bool Damage(string who, int d){
		sleeping = false;
		ready_for_mate = false;
		hp -= d;
		profession = "Guard";
		if(hp <= 0){
			cause_of_death = "Eaten by " + who;
		}
		return hp <= 0;
	}

	IEnumerator DeathWait(){
		yield return new WaitForSeconds(.6f);
		gameObject.SetActive(false);
	}
}
