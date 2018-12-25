using Athanor.Colors;
using System.Collections;
using UnityEngine;

namespace StrategyGame.Battle.Game.Enemy
{
    class BeginTurn : BattleScript
    {
        private static readonly Color clearWhite = Color.white.withA(0.0f);
        private static readonly Color clearRed = Color.red.withA(0.0f);

        public override IEnumerator Script()
        {
            ui.marqueeText.shown = true;
            ui.marqueeText.text = "Enemy Turn";
            yield return ui.marqueeText.ColorTween(clearWhite, Color.red, 1.0f);
            yield return ui.marqueeText.ColorTween(Color.red, clearRed, 1.0f);
            ui.marqueeText.shown = false;

            yield return game.state.SteadyChange(new NextTurn());
        }
    }
}
