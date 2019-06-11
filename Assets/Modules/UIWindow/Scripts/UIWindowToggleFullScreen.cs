using Dan.Manager.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dan.UIWindow
{
    public class UIWindowToggleFullScreen : MonoBehaviour
    {

        public WindowUIBehaviour MainWindow;

        public Sprite FullScreenIcon;

        public Sprite SmallScreenIcon;

        public Image ImageComponent;


        private void Start()
        {
            MainWindow.OnFullScreenSwitch.RemoveListener(SwitchIcon);
            MainWindow.OnFullScreenSwitch.AddListener(SwitchIcon);
        }

        /// <summary>
        /// Switch icon when fullscreen / smallsize is triggered
        /// </summary>
        /// <param name="bigSize"></param>
        public void SwitchIcon(bool bigSize)
        {
            DebugUtils.Log($"big size {bigSize}");
            Sprite icon = SmallScreenIcon;
            if (bigSize)
            {
                icon = FullScreenIcon;
            }
            ImageComponent.sprite = icon;
        }

        private void OnDestroy()
        {
            MainWindow.OnFullScreenSwitch.RemoveListener(SwitchIcon);
        }

    }
}
