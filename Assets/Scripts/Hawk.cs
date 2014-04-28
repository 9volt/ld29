using UnityEngine;
using System.Collections;

public class Hawk : MonoBehaviour {
	private GameObject[] rabbits;
	private bool seeking_rabbit = true;
	private GameObject my_rabbit;
	public WorldGen wg;
	public bool leaving = false;
	private Vector3 gone = new Vector3(20, 0 ,0);
	private bool facingleft = true;
	private int speed;

	// Use this for initialization
	void Start () {
		wg = GameObject.FindGameObjectWithTag("world").GetComponent<WorldGen>();
		speed = Random.Range(5,12);
		Camera.main.gameObject.GetComponent<CameraMove>().SetPosition(wg.Vector3ToVertex(transform.position));
	}
	
	// Update is called once per frame
	void Update () {

		if(!leaving){
			//if havent eaten a rabbit yet
				if(seeking_rabbit){
					//if hasnt picked a rabbit
					rabbits = GameObject.FindGameObjectsWithTag("Rabbit");
					for(int i = 0; i < rabbits.Length; i++){
						if(wg.VertexToType(rabbits[i].GetComponent<RabbitLogic>().mySquare)  == WorldGen.AIR){//look for a rabbit that is above ground
							my_rabbit = rabbits[i];
							seeking_rabbit = false;
							break;
						}

					}
				}else{

					//make sure this rabbit hasn't gone back underground
					if(wg.VertexToType(my_rabbit.GetComponent<RabbitLogic>().mySquare)  == WorldGen.AIR){

						//face the rabbit
						if( (facingleft && my_rabbit.transform.position.x > transform.position.x) || (!facingleft && my_rabbit.transform.position.x < transform.position.x)){
							Flip();
						}

						//get it
						transform.position = Vector3.MoveTowards(transform.position, my_rabbit.transform.position, speed * Time.deltaTime);

						//trigger animation
						if(Vector3.Distance(transform.position, my_rabbit.transform.position) < 7f){
							gameObject.GetComponent<Animator>().SetBool("Grabbing",true);
						}

						//eat it
						if(Vector3.Distance(transform.position, my_rabbit.transform.position) < .1f){
							my_rabbit.GetComponent<RabbitLogic>().Damage("hawk",my_rabbit.GetComponent<RabbitLogic>().hp);
							leaving = true;
						}
					}else{
						//the rabbit is no longer above ground, find another
						seeking_rabbit = true;
					}
			}
		}else{
			gameObject.GetComponent<Animator>().SetBool("Grabbing",false);

			//if rabbit eaten fly off of the screen and destroy self
			if(Vector3.Distance(transform.position, gone ) < .1f){
				Destroy(gameObject);
			}else{
				if( (facingleft && gone.x > transform.position.x) || (!facingleft && gone.x < transform.position.x)){
					Flip();
				}
				transform.position = Vector3.MoveTowards(transform.position, gone, speed * Time.deltaTime);
			}
		}
	}

	public void Flip(){
		facingleft = !facingleft;
		Vector3 s = transform.localScale;
		s.x *= -1;
		transform.localScale = s;
	}
}
