using CelluarAutomation.Model;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CellularAutomaton.Controller
{
    public class FireController : MonoBehaviour
    {
        PlaceNode[,] nodes;
        List<FireNode> firingPoints;
        List<GameObject> fires;
        static public int callControl = 0;
        GameObject MainRoom;

        static FireController instance;//单实例

        FireController()
        {
            firingPoints = new List<FireNode>();
            fires = new List<GameObject>();
        }

        static public FireController getInstance()
        {
            if (instance == null)
            {
                instance = GameObject.Find("GlobalController").AddComponent<FireController>();
            }
            return instance;
        }

        void Start()
        {
            MainRoom = GameObject.Find("整层楼");
        }
   /*     public void Stop()
        {
            while (firingPoints.Count != 0)
                firingPoints.RemoveAt(0);
            while (fires.Count != 0)
            {
                if (fires[0] != null)
                    Destroy(fires[0]);
                fires.RemoveAt(0);
            }
        }*/

        public List<GameObject> getFires()
        {
            return fires;
        }

        public void addPoint(FireNode point)
        {
            firingPoints.Add(point);
        }

        public void addPoints(List<FireNode> points)
        {
            firingPoints.AddRange(points);
        }

        public bool isNodeFired(FireNode point)
        {
            return nodes[point.x, point.y].isFired();
        }

        public bool isNodeFired(int x, int y)
        {
            return nodes[x, y].isFired();
        }

        public void setNodeFired(FireNode point)
        {
            nodes[point.x, point.y].setFired();
        }

        public void setFireSource(List<FireNode> points)
        {
            nodes = FloorPlanManager.getInstance().getNodes();
            //clear fire points
            firingPoints.Clear();
            //set fire source
            addPoints(points);
            for (int i = 0; i < firingPoints.Count; ++i)
            {
                setNodeFired(firingPoints[i]);
                nodes[firingPoints[i].x, firingPoints[i].y].type = PlaceNode.TYPE_BLOCK;
            }
        }

        public List<FireNode> getFireNodes()
        {
            return firingPoints;
        }

        //run the function to handle the fire nodes per times
        public void handleFireNodes(int times)
        {
            List<FireNode> tmpPoints = new List<FireNode>();
            int i = 0;
            while (i < firingPoints.Count)
            {
                //if the node is burning
                if (isNodeFired(firingPoints[i]))
                {
                    //show the fire in Unity
                    //first call
                    if (!firingPoints[i].IsCheck)
                    { 
                        firingPoints[i].IsCheck = true;
                        if (times < 100)
                        {
                            GameObject tmp = (GameObject)Instantiate(Resources.Load("Fire"), new Vector3(0.5f + firingPoints[i].x, 0, 0.5f + firingPoints[i].y), Quaternion.identity);
                            fires.Add(tmp);
                            if (Network.peerType == NetworkPeerType.Server)
                                MainRoom.GetComponent<NetworkView>().RPC("SyncFire", RPCMode.All, "Fire", firingPoints[i].x, firingPoints[i].y, Quaternion.identity);
                        }
                        else
                        {
                            GameObject tmp = (GameObject)Instantiate(Resources.Load("fx_fire_a"), new Vector3(0.5f + firingPoints[i].x, 0, 0.5f + firingPoints[i].y), Quaternion.identity);
                            fires.Add(tmp);
                            if (Network.peerType == NetworkPeerType.Server)
                                MainRoom.GetComponent<NetworkView>().RPC("SyncFire", RPCMode.All, "fx_fire_a", firingPoints[i].x, firingPoints[i].y, Quaternion.identity);
                        }
                        nodes[firingPoints[i].x, firingPoints[i].y].type = PlaceNode.TYPE_BLOCK;
                        //add the neighbor into the list
                        tmpPoints.AddRange(findNearFirePoints(firingPoints[i]));
                        //delete the node
                        firingPoints.Remove(firingPoints[i]);
                        --i;//由于Count减1，故保持i不变
                    }
                }
                else
                {
                    //reduce the fireDelay time by time
                    nodes[firingPoints[i].x, firingPoints[i].y].reduceFireDelayByTime();
                }
                ++i;
            }
            addPoints(tmpPoints);
            //delete 1/3 of all the nodes when countNum > 15
            if (firingPoints.Count > 15)
            {
                int t = firingPoints.Count / 3;
                firingPoints.RemoveRange(0, t);
            }
        }

        private List<FireNode> findNearFirePoints(FireNode point)
        {
            int height = FloorPlanManager.getInstance().getHeight();
            int width = FloorPlanManager.getInstance().getWidth();
            List<FireNode> pointNodes = new List<FireNode>();
            //up
            if (!nodes[point.x, Math.Max(point.y - 1, 0)].isGoingToFire())
            {
                pointNodes.Add(new FireNode(point.x, Math.Max(point.y - 1, 0)));
            }
            //down
            if (!nodes[point.x, Math.Min(point.y + 1, height - 1)].isGoingToFire())
            {
                pointNodes.Add(new FireNode(point.x, Math.Min(point.y + 1, height - 1)));
            }
            //left
            if (!nodes[Math.Max(point.x - 1, 0), point.y].isGoingToFire())
            {
                pointNodes.Add(new FireNode(Math.Max(point.x - 1, 0), point.y));
            }
            //right
            if (!nodes[Math.Min(point.x + 1, width - 1), point.y].isGoingToFire())
            {
                pointNodes.Add(new FireNode(Math.Min(point.x + 1, width - 1), point.y));
            }

            return pointNodes;
        }

        public List<Point> findAllNearFirePoints()
        {
            List<FireNode> pointNodes = new List<FireNode>();
            foreach(FireNode node in firingPoints)
                pointNodes.AddRange(findNearFirePoints(node));
            //change FireNode to Point
            List<Point> points = new List<Point>(pointNodes.Count);
            foreach (FireNode node in pointNodes)
            {
                points.Add(new Point(node.x, node.y));
            }
             return points;
        }

        public void testInit()
        {
            List<FireNode> firePoints = new List<FireNode>();
            firePoints.Add(new FireNode(5, 7));
            firePoints.Add(new FireNode(20, 2));
            firePoints.Add(new FireNode(15, 30));
            firePoints.Add(new FireNode(40, 19));
            firePoints.Add(new FireNode(55, 9));
            firePoints.Add(new FireNode(70, 26));

            setFireSource(firePoints);
        }
    }

    public class FireNode : Point
    {
        bool isCheck = false;//表示这个燃烧的节点已经处理过
        public bool IsCheck { get { return isCheck; } set { isCheck = value; } }

        public FireNode(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
}








