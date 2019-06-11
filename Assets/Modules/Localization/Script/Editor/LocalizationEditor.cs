#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;


namespace Dan.Localization
{
    /// <summary>
    /// Window editor for the localization menu
    /// </summary>
    public class LocalizationEditor : EditorWindow
    {

        /// <summary>
        /// Path where the data are saved
        /// </summary>
        private const string DATA_SAVE_PATH = "Assets/Data/Localization/";

        /// <summary>
        /// Name of the data file
        /// </summary>
        private const string DATA_FILE_NAME = "LocalizationSettings";

        /// <summary>
        /// Settings for the localization
        /// </summary>
        private LocalizationSettings _settings;

        /// <summary>
        /// Is the object loaded?
        /// </summary>
        private bool _settingsLoaded = false;

        /// <summary>
        /// Do we have initialized all menu tabulations
        /// </summary>
        private bool _menuTabInitialized = false;

        /// <summary>
        /// Current menu index
        /// </summary>
        private int _menuIndex = 0;

        /// <summary>
        /// Information text to display into a helpbox
        /// </summary>
        private string _helpBoxInformation;

        /// <summary>
        /// Warning text to display into a helpbox
        /// </summary> 
        private string _helpBoxWarning;

        /// <summary>
        /// Danger text to display into a helpbox
        /// </summary>
        private string _helpBoxDanger;

        /// <summary>
        /// Do user have complete the first seed
        /// </summary>
        private bool _needFirstSeed;

        /// <summary>
        /// Default language when creating it on the first seed
        /// </summary>
        private LocalizedLanguage _defaultLanguageFirstSeed;

        /// <summary>
        /// The menu height
        /// </summary>
        private const int _menuHeight = 18;

        /// <summary>
        /// All menu tab content (text title and icon)
        /// </summary>
        private List<GUIContent> _menuTab = new List<GUIContent>();

        #region Key panel variables
        /// <summary>
        /// Is the key list expanded or not?
        /// </summary>
        private bool _keyExpanded = false;

        /// <summary>
        /// All key as reordable list
        /// </summary>
        private ReorderableList _keys;

        /// <summary>
        /// Settings serialized
        /// </summary>
        private SerializedObject _settingSerialized;

        /// <summary>
        /// Keys properties serialized
        /// </summary>
        private SerializedProperty _settingKeyProperty;

        /// <summary>
        /// Internal styles
        /// </summary>
        internal class Styles
        {
            public static GUIContent Keys = EditorGUIUtility.TrTextContent("Keys");
        }

        #endregion

        #region Initialization
        /// <summary>
        /// Initialization of the editor script
        /// </summary>
        [MenuItem("Tools/Dan/Localization")]
        private static void Init()
        {
            LocalizationEditor window = (LocalizationEditor)EditorWindow.GetWindow(typeof(LocalizationEditor), false, "Localization", true);
            window.Show();
        }

        private void OnEnable()
        {
            CheckResource();
        }

        /// <summary>
        /// Initialize all localization key
        /// </summary>
        private void InitializeKeys()
        {
            _settingSerialized = new SerializedObject(_settings);
            _settingKeyProperty = _settingSerialized.FindProperty("DataPerLanguages");
            _keys = new ReorderableList(_settingSerialized, _settingKeyProperty,false,false,true,true);
            _keys.onAddDropdownCallback = CreateNewKey;
            _keys.drawElementCallback = DrawKeyListElement;
            _keys.onRemoveCallback = RemoveKey;
            _keys.headerHeight = 3;
        }

        /// <summary>
        /// List of unity icon name : https://unitylist.com/p/5c3/Unity-editor-icons
        /// </summary>
        private void InitializeTabulation()
        {
            _menuTab.Clear();
            _menuTab.Add(new GUIContent("Keys", EditorGUIUtility.IconContent("GUIText Icon").image));
            _menuTab.Add(new GUIContent("Languages", EditorGUIUtility.IconContent("d_AlphabeticalSorting").image));
            _menuTab.Add(new GUIContent("Export", EditorGUIUtility.IconContent("SaveActive").image));
            _menuTab.Add(new GUIContent("Settings", EditorGUIUtility.IconContent("d_Settings").image));
            _menuTabInitialized = true;
        }
        #endregion

        #region Key Reordable Method
        /// <summary>
        /// Draw the key on the reordable edition panel
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="index"></param>
        /// <param name="selected"></param>
        /// <param name="focused"></param>
        private void DrawKeyListElement(Rect rect, int index, bool selected, bool focused)
        {
            rect.height -= 2; // nicer looking with selected list row and a text field in it

            // De-indent by the drag handle width, so the text field lines up with others in the inspector.
            // Will have space in front of label for more space between it and the drag handle.
            rect.xMin -= ReorderableList.Defaults.dragHandleWidth;

            string oldName = _settings.DataPerLanguages[index].EditorName;
            GUIStyle style = new GUIStyle(EditorStyles.textField);
            Color defaultColor = GUI.backgroundColor;
            if(_settings.DataPerLanguages.Count(x => x.EditorName == oldName) > 1)
            {
                GUI.backgroundColor = Color.red;
            }
            string newName = EditorGUI.TextField(rect, "Key", oldName);
            GUI.backgroundColor = defaultColor;
            if (newName != oldName)
            {
                if(_settings.DataPerLanguages.Count(x => x.EditorName == newName) > 0)
                {
                    _helpBoxWarning = "Key already exist";
                }
                else
                {
                    _helpBoxWarning = null;
                }
                _settings.DataPerLanguages[index].EditorName = newName;
                _settingSerialized.ApplyModifiedProperties();
                _settingSerialized.Update();
            }
        }

        private void RemoveKey(ReorderableList list)
        {
            ReorderableList.defaultBehaviours.DoRemoveButton(list);
            _settingSerialized.ApplyModifiedProperties();
            _settingSerialized.Update();
        }

        /// <summary>
        /// Create a new key
        /// </summary>
        /// <param name="buttonRect"></param>
        /// <param name="list"></param>
        private void CreateNewKey(Rect buttonRect, ReorderableList list)
        {
            buttonRect.x -= 400;
            buttonRect.y -= 13;
            _settings.DataPerLanguages.Add(new LocalizedDataPerLanguage());
            _settingSerialized.ApplyModifiedProperties();
            _settingSerialized.Update();
            //PopupWindow.Show(buttonRect, new EnterNamePopup(m_Tags, s => { InternalEditorUtility.AddTag(s); }));
        }
        #endregion

        #region Draw UI method
        /// <summary>
        /// Handle the ui
        /// </summary>
        private void OnGUI()
        {
            CheckResource();
            DawHelpBox();
            if (_needFirstSeed)
            {
                DrawFirstSeed();
            }
            else
            {
                DrawMenuAndPanel();
            }
        }

        /// <summary>
        /// Draw the main menu and the actual window panel
        /// </summary>
        private void DrawMenuAndPanel()
        {
            var menuResult = DrawMenu(_menuIndex, _menuTab);

            _menuIndex = menuResult.index;
            switch (_menuIndex)
            {
                case 0:
                    DrawMainWindow();
                    break;
                case 1:
                    DrawSecondWindow();
                    break;
                case 2:
                    DrawThirdWindow();
                    break;
                case 3:
                    DrawSettings();
                    break;
                default:
                    break;
            }

            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Draw the first seed panel (for configuration the first time)
        /// </summary>
        private void DrawFirstSeed()
        {
            if(_defaultLanguageFirstSeed == null)
            {
                _defaultLanguageFirstSeed = new LocalizedLanguage();
                _defaultLanguageFirstSeed.GenerateGuid();
            }
            if (string.IsNullOrWhiteSpace(_defaultLanguageFirstSeed.Id))
            {
                _defaultLanguageFirstSeed.GenerateGuid();
            }
            GUILayout.BeginHorizontal();
            GUILayout.Label("Name of the language (example:\"English\")");
            _defaultLanguageFirstSeed.EditorName = GUILayout.TextField(_defaultLanguageFirstSeed.EditorName,GUILayout.MaxWidth(150f));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUI.BeginDisabledGroup(string.IsNullOrWhiteSpace(_defaultLanguageFirstSeed.EditorName));
            if (GUILayout.Button("Save", new GUILayoutOption[] { GUILayout.MaxWidth(100f)}))
            {
                _settings.DefaultLanguage = _defaultLanguageFirstSeed;
                _needFirstSeed = false;
                _helpBoxInformation = null;
                InitializeKeys();
                SaveObject();

            }
            EditorGUI.EndDisabledGroup();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// Draw help boxes
        /// </summary>
        private void DawHelpBox()
        {
            if(string.IsNullOrEmpty(_helpBoxInformation) == false)
            {
                EditorGUILayout.HelpBox(_helpBoxInformation, MessageType.Info);
            }
            if (string.IsNullOrEmpty(_helpBoxWarning) == false)
            {
                EditorGUILayout.HelpBox(_helpBoxWarning, MessageType.Warning);
            }
            if (string.IsNullOrEmpty(_helpBoxDanger) == false)
            {
                EditorGUILayout.HelpBox(_helpBoxDanger, MessageType.Error);
            }
        }

        private void DrawMainWindow()
        {
            _keyExpanded = EditorGUILayout.Foldout(_keyExpanded, Styles.Keys, true);
            if (_keyExpanded && _keys != null && _keys.count > 0)
            {
                EditorGUI.indentLevel++;
                _keys.DoLayoutList();
                EditorGUI.indentLevel--;
            }
            
        }

        private void DrawSecondWindow()
        {
            EditorGUILayout.TextArea("Second panel");
        }

        private void DrawThirdWindow()
        {

            
        }

        private void DrawSettings()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"Default language : \"{_settings.DefaultLanguage.EditorName}\"");
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("Reset (debug)"))
            {
                ResetAll();
            }
        }

        /// <summary>
        /// Inspired by line 3757 https://github.com/jamesjlinden/unity-decompiled/blob/master/UnityEditor/UnityEditor/EditorGUILayout.cs
        /// </summary>
        private (int index, float y) DrawMenu(int currentIndex, List<GUIContent> content)
        {
            EditorGUI.BeginChangeCheck();
            int index = currentIndex;
            Rect rect = EditorGUILayout.BeginVertical(GUI.skin.box);
            --rect.width;
            --rect.width;
            GUIStyle toolbarButton = EditorStyles.toolbarButton;
            float menuWidth = ((EditorGUIUtility.currentViewWidth - 2f) / _menuTab.Count()) - 2f;
            for (int i = 0; i < _menuTab.Count(); i++)
            {
                var toggleRect = new Rect(rect.x + 1 + i * menuWidth, rect.y, menuWidth, _menuHeight);
                //GUI.Toggle(toggleRect, currentIndex  == i, _menuTab[i], toolbarButton)
                //_menuTab[0]
                if (GUI.Toggle(toggleRect, currentIndex == i, _menuTab[i], toolbarButton))
                {
                    if (currentIndex != i)
                    {
                        Debug.Log($"select {i} / selected {currentIndex}");
                        index = i;
                    }
                }
            }
            GUILayout.Space(_menuHeight + 2f);
            return (index, _menuHeight);
        }

        #endregion

        /// <summary>
        /// Check if all resources are correctly loaded, created, etc
        /// </summary>
        private void CheckResource()
        {
            //Check of the data is loaded
            if (_settingsLoaded == false)
            {
                RefreshData();
            }
            //Check if the tabulation are initialized (all different window panel)
            if(_menuTabInitialized == false)
            {
                InitializeTabulation();
            }
            if (_keys == null)
            {
                InitializeKeys();
            }
            //Check if first seed must be filled ( if a default language is selected)
            if (_settings.DefaultLanguage == null || _settings.DefaultLanguage.IsInitialized() == false)
            {
                _needFirstSeed = true;
                _helpBoxInformation = "Welcome on Dan.Localization module. Some informations are required.";
            }
            else
            {
                _needFirstSeed = false;
            }
        }

        /// <summary>
        /// Reset all window and reload all datas
        /// </summary>
        private void ResetAll()
        {
            RefreshData();
            InitializeTabulation();
            InitializeKeys();
        }

        /// <summary>
        /// refresh data loading or create the settings
        /// </summary>
        private void RefreshData()
        {
            var objects = AssetDatabase.FindAssets("t:LocalizationSettings");
            if(objects != null && objects.Count() > 0)
            {
                var path = AssetDatabase.GUIDToAssetPath(objects.FirstOrDefault());
                var settings = AssetDatabase.LoadAssetAtPath<LocalizationSettings>(path);
                SelectSettings(settings);
            }
            else
            {
                //Create asset
                CreateNew();
            }
            
        }

        /// <summary>
        /// Create new object
        /// </summary>
        private void CreateNew()
        {
            string id = Guid.NewGuid().ToString();
            if (!Directory.Exists(DATA_SAVE_PATH))
            {
                Directory.CreateDirectory(DATA_SAVE_PATH);
            }
            LocalizationSettings settings = (LocalizationSettings)AssetDatabase.LoadAssetAtPath($"{DATA_SAVE_PATH}/{DATA_FILE_NAME}.asset", typeof(LocalizationSettings));
            if (settings == null)
            {
                settings = ScriptableObject.CreateInstance<LocalizationSettings>();
                settings.Id = id;
                AssetDatabase.CreateAsset(settings, $"{DATA_SAVE_PATH}/{DATA_FILE_NAME}.asset");
                AssetDatabase.SaveAssets();
                SelectSettings(settings);
            }
            else
            {
                Debug.LogError("Localization settings already exist");
            }
        }

        /// <summary>
        /// Select the specific settings
        /// </summary>
        /// <param name="settings"></param>
        private void SelectSettings(LocalizationSettings settings)
        {
            Debug.Log("Settings selected");
            _settings = settings;
            _settingsLoaded = true;
        }

        /// <summary>
        /// Save function
        /// </summary>
        protected virtual void SaveObject()
        {
            EditorUtility.SetDirty(_settings);
            AssetDatabase.SaveAssets();
        }
    }
}
#endif
