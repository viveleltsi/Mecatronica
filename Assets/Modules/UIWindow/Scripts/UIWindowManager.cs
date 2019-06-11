using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dan.UIWindow
{
    public class UIWindowManager : MonoBehaviour
    {
        /// <summary>
        /// Prefab of UI Window
        /// </summary>
        public GameObject UIWindowPrefab;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// Create a window
        /// </summary>
        /// <param name="initPosition"></param>
        /// <param name="initSize"></param>
        /// <param name="movable"></param>
        /// <param name="resizable"></param>
        /// <param name="minSize"></param>
        public void CreateWindow(Vector2 initPosition, Vector2 initSize, bool movable = true, bool resizable = true, Vector2? minSize = null)
        {
            var newWindow = Instantiate<GameObject>(UIWindowPrefab, transform);

        }
    }
}

