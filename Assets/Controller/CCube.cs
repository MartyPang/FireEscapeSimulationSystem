using UnityEngine;
using System.Collections;

public class CCube : MonoBehaviour {
	public int x1=0;
	public int x2=0;
	public int y1=0;
	public int y2=0;
	public int no=999999;
	public int no2=999999;
	public bool noBlock = false;
	public GameObject block;
	// Use this for initialization
	void Start () {
		if (x1 == 0 && x2 == 0 && y1 == 0 && y2 == 0)
			return;
		if (noBlock) {
			for (int i=x1; i<=x2; i++) {
				for (int j=y1; j<=y2; j++) {
					if (noBlock && no == i && no2 == j)
						continue;
					Instantiate (block, new Vector3 (i, 0, j), block.transform.rotation);
				}
			}
		} else {
			for (int i=x1; i<=x2; i++) {
				for (int j=y1;j<=y2;j++){
					Instantiate (block, new Vector3 (i,0,j), block.transform.rotation);
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
