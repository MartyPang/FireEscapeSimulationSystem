using CelluarAutomation.Model;
using CellularAutomaton;
using CellularAutomaton.Controller;
using Priority_Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Controller
{
    class MoveController
    {
        static int INF = 9999999;
        int[] preOdd, preEven;//作为最短路径的前驱记录
        int[,] pre;
        int[] distance;
        bool[] visit;
        int[] value;
        int[] first, next;/*first数组保存的是节点i的第一条边，next保存的是边e的下一条边*/
        int[] v;//记录每条边的后节点
        FastPriorityQueue<PathNode> queue;

        PlaceNode[,] nodes;
        int width = FloorPlanManager.getInstance().getWidth();
        int height = FloorPlanManager.getInstance().getHeight();
        List<Point> nextFirePoints;//接下来会着火的点，通过计算接下来的着火点，设置value为INF/10，使人物尽可能远离火
        int count;
        static MoveController instance;//单实例

        static public MoveController getInstance()
        {
            if (instance == null)
            {
                instance = new MoveController();
            }
            return instance;
        }
        MoveController()
        {
            count = 1;
            int length = width * height;
            first = new int[length];
            next = new int[length * 4];
            value = new int[length * 4];
            v = new int[length*4];
            preOdd = preEven = new int[length];//最大路径长度为所有节点遍历
            pre = new int[FloorPlanManager.getInstance().getExits().Count, length * 4];
            distance = new int[length];
            visit = new bool[length];
            queue = new FastPriorityQueue<PathNode>(length);
        }
        public void ReStart()
        {
            count = 1;
            int length = width * height;
            first = new int[length];
            next = new int[length * 4];
            value = new int[length * 4];
            v = new int[length * 4];
            preOdd = preEven = new int[length];//最大路径长度为所有节点遍历
            pre = new int[FloorPlanManager.getInstance().getExits().Count, length * 4];
            distance = new int[length];
            visit = new bool[length];
            queue = new FastPriorityQueue<PathNode>(length);
        }
        public void DijkstraShortestPath(int source, int exitIndex)
        {
            int length = width * height;
            int[] preCopy = new int[length*4];
            for(int i = 0; i < preCopy.Length; ++i)
            {
                preCopy[i] = -1;
            }
            initVar();
            distance[source] = 0;
            queue.Enqueue(new PathNode(source, distance[source]), distance[source]);
            while(queue.Count != 0)
            {
                PathNode pathNode = queue.Dequeue();
                if (visit[pathNode.point]) continue;//已计算
                visit[pathNode.point] = true;//k节点已遍历
                for(int m = first[pathNode.point]; m != -1; m = next[m])
                {
                    if(value[m] + distance[pathNode.point] < distance[v[m]])
                    {
                        distance[v[m]] = value[m] + distance[pathNode.point];
                        preCopy[v[m]] = pathNode.point;
                        queue.Enqueue(new PathNode(v[m], value[v[m]]), value[v[m]]);
                    }
                }   
            }
            for(int i = 0; i < preCopy.Length; ++i)
            {
                pre[exitIndex, i] = preCopy[i];
            }
        }
        //进行相关变量初始化操作
        private void initVar()
        {
            nextFirePoints = FireController.getInstance().findAllNearFirePoints();
            nodes = FloorPlanManager.getInstance().getNodes();
            queue.Clear();
            int length = width * height;
            for(int i = 0; i < length; ++i)
            {
                distance[i] = INF;
                visit[i] = false;
                first[i] = -1;
            }
            caculateValue();
        }
        //计算邻接数组值
        private void caculateValue()
        {
            int k = 0;
            List<Point> valuePoints;
            for(int i = 0; i < width; ++i)
            {
                for(int j = 0; j < height; ++j)
                {
                    valuePoints = GlobalController.getInstance().findAllNeighbors(new Point(i, j));
                    foreach(Point p in valuePoints){
                        v[k] = p.x*height + p.y;
                        next[k] = first[i*height + j];
                        first[i*height + j] = k;
                        value[k] = getValue(new Point(i, j), p);
                        ++k;
                    }
                }
            }
        }
        //返回两点间最大值作为路径值
        //如果有一点为Block，返回INF
        //如果一点为NextFiredPoint，返回INF/10
        private int getValue(Point a, Point b)
        {
            if(nodes[a.x, a.y].heightLine == PlaceNode.UNREACHABLE ||
                nodes[b.x, b.y].heightLine == PlaceNode.UNREACHABLE) 
            {
                return INF;
            }
            if(nextFirePoints.Contains(a) || nextFirePoints.Contains(b))
            {
                return INF;
            }
            return nodes[b.x, b.y].heightLine;
        }

        public Point getNextPoint(Point p, int exitIndex)
        {
            int v = p.x * height + p.y;
            int returnV = pre[exitIndex, v];

            //UnityEngine.Debug.Log("returnV: " + returnV + "\n");
            if (returnV == -1) throw new Exception();
            return new Point(returnV / height, returnV % height);
        } 
    }

    class PathNode : FastPriorityQueueNode
    {
        public int point;
        public int value;

        public PathNode(int p, int v)
        {
            point = p;
            value = v;
        }
    }
}
