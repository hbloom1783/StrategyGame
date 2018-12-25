using Athanor.Pooling;
using StrategyGame.Game.InputHandling;
using StrategyGame.Game.Persistence;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace StrategyGame.Game
{
    public enum SaveSlot
    {
        slot1,
        slot2,
        slot3,
    }

    public static class SaveSlotExt
    {
        public static string ToFileName(this SaveSlot slot)
        {
            switch (slot)
            {
                case SaveSlot.slot1: return "savedata1.dat";
                case SaveSlot.slot2: return "savedata2.dat";
                case SaveSlot.slot3: return "savedata3.dat";

                default: throw new ArgumentException("Invalid SaveSlot.");
            }
        }
    }

    public class GameController : MonoBehaviour
    {
        #region Super-singleton

        private static GameController _instance = null;
        public static GameController instance
        {
            get { return _instance; }
            private set { _instance = value; }
        }
        
        void OnEnable()
        {
            // Become the instance...
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            // ... or self-destruct.
            else
            {
                Destroy(gameObject);
            }
        }

        void OnDestroy()
        {
            // If we were the instance, clear the instance
            if (instance == this)
                instance = null;
        }

        #endregion

        #region Shorthands

        public StateMachine state = null;
        public PersistController persist = null;
        public GenericInputHandler input = null;
        public PoolContainer pools = null;

        #endregion

        #region Stage management

        public void LoadScene(string name)
        {
            if (name != SceneManager.GetActiveScene().name)
                FindObjectsOfType<PooledObject>()
                    .ToList()
                    .ForEach(x => x.Return());

            SceneManager.LoadScene(name);
        }

        public void Quit()
        {
            StartCoroutine(QuitGameState.Entry());
        }

        #endregion

        #region Save management

        // Saving the game is just a function
        public void SaveGame(SaveSlot slot = SaveSlot.slot1)
        {
            persist.SaveToFile(slot.ToFileName());
        }

        // Loading the game is a state change
        public void LoadGame(SaveSlot slot = SaveSlot.slot1)
        {
            StartCoroutine(LoadGameState.Entry());
        }

        public void NewGame()
        {
            StartCoroutine(NewGameState.Entry());
        }

        #endregion

        #region MonoBehaviour

        void Start()
        {
            string sceneName = SceneManager.GetActiveScene().name;
            if (sceneName == "Strategic")
            {
                //NewGame();
                LoadGame();
            }
            else if (sceneName == "Battle")
            {
                state.ChangeState(new Battle.Game.Create());
            }
            else state.ChangeState(new MainMenu.Game.Boot());
        }

        #endregion
    }
}
