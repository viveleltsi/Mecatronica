using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Dan.Manager
{
    public class SceneLoaderManager : Singleton<SceneLoaderManager>
    {
        //cool
        public static readonly string CoreSceneName = "CoreScene";
        public static readonly string UISceneName = "UIScene";
    }
}
