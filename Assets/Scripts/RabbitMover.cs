using UnityEngine;
using System.Collections;

public class RabbitMover : MonoBehaviour {
	Vector3 currentTarget;
	public bool Moving;
	WorldGen wg;

	// Use this for initialization
	void Start () {
		wg = GameObject.FindGameObjectWithTag("world").GetComponent<WorldGen>();
		Moving = false;
	}
	
	// Update is called once per frame
	void Update(){
		if(Moving){
			if(Vector3.Distance(transform.position, currentTarget) < .5f){
				Moving = false;
			}else{
				transform.position = Vector3.MoveTowards(transform.position, currentTarget, 5 * Time.deltaTime);
			}
		}
	}


	public void Move(Vector3 n){
		currentTarget = n;
		Moving = true;
	}

	public void SetPosition(Vector3 v){
		transform.position = v;
	}
}
