using UnityEngine;
using System.Collections;

public class CameraMove : MonoBehaviour {
	private WorldGen wg;
	public float scroll_speed = 1f;
	public float zoom_speed = 2f;
	public GameObject bg;
	public float bg_scroll_speed;
	public Texture[] seasons;
	private WorldLogic wl;
	// Use this for initialization
	void Start () {
		wl = GameObject.FindGameObjectWithTag("world").GetComponent<WorldLogic>();
		wg = GameObject.FindGameObjectWithTag("world").GetComponent<WorldGen>();
		transform.position = new Vector3(Mathf.RoundToInt(wg.transform.position.x + (wg.width / 2)), Mathf.RoundToInt(wg.transform.position.y - (wg.height / 2)), transform.position.z);
	}
	
	// Update is called once per frame
	void Update () {
		bg.renderer.material.mainTexture = seasons[wl.season];
		if((Input.mousePosition.x > Screen.width * .95f || Input.GetAxis("Horizontal") > 0f) && camera.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x < (wg.transform.position.x + wg.width - 2)){
			//scroll right
			transform.Translate(Vector3.right * scroll_speed * Time.deltaTime);
			bg.renderer.material.mainTextureOffset = new Vector2((bg.renderer.material.mainTextureOffset.x + bg_scroll_speed)%1, 0f);
		}
		if((Input.mousePosition.x < Screen.width * .05f || Input.GetAxis("Horizontal") < 0f) && camera.ScreenToWorldPoint(new Vector3(0, 0, 0)).x > wg.transform.position.x + 1){
			//scroll left
			transform.Translate(Vector3.left * scroll_speed * Time.deltaTime);
			bg.renderer.material.mainTextureOffset = new Vector2( (bg.renderer.material.mainTextureOffset.x + -bg_scroll_speed)%1, 0f);
		}
		if((Input.mousePosition.y > Screen.height * .95f || Input.GetAxis("Vertical") > 0f) && camera.ScreenToWorldPoint(new Vector3(0, Screen.height, 0)).y < wg.transform.position.y - 2){
			//scroll up
			transform.Translate(Vector3.up * scroll_speed * Time.deltaTime);
		}
		if((Input.mousePosition.y < Screen.height * .05f || Input.GetAxis("Vertical") < 0f) && camera.ScreenToWorldPoint(new Vector3(0, 0, 0)).y > (wg.transform.position.y - wg.height + 2)){
			//scroll down
			transform.Translate(Vector3.down * scroll_speed * Time.deltaTime);
		}
		camera.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * zoom_speed * Time.deltaTime;
	}

	public void SetPosition(Vertex v){
		transform.position = new Vector3(Mathf.RoundToInt(wg.transform.position.x + v.x), Mathf.RoundToInt(wg.transform.position.y - v.y), transform.position.z);
	}
}
