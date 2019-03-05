using UnityEngine;
using System.Collections;

public class WallControl : MonoBehaviour {
	public int x1=0;
	public int x2=0;
	public int y1=0;
	public int y2=0;
	public int door1=999999;
	public int door2=999999;
	public bool door = false;
	public GameObject block;
	// Use this for initialization
	void Start () {
		if (x1 == 0 && x2 == 0 && y1 == 0 && y2 == 0)
			return;
		if (x1 == x2){
			if (!door){
			    for (int i=y1;i<=y2;i++)
				    Instantiate (block, new Vector3 (x1,0,i), block.transform.rotation);
			}
			else
			{
				for (int i=y1;i<=y2;i++){
					if (i<door1 || i>door2)
				    	Instantiate (block, new Vector3 (x1,0,i), block.transform.rotation);
				}
			}
		}
		else if (y1 == y2) {
			if (!door){
			    for (int j=x1;j<=x2;j++)
			    	Instantiate (block, new Vector3 (j,0,y1), block.transform.rotation);
			}
			else
			{
				for (int j=x1;j<=x2;j++){
					if (j<door1 ||j>door2)
				    	Instantiate (block, new Vector3 (j,0,y1), block.transform.rotation);
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
