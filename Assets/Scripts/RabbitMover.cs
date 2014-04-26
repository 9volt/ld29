using UnityEngine;
using System.Collections;

public class RabbitMover : MonoBehaviour {
	Vector3 currentTarget;
	public bool Moving;
	WorldGen wg;
	Animator anim;
	bool facingleft = true;

	// Use this for initialization
	void Start () {
		wg = GameObject.FindGameObjectWithTag("world").GetComponent<WorldGen>();
		Moving = false;
		anim = gameObject.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update(){
		if(Moving){
			if(Vector3.Distance(transform.position, currentTarget) < .5f){
				Moving = false;
				anim.SetBool("Moving",false);
			}else{
				transform.position = Vector3.MoveTowards(transform.position, currentTarget, 5 * Time.deltaTime);
			}
		}
	}


	public void Move(Vector3 n){
		currentTarget = n;
		Moving = true;
		//Flip sprite if the target is in the other direction
		if( (facingleft && n.x > transform.position.x) || (!facingleft && n.x < transform.position.x)){
			Flip();
		}
		anim.SetBool("Moving",true);
	}

	public void SetPosition(Vector3 v){
		transform.position = v;
	}

	public void Flip(){
		Vector3 s = transform.localScale;
		s.x *= -1;
		transform.localScale = s;
	}
}
