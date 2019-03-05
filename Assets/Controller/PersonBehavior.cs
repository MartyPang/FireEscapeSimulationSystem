using UnityEngine;
using System.Collections;
using CellularAutomaton.Utils;
public class PersonBehavior : MonoBehaviour {
	Vector3 v;
	int x;
	int y;
	int direction;
    
	// Use this for initialization
	void Start () {
              this.GetComponent<Animator>().SetFloat("speed", 1.0f);
    }
    public void setRoad(Vector3 v2){
		v = v2;
	}
	public void init(int x,int y){
		this.x = x;
		this.y = y;
	}
	public void setDirection(int x,int y,int d){
		if (d == PersonLocationManager.MOVE_UP)
			direction = 0;
		if (d == PersonLocationManager.MOVE_DOWN)
			direction = 180;
		if (d == PersonLocationManager.MOVE_LEFT)
			direction = 270;
		if (d == PersonLocationManager.MOVE_RIGHT)
			direction = 90;
		this.x = x;
		this.y = y;
	}
	void Update(){
		if (direction != (int)this.transform.rotation.eulerAngles.y) {
			this.transform.rotation = Quaternion.Euler (this.transform.rotation.x, direction, this.transform.rotation.z);
			this.transform.position = new Vector3 (x + 0.5f, this.transform.position.y, y + 0.5f);
		} else {
			this.transform.Translate (v * Time.deltaTime, Space.World);
           
		}
	}
}