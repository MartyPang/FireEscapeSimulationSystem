using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityStandardAssets.Characters.ThirdPerson;
public class Client : MonoBehaviour {
	//serverIP
	string IP="127.0.0.1";
	//port
	int Port=10000;
	string inputMessage="请输入信息";
	string Message = "";
	Vector2 scrollPosition;
	//movespeed
	float speed=10.0f;
	//rotationspeed
	float rotationSpeed=100.0f;



	//new
	GameObject MainRoom;
	bool issynced=false;
	bool npcissynced=false;
//	bool fireissynced=false;
	bool playeriscreated=false;
	bool DiedFlag=false;
	List<GameObject> personList=null;
	List<GameObject> fires=null;
	List<GameObject> Players=null;
	int SelfIndex;
	GameObject camera;
	GameObject camera2;

    public Texture2D blood_red;
    public Texture2D blood_yellow;

    float red_width = 100.0f;

	void Start(){
		personList = new List<GameObject> ();
		fires = new List<GameObject> ();
		Players = new List<GameObject> ();
		MainRoom=GameObject.Find("整层楼");
		camera=GameObject.Find("Camera");
		camera2=GameObject.Find("Camera 1");
		SelfIndex = -1;
		camera.SetActive (false);
		camera2.SetActive (false);
	}
	void OnGUI(){
		//network situation
		switch (Network.peerType) {
			//disconnected
		case NetworkPeerType.Disconnected:
			StartConnect();
			break;
			//server
		case NetworkPeerType.Server:

			break;
			//client
		case NetworkPeerType.Client:
			OnClient();
			break;
			//connecting
		case NetworkPeerType.Connecting:
            //OnClient();
			break;
		}


	}
	void FixedUpdate(){
		if (Network.isClient) {

			if (!issynced) {
				MainRoom.GetComponent<NetworkView> ().RPC ("AskCreate", RPCMode.Server);
				issynced = true;
			}else{
			if (SelfIndex!=-1)
				{	
			MainRoom.GetComponent<NetworkView> ().RPC ("FixPlayer", RPCMode.All,Players[SelfIndex].transform.position,SelfIndex);
					}}
		} else {
			issynced = false;
		//	npcissynced=false;
		//	fireissynced=false;
			playeriscreated=false;
			DiedFlag=false;
			SelfIndex=-1;
			for (int i = 0; i < personList.Count; i++) {
				Destroy(personList[i]);
			}
			personList.Clear ();
			for (int j=0; j<fires.Count; j++) {
				Destroy(fires[j]);
			}
			fires.Clear ();
			for (int j=0; j<Players.Count; j++) {
				Destroy(Players[j]);
			}
			Players.Clear ();
		}
	}
	//connect server
	void StartConnect(){
		camera.SetActive (false);
		camera2.SetActive (false);
		IP = GUILayout.TextArea (IP);  //new
		if (GUILayout.Button ("加入游戏")) {
			NetworkConnectionError error=Network.Connect(IP,Port);
			Debug.Log("连接状态："+error);
		}
	}

	void OnClient(){
		scrollPosition = GUILayout.BeginScrollView (scrollPosition, GUILayout.Width (200), GUILayout.Height (500));
		//display chat message
		GUILayout.Box (Message);
		GUILayout.BeginHorizontal ();
		//input message
		inputMessage = GUILayout.TextArea (inputMessage);
		//send message
		if (GUILayout.Button ("发送消息")) {
			GetComponent<NetworkView>().RPC("RequestMessage",RPCMode.All,inputMessage);
		}
		GUILayout.EndHorizontal();
		//disconnect
		if (GUILayout.Button("断开连接")){
            if (SelfIndex!=-1)
                MainRoom.GetComponent<NetworkView>().RPC("PersonDie", RPCMode.All,SelfIndex);
            Network.Disconnect();
			Message="";
		}
		if (DiedFlag) {
			if (GUILayout.Button("视角转换")){
				camera2.SetActive(false);
				camera.SetActive(true);
				DiedFlag=false;
			}
		}
		GUILayout.EndScrollView ();

        GUI.Label(new Rect(205, 10, 20, 20), "血值");
        GUI.DrawTexture(new Rect(230, 10, 120, 20), blood_yellow);
        GUI.DrawTexture(new Rect(230, 10, red_width, 20), blood_red);
        
	}
	public int getIndex(){
		return SelfIndex;
	}
    public string getADiedMan(string man)
    {
        if (man == "ThirdPersonController(Clone)" || man == "ThirdPersonController1(Clone)")
            return "Man";
        if (man == "ThirdPersonController2(Clone)" || man == "ThirdPersonController3(Clone)")
            return "Man2";
        if (man == "ThirdPersonController4(Clone)" || man == "ThirdPersonController5(Clone)")
            return "Man3";
        if (man == "ThirdPersonController6(Clone)" || man == "ThirdPersonController7(Clone)")
            return "Man4";
        if (man == "ThirdPersonController8(Clone)" || man == "ThirdPersonController9(Clone)")
            return "Man5";
        return "Man6";
    }
    //receive message
    [RPC]
	void RequestMessage(string message,NetworkMessageInfo info){
		Message += "\n" + "发送者" + info.sender + ":" + message;
	}
	[RPC]
	void RequestCreate(string thing,int x,int y,Quaternion q,NetworkMessageInfo info){
		GameObject tmp=(GameObject)Instantiate(Resources.Load(thing),new Vector3(0.5f+(float)x,0,0.5f+(float)y),q);
	//	tmp.transform.Rotate(Vector3.up*UnityEngine.Random.value*360);
		tmp.GetComponent<PersonBehavior> ().init (x, y);
		UnityEngine.Debug.Log(x+" "+y);
		UnityEngine.Debug.Log(tmp.transform.position.x+" "+tmp.transform.position.z);
		personList.Add (tmp);
	}
	[RPC]
	void RequestCreate2(string thing,int x,int y,Quaternion q,NetworkMessageInfo info){
	//	if (npcissynced)
	//		return;
		GameObject tmp=(GameObject)Instantiate(Resources.Load(thing),new Vector3(0.5f+(float)x,0,0.5f+(float)y),q);
		//	tmp.transform.Rotate(Vector3.up*UnityEngine.Random.value*360);
		tmp.GetComponent<PersonBehavior> ().init (x, y);
		UnityEngine.Debug.Log(x+" "+y);
		UnityEngine.Debug.Log(tmp.transform.position.x+" "+tmp.transform.position.z);
		personList.Add (tmp);
	//	npcissynced = true;
	}
	[RPC]
	void AskCreate(NetworkMessageInfo info){

	}
	[RPC]
	void SyncAction(int index,int x,int y,int d,Vector3 v,NetworkMessageInfo info){
		personList [index].GetComponent<PersonBehavior> ().setRoad (v);
		personList [index].GetComponent<PersonBehavior> ().setDirection (x, y, d);
	}
	[RPC]
	void SyncIsEscaped(int index,NetworkMessageInfo info){
		Destroy (personList [index]);
		personList.RemoveAt (index);
	}
	[RPC]
	void SyncFire(string thing,int x,int y,Quaternion q,NetworkMessageInfo info){
		GameObject tmp=(GameObject)Instantiate(Resources.Load(thing),new Vector3(0.5f+(float)x,0,0.5f+(float)y),q);
		fires.Add (tmp);
	}

	[RPC]
	void SyncFire2(string thing,int x,int y,Quaternion q,NetworkMessageInfo info){
		Debug.Log ("fire");
	//	if (fireissynced)
	//		return;
		GameObject tmp=(GameObject)Instantiate(Resources.Load(thing),new Vector3(0.5f+(float)x,0,0.5f+(float)y),q);
		fires.Add (tmp);
	//	fireissynced = true;
		Debug.Log (fires.Count);
	}
	[RPC]
	void AddPlayer(string thing, Vector3 v,Quaternion q,NetworkMessageInfo info){
		if (!playeriscreated)
			Players.Add ((GameObject)Instantiate (Resources.Load (thing), v,q));
	}
	[RPC]
	void AddPlayerNone(NetworkMessageInfo info){
		Players.Add (null);
	}
	[RPC]
	void AddPlayerDied(string thing, Vector3 v,Quaternion q,NetworkMessageInfo info){
		if (!playeriscreated) {
			Players.Add ((GameObject)Instantiate (Resources.Load(thing), v, q));
            Destroy(Players[Players.Count-1].GetComponent<PersonBehavior>());
            Players[Players.Count-1].GetComponent<Animator>().SetInteger("death", 2);
        }
	}
    [RPC]
    void AddPlayer2(string thing, Vector3 v, NetworkMessageInfo info)
    {
        if (playeriscreated)
            Players.Add((GameObject)Instantiate(Resources.Load(thing), v, Quaternion.identity));
        else {
            if (thing == "ThirdPersonController1")
                thing = "ThirdPersonController";
            else if (thing == "ThirdPersonController3")
                thing = "ThirdPersonController2";
            else if (thing == "ThirdPersonController5")
                thing = "ThirdPersonController4";
            else if (thing == "ThirdPersonController7")
                thing = "ThirdPersonController6";
            else if (thing == "ThirdPersonController9")
                thing = "ThirdPersonController8";
            else if (thing == "ThirdPersonController11")
                thing = "ThirdPersonController10";

     //       Debug.Log("AAAAAAA"+thing);
            Players.Add((GameObject)Instantiate(Resources.Load(thing), v, Quaternion.identity));
            playeriscreated = true;
            SelfIndex = Players.Count - 1;
            Debug.Log("SelfIndex  " + Players.Count);
            Players[Players.Count - 1].GetComponent<ThirdPersonUserControl>().setIndex(Players.Count - 1);
        }
    }
	[RPC]
	void SyncPlayer(Vector3 move, bool crouch, bool jump,int index,NetworkMessageInfo info){
		if (index == SelfIndex)
			return;
		if (Players [index] == null)
			return;
		Players [index].GetComponent<ThirdPersonCharacter> ().Move (move, crouch, jump);
	}
	[RPC]
	void FixPlayer(Vector3 v,int index,NetworkMessageInfo info){
		if (index == SelfIndex)
			return;
		if (Players [index] == null)
			return;
		Players [index].transform.position = v;
	}
	[RPC]
	void PersonIsEscaped(int index,NetworkMessageInfo info){
		Destroy (Players [index]);
        //	Players.RemoveAt (index);
        if (SelfIndex == index)
        {
            SelfIndex = -1;
            //	if (SelfIndex > index){
            //		SelfIndex -= 1;
            //		Players[SelfIndex].GetComponent<ThirdPersonUserControl>().setIndex(SelfIndex);
            //	}
            camera.SetActive(true);
        }
	}
	[RPC]
	void PersonDie(int index,NetworkMessageInfo info){
		if (Players [index] == null)
			return;
		Vector3 v = Players [index].transform.position;
		Quaternion q = Players [index].transform.rotation;
        string man = Players[index].name;
        Debug.Log("Diedman " + index +" "+ man);
        man = getADiedMan(man);
        Debug.Log("Diedman " + index + " " + man);
        Destroy (Players [index]);
		Players [index] = (GameObject)Instantiate (Resources.Load(man), v, q);
        Destroy(Players[index].GetComponent<PersonBehavior>());
        Players[index].GetComponent<Animator>().SetInteger("death", 2);
        if (index == SelfIndex)
        {
            Vector3 v2 = new Vector3(Players[index].transform.position.x, camera2.transform.position.y, Players[index].transform.position.z - 2);
            camera2.transform.position = v2;
            camera2.SetActive(true);
            DiedFlag = true;
            SelfIndex = -1;
        }
	}
    [RPC]
    void LifeControl(int index,int life, NetworkMessageInfo info)
    {
        //do what you like
        if (index==SelfIndex)
              red_width = 120.0f * life / 5000.0f;
    }
}
