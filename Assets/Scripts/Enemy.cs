using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour {
	public Vertex pos;
	private Vertex next_node;
	private Vertex currentDestination;
	private RabbitLogic target;
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
	}

	public bool Damage(int d){
		hp -= d;
		return hp <= 0;
	}

	void FindTarget(){
		GameObject[] rabbits = GameObject.FindGameObjectsWithTag("Rabbit");
		float distance = 9999f;
		foreach(GameObject r in rabbits){
			RabbitLogic rl = r.GetComponent<RabbitLogic>();
			if(Vertex.Distance(pos, rl.mySquare) < distance){
				target = rl;
				distance = Vertex.Distance(pos, rl.mySquare);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(hp <= 0){
			for(int h = 0; h < wg.height; h++){
				if(wg.VertexToType(new Vertex(wg.width - 1, h)) == WorldGen.DIRT){
					currentDestination = new Vertex(wg.width - 1, h);
					wl.RemoveEnemy(this);
					h = wg.height;
				}
			}
		}
		if(hp <= 0 && currentDestination == pos){
			gameObject.SetActive(false);
		}
		// Move towards current target
		if(currentDestination != null && pos != currentDestination){
			if(!rm.Moving){
				pos = next_node;
				List<Vertex> p = rf.FindPath(pos, currentDestination, wg.GetPathfindingCosts());
				if(p != null && p.Count > 1){
					next_node = p[1];
					rm.Move(wg.VertexToVector3(next_node));
				} else {
					currentDestination = pos;
				}
			}
		} else {
			// Find Rabbit and attack
			if(target != null){
				if(pos == target.mySquare && Time.time > last_action + speed){
					anim.SetTrigger("Pounce");
					last_action = Time.time;
					bool b = target.Damage(str);
					if(b){
						hp -= str;
						target = null;
					}
				} else {
					currentDestination = target.mySquare;
				}
			} else {
				FindTarget();
			}
		}
	}
}
