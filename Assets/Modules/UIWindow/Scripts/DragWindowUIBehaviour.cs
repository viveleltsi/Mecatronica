using Dan.Manager.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Dan.UIWindow
{
    /// <summary>
    /// Handle the header behaviour
    /// Drag the window
    /// </summary>
    public class DragWindowUIBehaviour : MonoBehaviour, IDragHandler, IPointerDownHandler, IBeginDragHandler, IEndDragHandler
    {
        public WindowUIBehaviour MainWindow;

        /// <summary>
        /// Handle when the user click down on this element
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerDown(PointerEventData eventData)
        {
            MainWindow.DragClickDown(eventData.pressPosition);
        }

        #region Drag

        /// <summary>
        /// Handle when user drag the window
        /// </summary>
        /// <param name="eventData"></param>
        public void OnDrag(PointerEventData eventData)
        {
            MainWindow.OnDrag(eventData.position);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            MainWindow.EnableDrag(true);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            MainWindow.EnableDrag(false);
        }
        #endregion
    }
}
