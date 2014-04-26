using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RabbitLogic : MonoBehaviour {
	Vertex mySquare;
	Vertex currentDestination;
	RabbitFinder rf;
	RabbitMover rm;
	WorldGen wg;
	Vertex nextNode;

	// Use this for initialization
	void Start () {
		mySquare = new Vertex(5,8);
		nextNode = mySquare;
		wg = GameObject.FindGameObjectWithTag("world").GetComponent<WorldGen>();
		rf = gameObject.GetComponent<RabbitFinder>();
		rm = gameObject.GetComponent<RabbitMover>();
		rm.SetPosition(wg.VertexToVector3(mySquare));
		currentDestination = new Vertex(15, 8);
	}
	
	// Update is called once per frame
	void Update () {
		//Checks for a path to the destination (In case it has changed), and moves to the next node if not currently transitioning between nodes
		if(mySquare != currentDestination){
			if(!rm.Moving){
				mySquare = nextNode;
				List<Vertex> p = rf.FindPath(mySquare, currentDestination, wg.GetPathfindingCosts());
				if (p != null && p.Count > 1){
					nextNode = p[1];
					rm.Move(wg.VertexToVector3(nextNode));
				}else{
					currentDestination = mySquare;
				}
			}


		}//else pick new Destination or work
	}
}
