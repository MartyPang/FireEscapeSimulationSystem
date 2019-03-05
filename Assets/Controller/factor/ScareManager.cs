using Assets.Utils;
using CelluarAutomation.Model;
using CellularAutomaton;
using CellularAutomaton.Controller;
using CellularAutomaton.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Controller.factor
{
    class ScareManager
    {
        int scareValue = 50;// people init scare value ,when it large than 80, it will affect people's behavior
        System.Random random;

        public ScareManager()
        {
            random = new System.Random();
        }

        public void increase(int amount)
        {
            if (scareValue < 100)
                scareValue += amount;
        }

        public void decrease(int amount)
        {
            if(scareValue > 20)
                scareValue -= amount;
        }

        public void reset()
        {
            scareValue = 50;
        }

        //if the person is leaved alone, increase his scare value between 1 - 3
        public void increaseScareValueWhenLeaveAlone(PersonNode node)
        {
            node.getScareManager().increase((int)(random.Next(3) * Config.SCARE_SPEED));
        }

        //if the person isn't leaved alone, decrease his scare value
        public void decreaseScareValueWhenLeaveAlone(PersonNode node)
        {
            node.getScareManager().decrease(random.Next(3, 6));
        }
        //return a bool value indicate if the scare factor should affect person's behavior
        //depends by a random number and scare value
        public bool isAffected(Point position)
        {
            List<Point> exits = FloorPlanManager.getInstance().getExits();
            double ranNum = random.NextDouble();
            foreach (Point exit in exits) {
                if (distanceBetweenTwoPoint(exit, position) < 5)
                {
                    reset();
                    return false;
                }
            }
            return scareValue * ranNum >= 40;
        }

        private double distanceBetweenTwoPoint(Point p1, Point p2)
        {
            return Math.Sqrt((p1.x - p2.x) * (p1.x - p2.x) + (p1.y - p2.y) * (p1.y - p2.y));
        }

        public bool isPersonAlone(Point node)
        {
            PlaceNode[,] nodes = FloorPlanManager.getInstance().getNodes();
            List<Point> neighborPoints = GlobalController.getInstance().findNeighborsWithoutBlock(node, 3);
            foreach (Point point in neighborPoints)
            {
                if (nodes[point.x, point.y].getPersonNum() != 0)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
