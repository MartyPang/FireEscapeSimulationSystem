using CellularAutomaton.Controller;
using CellularAutomaton.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace CellularAutomaton.Utils
{
    class PersonLocationManager
    {
        public const int MOVE_LEFT = 0x003;
        public const int MOVE_RIGHT = 0x004;
        public const int MOVE_UP = 0x002;
        public const int MOVE_DOWN = 0x006;

        int x;
        int y;

        int originX;
        int originY;

        int direction;
        int originDirection;

        public PersonLocationManager()
        {
            direction = originDirection = -1;
            x = y = originX = originY = -1;
        }

        public void setXY(int x, int y)
        {
            originX = this.x;
            originY = this.y;
            this.x = x;
            this.y = y;
        }

        public void setX(int x)
        {
            originX = this.x;
            originY = this.y;
            this.x = x;
        }

        public void setY(int y)
        {
            originX = this.x;
            originY = this.y;
            this.y = y;
        }

        public int getX()
        {
            return x;
        }

        public int getOriginX()
        {
            return originX;
        }

        public int getOriginY()
        {
            return originY;
        }

        public void setOriginPosition(Point p)
        {
            originX = p.x;
            originY = p.y;
        }
        
        public int getY()
        {
            return y;
        }

        public int getDirection()
        {
            return direction;
        }

        public int getOriginDirection()
        {
            return originDirection;
        }

        public void setDirection(int d)
        {
            originDirection = direction;
            direction = d;
        }

        public bool isCircle()
        {
            return originDirection * direction == 12;
        }

		public bool isCircle(int dOld, int dNew)
		{
			return dOld * dNew == 12;
		}

        public int randomDirection(List<NeighborNode> neighborNodes)
        {
            int length = neighborNodes.Count();
            Random ran = new Random();
            return neighborNodes[ran.Next(length)].getDirection();
        }
	}
}