using StrategyGame.Battle.Persistence;
using UnityEngine;

namespace StrategyGame.Battle.Map
{
    [CreateAssetMenu(fileName = "Unit", menuName = "Athanor/Battle/Map Unit Recipe", order = 1)]
    public class MapUnitRecipe : ScriptableObject
    {
        public string spritePath = "";
        public Team team = Team.player;
        public int maxHp = 3;
        public int maxAp = 2;
        public int speed = 5;
        public int jump = 2;

        public string[] abilityList = new string[0];

        public BattleUnitPersist persist
        {
            get
            {
                return new BattleUnitPersist
                {
                    spritePath = spritePath,

                    team = team,

                    maxHp = maxHp,
                    hp = maxHp,

                    maxAp = maxAp,
                    ap = maxAp,

                    speed = speed,
                    jump = jump,

                    abilityList = abilityList,
                };
            }
        }
    }
}
