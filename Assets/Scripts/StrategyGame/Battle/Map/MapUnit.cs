using Athanor.Pooling;
using GridLib.Pathing;
using GridLib.Hex;
using StrategyGame.Battle.Game.Abilities;
using StrategyGame.Battle.Game.Ai;
using StrategyGame.Battle.Persistence;
using StrategyGame.Battle.UI;
using StrategyGame.Game;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StrategyGame.Battle.Map
{
    public enum Team
    {
        player,
        enemy,
    }

    public class MapUnit : MonoBehaviour, IPoolable, ICanPath<HexCoords>
    {
        private HexCoords _loc = HexCoords.O;
        public HexCoords loc
        {
            get { return _loc; }
            set { _loc = value; }
        }

        public MapUnit()
        {
            persist = new BattleUnitPersist();
        }

        #region Shorthands

        GameController game { get { return GameController.instance; } }

        #endregion

        #region Statistics

        public Team team = Team.player;

        public int maxHp;
        private int _hp = 0;
        public int hp
        {
            get { return _hp; }
            set
            {
                _hp = value;
                if (_hp < 0) _hp = 0;
                UpdateHealth();
            }
        }

        public int maxAp;
        private int _ap = 0;
        public int ap
        {
            get { return _ap; }
            set
            {
                _ap = value;
                if (_ap < 0) _ap = 0;
                UpdateActions();
            }
        }

        public int speed = 0;
        public int jump = 0;

        #endregion

        #region Abilities

        private string[] abilityList;

        public IEnumerable<IUnitAbility> abilities
        {
            get
            {
                return GetComponentsInChildren<IUnitAbility>();
            }
        }

        #endregion

        #region AI

        private string aiName;

        public UnitAi ai { get { return GetComponentInChildren<UnitAi>(); } }

        #endregion

        #region Verbs

        public void TakeDamage(int damage)
        {
            hp -= damage;
            if (hp < 0) hp = 0;
            if (hp == 0) game.StartCoroutine(Die());
        }

        public IEnumerator Die()
        {
            // Sound effect or something?
            yield return null;

            map.UnplaceUnit(this);
            sticker.Return();
        }

        #endregion

        #region Appearance

        private bool awake = false;

        public ElementHolder healthContainer;
        public ElementHolder actionContainer;

        private void UpdateHealth()
        {
            if (awake)
            {
                healthContainer.Clear();

                if (hp > 0) foreach (int idx in Enumerable.Range(0, hp))
                {
                    BattleIcon newIcon = game.pools.battleIconPool.Provide<BattleIcon>();
                    newIcon.form = IconForm.heart;
                    healthContainer.Store(newIcon);
                }
            }
        }

        private void UpdateActions()
        {
            if (awake)
            {
                actionContainer.Clear();

                if (ap > 0) foreach (int idx in Enumerable.Range(0, ap))
                {
                    BattleIcon newIcon = game.pools.battleIconPool.Provide<BattleIcon>();
                    newIcon.form = IconForm.action;
                    actionContainer.Store(newIcon);
                }
            }
        }

        private void UpdateAppearance()
        {
            UpdateHealth();
            UpdateActions();
        }

        private string _spritePath = null;
        public string spritePath
        {
            get { return _spritePath; }
            set
            {
                _spritePath = value;
                ReloadSprite();
            }
        }

        private SpriteRenderer spriteRenderer { get { return GetComponent<SpriteRenderer>(); } }

        private void ReloadSprite()
        {
            if (awake)
            {
                if (spritePath == null)
                    spriteRenderer.sprite = null;
                else
                    spriteRenderer.sprite = Resources.Load<Sprite>(spritePath);
            }
        }

        #endregion

        #region Persistence

        public MapUnit(BattleUnitPersist persist)
        {
            this.persist = persist;
        }

        public BattleUnitPersist persist
        {
            get
            {
                BattleUnitPersist result = new BattleUnitPersist();

                result.spritePath = spritePath;

                result.team = team;

                result.maxHp = maxHp;
                result.hp = hp;

                result.maxAp = maxAp;
                result.ap = ap;

                result.speed = speed;
                result.jump = jump;

                result.abilityList = abilityList;

                return result;
            }

            set
            {
                spritePath = value.spritePath;

                team = value.team;

                maxHp = value.maxHp;
                hp = value.hp;

                maxAp = value.maxAp;
                ap = value.ap;

                speed = value.speed;
                jump = value.jump;

                abilityList = value.abilityList;
                foreach (string name in abilityList)
                {
                    GameObject ability = Instantiate(Resources.Load<GameObject>(name));
                    ability.transform.SetParent(transform, false);
                }

                aiName = value.ai;
                if (aiName != "")
                {
                    GameObject ai = Instantiate(Resources.Load<GameObject>(aiName));
                    ai.transform.SetParent(transform, false);
                }
            }
        }

        #endregion

        #region Poolable

        public PooledObject sticker { get { return GetComponent<PooledObject>(); } }

        public void OnProvide()
        {
        }

        public void OnReturn()
        {
            healthContainer.Clear();
            actionContainer.Clear();

            foreach (IUnitAbility ability in abilities.Where(x => x is MonoBehaviour))
                Destroy((ability as MonoBehaviour).gameObject);

            persist = new BattleUnitPersist();
        }

        #endregion

        #region Pathing

        private MapController map { get { return MapController.instance; } }

        public IEnumerable<HexCoords> ValidNeighbors(HexCoords loc)
        {
            return loc.neighbors.Where(map.InBounds);
        }

        public bool CanEnter(HexCoords loc)
        {
            if (map.MapCellAt(loc).canWalkThru)
                return map.MapCellAt(loc).unitPresent == null;
            else
                return false;
        }

        public bool CanStay(HexCoords loc)
        {
            if (map.MapCellAt(loc).canWalkThru)
                return map.MapCellAt(loc).unitPresent == null;
            else
                return false;
        }

        public bool CanLeave(HexCoords loc)
        {
            return true;
        }

        public int CostToEnter(HexCoords loc)
        {
            return 1;
        }

        public bool CanTraverse(HexCoords src, HexCoords dst)
        {
            int dElev = Mathf.RoundToInt(Mathf.Abs(map.MapCellAt(dst).elevation - map.MapCellAt(src).elevation));
            return dElev <= 1;
        }

        public int Heuristic(HexCoords src, HexCoords dst)
        {
            return src.DistanceTo(dst);
        }

        #endregion

        #region Monobehaviour

        void Awake()
        {
            awake = true;
            UpdateAppearance();
        }

        #endregion
    }
}
