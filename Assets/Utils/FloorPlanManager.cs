
using System;
using System.Collections.Generic;
using CelluarAutomation.Model;
using UnityEngine;
using CellularAutomaton.Controller;

namespace CellularAutomaton
{
	struct coordinate{
		public int x{ get; set;}
		public int y{ get; set;}
	//	public coordinate(){
	//		x = 0;
	//		y = 0;
	//	}
		public coordinate(int a,int b){
			x = a;
			y = b;
		}
	}
	public class FloorPlanManager
	{
		public int xCount;
		public int yCount;
		public int padding;
        List<Point> exits;
		public PlaceNode[,] nodes;
        static FloorPlanManager instance;//单实例

        FloorPlanManager()
        {
            
        }

        static public FloorPlanManager getInstance()
        {
            if (instance == null)
            {
                instance = new FloorPlanManager();
            }
            return instance;
        }

		public void setInitConfig (int width, int height,List<Point> exits)
		{
			xCount = width;
			yCount = height;
            this.exits = exits;
			nodes = new PlaceNode[width, height];
            //初始化地点节点
            for (int i = 0; i < getWidth(); ++i)
            {
                for (int j = 0; j < getHeight(); ++j)
                {
                    nodes[i, j] = new PlaceNode();
                }
            }
        }

        public void initMap()
        {
            setBlocks();
            //设置出口
            foreach (Point exit in exits)
            {
                nodes[exit.x, exit.y].type = PlaceNode.TYPE_EXIT;
                nodes[exit.x, exit.y].heightLine = 0;
            }
            calculateHeightLine();
        }

        private void setBlocks()
        {
            GameObject[] blocks = (GameObject[])GameObject.FindGameObjectsWithTag("Block");

            foreach (GameObject nodeobj in blocks)
            {
                // 节点位置
                Vector3 pos = nodeobj.transform.position;

                // 如果节点的位置超出场景范围，则忽略
                if ((int)pos.x >= getWidth() || (int)pos.z >= getHeight())
                    continue;

                // 设置格子的属性
                setBlock((int)pos.x, (int)pos.z);
            }
        }

        public int getContainerNum(Point p)
        {
            return nodes[p.x, p.y].getPersonNum();
        }

        public PlaceNode[,] getNodes(){
			return nodes;
		}

        public Point getExit(int index)
        {
            return exits[index];
        }

		public List<Point> getExits(){
			return exits;
		}

        public bool isExit(Point p)
        {
            return exits.Contains(p);
        }
		//get the canvas width
		public int getWidth()
		{
			return xCount;
		}

		//get the canvas width	
		public int getHeight()
		{
			return yCount;
		}
		
		public void increaseHeightLine(int x, int y, int amount)
		{
			nodes [x, y].increaseHeight (amount);
		}

		
		public void setBlock(int x, int y) {
			nodes [x, y].type = PlaceNode.TYPE_BLOCK;
		}


	    bool valid(int x, int y)
		{
			return (x >= 0)&&(x < getWidth())&&(y >= 0)&&(y < getHeight());
		}

		private bool blockAt(int x, int y)
		{
			return !(nodes[x,y].isEnable());
		}

		public void calculateHeightLine()
		{
            if (exits.Count == 0) return;
			List<coordinate> MapList=new List<coordinate>();
			coordinate tmp;
			int min;
            Point firstExit = exits[0];
			MapList.Add (new coordinate (firstExit.x, firstExit.y));
			nodes [firstExit.x, firstExit.y].visited = true;
			while (MapList.Count>0) {
				tmp=MapList[0];
				MapList.RemoveAt(0);
				min=PlaceNode.UNREACHABLE;
				if (valid(tmp.x+1,tmp.y) && !blockAt(tmp.x+1,tmp.y)){
					if (nodes[tmp.x+1,tmp.y].visited==false){
						nodes[tmp.x+1,tmp.y].visited=true;
						MapList.Add (new coordinate(tmp.x+1,tmp.y));
					}
					else{
						min=nodes[tmp.x+1,tmp.y].heightLine;
					}
				}
				if (valid(tmp.x-1,tmp.y) && !blockAt(tmp.x-1,tmp.y)){
					if (nodes[tmp.x-1,tmp.y].visited==false){
						nodes[tmp.x-1,tmp.y].visited=true;
						MapList.Add (new coordinate(tmp.x-1,tmp.y));
					}
					else{
						if (nodes[tmp.x-1,tmp.y].heightLine<min)
						    min=nodes[tmp.x-1,tmp.y].heightLine;
					}
				}
				if (valid(tmp.x,tmp.y+1) && !blockAt(tmp.x,tmp.y+1)){
					if (nodes[tmp.x,tmp.y+1].visited==false){
						nodes[tmp.x,tmp.y+1].visited=true;
						MapList.Add (new coordinate(tmp.x,tmp.y+1));
					}
					else{
						if (nodes[tmp.x,tmp.y+1].heightLine<min)
							min=nodes[tmp.x,tmp.y+1].heightLine;
					}
				}
				if (valid(tmp.x,tmp.y-1) && !blockAt(tmp.x,tmp.y-1)){
					if (nodes[tmp.x,tmp.y-1].visited==false){
						nodes[tmp.x,tmp.y-1].visited=true;
						MapList.Add (new coordinate(tmp.x,tmp.y-1));
					}
					else{
						if (nodes[tmp.x,tmp.y-1].heightLine<min)
							min=nodes[tmp.x,tmp.y-1].heightLine;
					}
				}
				if (min==PlaceNode.UNREACHABLE)
					nodes[tmp.x,tmp.y].heightLine=0;
				else
					nodes[tmp.x,tmp.y].heightLine=min+1;
			}
        }
	}
}

