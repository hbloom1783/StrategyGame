using Athanor.StateMachine;
using StrategyGame.Game;
using StrategyGame.Strategic.UI;
using System.Collections;
using UnityEngine;
using StrategyGame.UI;
using StrategyGame.Game.Persistence;
using StrategyGame.Strategic.Map;
using GridLib.Hex;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;
using GridLib.Pathing;
using Athanor.Tweening;

namespace StrategyGame.Strategic.Game
{
    #region Abstracts

    public abstract class StrategicState : IStateMachineState
    {
        public string name { get { return "Strategic::" + GetType().Name; } }

        protected GameController game { get { return GameController.instance; } }
        protected MapController map { get { return MapController.instance; } }
        protected StrategicUi ui { get { return StrategicUi.instance; } }

        public abstract void EnterState();
        public abstract void LeaveState();
    }

    public abstract class StrategicScript : StrategicState
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

    public class Boot : StrategicScript
    {
        public override IEnumerator Script()
        {
            game.LoadScene("Strategic");
            yield return game.state.waitForSteady;
            game.persist.data.scene = SavedScene.strategic;

            // Load persistent state here
            ui.pieceMenu.LoadFromPersistence();
            map.LoadFromPersistence();

            game.state.ChangeState(new Idle());
        }
    }

    public class Idle : StrategicState
    {
        private MapUnit selectedUnit;

        #region State implementation

        public override void EnterState()
        {
            game.input.keyDown += KeyDown;
            game.input.uiSignal += UiSignal;

            map.events.pointerClick += PointerClick;

            ui.systemMenuButton.shown = true;
            ui.pieceMenuButton.shown = true;
            ui.battleButton.shown = true;
        }

        public override void LeaveState()
        {
            game.input.keyDown -= KeyDown;
            game.input.uiSignal -= UiSignal;

            map.events.pointerClick -= PointerClick;

            ui.pieceMenu.shown = false;
            ui.infoWindow.shown = false;

            ui.systemMenuButton.shown = false;
            ui.pieceMenuButton.shown = false;
            ui.battleButton.shown = false;
        }

        #endregion

        #region Event handling
        
        private void UiSignal(UiElement element)
        {
            if (element == ui.systemMenuButton)
                game.state.ChangeState(new SystemMenu());
            else if (element == ui.pieceMenuButton)
                ui.pieceMenu.ToggleShown();
            else if (element == ui.infoWindow.closeButton)
                ui.infoWindow.shown = false;
            else if (element == ui.infoWindow.moveButton)
                game.state.ChangeState(new MovingUnit { unit = selectedUnit });
            else if (element == ui.battleButton)
                game.state.ChangeState(new Battle.Game.Create());
            else if (element is PieceButton)
            {
                PieceButton pieceButton = element as PieceButton;
                game.state.ChangeState(new PlacingPiece { selectedPiece = pieceButton.piece });
            }
            else
                Debug.Log("Received signal from unknown UI Element " + element.name);
        }

        private void KeyDown(KeyCode kc)
        {
            switch(kc)
            {
                case KeyCode.Equals:
                    game.persist.data.strategic.mapPieces.Add(MapPiece.Generate());
                    ui.pieceMenu.LoadFromPersistence();
                    break;
            }
        }

        private void PointerClick(PointerEventData eventData, GameObject child)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                MapCell cell = child.GetComponent<MapCell>();

                ui.infoWindow.shown = true;
                ui.infoWindow.title = "Info for " + cell.gridCell.loc;
                ui.infoWindow.body = "Terrain is " + cell.type;
                if (cell.unitPresent == null)
                {
                    ui.infoWindow.body += "\nNo unit present.";
                    ui.infoWindow.moveButton.shown = false;
                }
                else
                {
                    ui.infoWindow.body += "\nUnit present: " + cell.unitPresent.name;
                    ui.infoWindow.moveButton.shown = true;
                    selectedUnit = cell.unitPresent;
                }
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                ui.infoWindow.shown = false;
            }
        }

        #endregion
    }

    public class SystemMenu : StrategicState
    {
        #region State implementation

        public override void EnterState()
        {
            ui.systemMenu.shown = true;

            game.input.uiSignal += UiSignal;
        }

        public override void LeaveState()
        {
            ui.systemMenu.shown = false;

            game.input.uiSignal -= UiSignal;
        }

        #endregion

        #region Event handling

        private void UiSignal(UiElement element)
        {
            if (element == ui.systemMenu.resumeGameButton)
                game.state.ChangeState(new Idle());
            else if (element == ui.systemMenu.saveGameButton)
                game.SaveGame();
            else if (element == ui.systemMenu.loadGameButton)
                game.LoadGame();
            else if (element == ui.systemMenu.quitToMenuButton)
                game.state.ChangeState(new MainMenu.Game.Boot());
            else
                Debug.Log("Received signal from unknown UI Element " + element.name);
        }

        #endregion
    }

    public class PlacingPiece : StrategicState
    {
        public MapPiece selectedPiece;
        private HexCoords center = HexCoords.O;
        private HexRotation rot = HexRotation.Neutral;

        #region State implementation

        private Coroutine runningScript = null;

        public override void EnterState()
        {
            map.events.pointerEnter += PointerEnter;
            map.events.pointerExit += PointerExit;
            map.events.pointerClick += PointerClick;

            game.input.keyDown += KeyDown;
            game.input.mouseDown += MouseDown;

            runningScript = game.StartCoroutine(Script());
        }

        public override void LeaveState()
        {
            map.events.pointerEnter -= PointerEnter;
            map.events.pointerExit -= PointerExit;
            map.events.pointerClick -= PointerClick;

            game.input.keyDown -= KeyDown;
            game.input.mouseDown -= MouseDown;

            if (runningScript != null) game.StopCoroutine(runningScript);

            UnmarkPiece(center);
            map.ClearEmpties();
        }

        #endregion

        #region Coroutine

        private IEnumerator Script()
        {
            while (true)
            {
                if (game.input.isMouseOnScreen && !map.InBounds(map.mousePosition))
                    map.InitCell(map.mousePosition);
                yield return null;
            }
        }

        #endregion

        #region Piece marking

        private bool OffsetValid(HexCoords offset)
        {
            if (offset == null) return false;
            else
            {
                IEnumerable<HexCoords> targetArea =
                    selectedPiece.GetTargetArea(offset, rot).Keys;

                IEnumerable<HexCoords> frontier =
                    targetArea.Frontier();

                targetArea.Union(frontier)
                    .Where(map.OutOfBounds)
                    .ToList()
                    .ForEach(x => map.InitCell(x));

                // Piece overlaps filled cells
                if (targetArea.Select(map.CellAt).Any(x => x.MapCell().isFilled))
                    return false;
                // Piece frontier overlaps filled cells
                else if (targetArea.Frontier().Select(map.CellAt).Any(x => x.MapCell().isFilled))
                    return true;
                // No overlaps
                else
                    return false;
            }
        }

        private void MarkPiece(HexCoords offset)
        {
            if (offset != null)
            {
                bool valid = OffsetValid(offset);

                IDictionary<HexCoords, CellType> targetArea =
                    selectedPiece.GetTargetArea(offset, rot);

                foreach (var kv in targetArea)
                {
                    if (!map.InBounds(kv.Key)) map.InitCell(kv.Key);

                    map.MapCellAt(kv.Key).overlayType = kv.Value;
                    if (!valid) map.MapCellAt(kv.Key).highlight = Highlight.invalid;
                }
            }
        }

        private void UnmarkPiece(HexCoords offset)
        {
            if (offset != null)
            {
                IEnumerable<HexCoords> targetArea =
                    selectedPiece.GetTargetArea(offset, rot).Keys;

                foreach (HexCoords loc in targetArea)
                {
                    map.MapCellAt(loc).overlayType = CellType.empty;
                    map.MapCellAt(loc).highlight = Highlight.neutral;
                }
            }
        }

        #endregion

        #region Event handlers

        private void PointerEnter(PointerEventData eventData, GameObject child)
        {
            center = child.GetComponent<MapCell>().gridCell.loc;
            MarkPiece(center);
        }

        private void PointerExit(PointerEventData eventData, GameObject child)
        {
            UnmarkPiece(center);
            center = null;
        }

        private void PointerClick(PointerEventData eventData, GameObject child)
        {
            if (OffsetValid(center))
            {
                IDictionary<HexCoords, CellType> targetArea =
                    selectedPiece.GetTargetArea(center, rot);

                // Update map
                foreach (var kv in targetArea)
                    map.MapCellAt(kv.Key).type = kv.Value;

                // Update piece menu
                map.SaveToPersistence();

                game.persist.data.strategic.mapPieces.Remove(selectedPiece);
                ui.pieceMenu.LoadFromPersistence();

                // Return to idle
                game.state.ChangeState(new Idle());
            }
        }

        private void MouseDown(PointerEventData.InputButton mb)
        {
            if (mb == PointerEventData.InputButton.Right)
                game.state.ChangeState(new Idle());
        }

        private void KeyDown(KeyCode kc)
        {
            if (kc == KeyCode.Comma)
            {
                UnmarkPiece(center);
                rot.CWcount -= 1;
                MarkPiece(center);
            }
            else if (kc == KeyCode.Period)
            {
                UnmarkPiece(center);
                rot.CWcount += 1;
                MarkPiece(center);
            }
        }

        #endregion
    }

    public class MovingUnit : StrategicState
    {
        public MapUnit unit;

        #region State implementation

        public override void EnterState()
        {
            map.events.pointerClick += PointerClick;

            game.input.mouseDown += MouseDown;
        }

        public override void LeaveState()
        {
            map.events.pointerClick -= PointerClick;

            game.input.mouseDown -= MouseDown;
        }

        #endregion

        #region Event handlers
        
        private void PointerClick(PointerEventData eventData, GameObject child)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                MapCell cell = child.GetComponent<MapCell>();
                game.state.ChangeState(new UnitIsMoving
                {
                    unit = unit,
                    path = unit.AStar(cell.gridCell.loc),
                });
            }
        }

        private void MouseDown(PointerEventData.InputButton mb)
        {
            if (mb == PointerEventData.InputButton.Right)
                game.state.ChangeState(new Idle());
        }

        #endregion
    }

    public class UnitIsMoving : StrategicScript
    {
        public MapUnit unit;
        public IEnumerable<HexCoords> path;

        public override IEnumerator Script()
        {
            foreach (HexCoords newLoc in path)
            {
                // animate
                yield return unit.transform.LinearTween(map.GridToWorld(newLoc), 0.25f);

                // re-place
                map.UnplaceUnit(unit);
                map.PlaceUnit(unit, newLoc);

                // update fog of war

                // save map
                map.SaveToPersistence();

                // check for random encounters
            }

            // move ends
            yield return game.state.SteadyChange(new Idle());
        }
    }
}
