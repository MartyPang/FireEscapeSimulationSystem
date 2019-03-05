using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Utils
{
    class Config
    {
        public static int PERSON_NUM = 10;//设置逃生的人数
        public static float FIRE_SPREAD_SPEED = 1.0f;//设置火焰的蔓延速度，0.5~1.5，1为15s蔓延一次
        public static float SCARE_SPEED = 1.0f;//设置恐慌增长速度，0.5~1.5
        public static int NUM_OF_EXITS = 0;
        public static int SCENE = 0;
    }
}
