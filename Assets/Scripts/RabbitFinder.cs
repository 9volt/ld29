using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Vertex {
	public int x;
	public int y;
	public Vertex(int nx, int ny){
		x = nx;
		y = ny;
	}

	public override string ToString (){
		return x + ":" + y;
	}

	public override bool Equals(System.Object obj){
		// If parameter is null return false.
		if (obj == null)
		{
			return false;
		}
		
		// If parameter cannot be cast to Point return false.
		Vertex p = obj as Vertex;
		if ((System.Object)p == null)
		{
			return false;
		}
		
		// Return true if the fields match:
		return (x == p.x) && (y == p.y);
	}

	public override int GetHashCode(){
		return x ^ y;
	}

	public static bool operator ==(Vertex a, Vertex b){
		// If both are null, or both are same instance, return true.
		if (System.Object.ReferenceEquals(a, b)){
			return true;
		}
		
		// If one is null, but not both, return false.
		if (((object)a == null) || ((object)b == null)){
			return false;
		}

		return (b.x == a.x) && (a.y == b.y);
	}

	public static bool operator !=(Vertex a, Vertex b){
		return !(a == b);
	}
}

public class RabbitFinder : MonoBehaviour {
	public WorldGen wg;
	public Vertex dest;
	public Vertex current;
	private List<Vertex> path;
	public Texture green;

	// Use this for initialization
	void Start () {
		wg = GameObject.FindGameObjectWithTag("world").GetComponent<WorldGen>();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown("Jump")){
			path = FindPath(new Vertex(1, 8), new Vertex(15, 8), wg.GetPathfindingCosts());
		}
	}

	void OnGUI(){
		if(path != null){
			foreach(Vertex v in path){
				Vector3 p = Camera.main.WorldToScreenPoint(new Vector3((wg.transform.position.x + v.x), (wg.transform.position.y - v.y), 0));
				//Debug.Log(v + " " + p);
				GUI.DrawTexture(new Rect(p.x, Screen.height - p.y, 6, 6), green);
			}
		}
	}

	Vertex GetLowestCost(List<Vertex> options, float[,] costs){
		Vertex lowest = new Vertex(-1, -1);
		float lowest_cost = 99999f;
		foreach(Vertex v in options){
			if(costs[v.x, v.y] > 0f && costs[v.x, v.y] < lowest_cost){
				lowest_cost = costs[v.x, v.y];
				lowest = v;
			}
		}
		return options[0];
	}

	private List<Vertex> FindPath(Vertex start, Vertex target, float[,] costs){
		Dictionary<Vertex, float> g_scores = new Dictionary<Vertex, float>();
		g_scores[start] = 0f;
		List<Vertex> closed_set = new List<Vertex>();
		List<Vertex> open_set = new List<Vertex>();
		open_set.Add(start);
		Dictionary<Vertex, Vertex> came_from = new Dictionary<Vertex, Vertex>();
		while(open_set.Count > 0){
			Vertex current = GetLowestCost(open_set, costs);
			if(current == target){
				return ReconstructPath(came_from, target);
			}
			open_set.Remove(current);
			closed_set.Add(current);
			foreach(Vertex neighbor in NeighborNodes(current, costs)){
				if(!closed_set.Contains(neighbor)){
					float tentative_g_score = g_scores[current] + costs[neighbor.x, neighbor.y];

					if(!open_set.Contains(neighbor) || tentative_g_score < g_scores[neighbor]){
						came_from[neighbor] = current;
						g_scores[neighbor] = tentative_g_score;
						if(!open_set.Contains(neighbor)){
							open_set.Add(neighbor);
						}
					}
				}
			}
		}
		return null;
	}

	private List<Vertex> NeighborNodes(Vertex current, float[,] costs){
		List<Vertex> ret = new List<Vertex>();
		if(current.y > 0){
			// top
			if(costs[current.x, current.y - 1] >= 0){
				ret.Add(new Vertex(current.x, current.y - 1));
			}
			// top left
			if(current.x > 0){
				if(costs[current.x - 1, current.y - 1] >= 0){
					ret.Add(new Vertex(current.x - 1, current.y - 1));
				}
			}
			// top right
			if(current.x + 1 < wg.width){
				if(costs[current.x + 1, current.y - 1] >= 0){
					ret.Add(new Vertex(current.x + 1, current.y - 1));
				}
			}
		}
		if(current.x + 1 < wg.width){
			// right
			if(costs[current.x + 1, current.y] >= 0){
				ret.Add(new Vertex(current.x + 1, current.y));
			}
		}
		if(current.y + 1 < wg.height){
			// bottom
			if(costs[current.x, current.y + 1] >= 0){
				ret.Add(new Vertex(current.x, current.y + 1));
			}
			// bottom left
			if(current.x > 0){
				if(costs[current.x - 1, current.y + 1] >= 0){
					ret.Add(new Vertex(current.x - 1, current.y + 1));
				}
			}
			// bottom right
			if(current.x + 1 < wg.width){
				if(costs[current.x + 1, current.y + 1] >= 0){
					ret.Add(new Vertex(current.x + 1, current.y + 1));
				}
			}
		}
		if(current.x > 0){
			// left
			if(costs[current.x - 1, current.y] >= 0){
				ret.Add(new Vertex(current.x - 1, current.y));
			}
		}
		return ret;
	}

	private List<Vertex> ReconstructPath(Dictionary<Vertex, Vertex> came_from, Vertex current_node){
		if(came_from.ContainsKey(current_node)){
			List<Vertex> p = ReconstructPath(came_from, came_from[current_node]);
			p.Add(current_node);
			return p;
		} else {
			List<Vertex> d = new List<Vertex>();
			d.Add(current_node);
			return d;
		}
	}
}
