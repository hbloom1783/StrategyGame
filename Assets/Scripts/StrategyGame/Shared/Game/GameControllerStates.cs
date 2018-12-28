using Athanor.StateMachine;
using StrategyGame.Game.Persistence;
using System.Collections;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StrategyGame.Game
{
    #region Abstracts

    abstract class GameControllerState : IStateMachineState
    {
        public string name { get { return "GameController::" + ToString(); } }

        protected static GameController game { get { return GameController.instance; } }

        public abstract void EnterState();
        public abstract void LeaveState();
    }

    abstract class GameControllerScript : GameControllerState
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

    class NewGameState : GameControllerScript
    {
        public static IEnumerator Entry()
        {
            yield return game.state.SteadyChange(new NewGameState());
        }

        public override IEnumerator Script()
        {
            yield return game.state.waitForSteady;

            game.persist.data = PersistData.newGame;
            game.state.ChangeState(new Strategic.Game.Boot());
        }
    }

    class LoadGameState : GameControllerScript
    {
        public static IEnumerator Entry()
        {
            yield return game.state.SteadyChange(new LoadGameState());
        }

        private SaveSlot slot = SaveSlot.slot1;

        public override IEnumerator Script()
        {
            yield return game.state.waitForSteady;

            game.persist.LoadFromFile(slot.ToFileName());

            switch (game.persist.data.scene)
            {
                case SavedScene.battle:
                    game.state.ChangeState(new Battle.Game.Boot());
                    break;

                case SavedScene.strategic:
                    game.state.ChangeState(new Strategic.Game.Boot());
                    break;

                default:
                case SavedScene.invalid:
                    Debug.Log("Loaded game with invalid scene!");
                    game.state.ChangeState(new MainMenu.Game.Boot());
                    break;
            }
        }
    }

    class QuitGameState : GameControllerScript
    {
        public static IEnumerator Entry()
        {
            yield return game.state.SteadyChange(new QuitGameState());
        }

        public override IEnumerator Script()
        {

            Debug.Log("Quitting...");

#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif

            yield return game.state.SteadyChange(null);
        }
    }
}
