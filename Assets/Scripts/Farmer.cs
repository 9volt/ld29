using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Farmer : MonoBehaviour {
	public Vertex pos;
	private Vertex next_node;
	private Vertex currentDestination;
	private RabbitMover rm;
	private RabbitFinder rf;
	private WorldGen wg;
	private WorldLogic wl;
	private int hp;
	private int maxhp = 200;
	private Animator anim;
	//private float last_action;
	public string type;
	private int i = 0; //number of ferrets currently spawned
	private int num_spawn = 3; //max number of ferrets to spawn
	private bool releasing = false;
	public int spd = 3;

	// Use this for initialization
	void Start () {
		next_node = pos;
		hp = maxhp;
		//last_action = Time.time;
		wg = GameObject.FindGameObjectWithTag("world").GetComponent<WorldGen>();
		wl = GameObject.FindGameObjectWithTag("world").GetComponent<WorldLogic>();
		rf = gameObject.GetComponent<RabbitFinder>();
		rm = gameObject.GetComponent<RabbitMover>();
		anim = gameObject.GetComponent<Animator>();
		rm.SetPosition(wg.VertexToVector3(pos));
		Camera.main.gameObject.GetComponent<CameraMove>().SetPosition(pos);
		FindTarget();
	}
	
	public bool Damage(int d){
		hp -= d;
		return hp <= 0;
	}
	
	void FindTarget(){
		for(int w = 1; w < wg.width-1; w++){
			for(int h = 1; h < wg.height -1; h++){
				Vertex v = new Vertex(w, h);
				if(wg.VertexToType(v) == WorldGen.TUNNEL){
					Vertex v2 = new Vertex(w+1, h);
					Vertex v3 = new Vertex(w, h+1);
					if (wg.VertexToType(v2) == WorldGen.TUNNEL || wg.VertexToType(v3) == WorldGen.TUNNEL){
						List<Vertex> p = rf.FindPath(pos, v, wg.GetPathfindingCosts());
						if(p != null && p.Count > 1){
							currentDestination = v;
							Debug.Log("Picked a tunnel " + currentDestination);
							return;
						}
					}
				}
			}
		}
	}

	void UpdateTarget(){
		if(hp <= 0 && currentDestination == pos){
			gameObject.SetActive(false);
		} else if(hp <= 0){
			for(int h = 0; h < wg.height; h++){
				if(wg.VertexToType(new Vertex(wg.width - 1, h)) == WorldGen.DIRT){
					currentDestination = new Vertex(wg.width - 1, h);
					h = wg.height;
				}
			}
		} else if(currentDestination == null){
				FindTarget();
		}
	}
	
	public void GoHome(){
		Debug.Log("leaving");
		hp = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if(currentDestination != null && pos != currentDestination){ //if not going to destination
			if(!rm.Moving){
				pos = next_node;
				UpdateTarget();
				List<Vertex> p = rf.FindPath(pos, currentDestination, wg.GetPathfindingCosts());
				if(p != null && p.Count > 1){
					next_node = p[1];
					rm.Move(wg.VertexToVector3(next_node));
				} else {
					Debug.Log("Reached " + currentDestination);
					currentDestination = pos;
				}
			}
		}else if(currentDestination == pos && !releasing){ //if havent started releasing
			releasing = true;
			anim.SetBool("Releasing", true);
			StartCoroutine(ReleaseFerrets());
		}
		//else currently releasing
	}

	IEnumerator ReleaseFerrets(){
		//wl.spawnFerret();
		Debug.Log("Spawn a Ferret");
		i++;
		if (i < num_spawn){
			yield return new WaitForSeconds(3);
			StartCoroutine(ReleaseFerrets());
		}else{
			Debug.Log("Going Home");
			GoHome();
			anim.SetBool("Releasing", false);
		}

	}

}
