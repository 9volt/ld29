using UnityEngine;
using System.Collections;

public class RabbitMover : MonoBehaviour {
	private Vector3 currentTarget;
	public bool Moving;
	private Animator anim;
	private bool facingleft = true;
	private int speed;

	// Use this for initialization
	void Start () {
		Moving = false;
		anim = gameObject.GetComponent<Animator>();
		if(gameObject.GetComponent<RabbitLogic>() != null){
			speed = gameObject.GetComponent<RabbitLogic>().spd;
		} else if(gameObject.GetComponent<Enemy>() != null){
			speed = gameObject.GetComponent<Enemy>().spd;
		} else if(gameObject.GetComponent<Farmer>() != null){
			speed = gameObject.GetComponent<Farmer>().spd;
		} else if(gameObject.GetComponent<Ferret>() != null){
			speed = gameObject.GetComponent<Ferret>().spd;
		}
	}
	
	// Update is called once per frame
	void Update(){
		if(Moving){
			if(Vector3.Distance(transform.position, currentTarget) < .1f){
				Moving = false;
				anim.SetBool("Moving",false);
			}else{
				transform.position = Vector3.MoveTowards(transform.position, currentTarget, speed * 2 * Time.deltaTime);
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
		facingleft = !facingleft;
		Vector3 s = transform.localScale;
		s.x *= -1;
		transform.localScale = s;
	}
}
