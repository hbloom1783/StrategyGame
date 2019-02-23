using Athanor.Collections;
using Athanor.Randomization;
using GridLib.Hex;
using StrategyGame.Battle.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace StrategyGame.Battle.Persistence
{
    [Serializable]
    public class BattleUnitPersist
    {
        public string spritePath = null;

        public Team team = Team.player;

        public int maxHp = 0;
        public int hp = 0;

        public int maxAp = 0;
        public int ap = 0;

        public int speed = 0;
        public int jump = 0;

        public string[] abilityList = new string[0];

        public string ai = "";
    }

    [Serializable]
    public class BattleCellPersist
    {
        public TerrainPersist type = TerrainPersist.greenOpen;
        public int elevation = 0;
        public BattleUnitPersist unitPresent = null;

        public BattleCellPersist(TerrainPersist type = TerrainPersist.greenOpen, int elevation = 0)
        {
            this.type = type;
            this.elevation = 0;
        }
    }

    [Serializable]
    public class BattlePersist
    {
        public Dictionary<HexCoords, BattleCellPersist> mapContents =
            new Dictionary<HexCoords, BattleCellPersist>();

        public Team whoseTurn = Team.player;

        public static BattlePersist Generate()
        {
            BattlePersist result = new BattlePersist();
            result.mapContents = MapGeneration.Generate(MapType.greenOpen);
            
            // Add units
            result.mapContents.Values
                .Where(x => (x.type == TerrainPersist.greenOpen) && (x.unitPresent == null))
                .ToList()
                .RandomPick()
                .unitPresent = Resources.Load<MapUnitRecipe>("Battle/Units/Sonic").persist;

            foreach (int idx in Enumerable.Range(0, Random.Range(2, 4)))
                result.mapContents.Values
                    .Where(x => (x.type == TerrainPersist.greenOpen) && (x.unitPresent == null))
                    .ToList()
                    .RandomPick()
                    .unitPresent = Resources.Load<MapUnitRecipe>("Battle/Units/Motobug").persist;

            return result;
        }
    }
}
