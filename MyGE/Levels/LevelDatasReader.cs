using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGE.Levels
{
    public class LevelDatasReader
    {
        public int Number;
        public int LevelWidth;
        public int LevelHeight;
        public List<List<int>> LevelConfiguration = new List<List<int>>();
        public int GoalPoints;
        public int MaxMines;
        public int MineSecurityDistance;
        public float[] Ennemies = new float[3];
        public List<List<float>> UnitsRates = new List<List<float>>();
    }
}
