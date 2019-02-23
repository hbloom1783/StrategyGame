using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace StrategyGame.Battle.Map
{
    [CreateAssetMenu(fileName = "Terrain", menuName = "Athanor/Battle/Battle Terrain", order = 1)]
    public class BattleTerrain : ScriptableObject
    {
        public Sprite sprite;

        public bool isWalkable;
        public bool canSeeThru;

        public Vector3 unitFooting;
    }
}
