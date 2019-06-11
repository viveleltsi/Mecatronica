using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Dan.SceneSmoothLoader.Utils;
using System;
using Dan.Manager;


/// <summary>
/// Handle the loading of the corescene if no already loaded
/// And handle the activation of all monoBehaviour script
/// </summary>
public class SceneScriptBehaviour : MonoBehaviour
{

    /// <summary>
    /// Dont disabled script on this scene
    /// </summary>
    public bool KeepEnable = false;

    [TextArea]
    public string Logs = string.Empty;

    private int _frameUsed = 0;

    private int _scriptLoaded = 0;

    private double _totalTimeUsed = 0;

    /// <summary>
    /// All script to activate
    /// </summary>
    [SerializeField]
    private List<MonoBehaviour> _scripts = new List<MonoBehaviour>();

    /// <summary>
    /// Alocation time for enabling scripts per frame
    /// </summary>
    private const double ALLOCATION_TIME = 5;

    /// <summary>
    /// Is the enabling finished?
    /// </summary>
    private bool _enablingFinish = false;

    /// <summary>
    /// Watch timer
    /// </summary>
    private WatchTimer _watchTimer;

    [SerializeField]
    [HideInInspector]
    private SmoothLoaderOptions _options;

    // Start is called before the first frame update
    public void Awake()
    {
        Logs = string.Empty;
#if UNITY_EDITOR
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            var scene = SceneManager.GetSceneAt(i);
            if (scene.name == SceneLoaderManager.CoreSceneName)
                return;
        }
        SceneManager.LoadScene(SceneLoaderManager.CoreSceneName, LoadSceneMode.Additive);
#endif
    }

    // Update is called once per frame
    public void Update()
    {
        if (_enablingFinish == false)
        {
            _frameUsed++;
            _watchTimer = WatchTimer.CreateNew(ALLOCATION_TIME);
            for (int i = _scripts.Count-1; i >= 0; i--)
            {
                if (_scripts[i] != null)
                {
                    if(_watchTimer.HaveTime() == false)
                    {
                        _totalTimeUsed += _watchTimer.TimeSinceStart();
                        return;
                    }
                    else
                    {
                        _scriptLoaded++;
                        //_watchTimer.Log($"Start enabling {i}");
                        _scripts[i].enabled = true;
                        //_watchTimer.Log($"Removing enabling {i}");
                        _scripts.RemoveAt(i);
                        //_watchTimer.Log($"End enabling {i}");
                        if (i == 0)
                        {
                            _enablingFinish = true;
                            _totalTimeUsed += _watchTimer.TimeSinceStart();
                            CreateLog();
                            return;
                        }
                        else
                            continue;
                    }
                }
                else
                {
                    _totalTimeUsed += _watchTimer.TimeSinceStart();
                    if (i == 0)
                    {
                        _enablingFinish = true;
                        _totalTimeUsed += _watchTimer.TimeSinceStart();
                        CreateLog();
                        return;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Find all script into the scene and disabled them (but this one)
    /// </summary>
    public void GetAllScriptAndDisabled()
    {
        _scripts.Clear();
        _options = ScriptableObjectManager.GetAllInstances<SmoothLoaderOptions>().FirstOrDefault();
        var allScripts = Resources.FindObjectsOfTypeAll<MonoBehaviour>().Where(x => x != null).ToList();
        for (int i = 0; i < allScripts.Count; i++)
        {
            var script = allScripts[i];
            if(script != this &&
                KeepEnable == false &&
                _options.IsValidForSmoothLoader(script)
                )
            {
                script.enabled = false;
                _scripts.Add(script);
            }
                
        }
        Debug.Log("Script found:" + _scripts.Count);
    }

    [ContextMenu("All visible")]
    private void SetAllScriptEnable()
    {
        foreach(var script in _scripts)
        {
            script.enabled = true;
        }
    }

    private void CreateLog()
    {
        Logs = $"----------- Script enabled --------------" + Environment.NewLine +
            $"Total script enable : {_scriptLoaded}" + Environment.NewLine +
            $"in {_frameUsed} frame(s)" + Environment.NewLine +
            $"total time used : {_totalTimeUsed}" + Environment.NewLine;
        if (_scriptLoaded > 0)
            Logs += $"average per script : {_totalTimeUsed / _scriptLoaded}" + Environment.NewLine;
    }

}
