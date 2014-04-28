using UnityEngine;
using System.Collections;

public class Hawk : MonoBehaviour {
	private GameObject[] rabbits;
	private bool seeking_rabbit = true;
	private GameObject my_rabbit;
	public WorldGen wg;
	private bool leaving = false;
	private Vector3 gone = new Vector3(20, 0 ,0);

	// Use this for initialization
	void Start () {
		wg = GameObject.FindGameObjectWithTag("world").GetComponent<WorldGen>();
	}
	
	// Update is called once per frame
	void Update () {
		if(!leaving){
			if(seeking_rabbit){
				rabbits = GameObject.FindGameObjectsWithTag("Rabbit");
				for(int i = 0; i < rabbits.Length; i++){
					if(wg.VertexToType(rabbits[i].GetComponent<RabbitLogic>().mySquare)  == WorldGen.AIR){
						my_rabbit = rabbits[i];
						seeking_rabbit = false;
						break;
					}

				}
			}else{
				if(wg.VertexToType(my_rabbit.GetComponent<RabbitLogic>().mySquare)  == WorldGen.AIR){
					transform.position = Vector3.MoveTowards(transform.position, my_rabbit.transform.position, 10 * Time.deltaTime);
					if(Vector3.Distance(transform.position, my_rabbit.transform.position) < .1f){
						my_rabbit.GetComponent<RabbitLogic>().hp = 0;
						leaving = true;
					}
				}else{
					seeking_rabbit = true;
				}
			}
		}else{
			if(Vector3.Distance(transform.position, gone ) < .1f){
				Destroy(gameObject);
			}else{
				transform.position = Vector3.MoveTowards(transform.position, gone, 10 * Time.deltaTime);
			}
		}
	}
}
