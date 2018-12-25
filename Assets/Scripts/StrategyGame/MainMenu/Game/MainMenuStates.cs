using System.Collections;
using UnityEngine;
using Athanor.StateMachine;
using StrategyGame.Game;
using StrategyGame.MainMenu.UI;
using StrategyGame.UI;

namespace StrategyGame.MainMenu.Game
{
    #region Abstracts

    public abstract class MainMenuState : IStateMachineState
    {
        public string name { get { return "MainMenu::" + GetType().Name; } }

        protected GameController game { get { return GameController.instance; } }
        protected MainMenuUi ui { get { return MainMenuUi.instance; } }

        public abstract void EnterState();
        public abstract void LeaveState();
    }

    public abstract class MainMenuScript : MainMenuState
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

    public class Boot : MainMenuScript
    {
        public override IEnumerator Script()
        {
            game.LoadScene("Main Menu");

            yield return game.state.SteadyChange(new Idle());
        }
    }

    public class Idle : MainMenuState
    {
        #region State implementation

        public override void EnterState()
        {
            game.input.uiSignal += UiSignal;
        }

        public override void LeaveState()
        {
            game.input.uiSignal -= UiSignal;
        }

        #endregion

        #region Event handling

        private void UiSignal(UiElement element)
        {
            if (element == ui.newGameButton)
            {
                game.NewGame();
            }
            else if (element == ui.loadGameButton)
            {
                game.LoadGame();
            }
            else if (element == ui.settingsButton)
            {
                // TODO: Settings menu
            }
            else if (element == ui.quitButton)
            {
                game.Quit();
            }
            else
            {
                Debug.Log("Received signal from unknown UiElement " + element.name);
            }
        }

        #endregion
    }
}
