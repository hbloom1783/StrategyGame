using Athanor.StateMachine;
using StrategyGame.Battle.Game.Abilities;
using StrategyGame.Battle.Map;
using StrategyGame.Battle.Persistence;
using StrategyGame.Battle.UI;
using StrategyGame.Game;
using StrategyGame.Game.Persistence;
using System.Collections;
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

            switch (game.persist.data.battle.turn)
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
            switch (game.persist.data.battle.turn)
            {
                case Team.enemy:
                    game.persist.data.battle.turn = Team.player;
                    yield return game.state.SteadyChange(new Player.BeginTurn());
                    break;

                case Team.player:
                    game.persist.data.battle.turn = Team.enemy;
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
