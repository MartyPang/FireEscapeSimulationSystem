using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CellularAutomaton.Model;
using System.Diagnostics;
using CelluarAutomation.Model;

using UnityEngine;
using System.Collections;
using Assets.Controller;
using Assets.Utils;
using Assets.Controller.factor;
using UnityEngine.SceneManagement;
namespace CellularAutomaton.Controller
{
    class PersonController : MonoBehaviour
    {
        static public int CALLPERCOUNT = 15;
        static public PersonController Instance;
        static public int callTimes = 0;
        static public List<PersonNode> personList;
        static public GameObject MainRoom = GameObject.Find("整层楼");
        float timer = 0.0f;
        static public int ifseted = 0;
        private PlaceNode[,] nodes;
        private List<Point> exits;
        static public Quit quit;
        bool flag = false;
        System.Random randomMan;
        int AllNPC;
        int EscapedNPC;
        int DiedNPC;
        float StartTime;
        float StopTime;

        public bool stopClicked = false;
        bool set = false;
        void Awake()
        {
            Instance = this;

        }
        public static PersonController getInstance()
        {
            return Instance;
        }
        void Start()
        {
            quit = new Quit();
            personList = new List<PersonNode>();
            randomMan = new System.Random();
            AllNPC = 0;
            EscapedNPC = 0;
            DiedNPC = 0;
            StartTime = 0.0f;
            StopTime = 0.0f;
            startOnUpdate();
            CALLPERCOUNT = (int)(CALLPERCOUNT * Config.FIRE_SPREAD_SPEED);
        }
        public void ReStart()
        {
            flag = true;
        }

        void FixedUpdate()
        {
            if (flag)
            {
                startOnUpdate();
                flag = false;
            }
            if (set)
                setRandomNum();
        }
        void OnGUI()
        {
            if (Config.NUM_OF_EXITS == 1)
            {
                if (GUILayout.Button("出口1"))
                {
                    GlobalController.getInstance().MakeABuild(1);
                    set = true;
                    //                 setRandomNum();
                }
                if (GUILayout.Button("出口2"))
                {
                    GlobalController.getInstance().MakeABuild(2);
                    set = true;
                    //                setRandomNum();
                }
            }
            else if (Config.NUM_OF_EXITS == 2)
            {
                if (GUILayout.Button("set"))
                {
                    GlobalController.getInstance().MakeABuild(3);
                    set = true;
                    //                setRandomNum();
                }
            }
            else if (Config.NUM_OF_EXITS == 0)
                GUILayout.Label("No exit");
            else
                GUILayout.Label("Not enough exits");
            if (GUILayout.Button("stop"))
            {
                StopTime = Time.time;

                GameObject tmp = GameObject.Find("整层楼");


                int i = PlayerPrefs.GetInt("OffSet", 0);
                i = (i + 1) % 10;
                PlayerPrefs.SetInt("OffSet", i);
                PlayerPrefs.SetInt("AllNPC" + i, AllNPC);
                PlayerPrefs.SetInt("EscapedNPC" + i, EscapedNPC);
                PlayerPrefs.SetInt("DiedNPC" + i, DiedNPC);
                PlayerPrefs.SetInt("AllPerson" + i, tmp.GetComponent<Server>().getAllPerson());
                PlayerPrefs.SetInt("EscapedPerson" + i, tmp.GetComponent<Server>().getEscapedPerson());
                PlayerPrefs.SetInt("DiedPerson" + i, tmp.GetComponent<Server>().getDiedPerson());
                PlayerPrefs.SetFloat("StartTime" + i, StartTime);
                PlayerPrefs.SetFloat("StopTime" + i, StopTime);
                PlayerPrefs.SetInt("Exits" + i, Config.NUM_OF_EXITS);
                PlayerPrefs.SetFloat("FireSpread" + i, Config.FIRE_SPREAD_SPEED);
                PlayerPrefs.SetFloat("ScareSpeed" + i, Config.SCARE_SPEED);
                float t = Config.NUM_OF_EXITS + Config.FIRE_SPREAD_SPEED * 2 + Config.SCARE_SPEED * 5 + AllNPC * 2.0f / 100;
                PlayerPrefs.SetFloat("Strange" + i, t);
                PlayerPrefs.SetInt("Scene" + i, Config.SCENE);

                stopClicked = true;
                tmp.GetComponent<Server>().Stop();
                Stop();
            }
            if (stopClicked)
            {
                if (GUILayout.Button("chart"))
                {
                    quit.windowSwitch = true;
                    SceneManager.LoadScene("chart");

                }
            }



        }

        void Stop()
        {
            /*       AllNPC = 0;
                   EscapedNPC = 0;
                   DiedNPC = 0;
                   StartTime = 0.0f;
                   StopTime = 0.0f;
                   callTimes = 0;
                   timer = 0.0f;
                   ifseted = 0;
                   while (personList.Count != 0)
                   {
                       Destroy(personList[0].DeletePointer());
                       personList.RemoveAt(0);
                   }
                   FireController.getInstance().Stop();
                   MoveController.getInstance().ReStart();*/

        }
        void Update()
        {
            timer += Time.deltaTime;
            if (ifseted == 1 && timer >= 1.0f)
            {
                moveNext();
                timer = 0.0f;
            }
            if (personList.Count == 0)
                ifseted = 0;
        }
        public PersonController(List<PersonNode> list)
        {
            personList = list;
        }
        public string getARandomMan()
        {//new

            int i = randomMan.Next(6);
            //         UnityEngine.Debug.Log(i);
            if (i == 0)
                return "Man";
            if (i == 1)
                return "Man2";
            if (i == 2)
                return "Man3";
            if (i == 3)
                return "Man4";
            if (i == 4)
                return "Man5";
            return "Man6";
        }
        public void startOnUpdate()
        {
            nodes = FloorPlanManager.getInstance().getNodes();
            exits = FloorPlanManager.getInstance().getExits();

            //绘制人物
            UpdateMap();
        }

        public List<PersonNode> getPersonList()
        {
            return personList;
        }

        private void handleAloneScare(PersonNode personNode)
        {
            if (personNode.isAlone() || personNode.isNearFire())
            {
                personNode.increaseScareValue();
                increaseNeighborScareValue(personNode);
            }
            else
            {
                personNode.decreaseScareValue();
            }
        }

        private void increaseNeighborScareValue(PersonNode personNode)
        {
            List<Point> neighborPoints = GlobalController.getInstance().findNeighborsWithoutBlock(personNode.getPosition(), 3);
            foreach (PersonNode node in personList)
            {
                if (neighborPoints.Contains(node.getPosition()))
                {
                    node.increaseScareValue();
                }
            }
        }

        public void addPerson(PersonNode node)
        {
            personList.Add(node);
            UpdateMap();
        }
        [ContextMenu("move")]
        public void moveNext()
        {
            bool isMove = true;

            if (callTimes == 0 || callTimes % CALLPERCOUNT >= CALLPERCOUNT - 1)
            {
                FireController.getInstance().handleFireNodes(callTimes);
                int i = 0;
                foreach (Point exit in exits)
                {
                    MoveController.getInstance().DijkstraShortestPath(exit.x * FloorPlanManager.getInstance().getHeight() + exit.y, i);
                    i++;
                }
                for (int j = 0; j < personList.Count; j++)
                {
                    personList[j].setExitIndex(getNearestExitIndex(personList[j].getPosition()));
                }
            }
            for (int i = 0; i < personList.Count; i++)
            {
                handleAloneScare(personList[i]);
                //if person's scare value > 80, then he will random a avaliable direction
                if (personList[i].isAffectedByScare())
                {
                    isMove = personList[i].moveNextRandom(i);
                    personList[i].decreaseScareValue();
                }
                else
                {
                    if (getNeighborNum(personList[i]) > 30)
                    {
                        //寻找别的道路
                        personList[i].changeExitIndex();
                        isMove = personList[i].moveNext(i);
                    }
                    else
                    {
                        isMove = personList[i].moveNext(i);
                    }
                }
                if (isMove)
                    updateContainerNum(new Point(personList[i].getOriginX(), personList[i].getOriginY()), personList[i].getPosition());
            }
            ++callTimes;
            UpdateMap();
        }
        [ContextMenu("set")]
        public void setRandomNum()
        {
            StartTime = Time.time;
            ifseted = 1;
            System.Random random = new System.Random();
            int maxY = FloorPlanManager.getInstance().getHeight();
            int maxX = FloorPlanManager.getInstance().getWidth();
            int randomX;
            int randomY;
            for (int i = 0; i < Config.PERSON_NUM;)
            {
                randomX = random.Next(maxX);
                randomY = random.Next(maxY);
                if (nodes[randomX, randomY].type != PlaceNode.TYPE_BLOCK && nodes[randomX, randomY].type != PlaceNode.TYPE_BLOCK &&
                    nodes[randomX, randomY].getPersonNum() == 0)
                {
                    nodes[randomX, randomY].setPersonNum(nodes[randomX, randomY].getPersonNum() + 1);
                    string randomgetedMan = getARandomMan();
                    GameObject tmp = (GameObject)Instantiate(Resources.Load(randomgetedMan), new Vector3(0.5f + (float)randomX, 0, 0.5f + (float)randomY), Quaternion.identity);
                    tmp.transform.Rotate(Vector3.up * UnityEngine.Random.value * 360);
                    tmp.GetComponent<PersonBehavior>().init(randomX, randomY);
                    //		tmp.AddComponent<PersonBehavior>();
                    personList.Add(new PersonNode(randomX, randomY, tmp, getNearestExitIndex(new Point(randomX, randomY))));
                    UnityEngine.Debug.Log("this is" + Network.peerType);
                    if (Network.peerType == NetworkPeerType.Server)
                    {
                        MainRoom.GetComponent<NetworkView>().RPC("RequestCreate", RPCMode.All, randomgetedMan, randomX, randomY, tmp.transform.rotation);
                    }
                    i++;
                    AllNPC += 1;
                }
            }
            UpdateMap();
            set = false;
        }

        public Vector3 setHuman()
        {
            PlaceNode[,] nodes = FloorPlanManager.getInstance().getNodes();
            System.Random random = new System.Random();
            int maxY = FloorPlanManager.getInstance().getHeight();
            int maxX = FloorPlanManager.getInstance().getWidth();
            int randomX;
            int randomY;
            for (int i = 0; i < Config.PERSON_NUM;)
            {
                randomX = random.Next(maxX);
                randomY = random.Next(maxY);
                if (nodes[randomX, randomY].type != PlaceNode.TYPE_BLOCK && nodes[randomX, randomY].type != PlaceNode.TYPE_BLOCK &&
                    nodes[randomX, randomY].getPersonNum() == 0)
                {
                    return new Vector3(randomX + 0.5f, 0, randomY + 0.5f);
                }
            }
            return new Vector3(-1, 0, -1);
        }

        private void UpdateMap()
        {
            //清除上一步人物
            //绘制人物
            drawPerson();
            //检测人物是否抵达出口
            clearEscapedPerson();
        }

        private void drawPerson()
        {
            for (int i = 0; i < personList.Count; i++)
            {
                updateContainerNum(new Point(personList[i].getOriginX(), personList[i].getOriginY()), personList[i].getPosition());
            }
        }

        //原位置计数器减1，现位置计数器加1
        private void updateContainerNum(Point oldPoint, Point newPoint)
        {
            if (oldPoint.x != -1)
                nodes[oldPoint.x, oldPoint.y].setPersonNum(nodes[oldPoint.x, oldPoint.y].getPersonNum() - 1);
            nodes[newPoint.x, newPoint.y].setPersonNum(nodes[newPoint.x, newPoint.y].getPersonNum() + 1);
        }

        private void clearPerson()
        {
            int x, y;
            for (int i = 0; i < personList.Count; i++)
            {
                x = personList[i].getOriginX();
                y = personList[i].getOriginY();
                nodes[x, y].setPersonNum(nodes[x, y].getPersonNum() - 1);
            }
        }


        private void clearEscapedPerson()
        {
            for (int i = 0; i < personList.Count; i++)
            {
                if (isEscape(personList[i]))
                {
                    Destroy(personList[i].DeletePointer());
                    //总共逃出的总人数
                    nodes[personList[i].getX(), personList[i].getY()].setPersonNum(nodes[personList[i].getX(), personList[i].getY()].getPersonNum() + 1);
                    personList.RemoveAt(i);
                    if (Network.peerType == NetworkPeerType.Server)
                    {
                        MainRoom.GetComponent<NetworkView>().RPC("SyncIsEscaped", RPCMode.All, i);

                    }
                    EscapedNPC += 1;
                }
            }
        }

        private bool isEscape(PersonNode node)
        {
            foreach (Point exit in exits)
            {
                if (node.getX() == exit.x && node.getY() == exit.y)
                {
                    return true;
                }
            }
            return false;
        }

        private int getNeighborNum(PersonNode node)
        {
            int sum = 0;
            PlaceNode[,] nodes = FloorPlanManager.getInstance().getNodes();
            List<Point> neighborPoints = GlobalController.getInstance().findNeighborsWithoutBlock(node.getPosition(), 3);
            foreach(Point point in neighborPoints)
            {
                sum += nodes[point.x, point.y].getPersonNum();
            }
            return sum;
        }

        private int getNearestExitIndex(Point location)
        {
            int minIndex = 0;
            int tmpVal;
            //UnityEngine.Debug.Log("this " + location.x);
            int minVal = (exits[minIndex].x - location.x)*(exits[minIndex].x - location.x) + (exits[minIndex].y - location.y)*(exits[minIndex].y - location.y);
            for(int i = 1; i < exits.Count; ++i)
            {
                tmpVal = (exits[i].x - location.x) * (exits[i].x - location.x) + (exits[i].y - location.y) * (exits[i].y - location.y);
                if (minVal > tmpVal)
                {
                    minVal = tmpVal;
                    minIndex = i;
                }
            }
            return minIndex;
        }
    }
}