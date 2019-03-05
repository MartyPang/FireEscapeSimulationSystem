using UnityEngine;
using System.Collections;
using CelluarAutomation.Model;
namespace CellularAutomaton.Controller
{
public class HumanController : MonoBehaviour {
	PlaceNode[,] nodes;
		int x;
		int y;
		int t;
		int life=5000;
		GameObject MainRoom;     //new
		int index;   //new
		bool WinButton=false;
	// Use this for initialization
	void Start () {
			x = -1;
			y = -1;
			t = 0x000;
		//	index = -1;   //new
			MainRoom = GameObject.Find ("整层楼");   //new
	}
		public	void SetIndex(int i){   //new
			this.index = i;
		}
		public int GetIndex(){   //new
			return index;
		}
	// Update is called once per frame
	void FixedUpdate () {
            nodes = FloorPlanManager.getInstance().getNodes();
			if (x!=-1 && y!=-1)
			    nodes [x, y].type = t;
			x = (int)this.transform.position.x;
			y = (int)this.transform.position.z;
			t = nodes [x, y].type;
            life -= 1;
          
			if (nodes [x, y].type == PlaceNode.TYPE_EXIT && !WinButton) {
				Debug.Log ("Win!");
				Debug.Log(index);
                if (Network.peerType == NetworkPeerType.Server)
                {
                    MainRoom.GetComponent<NetworkView>().RPC("PersonIsEscaped",RPCMode.All,index);
                }
                    
				WinButton=true;
				return;
			}
            Debug.Log("fire XXX " + nodes[x, y].isFired());
			if (nodes [x, y].isFired ())
            {
                life -= 50;
                
            }
            nodes[x, y].type = PlaceNode.TYPE_BLOCK;
            if (life <= 0) {
				Debug.Log ("Died!");
                if (Network.peerType == NetworkPeerType.Server)
                    MainRoom.GetComponent<NetworkView>().RPC("PersonDie",RPCMode.All,index);
			}

            if (Network.peerType == NetworkPeerType.Server)
                MainRoom.GetComponent<NetworkView>().RPC("LifeControl", RPCMode.All,index, life);
	}
}
}
