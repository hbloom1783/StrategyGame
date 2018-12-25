using GridLib.Hex;
using StrategyGame.Strategic.Map;
using System;
using System.Collections.Generic;

namespace StrategyGame.Strategic.Persistence
{

    [Serializable]
    public class StrategicUnitPersist
    {
    }

    [Serializable]
    public class StrategicCellPersist
    {
        public CellType type = CellType.empty;
        public StrategicUnitPersist unitPresent = null;
    }

    [Serializable]
    public class StrategicPersist
    {
        public List<MapPiece> mapPieces = new List<MapPiece>();
        public Dictionary<HexCoords, StrategicCellPersist> mapContents =
            new Dictionary<HexCoords, StrategicCellPersist>();

        #region New game

        private readonly static StrategicCellPersist crashSite =
            new StrategicCellPersist { type = CellType.moonCrashSite };

        private readonly static StrategicCellPersist lunarSurface =
            new StrategicCellPersist { type = CellType.moonPlain };

        public static StrategicPersist newGame
        {
            get
            {
                StrategicPersist result = new StrategicPersist();

                // Start with one map piece
                result.mapPieces.Add(MapPiece.Generate());

                // Start with a crash site at 0,0 and a ring of lunar surface around that
                result.mapContents[HexCoords.O] = crashSite;
                foreach (HexCoords loc in HexCoords.O.neighbors)
                    result.mapContents[loc] = lunarSurface;

                // Start with a unit at 0,0
                result.mapContents[HexCoords.O].unitPresent = new StrategicUnitPersist();

                return result;
            }
        }

        #endregion
    }
}
