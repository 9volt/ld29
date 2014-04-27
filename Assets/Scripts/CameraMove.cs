﻿using UnityEngine;
using System.Collections;

public class CameraMove : MonoBehaviour {
	private WorldGen wg;
	public float scroll_speed = 5f;
	public float zoom_speed = 2f;
	public GameObject bg;
	public float bg_scroll_speed;
	// Use this for initialization
	void Start () {
		wg = GameObject.FindGameObjectWithTag("world").GetComponent<WorldGen>();
		transform.position = new Vector3(wg.transform.position.x + (wg.width / 2), wg.transform.position.y - (wg.height / 2), transform.position.z);
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.mousePosition.x > Screen.width * .9f && camera.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x < (wg.transform.position.x + wg.width)){
			//scroll right
			transform.Translate(Vector3.right * scroll_speed * Time.deltaTime);
			bg.renderer.material.mainTextureOffset = new Vector2( (bg.renderer.material.mainTextureOffset.x + bg_scroll_speed)%1, 0f);
		}
		if(Input.mousePosition.x < Screen.width * .1f && camera.ScreenToWorldPoint(new Vector3(0, 0, 0)).x > wg.transform.position.x){
			//scroll left
			transform.Translate(Vector3.left * scroll_speed * Time.deltaTime);
			bg.renderer.material.mainTextureOffset = new Vector2( (bg.renderer.material.mainTextureOffset.x + -bg_scroll_speed)%1, 0f);
		}
		if(Input.mousePosition.y > Screen.height * .9f && camera.ScreenToWorldPoint(new Vector3(0, 0, 0)).y > wg.transform.position.y){
			//scroll up
			transform.Translate(Vector3.up * scroll_speed * Time.deltaTime);
		}
		if(Input.mousePosition.y < Screen.height * .1f && camera.ScreenToWorldPoint(new Vector3(0, Screen.height, 0)).y < (wg.transform.position.y - wg.height)){
			//scroll down
			transform.Translate(Vector3.down * scroll_speed * Time.deltaTime);
		}
		camera.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * zoom_speed;
	}
}
