using UnityEngine;
using System.Collections;

public class WorldGen : MonoBehaviour {
	public Sprite dirt;
	public Sprite grass;
	public Sprite carrot;
	public GameObject block;
	public int width;
	public int height;
	private int[,] world;
	private GameObject[,] prefabs;
	// 1 = dirt
	// 2 = grass
	// 3 = carrot
	const int DIRT = 1;
	const int GRASS = 2;
	const int CARROT = 3;

	// Use this for initialization
	void Start () {
		prefabs = new GameObject[width,height];
		InstantiateWorld();
		world = new int[width, height];
		BuildGround();
		DrawGround();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonDown(0)){
			world[CameraW(), CameraH()] = 0;
		}
		if(Input.GetMouseButtonDown(1)){
			world[CameraW(), CameraH()] = 1;
		}
		if(Input.GetMouseButtonDown(2)){
			world[CameraW(), CameraH()] = 3;
		}
		DrawGround();
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
	}

	void DrawGround(){
		for(int w = 0; w < width; w++){
			for(int h = 0; h < height; h++){
				switch (world[w, h]){
					case DIRT:
						prefabs[w, h].GetComponent<SpriteRenderer>().sprite = dirt;
						break;
					case GRASS:
						prefabs[w, h].GetComponent<SpriteRenderer>().sprite = grass;
						break;
					case CARROT:
						prefabs[w, h].GetComponent<SpriteRenderer>().sprite = carrot;
						break;
					default:
						prefabs[w, h].GetComponent<SpriteRenderer>().sprite = null;
						break;
				}
			}
		}
	}
}
