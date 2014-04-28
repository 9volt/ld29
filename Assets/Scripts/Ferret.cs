using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ferret : MonoBehaviour {
	public Vertex pos;
	private Vertex next_node;
	private Vertex currentDestination;
	private Burrow target;
	public int maxhp;
	private int hp;
	private RabbitMover rm;
	private WorldGen wg;
	private WorldLogic wl;
	private RabbitFinder rf;
	private Animator anim;
	private float last_action;
	private float speed = 1f;
	public int str = 5;
	public int spd = 3;
	public string type;

	// Use this for initialization
	void Start () {
		next_node = pos;
		hp = maxhp;
		last_action = Time.time;
		wg = GameObject.FindGameObjectWithTag("world").GetComponent<WorldGen>();
		wl = GameObject.FindGameObjectWithTag("world").GetComponent<WorldLogic>();
		rf = gameObject.GetComponent<RabbitFinder>();
		rm = gameObject.GetComponent<RabbitMover>();
		anim = gameObject.GetComponent<Animator>();
		rm.SetPosition(wg.VertexToVector3(pos));
		Camera.main.gameObject.GetComponent<CameraMove>().SetPosition(pos);
	}

	public bool Damage(int d){
		hp -= d;
		return hp <= 0;
	}

	void FindTarget(){
		Burrow b = wl.GetClosestBurrowOccupy(pos);
		if(b != null){
			currentDestination = b.main_block;
			target = b;
		}
	}
	
	void UpdateTarget(){
		if(hp <= 0){
			gameObject.SetActive(false);
		} else if(target != null){
			if(Time.time > last_action + speed){
				//anim.SetTrigger("Pounce");
				last_action = Time.time;
				bool b = target.Damage(type, str);
				if(b){
					hp -= str;
					target = null;
				}
			} else {
				currentDestination = target.mySquare;
			}
		}
	}

	public void GoHome(){
		Debug.Log("leaving");
		hp = 0;
	}

	// Update is called once per frame
	void Update () {
		// Move towards current target
		if(currentDestination != null && pos != currentDestination){
			if(!rm.Moving){
				pos = next_node;
				UpdateTarget();
				List<Vertex> p = rf.FindPath(pos, currentDestination, wg.GetPathfindingCosts());
				if(p != null && p.Count > 1){
					next_node = p[1];
					rm.Move(wg.VertexToVector3(next_node));
				} else {
					currentDestination = pos;
				}
			}
		} else {
			FindTarget();
			UpdateTarget();
		}
	}
}
