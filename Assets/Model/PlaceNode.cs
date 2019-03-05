using System;
using System.Collections.Generic;

namespace CelluarAutomation.Model
{
	public class PlaceNode
	{
		public static int UNREACHABLE = 999999;
		public static int TYPE_EMPTY = 0x000;
		public static int TYPE_BLOCK = 0x001;
        public static int TYPE_EXIT = 0x002;

        public static int MAX_CONTAIN_NUM = 3;

        public static int DELAY_BLOCK = 4;
		public static int DELAY_EMPTY = 1;
		
		public int heightLine { get; set; }
        int nodeType;
		public int type { get { return nodeType; }  set { nodeType = value; if (nodeType == TYPE_BLOCK) heightLine = UNREACHABLE; } }
		
		public bool visited{ get; set;}
		
		int personNum = 0;//record the person number of the node
		
		int fireDelay;//the node will be fired when fireDelay is 0
		
		public PlaceNode()
		{
			this.heightLine = UNREACHABLE;
			this.type = TYPE_EMPTY;
			fireDelay = DELAY_EMPTY;
			this.visited = false;
		}
		
		public PlaceNode(int heightLine, int type)
		{
			this.heightLine = heightLine;
			this.type = type;
			if(type == TYPE_BLOCK)
			{
				fireDelay = DELAY_BLOCK;
			}
			else
			{
				//fireDelay = (heightLine / 2 < DELAY_BLOCK) ? heightLine / 2 : DELAY_BLOCK;
				fireDelay = DELAY_EMPTY;
			}
		}
		
		public void setPersonNum(int num)
		{
			personNum = num;
		}
		
		public int getPersonNum()
		{
			return personNum;
		}
		
		public void setHeight(int height)
		{
			heightLine = height;
		}
		
		public void increaseHeight(int amount)
		{
			heightLine += amount;
		}
		
		public bool isEnable()
		{
			return type != PlaceNode.TYPE_BLOCK;
		}
		
		//if the node is fired, the fireDelay = 0
		public bool isFired()
		{
			return fireDelay == 0;
		}
		
		public bool isGoingToFire()
		{
			return (type == TYPE_BLOCK && fireDelay != DELAY_BLOCK) ||
				(type == TYPE_EMPTY && fireDelay != DELAY_EMPTY);
		}
		
		public void reduceFireDelayByTime()
		{
			fireDelay--;
		}
		
		public int getFireDelay()
		{
			return fireDelay;
		}

        public void setFired()
        {
            fireDelay = 0;
        }
	}
}
