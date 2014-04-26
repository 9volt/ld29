using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldLogic : MonoBehaviour {
	private WorldGen wg;
	private List<Vertex> food_targets;
	private List<Vertex> dig_targets;
	private List<Vertex> attack_targets;
	private Dictionary<Vertex, int> food_counts;
	private Dictionary<Vertex, int> dig_counts;
	public Texture green;
	public Texture yellow;
	public int dirt_strength = 10;

	// Use this for initialization
	void Start () {
		food_targets = new List<Vertex>();
		dig_targets = new List<Vertex>();
		attack_targets = new List<Vertex>();
		food_counts = new Dictionary<Vertex, int>();
		dig_counts = new Dictionary<Vertex, int>();
		wg = gameObject.GetComponent<WorldGen>();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown("Fire1")){
			// Attempt to set new target
			int type = wg.ClickToType();
			Vertex target = wg.ClickToVertex();
			if(type == WorldGen.CARROT){
				if(!food_targets.Contains(target)){
					food_targets.Add(target);
				}
			} else if(type == WorldGen.DIRT || type == WorldGen.GRASS){
				if(!dig_targets.Contains(target)){
					dig_targets.Add(target);
				}
			}
		}
		if(Input.GetButtonDown("Fire2")){
			// Attempt to remove target
			int type = wg.ClickToType();
			Vertex target = wg.ClickToVertex();
			if(type == WorldGen.CARROT){
				if(food_targets.Contains(target)){
					food_targets.Remove(target);
				}
			} else if(type == WorldGen.DIRT || type == WorldGen.GRASS){
				if(dig_targets.Contains(target)){
					dig_targets.Remove(target);
				}
			}
		}
	}

	void OnGUI(){
		foreach(Vertex v in food_targets){
			Vector3 p = Camera.main.WorldToScreenPoint(new Vector3((wg.transform.position.x + v.x), (wg.transform.position.y - v.y), 0));
			//Debug.Log(v + " " + p);
			GUI.DrawTexture(new Rect(p.x, Screen.height - p.y, 6, 6), green);
		}
		foreach(Vertex v in dig_targets){
			Vector3 p = Camera.main.WorldToScreenPoint(new Vector3((wg.transform.position.x + v.x), (wg.transform.position.y - v.y), 0));
			//Debug.Log(v + " " + p);
			GUI.DrawTexture(new Rect(p.x, Screen.height - p.y, 6, 6), yellow);
		}
	}

	public void PopulateWorld(){
		// Populate Dig strength
		for(int w = 0; w < wg.width; w++){
			for(int h = 0; h < wg.height; h++){
				Vertex v = new Vertex(w, h);
				int t = wg.VertexToType(v);
				if(t == WorldGen.GRASS || t == WorldGen.DIRT){
					dig_counts[v] = dirt_strength;
				}
			}
		}
		// Create Farm
	}

	public Vertex GetClosestEnemy(Vertex v){
		float cur_distance = -1f;
		Vertex closest = null;
		foreach(Vertex t in attack_targets){
			if(cur_distance == -1f || Vertex.Distance(v, t) < cur_distance){
				cur_distance = Vertex.Distance(v, t);
				closest = t;
			}
		}
		return closest;
	}

	public Vertex GetClosestDig(Vertex v){
		float cur_distance = -1f;
		Vertex closest = null;
		foreach(Vertex t in dig_targets){
			if(cur_distance == -1f || Vertex.Distance(v, t) < cur_distance){
				cur_distance = Vertex.Distance(v, t);
				closest = t;
			}
		}
		return closest;
	}

	public Vertex GetClosestFood(Vertex v){
		float cur_distance = -1f;
		Vertex closest = null;
		foreach(Vertex t in food_targets){
			if(cur_distance == -1f || Vertex.Distance(v, t) < cur_distance){
				cur_distance = Vertex.Distance(v, t);
				closest = t;
			}
		}
		return closest;
	}

	public void Dig(Vertex v, int str){
		dig_counts[v] -= str;
		if(dig_counts[v] < 0){
			dig_counts.Remove(v);
			dig_targets.Remove(v);
			wg.SetVertex(v, WorldGen.TUNNEL);
		}
	}
}
