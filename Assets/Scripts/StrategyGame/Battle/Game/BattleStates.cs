using Athanor.Collections;
using Athanor.StateMachine;
using GridLib.Hex;
using StrategyGame.Battle.Map;
using StrategyGame.Battle.Persistence;
using StrategyGame.Battle.UI;
using StrategyGame.Game;
using StrategyGame.Game.Persistence;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StrategyGame.Battle.Game
{
    #region Abstracts

    public abstract class BattleState : IStateMachineState
    {
        public string name { get { return "Battle::" + GetType().Name; } }

        protected GameController game { get { return GameController.instance; } }
        protected MapController map { get { return MapController.instance; } }
        protected BattleUi ui { get { return BattleUi.instance; } }

        protected IEnumerable<MapUnit> playerUnits
        {
            get { return map.units.Where(x => x.team == Team.player); }
        }

        protected IEnumerable<MapUnit> enemyUnits
        {
            get { return map.units.Where(x => x.team == Team.enemy); }
        }

        protected IEnumerable<MapUnit> TeamUnits(Team team)
        {
            return map.units.Where(x => x.team == team);
        }

        public abstract void EnterState();
        public abstract void LeaveState();
    }

    public abstract class BattleScript : BattleState
    {
        Coroutine runningScript = null;

        public override void EnterState()
        {
            runningScript = game.StartCoroutine(Script());
        }

        public override void LeaveState()
        {
            if (runningScript != null)
                game.StopCoroutine(runningScript);
        }

        public abstract IEnumerator Script();
    }

    #endregion

    public class Create : BattleScript
    {
        public override IEnumerator Script()
        {
            // Set up new battle state here
            game.persist.data.battle = BattlePersist.Generate();

            yield return game.state.SteadyChange(new Boot());
        }
    }

    public class Boot : BattleScript
    {
        public override IEnumerator Script()
        {
            game.LoadScene("Battle");
            yield return game.state.waitForSteady;
            game.persist.data.scene = SavedScene.battle;

            // Load persistent state here
            map.LoadFromPersistence();

            // Arrange camera
            Extremes extremes = map.coords.FindExtremes();
            Vector3 minCorner = map.GridToWorld(new HexCoords(extremes.minX, extremes.minY));
            Vector3 maxCorner = map.GridToWorld(new HexCoords(extremes.maxX, extremes.maxY));
            ui.smoothCam.posBounds = Rect.MinMaxRect(
                minCorner.x, minCorner.y,
                maxCorner.x, maxCorner.y);
            ui.smoothCam.pos = new[] { minCorner, maxCorner }.Mean();
            ui.smoothCam.size = 5.0f;

            switch (game.persist.data.battle.whoseTurn)
            {
                case Team.player:
                    game.state.ChangeState(new Player.BeginTurn());
                    break;

                case Team.enemy:
                    game.state.ChangeState(new Enemy.BeginTurn());
                    break;

                default:
                    Debug.Log("Can't identify whose turn it is.");
                    game.state.ChangeState(new MainMenu.Game.Boot());
                    break;
            }
        }
    }

    public class NextTurn : BattleScript
    {
        public override IEnumerator Script()
        {
            switch (game.persist.data.battle.whoseTurn)
            {
                case Team.enemy:
                    game.persist.data.battle.whoseTurn = Team.player;
                    yield return game.state.SteadyChange(new Player.BeginTurn());
                    break;

                case Team.player:
                    game.persist.data.battle.whoseTurn = Team.enemy;
                    yield return game.state.SteadyChange(new Enemy.BeginTurn());
                    break;

                default:
                    Debug.Log("Can't identify whose turn it is.");
                    yield return game.state.SteadyChange(new MainMenu.Game.Boot());
                    break;
            }
        }
    }
}
