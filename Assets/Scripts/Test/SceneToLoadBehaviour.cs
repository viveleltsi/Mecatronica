using Dan.Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneToLoadBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SceneManager.LoadScene(SceneLoaderManager.UISceneName, LoadSceneMode.Additive);  
    }
}
