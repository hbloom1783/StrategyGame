using Athanor.Collections;
using GridLib.Hex;
using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

namespace StrategyGame.Strategic.Map
{
    [Serializable]
    public class MapPiece
    {
        public Dictionary<HexCoords, CellType> content =
            new Dictionary<HexCoords, CellType>();

        public static MapPiece Generate()
        {
            return Generate(Random.Range(4, 6));
        }

        public static MapPiece Generate(int cellCount)
        {
            MapPiece newPiece = new MapPiece();

            foreach (int idx in Enumerable.Range(0, cellCount))
            {
                HexCoords newPoint = newPiece.content.Keys
                    .Frontier()
                    .Where(x => HexCoords.O.DistanceTo(x) <= 2)
                    .ToList()
                    .RandomPick();

                newPiece.content[newPoint] = CellType.greenPlain;
            }

            // Rearrange to center?

            return newPiece;
        }

        private IEnumerable<KeyValuePair<HexCoords, CellType>> OffsetAndRotate(HexCoords offset, HexRotation rot)
        {
            foreach (KeyValuePair<HexCoords, CellType> kv in content)
            {
                yield return new KeyValuePair<HexCoords, CellType>(
                    kv.Key.Rotate(rot) + offset,
                    kv.Value);
            }
        }

        public IDictionary<HexCoords, CellType> GetTargetArea(HexCoords offset, HexRotation rot)
        {
            return OffsetAndRotate(offset, rot).FormDictionary();
        }
    }
}
