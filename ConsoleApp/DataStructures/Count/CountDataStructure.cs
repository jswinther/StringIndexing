﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.DataStructures.Count
{
    public abstract class CountDataStructure
    {
        protected int x;
        protected int y_min;
        protected int y_max;
        protected CountDataStructure(string str, int x, int ymin, int ymax)
        {
            this.x = x;
            y_min = ymin;
            y_max = ymax;
        }
        public abstract int Matches(string pattern);
        public abstract int MatchesFixed(string pattern1, string pattern2);
        public abstract int MatchesVariable(string pattern1, string pattern2);
    }
}