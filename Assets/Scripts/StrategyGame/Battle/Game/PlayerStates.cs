﻿using Athanor.Colors;
using Athanor.StateMachine;
using Athanor.Tweening;
using GridLib.Hex;
using StrategyGame.Battle.Game.Abilities;
using StrategyGame.Battle.Map;
using StrategyGame.Battle.UI;
using StrategyGame.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace StrategyGame.Battle.Game.Player
{
    #region Abstracts

    public abstract class BattleShimmerState : BattleState
    {
        public float cycleTime = 0.5f;

        #region State implementation

        private Coroutine runningScript = null;

        public override void EnterState()
        {
            runningScript = game.StartCoroutine(ShimmerScript());
        }

        public override void LeaveState()
        {
            ClearSelected();
            ClearShimmer();

            if (runningScript != null)
            {
                game.StopCoroutine(ShimmerScript());
                runningScript = null;
            }
        }

        #endregion

        #region Shimmer mechanism

        private HashSet<MapCell> shimmerSet = new HashSet<MapCell>();

        private IEnumerator ShimmerScript()
        {
            while (true)
            {
                foreach (float t in TweeningExt.LinearTimeTween(cycleTime))
                {
                    foreach (MapCell cell in shimmerSet)
                        cell.shimmerT = t;

                    yield return null;
                }

                foreach (float t in TweeningExt.ReverseLinearTimeTween(cycleTime))
                {
                    foreach (MapCell cell in shimmerSet)
                        cell.shimmerT = t;

                    yield return null;
                }
            }
        }

        public void AddShimmer(MapCell cell, Color a, Color b)
        {
            if (runningScript != null)
            {
                cell.shimColorA = a;
                cell.shimColorB = b;
                if (!selectedSet.Contains(cell))
                    cell.state = HighlightState.shimmer;

                shimmerSet.Add(cell);
            }
        }

        public void AddShimmer(MapCell cell)
        {
            AddShimmer(
                cell,
                Color.white,
                Color.Lerp(Color.white, Color.black, 0.5f));
        }

        public void RemoveShimmer(MapCell cell)
        {
            if (!selectedSet.Contains(cell))
                cell.state = HighlightState.none;

            shimmerSet.Remove(cell);
        }

        public void ClearShimmer()
        {
            shimmerSet.ToList().ForEach(RemoveShimmer);
        }

        #endregion

        #region Selection mechanism

        protected static Color redTint = Color.Lerp(Color.red, Color.white, 0.25f);
        protected static Color yellowTint = Color.Lerp(Color.yellow, Color.white, 0.25f);
        protected static Color greenTint = Color.Lerp(Color.green, Color.white, 0.25f);
        protected static Color blueTint = Color.Lerp(Color.blue, Color.white, 0.25f);

        private HashSet<MapCell> selectedSet = new HashSet<MapCell>();

        public void AddSelected(MapCell cell, Color h)
        {
            if (runningScript != null)
            {
                cell.highColor = h;
                cell.state = HighlightState.steady;

                selectedSet.Add(cell);
            }
        }

        public void RemoveSelected(MapCell cell)
        {
            if (shimmerSet.Contains(cell))
                cell.state = HighlightState.shimmer;
            else
                cell.state = HighlightState.none;

            selectedSet.Remove(cell);
        }

        public void ClearSelected()
        {
            selectedSet.ToList().ForEach(RemoveSelected);
        }

        #endregion
    }

    #endregion

    class BeginTurn : BattleScript
    {
        private static readonly Color clearWhite = Color.white.withA(0.0f);
        private static readonly Color clearBlue = Color.blue.withA(0.0f);

        public override IEnumerator Script()
        {
            ui.marqueeText.shown = true;
            ui.marqueeText.text = "Player Turn";
            yield return ui.marqueeText.ColorTween(clearWhite, Color.blue, 1.0f);
            yield return ui.marqueeText.ColorTween(Color.blue, clearBlue, 1.0f);
            ui.marqueeText.shown = false;

            yield return game.state.SteadyChange(new SelectUnit());
        }
    }

    class SelectUnit : BattleShimmerState
    {
        private List<MapCell> selectableCells = null;

        #region State implementation

        public override void EnterState()
        {
            base.EnterState();

            game.input.uiSignal += UiSignal;

            ui.endTurnButton.shown = true;

            map.events.pointerClick += PointerClick;
            map.events.pointerEnter += PointerEnter;
            map.events.pointerExit += PointerExit;

            selectableCells = map.cells
                .Where(x => x.unitPresent != null)
                .Where(x => x.unitPresent.team == Team.player)
                .Where(x => x.unitPresent.ap > 0)
                .ToList();
            selectableCells.ForEach(AddShimmer);
            
            if (selectableCells.Contains(map.mouseCell))
                AddSelected(map.mouseCell, greenTint);

            ui.instructions.text = "Select a unit.";
        }

        public override void LeaveState()
        {
            ui.instructions.text = "";

            map.events.pointerClick -= PointerClick;
            map.events.pointerEnter -= PointerEnter;
            map.events.pointerExit -= PointerExit;

            ui.endTurnButton.shown = false;

            game.input.uiSignal -= UiSignal;

            base.LeaveState();
        }

        #endregion

        #region Event handlers

        private void UiSignal(UiElement element)
        {
            if (element == ui.endTurnButton)
            {
                game.state.ChangeState(new EndTurn());
            }
            else
            {
                Debug.Log("Received UI signal from unknown element " + element.name);
            }
        }

        private void PointerClick(PointerEventData eventData, GameObject child)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                MapCell cell = child.GetComponent<MapCell>();

                if (selectableCells.Contains(cell))
                {
                    game.state.ChangeState(new SelectAbility { unit = cell.unitPresent });
                }
            }
        }

        private void PointerEnter(PointerEventData eventData, GameObject child)
        {
            MapCell cell = child.GetComponent<MapCell>();

            if (selectableCells.Contains(cell))
            {
                AddSelected(cell, greenTint);
            }
        }

        private void PointerExit(PointerEventData eventData, GameObject child)
        {
            MapCell cell = child.GetComponent<MapCell>();

            if (selectableCells.Contains(cell))
            {
                RemoveSelected(cell);
            }
        }

        #endregion
    }

    class SelectAbility : BattleState
    {
        public MapUnit unit = null;

        #region State implementation

        public override void EnterState()
        {
            ui.instructions.text = "Select an ability.";

            game.input.mouseDown += MouseDown;
            game.input.uiSignal += UiSignal;

            foreach(IUnitAbility ability in unit.abilities)
            {
                AbilityButton newButton = game.pools.battleAbilityButtonPool.Provide<AbilityButton>();
                newButton.ability = ability;
                newButton.interactable = ability.canPayCost && ability.CanTarget();
                ui.abilityBar.Store(newButton);
            }
        }

        public override void LeaveState()
        {
            ui.instructions.text = "";

            game.input.mouseDown -= MouseDown;

            ui.abilityBar.Clear();
        }

        #endregion

        #region Event handlers

        private void MouseDown(PointerEventData.InputButton mb)
        {
            if (mb == PointerEventData.InputButton.Right)
            {
                game.state.ChangeState(new SelectUnit());
            }
        }

        private void UiSignal(UiElement element)
        {
            if (element is AbilityButton)
            {
                IUnitAbility ability = (element as AbilityButton).ability;

                game.state.ChangeState(new SelectTargets { ability = ability });
            }
            else
            {
                Debug.Log("Received signal from unknown element " + element.name);
            }
        }

        #endregion
    }

    class SelectTargets : BattleShimmerState
    {
        public IUnitAbility ability = null;

        private List<MapCell> selectableCells = null;

        #region State implementation

        public override void EnterState()
        {
            base.EnterState();

            game.input.mouseDown += MouseDown;
            game.input.uiSignal += UiSignal;

            map.events.pointerClick += PointerClick;
            map.events.pointerEnter += PointerEnter;
            map.events.pointerExit += PointerExit;

            ResetShimmer();

            UpdateInstructions();
        }

        public override void LeaveState()
        {
            ui.instructions.text = "";

            game.input.mouseDown -= MouseDown;

            map.events.pointerClick -= PointerClick;
            map.events.pointerEnter -= PointerEnter;
            map.events.pointerExit -= PointerExit;

            base.LeaveState();
        }

        #endregion

        #region Helper functions

        private void UpdateInstructions()
        {
            if (ability.targetsMax == 0)
                ui.instructions.text = "Press confirm.";
            else if ((ability.targetsMin == 1) && (ability.targetsMax == 1))
                ui.instructions.text = "Select a target.";
            else
            {
                if (ability.HasMaxTargets())
                {
                    ui.instructions.text = "Press confirm.";
                }
                else if (ability.HasMinTargets())
                {
                    if (ability.targetsHeld > 0)
                        ui.instructions.text = "Press confirm or select another target.";
                    else
                        ui.instructions.text = "Select a target or press confirm.";
                }
                else
                {
                    if (ability.targetsHeld > 0)
                        ui.instructions.text = "Select another target.";
                    else
                        ui.instructions.text = "Select a target.";
                }
            }
        }

        private void ResetSelection()
        {
            ClearSelected();
            
            if (ability.HasMaxTargets())
            {
                ability.GetCoveredArea()
                    .ToList()
                    .ForEach(x => AddSelected(map[x], blueTint));
            }
            else
            {
                if (selectableCells.Contains(map.mouseCell))
                    ability.GetAoE(map.mousePosition)
                        .Select(map.CellAt)
                        .ToList()
                        .ForEach(x => AddSelected(x, blueTint));
            }
        }

        private void ResetShimmer()
        {
            ClearShimmer();

            selectableCells = ability.GetRange()
                .Where(map.InBounds)
                .Select(map.CellAt)
                .ToList();

            if (!ability.HasMaxTargets())
            {
                selectableCells.ForEach(AddShimmer);
            }
        }

        #endregion

        #region Event handlers

        private void MouseDown(PointerEventData.InputButton mb)
        {
            if (mb == PointerEventData.InputButton.Right)
            {
                // If we can deselect a target, do so
                if (ability.targetsHeld > 0)
                {
                    ability.DeselectTarget();
                    UpdateInstructions();

                    if (ability.HasMinTargets())
                    {
                        // Enable "confirm" button
                        ui.confirmButton.shown = true;
                    }
                    else
                    {
                        // Disable "confirm" button
                        ui.confirmButton.shown = false;
                    }

                    ResetShimmer();
                    ResetSelection();
                }
                // Otherwise, fall back to ability selection
                else
                {
                    game.state.ChangeState(new SelectAbility { unit = ability.unit });
                }
            }
        }

        private void UiSignal(UiElement element)
        {
            if (element == ui.confirmButton)
            {
                game.state.ChangeState(new ExecuteAbility { ability = ability });
            }
            else
            {
                Debug.Log("Recieved signal from unknown UI element " + element.name);
            }
        }

        private void PointerClick(PointerEventData eventData, GameObject child)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                MapCell cell = child.GetComponent<MapCell>();
                if (selectableCells.Contains(cell))
                {
                    ability.SelectTarget(cell.loc);
                    UpdateInstructions();

                    if (ability.HasMinTargets())
                    {
                        // If this ability is single-targeted, just fire it
                        if ((ability.targetsMax == 1) && (ability.targetsMin == 1))
                        {
                            game.state.ChangeState(new ExecuteAbility { ability = ability });
                        }
                        else
                        {
                            // Enable "confirm" button
                            ui.confirmButton.shown = true;
                        }
                    }
                    else
                    {
                        // Disable "confirm" button
                        ui.confirmButton.shown = false;
                    }

                    ResetShimmer();
                    ResetSelection();
                }
            }
        }

        private void PointerEnter(PointerEventData eventData, GameObject child)
        {
            ResetSelection();
        }

        private void PointerExit(PointerEventData eventData, GameObject child)
        {
            ResetSelection();
        }

        #endregion
    }

    class ExecuteAbility : BattleScript
    {
        public IUnitAbility ability = null;

        public override IEnumerator Script()
        {
            ability.PayCost();

            yield return ability.Execute();
            ability.ResetInternalState();

            if (ability.unit.ap > 0)
                yield return game.state.SteadyChange(new SelectAbility
                {
                    unit = ability.unit,
                });
            else
                yield return game.state.SteadyChange(new SelectUnit());
        }
    }

    class EndTurn : BattleScript
    {
        public override IEnumerator Script()
        {
            map.units
                .Where(x => x.team == Team.player)
                .ToList()
                .ForEach(x => x.ap = x.maxAp);

            yield return game.state.SteadyChange(new NextTurn());
        }
    }
}
