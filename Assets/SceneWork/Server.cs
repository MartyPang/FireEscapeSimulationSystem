using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CellularAutomaton.Model;
using UnityStandardAssets.Characters.ThirdPerson;
namespace CellularAutomaton.Controller{
    public class Server : MonoBehaviour
    {
        //port
        int Port = 10000;
        //message
        string Message = "";
        //move
        string MoveInfo = "";
        //position
        Vector2 scrollPosition;
        GameObject PersonControllerIns;
        GameObject MainRoom;
        List<GameObject> Players;
        List<PersonNode> personList;
        int DiedPerson;
        int EscapedPerson;
        int AllPerson;
        System.Random randomMan;
        void Start()
        {
            MainRoom = GameObject.Find("整层楼");
            Players = new List<GameObject>();
            DiedPerson = 0;
            EscapedPerson = 0;
            AllPerson = 0;
            randomMan = new System.Random();
        }
        public void Stop() {
            Network.Disconnect();
            Message = "";
            MoveInfo = "";
            DiedPerson = 0;
            EscapedPerson = 0;
            AllPerson = 0;
            while (Players.Count != 0)
            {
                if (Players[0] != null)
                    Destroy(Players[0]);
                Players.RemoveAt(0);
            }
        }
        void OnGUI()
        {
            //network situation
            GUILayout.Space(50);
            switch (Network.peerType)
            {
                //disconnected
                case NetworkPeerType.Disconnected:
                    StartServer();
                    break;
                //server
                case NetworkPeerType.Server:
                    OnServer();
                    break;
                //client
                case NetworkPeerType.Client:

                    break;
                //connecting
                case NetworkPeerType.Connecting:

                    break;
            }
        }
        void StartServer()
        {
            DiedPerson = 0;
            EscapedPerson = 0;
            AllPerson = 0;
            GUILayout.Space(45);
            if (GUILayout.Button("创建本机服务器"))
            {
                NetworkConnectionError error = Network.InitializeServer(10, Port, false);
                Debug.Log("连接状态：" + error);
            }
        }
        void OnServer()
        {
            GUILayout.Label("服务器创建完毕，等待客户端连接");
            //server number
            int length = Network.connections.Length;
         //   for (int i = 0; i < length; i++)
        //    {
       //         GUILayout.Label("连接服务器客户端ID" + i);
        //        GUILayout.Label("连接服务器客户端IP" + Network.connections[i].ipAddress);
       //         GUILayout.Label("连接服务器客户端端口号" + Network.connections[i].port);
       //     }
            //disconnect
            if (GUILayout.Button("断开服务器"))
            {
                Network.Disconnect();
                Message = "";
                MoveInfo = "";
            }
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(200), GUILayout.Height(Screen.height));
            //Display chat message
            GUILayout.Box(Message);
            //Display Move message
            GUILayout.Box(MoveInfo);
            GUILayout.EndScrollView();
        }
        public int getAllPerson()
        {
            return AllPerson;
        }
        public int getEscapedPerson()
        {
            return EscapedPerson;
        }
        public int getDiedPerson()
        {
            return DiedPerson;
        }
        public string getARandomMan()
        {//new

            int i = randomMan.Next(6);
            //   UnityEngine.Debug.Log(i);
            if (i == 0)
                return "ThirdPersonController1";
            if (i == 1)
                return "ThirdPersonController3";
            if (i == 2)
                return "ThirdPersonController5";
            if (i == 3)
                return "ThirdPersonController7";
            if (i == 4)
                return "ThirdPersonController9";
            return "ThirdPersonController11";
        }
        public string getAManName(string ManName)
        {
            if (ManName == "Man(Clone)")
                ManName = "Man";
            else if (ManName == "Man2(Clone)")
                ManName = "Man2";
            else if (ManName == "Man3(Clone)")
                ManName = "Man3";
            else if (ManName == "Man4(Clone)")
                ManName = "Man4";
            else if (ManName == "Man5(Clone)")
                ManName = "Man5";
            else if (ManName == "Man6(Clone)")
                ManName = "Man6";
            else
                ManName = "";
            return ManName;
        }
        public string getADiedMan(string man) {
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
        public string getAPlayerName(string ManName)
        {
            if (ManName == "ThirdPersonController1(Clone)")
                ManName = "ThirdPersonController1";
            else if (ManName == "ThirdPersonController3(Clone)")
                ManName = "ThirdPersonController3";
            else if (ManName == "ThirdPersonController5(Clone)")
                ManName = "ThirdPersonController5";
            else if (ManName == "ThirdPersonController7(Clone)")
                ManName = "ThirdPersonController7";
            else if (ManName == "ThirdPersonController9(Clone)")
                ManName = "ThirdPersonController9";
            else if (ManName == "ThirdPersonController11(Clone)")
                ManName = "ThirdPersonController11";
            else
                ManName = "";
            return ManName;
        }
        //receive message
        [RPC]
        void RequestMessage(string message, NetworkMessageInfo info)
        {
  //          Message += "\n" + "发送者" + info.sender + ":" + message;
        }
        [RPC]
        void RequestCreate(string thing, int x, int y, Quaternion q, NetworkMessageInfo info)
        {
    //        MoveInfo += "\n" + "客户" + info.sender + ":执行了" + thing + " " + x + " " + y + " 创建事件";
        }
        [RPC]
        void RequestCreate2(string thing, int x, int y, Quaternion q, NetworkMessageInfo info)
        {
   //         MoveInfo += "\n" + "客户" + info.sender + ":执行了" + thing + " " + x + " " + y + " 创建事件";
        }
        [RPC]
        void AskCreate(NetworkMessageInfo info)
        {
     //       MoveInfo += "\n" + "客户" + info.sender + ":Asked create";
            PersonControllerIns = GameObject.Find("PersonController");
            personList = PersonControllerIns.GetComponent<PersonController>().getPersonList();
            string ManName;
            for (int i = 0; i < personList.Count; i++)
            {
                ManName = personList[i].getPointer().name;
                ManName = getAManName(ManName);
                MainRoom.GetComponent<NetworkView>().RPC("RequestCreate2", RPCMode.All, ManName, personList[i].getX(), personList[i].getY(), personList[i].getPointer().transform.rotation);
            }
            List<GameObject> fires = FireController.getInstance().getFires();
            Debug.Log("fireCount:" + fires.Count);
            for (int j = 0; j < fires.Count; j++)
            {
                Debug.Log("fire" + j);
                if (fires[j].name == "Fire(Clone)")
                    MainRoom.GetComponent<NetworkView>().RPC("SyncFire2", RPCMode.All, "Fire", (int)fires[j].transform.position.x, (int)fires[j].transform.position.z, Quaternion.identity);
                else
                    MainRoom.GetComponent<NetworkView>().RPC("SyncFire2", RPCMode.All, "fx_fire_a", (int)fires[j].transform.position.x, (int)fires[j].transform.position.z, Quaternion.identity);
            }

            for (int k = 0; k < Players.Count; k++)
            {
                if (Players[k] == null)
                    MainRoom.GetComponent<NetworkView>().RPC("AddPlayerNone", RPCMode.All);
                else if (Players[k].name == "Man(Clone)" || Players[k].name == "Man2(Clone)" || Players[k].name == "Man3(Clone)" || Players[k].name == "Man4(Clone)" || Players[k].name == "Man5(Clone)" || Players[k].name == "Man6(Clone)")//needchange
                {
                    ManName = getAManName(Players[k].name);
                    MainRoom.GetComponent<NetworkView>().RPC("AddPlayerDied", RPCMode.All, ManName, Players[k].transform.position, Players[k].transform.rotation);
                }
                else {
                    ManName = Players[k].name;
                    ManName = getAPlayerName(ManName);
                    MainRoom.GetComponent<NetworkView>().RPC("AddPlayer", RPCMode.All, ManName, Players[k].transform.position, Players[k].transform.rotation);

                }
            }
            Vector3 HumanPos = PersonControllerIns.GetComponent<PersonController>().setHuman();
            ManName = getARandomMan();
            Players.Add((GameObject)Instantiate(Resources.Load(ManName), HumanPos, Quaternion.identity));
            Players[Players.Count - 1].AddComponent<HumanController>();
            Players[Players.Count - 1].GetComponent<HumanController>().SetIndex(Players.Count - 1);    //new
            MainRoom.GetComponent<NetworkView>().RPC("AddPlayer2", RPCMode.Others, ManName, HumanPos);
            AllPerson += 1;
        }
        [RPC]
        void SyncAction(int index, int x, int y, int d, Vector3 v, NetworkMessageInfo info)
        {
        }
        [RPC]
        void SyncIsEscaped(int index, NetworkMessageInfo info)
        {   
        }
        [RPC]
        void SyncFire(string thing, int x, int y, Quaternion q, NetworkMessageInfo info)
        {
        }
        [RPC]
        void SyncFire2(string thing, int x, int y, Quaternion q, NetworkMessageInfo info)
        {
        }
        [RPC]
        void AddPlayer(string thing, Vector3 v, Quaternion q, NetworkMessageInfo info)
        {
        }
        [RPC]
        void AddPlayerDied(string thing, Vector3 v, Quaternion q, NetworkMessageInfo info)
        {
        }
        [RPC]
        void AddPlayerNone(NetworkMessageInfo info)
        {
        }
        [RPC]
        void AddPlayer2(string thing, Vector3 v, NetworkMessageInfo info)
        {
        }
        [RPC]
        void SyncPlayer(Vector3 move, bool crouch, bool jump, int index, NetworkMessageInfo info)
        {
       //     Debug.Log("Sync " + index + " Players " + Players.Count);
            if (Players[index] == null || Players[index].name == "Man(Clone)" || Players[index].name == "Man2(Clone)" || Players[index].name == "Man3(Clone)" || Players[index].name == "Man4(Clone)" || Players[index].name == "Man5(Clone)" || Players[index].name == "Man6(Clone)")
                return;
        //    Debug.Log(Players[index].name);
            Players[index].GetComponent<ThirdPersonCharacter>().Move(move, crouch, jump);
        }
        [RPC]
        void FixPlayer(Vector3 v, int index, NetworkMessageInfo info)
        {
         //   Debug.Log("Fix " + index + " Players " + Players.Count);
            if (Players[index] == null)
                return;
            Players[index].transform.position = v;
        }
        [RPC]
        void PersonIsEscaped(int index, NetworkMessageInfo info)
        {
            Destroy(Players[index]);
            //	Players.RemoveAt (index);
            Debug.Log("Server Index " + index);
            //	for (int k=index; k<Players.Count; k++) {
            //		Players[k].GetComponent<HumanController>().SetIndex(k);
            //	}
      //      Debug.Log("Server " + Players.Count);
            EscapedPerson += 1;
        }
        [RPC]
        void PersonDie(int index, NetworkMessageInfo info)
        {
            if (Players[index] == null)
                return;
            Vector3 v = Players[index].transform.position;
            Quaternion q = Players[index].transform.rotation;
            string man = Players[index].name;
            man = getADiedMan(man);
            Debug.Log("diedman is " + man);
            Destroy(Players[index]);
            Players[index] = (GameObject)Instantiate(Resources.Load(man), v, q);
            Destroy(Players[index].GetComponent<PersonBehavior>());
            Players[index].GetComponent<Animator>().SetInteger("death", 2);
            DiedPerson += 1;
        }
        [RPC]                 //for example index is what we want to pass to client use
                              //if (Network.peerType == NetworkPeerType.Server)
                              //        MainRoom.GetComponent<NetworkView>().RPC("Example", RPCMode.All, index);
                              //at where you want to use this function 
                              //if MainRoom was not defined ,define it as i did
        void LifeControl(int index,int life, NetworkMessageInfo info)
        {
            //for you,do nothing 
            //this function is for server use

        }
    }
}