using UnityEngine;
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
	public float hunger_tick = 30f;
	private float last_hunger;
	public float sleep_interval = 100f;
	private float last_sleep;
	private Animator anim;

	private bool need_sleep = false;
	private bool need_food = false;

	private bool digging = false;
	private bool filling = false;

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

	public const int MALE = 0;
	public const int FEMALE = 1;

	// Use this for initialization
	void Start () {
		last_hunger = Time.time;
		last_action = Time.time;
		last_sleep = Time.time + Random.Range(0, sleep_interval);
		mySquare = new Vertex(5,7);
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

	void Die(){
		anim.SetTrigger("Dead");
		StartCoroutine(DeathWait());
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
			// First check to make sure don't need sleep
			if(need_sleep){
				
				
			} else if(need_food){
				// Next check for food
				

			//else pick new Destination or work
			} else {
				if(profession == "Burrower"){
					currentDestination = wl.GetClosestDig(mySquare);
					if(currentDestination == null){
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
				}
			}
		}
		if(Time.time > last_hunger + hunger_tick){
			hunger--;
			last_hunger = Time.time;
			if(hunger <= 0){
				hp--;
				hunger = 0;
			}
		}
		if(hp <= 0){
			Die();
		}
	}

	IEnumerator DeathWait(){
		yield return new WaitForSeconds(.6f);
		gameObject.SetActive(false);
	}
}
