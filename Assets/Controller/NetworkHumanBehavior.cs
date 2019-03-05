using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.ThirdPerson;
public class NetworkHumanBehavior : MonoBehaviour {
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		this.GetComponent<ThirdPersonCharacter> ().Move (new Vector3 (1, 0, 1), false, false);
	}
}
