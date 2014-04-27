using UnityEngine;
using System.Collections;

public class WorldGen : MonoBehaviour {
	public Sprite dirt;
	public Sprite grass;
	public Sprite carrot;
	private Sprite[] tunnels;
	private Sprite[] grasses;
	public GameObject block;
	public int width;
	public int height;
	private int[,] world;
	private GameObject[,] prefabs;
	private WorldLogic wl;

	public const int OUT = -1;
	public const int AIR = 0;
	public const int DIRT = 1;
	public const int GRASS = 2;
	public const int CARROT = 3;
	public const int TUNNEL = 4;


	// tunnel index for position of open spot
	const int FULL = 0;
	const int TOP = 1;
	const int RIGHT = 2;
	const int TOPRIGHT = 3;
	const int BOTTOM = 4;
	const int TOPBOTTOM = 5;
	const int BOTTOMRIGHT = 6;
	const int TOPRIGHTBOTTOM = 7;
	const int LEFT = 8;
	const int TOPLEFT = 9;
	const int LEFTRIGHT = 10;
	const int LEFTTOPRIGHT = 11;
	const int LEFTBOTTOM = 12;
	const int BOTTOMLEFTTOP = 13;
	const int LEFTBOTTOMRIGHT = 14;
	const int EMPTY = 15;

	const int GFULL = 0;
	const int GLEFT = 1;
	const int GRIGHT = 2;
	const int GBUMB = 3;

	// Use this for initialization
	void Start () {
		tunnels = Resources.LoadAll<Sprite>("tunnels");
		grasses = Resources.LoadAll<Sprite>("grasses");
		wl = gameObject.GetComponent<WorldLogic>();
		prefabs = new GameObject[width,height];
		InstantiateWorld();
		world = new int[width, height];
		BuildGround();
		DrawGround();
		wl.PopulateWorld();
	}
	
	// Update is called once per frame
	void Update () {
		DrawGround();
	}

	public float[,] GetPathfindingCosts(){
		float[,] ret = new float[width,height];
		for(int w = 0; w < width; w++){
			for(int h = 0; h < height; h++){
				//first check to make sure this tile is open
				if(world[w, h] == AIR || world[w, h] == TUNNEL || world[w, h] == CARROT){
					//Next check to make sure we aren't on the bottom of the world
					if(h + 1 == height){
						ret[w,h] = 0f;
					} else if(world[w,h] == TUNNEL){
						ret[w,h] = 0f;
					} else {
						switch(world[w,h+1]){
							case DIRT:
								ret[w,h] = 0f;
								break;
//							case GRASS:
//								ret[w,h] = .5f;
//								break;
							case CARROT:
								ret[w,h] = 1f;
								break;
							case AIR:
								if(h+1 < height && world[w, h+1] == DIRT || world[w, h+1] == TUNNEL || world[w, h+1] == CARROT){
									ret[w, h] = 1f;
								// Fake Diagnal
								} else if(h+2 < height && (world[w, h+2] == DIRT || world[w, h+2] == TUNNEL || world[w, h+2] == CARROT)
								          && ((w-1 > 0 && (world[w-1, h+1] == DIRT || world[w-1, h+1] == TUNNEL || world[w-1, h+1] == CARROT)) || 
								    		 (w+1 < width && (world[w+1, h+1] == DIRT || world[w+1, h+1] == TUNNEL || world[w+1, h+1] == CARROT)))){
									ret[w, h] = 10f;
								} else {
									ret[w,h] = -1f;
								}
								break;
							case TUNNEL:
								ret[w,h] = 1f;
								break;
							default:
								ret[w,h] = -1f;
								break;
						}
					}
				} else {
					ret[w,h] = -1f;
				}
			}
		}
		return ret;
	}

	public void SetVertex(Vertex v, int t){
		if(v.x < world.GetLength(0) && v.x >= 0 && v.y < world.GetLength(1) && v.y >= 0){
			world[v.x, v.y] = t;
		}
	}

	public int VertexToType(Vertex v){
		if(v.x < world.GetLength(0) && v.x >= 0 && v.y < world.GetLength(1) && v.y >= 0){
			return world[v.x, v.y];
		}
		return OUT;
	}

	public Vertex Vector3ToVertex(Vector3 v){
		return new Vertex(Mathf.RoundToInt(transform.position.x - v.x), Mathf.RoundToInt(transform.position.y + v.y));
	}

	public Vertex ClickToVertex(){
		return new Vertex(CameraW(), CameraH());

	}

	public int ClickToType(){
		if(CameraW() >= world.GetLength(0) || CameraW() < 0){
			return OUT;
		}
		if(CameraH() >= world.GetLength(1) || CameraH() < 0){
			return OUT;
		}
		return world[CameraW(), CameraH()];
	}

	public bool ValidDig(Vertex v){
		if(v.x < world.GetLength(0) && v.x >= 0 && v.y < world.GetLength(1) && v.y >= 0){
			bool top = world[v.x, v.y - 1] == TUNNEL;
			bool topleft = world[v.x- 1, v.y- 1] == TUNNEL;
			bool topright = world[v.x+ 1, v.y- 1] == TUNNEL;
			bool right = world[v.x+ 1, v.y] == TUNNEL;
			bool left = world[v.x- 1, v.y] == TUNNEL;
			bool bottomleft = world[v.x- 1, v.y+ 1] == TUNNEL;
			bool bottom = world[v.x, v.y+ 1] == TUNNEL;
			bool bottomright = world[v.x + 1, v.y+ 1] == TUNNEL;
			top = top || world[v.x, v.y - 1] == AIR;
			topleft = topleft || world[v.x- 1, v.y- 1] == AIR;
			topright = topright || world[v.x+ 1, v.y- 1] == AIR;
			right = right || world[v.x+ 1, v.y] == AIR;
			left = left || world[v.x- 1, v.y] == AIR;
			bottomleft = bottomleft || world[v.x- 1, v.y+ 1] == AIR;
			bottom = bottom || world[v.x, v.y+ 1] == AIR;
			bottomright = bottomright || world[v.x + 1, v.y+ 1] == AIR;
			return (top || topleft || topright || right || left || bottomleft || bottom || bottomright);
		}
		return false;
	}

	public Vector3 VertexToVector3(Vertex v){
		return new Vector3(transform.position.x + v.x, transform.position.y - v.y, 0);
	}
	
	int CameraW(){
		return Mathf.RoundToInt((transform.position.x - Camera.main.ScreenToWorldPoint(Input.mousePosition).x) * -1);
	}

	int CameraH(){
		return Mathf.RoundToInt((transform.position.y + Camera.main.ScreenToWorldPoint(Input.mousePosition).y) * -1);
	}

	void InstantiateWorld(){
		for(int w = 0; w < width; w++){
			for(int h = 0; h < height; h++){
				prefabs[w, h] = (GameObject)Instantiate(block, new Vector3(transform.position.x + w, transform.position.y - h, 0), Quaternion.identity);
			}
		}
	}

	void BuildGround(){
		for(int w = 0; w < width; w++){
			for(int h = 0; h < height; h++){
				world[w, h] = AIR;
			}
		}
		int curGround = height/2;
		for(int w = 0; w < width; w++){
			curGround += Random.Range(-1, 2);
			if(curGround > height / 2 + (height * .1f)){
				curGround--;
			} else if(curGround < height / 2 - (height * .1f)){
				curGround++;
			}
			for(int h = 0; h < height; h++){
				if(h > curGround){
					world[w, h] = DIRT;
				}
			}
		}
	}

	Sprite DrawDirt(int w, int h){
		if(w == 0 || h == 0 || w == width - 1 || h == height - 1){
			return dirt;
		}

		bool right = world[w + 1, h] == DIRT || world[w + 1, h] == TUNNEL || world[w + 1, h] == CARROT;
		bool left = world[w - 1, h] == DIRT || world[w - 1, h] == TUNNEL || world[w - 1, h] == CARROT;
		bool is_grass = world[w, h - 1] == AIR;
		int season = wl.season * 4;
		if(is_grass){
			if(left && right){
				return grasses[season + GFULL];
			}
			if(!left && right){
				return grasses[season + GRIGHT];
			}
			if(!right && left){
				return grasses[season + GLEFT];
			}
			if(!right && !left){
				return grasses[season + GBUMB];
			}
		}
		return tunnels[FULL];
	}

	Sprite DrawTunnel(int w, int h){
		if(w == 0 || h == 0 || w == width - 1 || h == height - 1){
			return dirt;
		}
		bool top = world[w, h - 1] == TUNNEL || world[w, h - 1] == AIR;
		bool topleft = world[w - 1, h - 1] == TUNNEL;
		bool topright = world[w + 1, h - 1] == TUNNEL;
		bool right = world[w + 1, h] == TUNNEL || world[w + 1, h] == AIR;
		bool left = world[w - 1, h] == TUNNEL || world[w - 1, h] == AIR;
		bool bottomleft = world[w - 1, h + 1] == TUNNEL;
		bool bottom = world[w, h + 1] == TUNNEL;
		bool bottomright = world[w + 1, h + 1] == TUNNEL;
		if(top && topleft && topright && right && left && bottomleft && bottomright && bottom){
			return tunnels[EMPTY];
		}
		if(top && !right && !left && !bottom){
			return tunnels[TOP];
		}
		if(!top && !topleft && right && !left && !bottomleft && !bottom){
			return tunnels[RIGHT];
		}
		if(top && right && !left && !bottom){
			return tunnels[TOPRIGHT];
		}
		if(!top && !right && !left && bottom){
			return tunnels[BOTTOM];
		}
		if(top && !right && !left && bottom){
			return tunnels[TOPBOTTOM];
		}
		if(!top && right && !left && bottom){
			return tunnels[BOTTOMRIGHT];
		}
		if(top && right && !left && bottom){
			return tunnels[TOPRIGHTBOTTOM];
		}
		if(!top && !topright && !right && left && !bottomright && !bottom){
			return tunnels[LEFT];
		}
		if(top && !right && left && !bottomright && !bottom){
			return tunnels[TOPLEFT];
		}
		if(!top && right && left && !bottom){
			return tunnels[LEFTRIGHT];
		}
		if(top && right && left && !bottom){
			return tunnels[LEFTTOPRIGHT];
		}
		if(!top && !right && left && bottom){
			return tunnels[LEFTBOTTOM];
		}
		if(top && topleft && !topright && !right && left && bottomleft && !bottomright && bottom){
			return tunnels[BOTTOMLEFTTOP];
		}
		if(!top && right && left && bottom){
			return tunnels[LEFTBOTTOMRIGHT];
		}
		return tunnels[EMPTY];
	}

	void DrawGround(){
		for(int w = 0; w < width; w++){
			for(int h = 0; h < height; h++){
				switch (world[w, h]){
					case TUNNEL:
						prefabs[w, h].GetComponent<SpriteRenderer>().sprite = DrawTunnel(w, h);
						break;
					case DIRT:
						prefabs[w, h].GetComponent<SpriteRenderer>().sprite = DrawDirt(w, h);
						break;
					case AIR:
						prefabs[w, h].GetComponent<SpriteRenderer>().sprite = null;
						break;
					case CARROT:
						prefabs[w, h].GetComponent<SpriteRenderer>().sprite = carrot;
						//Debug.Log(w + " " + h);
						break;
					default:
						prefabs[w, h].GetComponent<SpriteRenderer>().sprite = null;
						break;
				}
			}
		}
	}
}
