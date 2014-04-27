using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldLogic : MonoBehaviour {
	private WorldGen wg;
	private List<Vertex> food_targets;
	private List<Vertex> dig_targets;
	private List<Vertex> fill_targets;
	private List<Vertex> attack_targets;
	private Dictionary<Vertex, int> food_counts;
	private Dictionary<Vertex, int> dig_counts;
	public Texture green;
	public Texture yellow;
	public int dirt_strength = 10;
	public int food_amount = 10;
	public float random_food = .05f;

	public const int SPRING = 0;
	public const int SUMMER = 1;
	public const int FALL = 2;
	public const int WINTER = 3;

	public int season = SPRING;
	public float season_length = 150f;
	private float last_season;

	public AudioClip[] seasons;
	private AudioSource ass;

	// Use this for initialization
	void Start () {
		food_targets = new List<Vertex>();
		dig_targets = new List<Vertex>();
		fill_targets = new List<Vertex>();
		attack_targets = new List<Vertex>();
		food_counts = new Dictionary<Vertex, int>();
		dig_counts = new Dictionary<Vertex, int>();
		wg = gameObject.GetComponent<WorldGen>();
		ass = gameObject.GetComponent<AudioSource>();
		ass.clip = seasons[season];
		ass.Play();
		last_season = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		if(Time.time > season_length + last_season){
			season = (season + 1) % seasons.Length;
			ass.clip = seasons[season];
			ass.Play();
			last_season = Time.time;
		}
		if(Input.GetButtonDown("Fire1") && GUIUtility.hotControl == 0){
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
		if(Input.GetButtonDown("Fire2") && GUIUtility.hotControl == 0){
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
			} else if(type == WorldGen.TUNNEL){
				if(!fill_targets.Contains(target)){
					fill_targets.Add(target);
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
		foreach(Vertex v in fill_targets){
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
				if(t == WorldGen.DIRT){
					dig_counts[v] = dirt_strength;
				}
			}
		}
		// Create Farm
		for(int w = 1; w < wg.width - 1; w++){
			for(int h = 1; h < wg.height - 1; h++){
				Vertex v = new Vertex(w, h - 1);
				int t = wg.VertexToType(v);
				if(t == WorldGen.AIR){
					v = new Vertex(w, h);
					t = wg.VertexToType(v);
					if(t == WorldGen.DIRT && Random.Range(0f, 1f) < random_food){
						food_counts[v] = food_amount;
						wg.SetVertex(v, WorldGen.CARROT);
					}
				}
			}
		}
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
			if((cur_distance == -1f || Vertex.Distance(v, t) < cur_distance) && wg.ValidDig(t)){
				cur_distance = Vertex.Distance(v, t);
				closest = t;
			}
		}
		return closest;
	}

	public Vertex GetClosestFill(Vertex v){
		float cur_distance = -1f;
		Vertex closest = null;
		foreach(Vertex t in fill_targets){
			if((cur_distance == -1f || Vertex.Distance(v, t) < cur_distance) && wg.ValidDig(t)){
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
			dig_targets.Remove(v);
			wg.SetVertex(v, WorldGen.TUNNEL);
		}
	}

	public void Fill(Vertex v, int str){
		dig_counts[v] += str;
		if(dig_counts[v] > dirt_strength){
			fill_targets.Remove(v);
			wg.SetVertex(v, WorldGen.DIRT);
		}
	}
}
