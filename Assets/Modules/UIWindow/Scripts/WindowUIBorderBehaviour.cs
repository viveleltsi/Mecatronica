using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Dan.UIWindow
{
    public class WindowUIBorderBehaviour : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IPointerUpHandler
    {
        /// <summary>
        /// Link to the main window behaviour controller
        /// </summary>
        public WindowUIBehaviour MainWindowBehaviour;

        public void OnPointerDown(PointerEventData eventData)
        {
            MainWindowBehaviour.ClickOnBorder(eventData);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            MainWindowBehaviour.CursorEnterOnBorder();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            MainWindowBehaviour.CursorExitFromBorder();
        }

        public void OnDrag(PointerEventData eventData)
        {
            MainWindowBehaviour.CursorDragOnBorder(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            MainWindowBehaviour.CursorPointerUpOnBorder();
        }
    }
}
