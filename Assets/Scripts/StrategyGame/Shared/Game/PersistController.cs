using StrategyGame.Battle.Persistence;
using StrategyGame.Game.Persistence.SerializationExt;
using StrategyGame.Strategic.Persistence;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace StrategyGame.Game.Persistence
{
    public enum SavedScene
    {
        strategic,
        battle,
        invalid,
    }

    namespace SerializationExt
    {
        static class SerializationExt
        {
            public static void Write<T>(this SerializationInfo info, string name, T value)
            {
                info.AddValue(name, value, typeof(T));
            }

            public static T Read<T>(this SerializationInfo info, string name)
            {
                return (T)info.GetValue(name, typeof(T));
            }
        }
    }

    [Serializable]
    public class PersistData : ISerializable
    {
        #region Data

        public PersistData()
        {
            // Blank state
        }

        public SavedScene scene = SavedScene.invalid;
        public StrategicPersist strategic = new StrategicPersist();
        public BattlePersist battle = new BattlePersist();

        #endregion

        #region Serialization

        public PersistData(SerializationInfo info, StreamingContext context)
        {
            scene = info.Read<SavedScene>("scene");

            switch(scene)
            {
                default:
                case SavedScene.invalid:
                    break;

                case SavedScene.battle:
                    battle = info.Read<BattlePersist>("battle");
                    goto case SavedScene.strategic;

                case SavedScene.strategic:
                    strategic = info.Read<StrategicPersist>("strategic");
                    break;
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.Write("scene", scene);

            switch (scene)
            {
                default:
                case SavedScene.invalid:
                    break;

                case SavedScene.battle:
                    info.Write("battle", battle);
                    goto case SavedScene.strategic;

                case SavedScene.strategic:
                    info.Write("strategic", strategic);
                    break;
            }
        }

        #endregion

        #region New game

        public static PersistData newGame
        {
            get
            {
                PersistData result = new PersistData();

                // Set up new game here.
                result.strategic = StrategicPersist.newGame;

                return result;
            }
        }

        public static object HexCooords { get; private set; }

        #endregion
    }

    public class PersistController : MonoBehaviour
    {
        public PersistData data = new PersistData();

        private IFormatter formatter = new BinaryFormatter();

        public void SaveToFile(string fileName)
        {
            FileStream file = null;

            try
            {
                file = File.Open(fileName, FileMode.Create);
                formatter.Serialize(file, data);
            }
            finally
            {
                if (file != null) file.Close();
            }
        }

        public void LoadFromFile(string fileName)
        {
            FileStream file = null;

            try
            {
                file = File.Open(fileName, FileMode.Open);
                data = (PersistData)formatter.Deserialize(file);
            }
            catch (TypeLoadException)
            {
                Debug.Log("Load failed, saved game unusuable.");
            }
            finally
            {
                if (file != null) file.Close();
            }
        }
    }
}
