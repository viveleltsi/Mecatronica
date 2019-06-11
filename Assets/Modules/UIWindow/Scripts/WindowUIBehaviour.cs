using Dan.Manager.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace Dan.UIWindow
{
    [RequireComponent(typeof(RectTransform))]
    public class WindowUIBehaviour : MonoBehaviour
    {
        /// <summary>
        /// Minimum size of the window
        /// </summary>
        public Vector2 MinimumSize = new Vector2(100f, 50f);

        /// <summary>
        /// Is the window draggeable
        /// </summary>
        public bool IsDraggeable = true;

        /// <summary>
        /// Is the window resizable
        /// </summary>
        public bool IsResizable = true;

        /// <summary>
        /// Is the window closable
        /// </summary>
        public bool IsClosable = true;

        /// <summary>
        /// Is the window open and visible or not
        /// </summary>
        public bool IsVisible { get; private set; } = true;

        /// <summary>
        /// How to handle the switch size
        /// </summary>
        public SwitchSizeOption SwitchSizeOption = SwitchSizeOption.SwitchAvailable;

        /// <summary>
        /// Event when the window switch size (from fullsize to smallsize)
        /// </summary>
        public SwitchFullScreenMode OnFullScreenSwitch;

        /// <summary>
        /// Cursor when resizing
        /// </summary>
        [Space(20)]
        [Header("Cursor texture")]
        public Texture2D _cursorResizeVertical;

        /// <summary>
        /// Cursor when resizing
        /// </summary>
        public Texture2D _cursorResizeHorizontal;

        /// <summary>
        /// Cursor when resizing in diagonal from bottom left to top right
        /// </summary>
        public Texture2D _cursorResizeDiagonalBLTR;

        /// <summary>
        /// Cusor when resizing in diagonal from top left to bottom right
        /// </summary>
        public Texture2D _cursorResizeDiangonalTLBR;

        /// <summary>
        /// Is the cursor on a resize area?
        /// </summary>
        private bool _cursorOnResizeArea = false;

        /// <summary>
        /// Border size
        /// </summary>
        private const int BorderSize = 5;

        #region Resize variable
        private Vector2 _startPressPosition;

        private Vector2 _startPosition;

        private Vector2? _startWindowSize;

        private Vector2 _axisResize;

        private Vector2 _axisMoveWhenResize;
        #endregion

        #region SwitchSize variable

        private bool _fullScreen = false;
        private Vector2 _smallSizePosition;
        private Vector2 _smallSizeSize;

        #endregion

        #region Drag variable

        private Vector2 _startPositionOnClick;
        private Vector2 _startPositionLocal;
        private Vector2 _deltaClickPosition;
        private bool _drag = false;

        #endregion
        /// <summary>
        /// Save the reference to the rect transform
        /// </summary>
        [SerializeField]
        [HideInInspector]
        private RectTransform _rectTransform = null;

        /// <summary>
        /// All borders
        /// </summary>
        [SerializeField]
        private List<WindowUIBorderBehaviour> _borders = new List<WindowUIBorderBehaviour>();


        // Update is called once per frame
        void Update()
        {
            if (_cursorOnResizeArea && _startWindowSize == null && _fullScreen == false)
            {
                DetectCursorNearBorder();
            }
        }

        #region Close

        /// <summary>
        /// Close / hide the window
        /// </summary>
        public void CloseWindow()
        {
            IsVisible = false;
            HandleOpenCloseWindow();
        }

        /// <summary>
        /// Open / visible the window
        /// </summary>
        public void OpenWindow(Vector2? position = null)
        {
            IsVisible = true;
            if(position != null)
            {
                MoveWindow(position.Value);
            }
            HandleOpenCloseWindow();
        }

        /// <summary>
        /// Handle the open/close window
        /// </summary>
        private void HandleOpenCloseWindow()
        {
            gameObject.SetActive(IsVisible);
        }

        #endregion

        #region Resize
        /// <summary>
        /// Cursor enter the border area
        /// </summary>
        public void CursorEnterOnBorder()
        {
            _cursorOnResizeArea = true;
        }

        /// <summary>
        /// Cursor leave the border area
        /// </summary>
        public void CursorExitFromBorder()
        {
            _cursorOnResizeArea = false;
            if (_startWindowSize == null)
                UnityEngine.Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }

        /// <summary>
        /// Cursor stop clicking on border
        /// </summary>
        public void CursorPointerUpOnBorder()
        {
            _startWindowSize = null;
            UnityEngine.Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }

        /// <summary>
        /// Cursor click on border
        /// </summary>
        /// <param name="eventData"></param>
        public void ClickOnBorder(PointerEventData eventData)
        {
            _startWindowSize = _rectTransform.sizeDelta;
            _startPressPosition = eventData.position;
            _startPosition = _rectTransform.position;
        }

        /// <summary>
        /// Cursor is dragging on border
        /// </summary>
        /// <param name="eventData"></param>
        public void CursorDragOnBorder(PointerEventData eventData)
        {
            if (_startWindowSize != null)
            {
                var inputPosition = ClampPositionToScreen(eventData.position);
                Vector2 delta = (inputPosition - (Vector3)_startPressPosition) * _axisResize;

                var newSize = _startWindowSize.Value + delta * _axisMoveWhenResize;
                newSize.x = Mathf.Clamp(newSize.x, MinimumSize.x, Screen.width);
                newSize.y = Mathf.Clamp(newSize.y, MinimumSize.y, Screen.height);
                _rectTransform.sizeDelta = newSize;
                delta = new Vector2(
                       _axisMoveWhenResize.x < 0f ? (newSize.x - _startWindowSize.Value.x) / _axisMoveWhenResize.x : 0f,
                       _axisMoveWhenResize.y < 0f ? (newSize.y - _startWindowSize.Value.y) / _axisMoveWhenResize.y : 0f
                       );
                var actualPosition = ClampPositionToScreen(_startPosition + delta);
                _rectTransform.position = actualPosition;

            }
        }

        /// <summary>
        /// Check near wich border the cursor are
        /// </summary>
        private void DetectCursorNearBorder()
        {
            var delta = Input.mousePosition - _rectTransform.position;
            var delta2 = delta - (Vector3)_rectTransform.sizeDelta;
            _axisResize = new Vector2(0, 0);
            _axisMoveWhenResize = new Vector2(1f, 1f);

            if (delta.x >= 0f && delta.x <= BorderSize)
            {
                _axisMoveWhenResize.x = -1f;
                _axisResize.x = 1f;
            }

            if (delta.y >= 0f && delta.y <= BorderSize)
            {
                _axisMoveWhenResize.y = -1f;
                _axisResize.y = 1f;
            }

            if (delta2.x <= 0f && delta2.x >= -BorderSize)
            {
                _axisMoveWhenResize.x = 1f;
                _axisResize.x = 1f;
            }

            if (delta2.y <= 0f && delta2.y >= -BorderSize)
            {
                _axisMoveWhenResize.y = 1f;
                _axisResize.y = 1f;
            }

            if (_axisResize == Vector2.zero)
            {
                UnityEngine.Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            }
            else
            {
                Vector2 hotspot = new Vector2(_cursorResizeVertical.width / 2f, _cursorResizeVertical.height / 2f);
                if (_axisResize.y == 0f)
                {

                    UnityEngine.Cursor.SetCursor(_cursorResizeHorizontal, hotspot, CursorMode.Auto);
                }
                else if (_axisResize.x == 0f)
                {

                    UnityEngine.Cursor.SetCursor(_cursorResizeVertical, hotspot, CursorMode.Auto);
                }
                else
                {
                    if ((_axisMoveWhenResize.x == 1f && _axisMoveWhenResize.y == 1f) ||
                            (_axisMoveWhenResize.x == -1f && _axisMoveWhenResize.y == -1f))
                    {
                        UnityEngine.Cursor.SetCursor(_cursorResizeDiagonalBLTR, hotspot, CursorMode.Auto);
                    }
                    else
                    {
                        UnityEngine.Cursor.SetCursor(_cursorResizeDiangonalTLBR, hotspot, CursorMode.Auto);
                    }

                }
            }
        }

        #endregion

        #region Switch Size

        /// <summary>
        /// Switch the fullScreen mode
        /// </summary>
        public void SwitchFullScreen()
        {
            if (SwitchSizeOption == SwitchSizeOption.SwitchAvailable)
            {
                _fullScreen = !_fullScreen;
                if (_fullScreen)
                {
                    _smallSizePosition = _rectTransform.position;
                    _smallSizeSize = _rectTransform.sizeDelta;
                    _rectTransform.position = Vector2.zero;
                    _rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);
                }
                else
                {
                    _rectTransform.position = _smallSizePosition;
                    _rectTransform.sizeDelta = _smallSizeSize;
                }
                OnFullScreenSwitch.Invoke(_fullScreen);
            }
        }

        #endregion

        #region Drag

        /// <summary>
        /// Move the window to the position (clamped to screen)
        /// </summary>
        /// <param name="position"></param>
        public void MoveWindow(Vector2 position)
        {
            _rectTransform.position = ClampPositionWithSizeToScreen(position);
        }

        /// <summary>
        /// When drag click down
        /// </summary>
        /// <param name="pressPosition"></param>
        public void DragClickDown(Vector2 pressPosition)
        {
            if (IsDraggeable)
            {
                _startPositionLocal = pressPosition;
                _startPositionOnClick = _rectTransform.position;
            }
        }

        public void OnDrag(Vector2 position)
        {
            if (IsDraggeable)
            {
                MoveWindow(position - _startPositionLocal + _startPositionOnClick);
            }
        }

        public void EnableDrag(bool drag)
        {
            if (IsDraggeable)
            {
                _drag = drag;
            }
            else
            {
                _drag = false;
            }
        }

        #endregion

        #region Utils
        /// <summary>
        /// Clamp the position to the screen
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private Vector3 ClampPositionToScreen(Vector3 position)
        {
            position.x = Mathf.Clamp(position.x, 0f, Screen.width);
            position.y = Mathf.Clamp(position.y, 0f, Screen.height);
            return position;
        }

        /// <summary>
        /// Clamp considering the size of the window , to the screen size
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private Vector3 ClampPositionWithSizeToScreen(Vector3 position)
        {
            position.x = Mathf.Clamp(position.x, 0f, Screen.width - _rectTransform.sizeDelta.x);
            position.y = Mathf.Clamp(position.y, 0f, Screen.height - _rectTransform.sizeDelta.y);
            return position;
        }

        /// <summary>
        /// Get the rect transform reference
        /// </summary>
        private void OnValidate()
        {
#if UNITY_EDITOR
            _rectTransform = GetComponent<RectTransform>();
            _borders = transform.GetComponentsInChildren<WindowUIBorderBehaviour>().ToList();
            _borders.ForEach(x => x.gameObject.GetComponent<Image>().raycastTarget = IsResizable);
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
        #endregion
    }

    [System.Serializable]
    public class SwitchFullScreenMode : UnityEvent<bool>
    {
    }

    public enum SwitchSizeOption
    {
        SwitchAvailable,
        OnlyFullsize,
        OnlySmallSize
    }
}
