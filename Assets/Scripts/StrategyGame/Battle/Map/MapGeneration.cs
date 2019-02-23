using Athanor.Randomization;
using GridLib.Hex;
using StrategyGame.Battle.Persistence;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StrategyGame.Battle.Map
{
    public enum MapType
    {
        // Lunar Surface
        moonOpen,
        moonCrashSite,

        // Big Green
        greenOpen,
        greenCliffs,
        greenWater,

        greenCity,
        greenFarmland,

        // Mountains
        mountainHills,
        mountainBridge,
        mountainVolcano,

        // Desert
        desertOpen,
        desertVillage,
        desertMine,

        // Ruins
        greenRuins,
        desertRuins,
        mountainRuins,

        // Factory
        factoryConveyors,
        factoryPistons,
        factoryChemical,

        // Casino
        casino,

        // Fortress
        fortressBattlements,
        fortressInterior,
        fortressReactor,

        // Final zone
        finalZone,

        // etc
        invalid,
    }

    public enum WalkableType
    {
        walkable,
        notWalkable,
    }

    public static class MapGeneration
    {
        public static Dictionary<HexCoords, BattleCellPersist> Generate(MapType type)
        {
            switch(type)
            {
                case MapType.greenOpen:
                    return MakeGreenOpen();

                default:
                    return MakeBlank(20);
            }
        }

        #region Helpers

        private static Dictionary<HexCoords, BattleCellPersist> MakeBlank(int r)
        {
            Dictionary<HexCoords, BattleCellPersist> result
                = new Dictionary<HexCoords, BattleCellPersist>();

            foreach (HexCoords coords in HexCoords.O.CompoundRing(0, (uint)r))
                result[coords] = new BattleCellPersist();

            return result;
        }

        private static Dictionary<HexCoords, float> PerlinMap<T>(
            Dictionary<HexCoords, T> orig,
            float scaleX,
            float scaleY)
        {
            Dictionary<HexCoords, float> result = 
                new Dictionary<HexCoords, float>();

            PerlinState perlin = new PerlinState(scaleX, scaleY);

            foreach (HexCoords loc in orig.Keys)
                result[loc] = perlin.Sample(loc.x, loc.y);

            return result;
        }

        private static Dictionary<HexCoords, float> PerlinMap<T>(Dictionary<HexCoords, T> orig)
        {
            return PerlinMap(orig, 1.0f, 1.0f);
        }

        private static void WrapHex(
            Dictionary<HexCoords, BattleCellPersist> result,
            TerrainPersist type)
        {
            int minX = result.Keys.Min(q => q.x);
            int minY = result.Keys.Min(q => q.y);
            int minZ = result.Keys.Min(q => q.z);

            int maxX = result.Keys.Max(q => q.x);
            int maxY = result.Keys.Max(q => q.y);
            int maxZ = result.Keys.Max(q => q.z);

            int rangeX = maxX - minX + 1;
            int rangeY = maxY - minY + 1;

            foreach(int x in Enumerable.Range(minX, rangeX))
            {
                foreach(int y in Enumerable.Range(minY, rangeY))
                {
                    HexCoords coords = new HexCoords(x, y);
                    if (!result.ContainsKey(coords) && (coords.z >= minZ) && (coords.z <= maxZ))
                    {
                        result[coords] = new BattleCellPersist(type);
                    }
                }
            }
        }

        private static void PenDraw(
            Dictionary<HexCoords, BattleCellPersist> result,
            IEnumerable<HexCoords> locus,
            uint radius,
            TerrainPersist color,
            bool overwrite = true)
        {
            foreach (HexCoords loc in locus)
            {
                foreach (HexCoords penLoc in loc.CompoundRing(0, radius))
                {
                    if (overwrite || !result.ContainsKey(penLoc))
                    {
                        result[penLoc] = new BattleCellPersist(color);
                    }
                }
            }
        }

        #endregion

        private static Dictionary<HexCoords, BattleCellPersist> MakeGreenOpen()
        {
            Dictionary<HexCoords, BattleCellPersist> result = new Dictionary<HexCoords, BattleCellPersist>();
            
            IEnumerable<HexCoords> roadLine = Enumerable.Range(0, 40)
                .Select(x => HexCoords.O + (HexCoords.pE * x));

            PenDraw(result, roadLine, 1, TerrainPersist.greenRoad);
            PenDraw(result, roadLine.Skip(2).Take(36), 3, TerrainPersist.greenOpen, false);
            PenDraw(result, roadLine, 1, TerrainPersist.greenRoad);

            WrapHex(result, TerrainPersist.greenOpen);

            foreach(BattleCellPersist cell in result.Values)
                cell.elevation = Random.Range(0, 5);

            foreach (var kv in PerlinMap(result))
                result[kv.Key].elevation = (int)(kv.Value * 5);

            return result;
        }
    }
}
