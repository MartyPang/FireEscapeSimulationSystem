using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CelluarAutomation.Model;
using System.Collections;
using CellularAutomaton.Utils;
using System.Diagnostics;
using UnityEngine;
using CellularAutomaton.Controller;
using Assets.Controller.factor;
using Assets.Controller;

#pragma warning disable CS0618 // 类型或成员已过时
namespace CellularAutomaton.Model
{
	class PersonNode
    {
        PersonLocationManager locationManager;
		int step = 1;//people vision distance, default = 3
        ScareManager scareManager;//every person has it's scare manager
		GameObject PersonIns;   //person instance
		Vector3 v;   //for person instance
        int bloodPercent = 100;//single agent's life
		GameObject MainRoom=GameObject.Find("整层楼");  //for network
        int exitIndex;

        bool changeExitFlag = false;

        public PersonNode(int exitIndex)
        {
            locationManager = new PersonLocationManager();
            scareManager = new ScareManager();
            this.exitIndex = exitIndex;
        }

        public PersonNode(int x, int y, GameObject p, int exitIndex) : this(exitIndex)
        {
            locationManager.setXY(x, y);

			PersonIns = p;
        }

		public GameObject DeletePointer(){
			return PersonIns;
		}

        public void setPoint(Point p)
        {
            locationManager.setXY(p.x, p.y);
        }
        public void setX(int x)
        {
            locationManager.setX(x);
        }

        public void setExitIndex(int index)
        {
            exitIndex = index;
        }

        public void setY(int y)
        {
            locationManager.setY(y);
        }

        public int getX()
        {
            return locationManager.getX();
        }

        public Point getPosition()
        {
            return new Point(getX(), getY());
        }
        public int getOriginX()
        {
            return locationManager.getOriginX();
        }

        public int getOriginY()
        {
            return locationManager.getOriginY();
        }
        
        public Point getOriginPosition()
        {
            return new Point(locationManager.getOriginX(), locationManager.getOriginY());
        }

        public int getY()
        {
            return locationManager.getY();
        }
		public GameObject getPointer(){
			return PersonIns;
		}

        public bool isDead()
        {
            return bloodPercent == 0;
        }

        public void reduceBloodByPercent(int reducePercent)
        {
            bloodPercent -= reducePercent;
        }

        public int getBloodPercent()
        {
            return bloodPercent;
        }

        public ScareManager getScareManager()
        {
            return scareManager;
        }

        public bool moveNext(int index)
        {
            Point originPosition = getOriginPosition();
            Point nextMove = new Point();
            try {
                nextMove = MoveController.getInstance().getNextPoint(new Point(getX(), getY()), exitIndex);
                locationManager.setDirection(getNodeDirection(new Point(getX(), getY()), new Point(nextMove.x, nextMove.y)));
            }
            catch
            {
                locationManager.setDirection(moveRules());
                switch (locationManager.getDirection())
                {
                    case PersonLocationManager.MOVE_DOWN:
                        nextMove = new Point(getX(), getY() - 1);
                        break;
                    case PersonLocationManager.MOVE_UP:
                        nextMove = new Point(getX(), getY() + 1);
                        break;
                    case PersonLocationManager.MOVE_LEFT:
                        nextMove = new Point(getX() - 1, getY());
                        break;
                    case PersonLocationManager.MOVE_RIGHT:
                        nextMove = new Point(getX() + 1, getY());
                        break;
                }
            }

            return reverseOrMove(nextMove, originPosition, index);
        }

        public bool moveNextRandom(int index)
        {
            Point originPosition = getOriginPosition();
            Point nextMove = new Point();
            //get neighbor points
            Point oldPoint = getPosition();
            List<Point> neighborPoints = GlobalController.getInstance().findNeighborsWithoutBlock(new Controller.Point(oldPoint.x, oldPoint.y), step);
            //get neighbor place nodes info
            PlaceNode[,] nodes = FloorPlanManager.getInstance().getNodes();
            List<NeighborNode> neighbors = new List<NeighborNode>();
            for (int i = 0; i < neighborPoints.Count(); ++i)
            {
                neighbors.Add(new NeighborNode(nodes[neighborPoints[i].x, neighborPoints[i].y],
                    getNodeDirection(new Point(oldPoint.x, oldPoint.y), neighborPoints[i])));
            }
            locationManager.setDirection(locationManager.randomDirection(neighbors));
            switch (locationManager.getDirection())
            {
                case PersonLocationManager.MOVE_DOWN:
                    nextMove = new Point(getX(), getY() - 1);
                    break;
                case PersonLocationManager.MOVE_UP:
                    nextMove = new Point(getX(), getY() + 1);
                    break;
                case PersonLocationManager.MOVE_LEFT:
                    nextMove = new Point(getX() - 1, getY());
                    break;
                case PersonLocationManager.MOVE_RIGHT:
                    nextMove = new Point(getX() + 1, getY());
                    break;
            }
            return reverseOrMove(nextMove, originPosition, index);
        }
        //检测是否确定移动
        public bool reverseOrMove(Point nextMove, Point originPosition, int index)
        {
            //if (!FloorPlanManager.getInstance().isExit(nextMove)
            //     && FloorPlanManager.getInstance().getContainerNum(nextMove) >= PlaceNode.MAX_CONTAIN_NUM)
            //{

             //   UnityEngine.Debug.Log("reverse");
                // 让人停下
            //    return false;
            //}
            //else
            {
                setPoint(nextMove);
                v = new Vector3(getX() - getOriginX(), 0, getY() - getOriginY());
                PersonIns.GetComponent<PersonBehavior>().setDirection(getOriginX(), getOriginY(), locationManager.getDirection());
                PersonIns.GetComponent<PersonBehavior>().setRoad(v);
                if (Network.peerType == NetworkPeerType.Server)
                    MainRoom.GetComponent<NetworkView>().RPC("SyncAction", RPCMode.All, index, getOriginX(), getOriginY(), locationManager.getDirection(), v);
                return true;
            }
        }

        public int moveRules()
        {
            //get neighbor points
			int oldX = getX ();
			int oldY = getY ();
            List<Point> neighborPoints = GlobalController.getInstance().findNeighborsWithoutBlock(new Controller.Point(oldX, oldY), step);

            //get neighbor place nodes info
            PlaceNode[,] nodes = FloorPlanManager.getInstance().getNodes();
            List<NeighborNode> neighbors = new List<NeighborNode>();
            for (int i = 0; i < neighborPoints.Count(); ++i)
			{
                neighbors.Add(new NeighborNode(nodes[neighborPoints[i].x, neighborPoints[i].y], 
                    getNodeDirection(new Point(oldX, oldY), neighborPoints[i])));
            }
            //get next direction
            return decideMoveDirection(neighbors);
		}

        private int getNodeDirection(Point oldPoint, Point newPoint)
        {
            if(oldPoint.x < newPoint.x)
            {
                return PersonLocationManager.MOVE_RIGHT;
            }
            if(oldPoint.x > newPoint.x)
            {
                return PersonLocationManager.MOVE_LEFT;
            }
            if(oldPoint.y < newPoint.y)
            {
                return PersonLocationManager.MOVE_UP;
            }
            if(oldPoint.y > newPoint.y)
            {
                return PersonLocationManager.MOVE_DOWN;
            }
            //imposible
            return -1;
        }
		
		public int decideMoveDirection(List<NeighborNode> neighbors)
		{
			neighbors.Sort(new INeighborComparer());
            if(neighbors.Count() == 0)
            {
                UnityEngine.Debug.Log("no Match!!!");
                return locationManager.randomDirection(neighbors);
            }
            return neighbors.ElementAt(0).getDirection();
        }

		public bool directionEnable(NeighborNode node){
			int nextDirection = node.getDirection();
			PlaceNode[,] nodes = FloorPlanManager.getInstance().getNodes();

            switch (nextDirection) {
				case PersonLocationManager.MOVE_LEFT:
					return nodes[getX() - 1, getY()].isEnable();
				case PersonLocationManager.MOVE_RIGHT:
					return nodes[getX() + 1, getY()].isEnable();
				case PersonLocationManager.MOVE_UP:
					return nodes[getX(), getY() + 1].isEnable();
				case PersonLocationManager.MOVE_DOWN:
					return nodes[getX(), getY() - 1].isEnable();
			}
			return true;
		}

        public void increaseScareValue()
        {
            scareManager.increaseScareValueWhenLeaveAlone(this);
        }

        //if the person isn't leaved alone, decrease his scare value between 1 - 3
        public void decreaseScareValue()
        {
            scareManager.decreaseScareValueWhenLeaveAlone(this);
        }

        public bool isAffectedByScare()
        {
            return scareManager.isAffected(getPosition());
        }

        public bool isAlone()
        {
            return scareManager.isPersonAlone(getPosition());
        }

        public bool isNearFire()
        {
            PlaceNode[,] nodes = FloorPlanManager.getInstance().getNodes();
            List<Point> neighborPoints = GlobalController.getInstance().findNeighborsWithoutBlock(getPosition(), 3);
            foreach (Point point in neighborPoints)
            {
                if (nodes[point.x, point.y].isFired())
                {
                    return true;
                }
            }
            return false;
        }

        public void changeExitIndex()
        {
            if (!changeExitFlag)
            {
                List<Point> exits = FloorPlanManager.getInstance().getExits();
                changeExitFlag = true;
                int minIndex = 0;
                int tmpVal;
                int minVal = (exits[0].x - getPosition().x) * (exits[0].x - getPosition().x) + (exits[0].y - getPosition().y) * (exits[0].y - getPosition().y);
                for (int i = 1; i < exits.Count; ++i)
                {
                    tmpVal = (exits[i].x - getPosition().x) * (exits[i].x - getPosition().x) + (exits[i].y - getPosition().y) * (exits[i].y - getPosition().y);
                    if (minVal > tmpVal && minIndex != exitIndex)
                    {
                        minVal = tmpVal;
                        minIndex = i;
                    }
                }
                exitIndex = minIndex;
            }
        }
    }
	
	class NeighborNode
	{
		int heightLine;
		int direction;
        int x;
        int y;
		
		public NeighborNode(PlaceNode node, int direction)
		{
            heightLine = node.heightLine;
            this.direction = direction;
		}
		
		public int getHeightLine()
        {
            return heightLine;
        }

        public int getDirection()
        {
            return direction;
        }

    }

    class INeighborComparer : IComparer<NeighborNode>
    {
        public int Compare(NeighborNode x, NeighborNode y)
        {
            NeighborNode nodeX = x as NeighborNode;
            NeighborNode nodeY = y as NeighborNode;

            if(nodeX.getHeightLine() > nodeY.getHeightLine())
            {
                return 1;
            }else if(nodeX.getHeightLine() == nodeY.getHeightLine())
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }
    }
}
