using StrategyGame.Strategic.Map;
using StrategyGame.UI;
using System.Collections.Generic;
using UnityEngine;

namespace StrategyGame.Strategic.UI
{
    public class PieceMenu : UiElement
    {
        public Transform pieceButtonParent = null;
        List<PieceButton> pieceButtons = new List<PieceButton>();

        public void LoadFromPersistence()
        {
            pieceButtons.ForEach(game.pools.strategicPieceButtonPool.Return);
            pieceButtons.Clear();

            foreach (MapPiece piece in game.persist.data.strategic.mapPieces)
            {
                PieceButton newButton = game.pools.strategicPieceButtonPool.Provide<PieceButton>();

                newButton.piece = piece;
                newButton.transform.SetParent(pieceButtonParent, false);

                pieceButtons.Add(newButton);
            }

            Canvas.ForceUpdateCanvases();
        }
    }
}
