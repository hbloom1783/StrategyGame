using StrategyGame.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace StrategyGame.Game.InputHandling
{
    public class GenericInputHandler : MonoBehaviour
    {
        public delegate void KeyCodeEvent(KeyCode kc);
        public delegate void MouseButtonEvent(PointerEventData.InputButton mb);
        public delegate void UiElementEvent(UiElement element);

        public event KeyCodeEvent keyUp = null;
        public event KeyCodeEvent keyDown = null;

        public event MouseButtonEvent mouseUp = null;
        public event MouseButtonEvent mouseDown = null;

        public event UiElementEvent uiSignal = null;

        public void ClearEvents()
        {
            keyDown = null;
            keyUp = null;

            mouseDown = null;
            mouseUp = null;

            uiSignal = null;
        }

        private PointerEventData.InputButton IdentifyButton(int mb)
        {
            switch (mb)
            {
                default:
                case 0: return PointerEventData.InputButton.Left;
                case 1: return PointerEventData.InputButton.Right;
                case 2: return PointerEventData.InputButton.Middle;
            }
        }

        void OnGUI()
        {
            Event e = Event.current;

            if (e.isKey)
            {
                if (e.keyCode != KeyCode.None)
                {
                    if ((e.type == EventType.keyDown) && (keyDown != null))
                        keyDown(e.keyCode);
                    else if ((e.type == EventType.keyUp) && (keyUp != null))
                        keyUp(e.keyCode);
                }
            }
            else if (e.isMouse)
            {
                if ((e.type == EventType.mouseDown) && (mouseDown != null))
                    mouseDown(IdentifyButton(e.button));
                if ((e.type == EventType.mouseUp) && (mouseUp != null))
                    mouseUp(IdentifyButton(e.button));
            }
        }

        public void ReceiveUISignal(UiElement element)
        {
            if (uiSignal != null)
                uiSignal(element);
        }

        private Rect screenRect { get { return new Rect(0, 0, Screen.width, Screen.height); } }

        public bool isMouseOnScreen { get { return screenRect.Contains(Input.mousePosition); } }

        public bool isMouseOverSomething { get { return EventSystem.current.currentSelectedGameObject == null; } }
    }
}
