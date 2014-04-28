using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Burrow {
	public List<Vertex> blocks;
	public int food;
	public int rabbits;
	public Vertex main_block;
	
	public Burrow(List<Vertex> nblocks){
		blocks = nblocks;
		main_block = nblocks[0];
		rabbits = 0;
	}

	public int FoodCapacity(){
		return blocks.Count * 2;
	}
	
	public int RabbitCapacity(){
		return 1;
	}
	
	public Vertex GetRandomBlock(int seed){
		return blocks[seed % blocks.Count];
	}
	
	public bool Contains(Vertex v){
		return blocks.Contains(v);
	}

	public int DepositFood(int f){
		if(food + f < FoodCapacity()){
			food += f;
			return 0;
		} else {
			int room = FoodCapacity() - food;
			food += room;
			return f - room;
		}
	}
	
	public void Merge(Burrow b){
		blocks.AddRange(b.blocks);
		food = food + b.food;
		if(food > FoodCapacity()){
			food = FoodCapacity();
		}
		rabbits = rabbits + b.rabbits;
	}
}


public class WorldLogic : MonoBehaviour {
	private WorldGen wg;
	private List<Vertex> food_targets;
	private List<Vertex> dig_targets;
	private List<Vertex> fill_targets;
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
	public int year = 1;
	public float season_length = 150f;
	private float last_season;

	public AudioClip[] seasons;
	public AudioClip oh_shit_a_fox;
	private AudioSource ass;
	private AudioSource ass2;
	
	private List<Burrow> burrows;

	public GameObject rabbit;
	public int starting_rabbits = 4;

	public GameObject fox;
	public GameObject hawk;
	public GameObject farmer;

	public int annual_carrots = 5;


	// Use this for initialization
	void Start () {
		food_targets = new List<Vertex>();
		dig_targets = new List<Vertex>();
		fill_targets = new List<Vertex>();
		food_counts = new Dictionary<Vertex, int>();
		dig_counts = new Dictionary<Vertex, int>();
		wg = gameObject.GetComponent<WorldGen>();
		ass = gameObject.GetComponent<AudioSource>();
		ass2 = Camera.main.gameObject.GetComponent<AudioSource>();
		ass.clip = seasons[season];
		ass.Play();
		last_season = Time.time;
		burrows = new List<Burrow>();

	}
	
	// Update is called once per frame
	void Update () {
		// Update the season
		if(Time.time > season_length + last_season){
			season = (season + 1) % seasons.Length;
			ass.clip = seasons[season];
			ass.Play();
			if(season == FALL){
				SpawnFoxes();
			}
			if(season == SUMMER){
				SpawnHawks();
			}
			if(season == SPRING){
				year++;
				PlantCrops();
			}
			if(season == WINTER){
				DespawnFox();
				DespawnHawk();
				SpawnFarmer();
				KillCrops();
			}
			last_season = Time.time;
		}
		if(Input.GetButtonDown("Fire1") && GUIUtility.hotControl == 0){
			// Attempt to set new target
			int type = wg.ClickToType();
			Vertex target = wg.ClickToVertex();
			if(type == WorldGen.CARROT){
				if(!food_targets.Contains(target) && target.InBounds(new Vertex(wg.width, wg.height))){
					food_targets.Add(target);
				}
			} else if(type == WorldGen.DIRT || type == WorldGen.GRASS){
				if(!dig_targets.Contains(target) && target.InBounds(new Vertex(wg.width, wg.height))){
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
				} else {
					fill_targets.Remove(target);
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
		foreach(Burrow b in burrows){
			foreach(Vertex v in b.blocks){
				Vector3 p = Camera.main.WorldToScreenPoint(new Vector3((wg.transform.position.x + v.x), (wg.transform.position.y - v.y), 0));
				//Debug.Log(v + " " + p);
				GUI.DrawTexture(new Rect(p.x, Screen.height - p.y, 3, 3), green);
			}
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

		PlantCrops();

		// Spawn Rabbits
		for(int w = wg.width / 2; w < (wg.width / 2) + starting_rabbits; w++){
			for(int h = 0; h < wg.height; h++){
				Vertex v = new Vertex(w, h);
				if(wg.VertexToType(v) == WorldGen.DIRT){
					v = new Vertex(v.x, v.y - 1);
					rabbit.SetActive(false);
					GameObject r = (GameObject)Instantiate(rabbit, transform.position, transform.rotation);
					r.GetComponent<RabbitLogic>().mySquare = v;
					r.GetComponent<RabbitLogic>().sex = w % 2;
					r.SetActive(true);
					h = wg.height;
				}
			}
		}

	}

	void DespawnFox(){
		foreach(GameObject go in GameObject.FindGameObjectsWithTag("fox")){
			go.GetComponent<Enemy>().GoHome();
		}
	}

	void DespawnHawk(){
		foreach(GameObject go in GameObject.FindGameObjectsWithTag("hawk")){
			go.GetComponent<Hawk>().leaving = true;
		}
	}

	void SpawnFoxes(){
		// Spawn Fox
		for(int w = wg.width - year - 6; w < wg.width - 6; w++){
			for(int h = 0; h < wg.height; h++){
				Vertex v = new Vertex(w, h);
				if(wg.VertexToType(v) == WorldGen.DIRT){
					v = new Vertex(v.x, v.y - 1);
					fox.SetActive(false);
					GameObject f = (GameObject)Instantiate(fox, transform.position, transform.rotation);
					f.GetComponent<Enemy>().pos = v;
					f.SetActive(true);
					h = wg.height;
				}
			}
		}
	}

	void SpawnHawks(){
		for(int w = wg.width - year - 1; w < wg.width - 1; w++){ 
			for(int h = 0; h < wg.height; h++){
				Vertex v = new Vertex(w, h);
				if(wg.VertexToType(v) == WorldGen.AIR){
					v = new Vertex(v.x, v.y - 1);
					GameObject f = (GameObject)Instantiate(hawk, transform.position, transform.rotation);
					f.SetActive(true);
					h = wg.height;
				}
			}
		}
	}

	void SpawnFarmer(){
		// Spawn Farmer
		int w = 5;
		for(int h = 0; h < wg.height; h++){
			Vertex v = new Vertex(w, h);
			if(wg.VertexToType(v) == WorldGen.DIRT){
				v = new Vertex(v.x, v.y - 1);
				farmer.SetActive(false);
				GameObject f = (GameObject)Instantiate(farmer,wg.VertexToVector3(v), transform.rotation);//wg.VertexToVector3(v)
				f.GetComponent<Farmer>().pos = v;
				f.SetActive(true);
				h = wg.height;
			}
		}
		ass2.clip = oh_shit_a_fox;
		ass2.loop = false;
		ass2.Play();
	}

	void PlantCrops(){
		// Create Farm
		while(food_counts.Keys.Count < annual_carrots){
			for(int w = 1; w < wg.width - 1; w++){
				for(int h = 1; h < wg.height - 1; h++){
					Vertex v = new Vertex(w, h - 1);
					int t = wg.VertexToType(v);
					if(t == WorldGen.AIR && food_counts.Keys.Count < annual_carrots){
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
	}

	void KillCrops(){
		foreach(Vertex crop in food_counts.Keys){
			wg.SetVertex(crop, WorldGen.TUNNEL);
			food_targets.Remove(crop);
		}
		food_counts = new Dictionary<Vertex, int>();
	}

	public void BirthRabbit(RabbitLogic father, RabbitLogic mother){
		rabbit.SetActive(false);
		GameObject r = (GameObject)Instantiate(rabbit, transform.position, transform.rotation);
		r.GetComponent<RabbitLogic>().mySquare = mother.mySquare;
		r.GetComponent<RabbitLogic>().profession = "Guard";
		r.SetActive(true);
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

	public Burrow GetClosestBurrowFood(Vertex v){
		float cur_distance = -1f;
		Burrow closest = null;
		foreach(Burrow b in burrows){
			if((cur_distance == -1f || Vertex.Distance(b.main_block, v) < cur_distance) && b.food > 0){
				cur_distance = Vertex.Distance(b.main_block, v);
				closest = b;
			}
		}
		return closest;
	}

	public Burrow GetClosestBurrowDepositFood(Vertex v){
		float cur_distance = -1f;
		Burrow closest = null;
		foreach(Burrow b in burrows){
			if((cur_distance == -1f || Vertex.Distance(b.main_block, v) < cur_distance) && b.food < b.FoodCapacity()){
				cur_distance = Vertex.Distance(b.main_block, v);
				closest = b;
			}
		}
		return closest;
	}


	public bool StartSleep(Vertex v){
		Burrow b = VertexInBurrow(v);
		if(b != null && b.rabbits < b.RabbitCapacity()){
			b.rabbits++;
			return true;
		}
		return false;
	}

	public void EndSleep(Vertex v){
		Burrow b = VertexInBurrow(v);
		if(b != null && b.rabbits > 0){
			b.rabbits--;
		}
	}

	public int DepositFood(Vertex v, int food){
		Burrow b = VertexInBurrow(v);
		if(b != null){
			return b.DepositFood(food);
		}
		return food;
	}

	public bool EatFood(Vertex v){
		Burrow b = VertexInBurrow(v);
		if(b != null && b.food > 0){
			b.food--;
			return true;
		}
		return false;
	}

	public Burrow GetClosestBurrowSleep(Vertex v){
		float cur_distance = -1f;
		Burrow closest = null;
		foreach(Burrow b in burrows){
			if((cur_distance == -1f || Vertex.Distance(b.main_block, v) < cur_distance) && b.rabbits < b.RabbitCapacity()){
				cur_distance = Vertex.Distance(b.main_block, v);
				closest = b;
			}
		}
		return closest;
	}

	public void Dig(Vertex v, int str){
		dig_counts[v] -= str;
		if(dig_counts[v] < 0){
			dig_targets.Remove(v);
			wg.SetVertex(v, WorldGen.TUNNEL);
			PopulateBurrows();
		}
	}

	public int TakeFood(Vertex v, int str){
		if(food_counts.ContainsKey(v)){
			food_counts[v] -= str;
			if(food_counts[v] < 0){
				int ret_food = str + food_counts[v];
				food_counts.Remove(v);
				food_targets.Remove(v);
				wg.SetVertex(v, WorldGen.TUNNEL);
				return ret_food;
			}
			return str;
		}
		return 0;
	}

	public void Fill(Vertex v, int str){
		dig_counts[v] += str;
		if(dig_counts[v] > dirt_strength){
			fill_targets.Remove(v);
			wg.SetVertex(v, WorldGen.DIRT);
			PopulateBurrows();
		}
	}

	public int FoodCount(){
		int total = 0;
		foreach(Burrow b in burrows){
			total += b.food;
		}
		return total;
	}

	public int FoodCapacity(){
		int total = 0;
		foreach(Burrow b in burrows){
			total += b.FoodCapacity();
		}
		return total;
	}
	
	public int BurrowSleepCapacity(){
		int total = 0;
		foreach(Burrow b in burrows){
			total += b.RabbitCapacity();
		}
		return total;
	}

	public int BurrowCount(){
		return burrows.Count;
	}

	// Burrow logic
	private Burrow VertexInBurrow(Vertex v){
		foreach(Burrow b in burrows){
			if(b.Contains(v)){
				return b;
			}
		}
		return null;
	}

	// How to find burrows
	// Walk map looking for a tunnel with a tunnel over it and dirt bellow it, then continue right until either one of those stops being true
	// Check each vertex to make sure it doesn't belong to a burrow, if it does make a new burrow out of what has been found and then merge
	private void PopulateBurrows(){
		Vertex cur_point = null;
		Vertex start_of_burrow = null;
		List<Vertex> vertex_in_burrow = new List<Vertex>();
		Vertex one_above = null;
		Vertex two_above = null;
		Vertex one_below = null;
		Burrow growing = null;
		for(int h = 2; h < wg.height - 1; h++){
			for(int w = 1; w < wg.width - 1; w++){
				cur_point = new Vertex(w, h);
				one_above = new Vertex(w, h - 1);
				two_above = new Vertex(w, h - 2);
				one_below = new Vertex(w, h + 1);
				if(start_of_burrow == null
				   && wg.VertexToType(cur_point) == WorldGen.TUNNEL
				   && wg.VertexToType(one_above) == WorldGen.TUNNEL
				   && wg.VertexToType(two_above) == WorldGen.DIRT
				   && wg.VertexToType(one_below) == WorldGen.DIRT){
					// We have found the start of a burrow
					growing = VertexInBurrow(cur_point);
					start_of_burrow = cur_point;
					vertex_in_burrow.Add(cur_point);
					vertex_in_burrow.Add(one_above);
				} else if(wg.VertexToType(cur_point) == WorldGen.TUNNEL
				          && wg.VertexToType(one_above) == WorldGen.TUNNEL
				          && wg.VertexToType(two_above) == WorldGen.DIRT
				          && wg.VertexToType(one_below) == WorldGen.DIRT){
					vertex_in_burrow.Add(cur_point);
					vertex_in_burrow.Add(one_above);
				} else if(start_of_burrow != null
				          && (wg.VertexToType(cur_point) != WorldGen.TUNNEL
				          || wg.VertexToType(one_above) != WorldGen.TUNNEL
				    	  || wg.VertexToType(two_above) != WorldGen.DIRT
				          || wg.VertexToType(one_below) != WorldGen.DIRT)){
					// We have found the end
					if(vertex_in_burrow.Count < 4){
						// burrow was too small
						if(growing != null){
							burrows.Remove(growing);
						}
					} else {
						if(growing != null){
							growing.blocks = vertex_in_burrow;
						} else {
							growing = new Burrow(vertex_in_burrow);
							burrows.Add(growing);
						}
					}
					growing = null;
					start_of_burrow = null;
					vertex_in_burrow = new List<Vertex>();
				}
			}
		}
	}
}
