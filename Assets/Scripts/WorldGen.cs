using UnityEngine;
using System.Collections;

public class WorldGen : MonoBehaviour {
	public GameObject dirt;
	public GameObject grass;
	public GameObject carrot;
	public int width;
	public int height;
	private int[,] world;
	// 1 = dirt
	// 2 = grass
	// 3 = carrot
	const int DIRT = 1;
	const int GRASS = 2;



	// Use this for initialization
	void Start () {
		world = new int[width, height];
		BuildGround();
		DrawGround();
	}
	
	// Update is called once per frame
	void Update () {
	
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
						Instantiate(dirt, new Vector3(transform.position.x + w, transform.position.y - h, 0), Quaternion.identity);
						break;
					case GRASS:
						Instantiate(grass, new Vector3(transform.position.x + w, transform.position.y - h, 0), Quaternion.identity);
						break;
					default:
						break;
				}
			}
		}
	}
}
