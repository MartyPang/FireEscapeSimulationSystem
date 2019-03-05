using CelluarAutomation.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using CellularAutomaton.Controller;
using Assets.Utils;
using UnityEngine;
using System.Collections;
using Assets.Controller;

namespace CellularAutomaton{
	public class GlobalController : MonoBehaviour 
	{
	    public bool m_debug = false;
        static GlobalController Instance;//作为单实例
        public int x=0;
		public int y=0;
        public List<Point> exits = new List<Point>();
		bool flag = false;
        int i = 0;   //出口选择
        public static GlobalController getInstance() {
			return Instance;
	    }

        GlobalController()
        {
            flag = false;
        }

		void Awake() {          // 早于Start方法执行
			Instance = this;
			// 初始化场景信息
			flag = false;
			//this.BuildMap();
			MakeABuild (0);    //new
		}
		void FixedUpdate(){
            //	BuildMap ();
            if (flag == true) {
                this.BuildMap(i);
				PersonController.getInstance().ReStart();
				flag = false;
			}
		}
          
	   [ContextMenu("BuildMap")]   
		public void BuildMap(int i)
	    {
            //模拟出口位置
            exits.Clear();
            if (i == 1)
            {
                if (Config.SCENE == 0)
                    exits.Add(new Point(26, 40));
                else if (Config.SCENE == 1)
                {
                    exits.Add(new Point(38, 0));
                    exits.Add(new Point(39, 0));
                }
                else
                    UnityEngine.Debug.Log("SceneUse Error");
            }
            else if (i == 2)
            {
                if (Config.SCENE == 0)
                    exits.Add(new Point(54, 40));
                else if (Config.SCENE == 1)
                {
                    exits.Add(new Point(42,38));
                    exits.Add(new Point(42,37));
                    exits.Add(new Point(42,36));
                }
                else
                    UnityEngine.Debug.Log("SceneUse Error");
            }
            else if (i == 3)
            {
                if (Config.SCENE == 0)
                {
                    exits.Add(new Point(26, 40));
                    exits.Add(new Point(54, 40));
                }
                else if (Config.SCENE == 1)
                {
                    exits.Add(new Point(38, 0));
                    exits.Add(new Point(39, 0));
                    exits.Add(new Point(42, 38));
                    exits.Add(new Point(42, 37));
                    exits.Add(new Point(42, 36));
                }
                else
                    UnityEngine.Debug.Log("SceneUse Error");
            }
           
            FloorPlanManager.getInstance().setInitConfig(x, y, exits);

            FloorPlanManager.getInstance().initMap();
            //test
            FireController.getInstance().testInit();
        }

        //find all neighbors in the distance smaller than @distance,
        //except those points which type is block
        public List<Point> findNeighborsWithoutBlock(Point point, int distance)
        {
            PlaceNode[,] nodes = FloorPlanManager.getInstance().getNodes();
            int height = FloorPlanManager.getInstance().getHeight();
            int width = FloorPlanManager.getInstance().getWidth();
            List<Point> pointNodes = new List<Point>();
            //up
            for (int i = point.y - 1; i >= Math.Max(point.y - distance, 0); --i)
            {
                if (nodes[point.x, i].isEnable()) {
                    pointNodes.Add(new Point(point.x, i));
                }
                else
                {
                    break;
                }
            }
            //down
            for (int i = point.y + 1; i <= Math.Min(point.y + distance, height - 1); ++i)
            {
                if (nodes[point.x, i].isEnable())
                {
                    pointNodes.Add(new Point(point.x, i));
                }
                else
                {
                    break;
                }
            }
            //left
            for (int i = point.x - 1; i >= Math.Max(point.x - distance, 0); --i)
            {
                if (nodes[i, point.y].isEnable())
                {
                    pointNodes.Add(new Point(i, point.y));
                }
                else
                {
                    break;
                }
            }
            //right
            for (int i = point.x + 1; i <= Math.Min(point.x + distance, width - 1); ++i)
            {
                if (nodes[i, point.y].isEnable())
                {
                    pointNodes.Add(new Point(i, point.y));
                }
                else
                {
                    break;
                }
            }

            return pointNodes;
        }

        public List<Point> findNeighborsWithoutBlock(Point point)
        {
            return findNeighborsWithoutBlock(point, 1);
        }


        public List<Point> findAllNeighbors(Point point)
        {
            PlaceNode[,] nodes = FloorPlanManager.getInstance().getNodes();
            int height = FloorPlanManager.getInstance().getHeight();
            int width = FloorPlanManager.getInstance().getWidth();
            List<Point> pointNodes = new List<Point>();
            //up
            for (int i = point.y - 1; i >= Math.Max(point.y - 1, 0); --i)
            {
                pointNodes.Add(new Point(point.x, i));
            }
            //down
            for (int i = point.y + 1; i <= Math.Min(point.y + 1, height - 1); ++i)
            {
                pointNodes.Add(new Point(point.x, i));
            }
            //left
            for (int i = point.x - 1; i >= Math.Max(point.x - 1, 0); --i)
            {
                pointNodes.Add(new Point(i, point.y));
            }
            //right
            for (int i = point.x + 1; i <= Math.Min(point.x + 1, width - 1); ++i)
            {
                pointNodes.Add(new Point(i, point.y));
            }

            return pointNodes;
        }

        public void MakeABuild(int i)
        {
            flag = true;
            this.i = i;
		}

		void OnDrawGizmos()
        {
            PlaceNode[,] nodes = FloorPlanManager.getInstance().getNodes();
            if (!m_debug || nodes == null)
				return;

			Gizmos.color = Color.black;
            foreach (Point exit in exits)
                Gizmos.DrawCube(new Vector3(exit.x + 0.5f, 0, exit.y + 0.5f), new Vector3(1, 0 + 0.1f, 1));
			// 线条的颜色
			Gizmos.color = Color.blue;

            int CanvasHeight = FloorPlanManager.getInstance().getHeight();
            int CanvasWidth = FloorPlanManager.getInstance().getWidth();
            // 绘制线条的高度
            float height = 0;
			
			// 绘制网格
			for (int i = 0; i < CanvasWidth; i++) {
				// i决定了格子的宽度
				Gizmos.DrawLine(new Vector3(i, height, 0), new Vector3(i, height, CanvasHeight));
			}
			for (int k = 0; k < CanvasHeight; k++) {
				// k决定格子高度
				Gizmos.DrawLine(new Vector3(0, height, k), new Vector3(CanvasWidth, height, k));
			}
			
			// 改为红色
			Gizmos.color = Color.red;
			
			for (int i = 0; i < CanvasWidth; i++) {
				for (int k = 0; k < CanvasHeight; k++) {
					//在不能放置防守区域的方格内绘制红色的方块
					if (nodes[i,k].type == PlaceNode.TYPE_BLOCK) {
						Gizmos.color = new Color(1, 0, 0, 0.5f);
						
						Gizmos.DrawCube(new Vector3(i + 0.5f, height, k + 0.5f), new Vector3(1, height + 0.1f, 1));
					}
				}
			}
			if (PersonController.personList == null)
				return;
			Gizmos.color = Color.green;
			for (int i = 0; i < PersonController.personList.Count; i++)
			{
					//在不能放置防守区域的方格内绘制红色的方块
				int x = PersonController.personList[i].getX();
				int y = PersonController.personList[i].getY();
						
						Gizmos.DrawCube(new Vector3(x + 0.5f, height, y + 0.5f), new Vector3(1, height + 0.1f, 1));
			}
		}
	}
}