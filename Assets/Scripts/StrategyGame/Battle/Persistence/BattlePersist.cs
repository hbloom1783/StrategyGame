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

        public int maxHp = 5;
        public int hp = 5;

        public int maxAp = 2;
        public int ap = 2;

        public int speed = 5;
        public int jump = 3;

        public string[] abilityList = new string[0];
    }

    [Serializable]
    public class BattleCellPersist
    {
        public CellType type = CellType.walkable;
        public BattleUnitPersist unitPresent = null;
    }

    [Serializable]
    public class BattlePersist
    {
        public Dictionary<HexCoords, BattleCellPersist> mapContents =
            new Dictionary<HexCoords, BattleCellPersist>();

        public Team turn = Team.player;

        public static BattlePersist Generate()
        {
            BattlePersist result = new BattlePersist();
            
            // Generate blank
            foreach (HexCoords loc in HexCoords.O.CompoundRing(0, 6))
                result.mapContents[loc] = new BattleCellPersist();

            // Perlin-fill inside
            PerlinState perlin = new PerlinState();
            foreach (var kv in result.mapContents)
            {
                if (perlin.Sample(kv.Key.x, kv.Key.y) > 0.5f)
                    kv.Value.type = CellType.notWalkable;
                else
                    kv.Value.type = CellType.walkable;
            }
            
            // Add units
            result.mapContents.Values
                .Where(x => (x.type == CellType.walkable) && (x.unitPresent == null))
                .RandomPick()
                .unitPresent = Resources.Load<MapUnitRecipe>("Battle Units/Sonic").persist;

            foreach (int idx in Enumerable.Range(0, Random.Range(2, 4)))
                result.mapContents.Values
                    .Where(x => (x.type == CellType.walkable) && (x.unitPresent == null))
                    .RandomPick()
                    .unitPresent = Resources.Load<MapUnitRecipe>("Battle Units/Motobug").persist;

            return result;
        }
    }
}
