using UnityEngine;
using System.Collections;

public class WorldGen : MonoBehaviour {
	public Sprite dirt;
	public Sprite grass;
	public Sprite carrot;
	public Sprite[] tunnels;
	public GameObject block;
	public int width;
	public int height;
	private int[,] world;
	private GameObject[,] prefabs;

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


	// Use this for initialization
	void Start () {
		tunnels = Resources.LoadAll<Sprite>("tunnels");
		prefabs = new GameObject[width,height];
		InstantiateWorld();
		world = new int[width, height];
		BuildGround();
		DrawGround();
	}
	
	// Update is called once per frame
	void Update () {
//		if(Input.GetMouseButtonDown(0)){
//			world[CameraW(), CameraH()] = TUNNEL;
//		}
//		if(Input.GetMouseButtonDown(1)){
//			world[CameraW(), CameraH()] = DIRT;
//		}
//		if(Input.GetMouseButtonDown(2)){
//			world[CameraW(), CameraH()] = CARROT;
//		}
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
					} else {
						switch(world[w,h+1]){
							case DIRT:
								ret[w,h] = 0f;
								break;
							case GRASS:
								ret[w,h] = .5f;
								break;
							case CARROT:
								ret[w,h] = -1f;
								break;
							case AIR:
								ret[w,h] = -1f;
								break;
							case TUNNEL:
								ret[w,h] = -1f;
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
			world[w, height/2 - 1] = GRASS;
			for(int h = height / 2; h < height; h++){
				world[w, h] = DIRT;
			}
		}
		world[8, height/2 - 1] = CARROT;
	}

	Sprite DrawDirt(int w, int h){
		if(w == 0 || h == 0 || w == width - 1 || h == height - 1){
			return dirt;
		}
		bool top = world[w, h - 1] == TUNNEL;
		bool topleft = world[w - 1, h - 1] == TUNNEL;
		bool topright = world[w + 1, h - 1] == TUNNEL;
		bool right = world[w + 1, h] == TUNNEL;
		bool left = world[w - 1, h] == TUNNEL;
		bool bottomleft = world[w - 1, h + 1] == TUNNEL;
		bool bottom = world[w, h + 1] == TUNNEL;
		bool bottomright = world[w + 1, h + 1] == TUNNEL;
//		if(right && top){
//			return tunnels[TOPRIGHT];
//		}
//		if(right && bottom){
//			return tunnels[BOTTOMRIGHT];
//		}
//		if(left && bottom){
//			return tunnels[LEFTBOTTOM];
//		}
//		if(left && top){
//			return tunnels[TOPLEFT];
//		}
//		if(top && right){
//			return tunnels[TOPRIGHT];
//		}

		return tunnels[FULL];
	}

	Sprite DrawTunnel(int w, int h){
		if(w == 0 || h == 0 || w == width - 1 || h == height - 1){
			return dirt;
		}
		bool top = world[w, h - 1] == TUNNEL;
		bool topleft = world[w - 1, h - 1] == TUNNEL;
		bool topright = world[w + 1, h - 1] == TUNNEL;
		bool right = world[w + 1, h] == TUNNEL;
		bool left = world[w - 1, h] == TUNNEL;
		bool bottomleft = world[w - 1, h + 1] == TUNNEL;
		bool bottom = world[w, h + 1] == TUNNEL;
		bool bottomright = world[w + 1, h + 1] == TUNNEL;
		if(top && topleft && topright && right && left && bottomleft && bottomright && bottom){
			return tunnels[EMPTY];
		}
		if(top && !right && !left && !bottomleft && !bottomright && !bottom){
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
		if(!top && !topleft && right && !left && bottom){
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
					case GRASS:
						prefabs[w, h].GetComponent<SpriteRenderer>().sprite = grass;
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
