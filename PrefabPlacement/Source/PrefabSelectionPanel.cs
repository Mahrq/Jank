using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace Mahrq
{
    /// <summary>
    /// The class will display a selection of prefabs.
    /// 
    /// Users can click on the button to open the file explorer or copy and paste the filepath from their 
    /// project window into the directory to display the prefabs.
    /// </summary>
    [Serializable]
    public class PrefabSelectionPanel : PanelTemplate
    {
        [SerializeField]
        private string _inputFilePath;
        private int _selectionIndex = 0;
        private int selectionDisplayColumns = 1;
        private List<GameObject> prefabAssets = new List<GameObject>();
        private GUIContent[] prefabDisplays;
        private bool isDragging = false;
        public PrefabSelectionPanel(Rect rect, GUISkin skin) : base(rect, skin) { }
        public static event Action OnSelectionChanged;

        public override void Draw(Event e)
        {
            GUILayout.BeginArea(panelRect, style);
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(panelRect.width));
            EditorGUILayout.BeginVertical();
            EditorGUILayout.Space(8);
            GUI_GetPrefabs();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        public override bool ProcessEvents(Event e)
        {
            switch (e.type)
            {
                //For dragging the selection canvas with left click incase the user has no scroll wheel.
                //For some reason the scroll bars disappear when inclosed in an Area.
                case EventType.MouseDown:
                    if (panelRect.Contains(e.mousePosition))
                    {
                        if (e.button == 0)
                        {
                            isDragging = true;
                            e.Use();
                            return true;
                        }
                    }
                    break;
                case EventType.MouseUp:
                    if (e.button == 0)
                    {
                        isDragging = false;
                        e.Use();
                        return true;
                    }
                    break;
                case EventType.MouseMove:
                    break;
                case EventType.MouseDrag:
                    if (e.button == 0 && isDragging)
                    {
                        OnMouseDragPrefabSelection(e.delta);
                        e.Use();
                        return true;
                    }
                    break;
                case EventType.KeyDown:
                    break;
                case EventType.KeyUp:
                    break;
                case EventType.ScrollWheel:
                    break;
                case EventType.Repaint:
                    break;
                case EventType.Layout:
                    break;
                case EventType.DragUpdated:
                    break;
                case EventType.DragPerform:
                    break;
                case EventType.DragExited:
                    break;
                case EventType.Ignore:
                    break;
                case EventType.Used:
                    break;
                case EventType.ValidateCommand:
                    break;
                case EventType.ExecuteCommand:
                    break;
                case EventType.ContextClick:
                    break;
                case EventType.MouseEnterWindow:
                    break;
                case EventType.MouseLeaveWindow:
                    break;
                case EventType.TouchDown:
                    break;
                case EventType.TouchUp:
                    break;
                case EventType.TouchMove:
                    break;
                case EventType.TouchEnter:
                    break;
                case EventType.TouchLeave:
                    break;
                case EventType.TouchStationary:
                    break;
                default:
                    break;
            }
            return false;
        }
        private void OnMouseDragPrefabSelection(Vector2 delta)
        {
            scrollPos -= delta;
        }
        private void GUI_GetPrefabs()
        {
            _inputFilePath = EditorGUILayout.TextField("Directory", _inputFilePath);
            EditorGUILayout.Space(4);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Get Prefabs"))
            {
                GetPrefabAssets(_inputFilePath);
            }
            if (prefabAssets != null)
            {
                if (prefabAssets.Count > 0)
                {
                    if (GUILayout.Button("Clear Prefabs"))
                    {
                        prefabAssets.Clear();
                    }
                }
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(8);
            selectionDisplayColumns = GetDisplayColumns(panelRect, skin.customStyles[3].fixedWidth);
            if (prefabAssets != null)
            {
                if (prefabAssets.Count > 0)
                {
                    selectionIndex = GUILayout.SelectionGrid(_selectionIndex, prefabDisplays, selectionDisplayColumns, skin.customStyles[3]);
                }
                else
                {
                    selectionIndex = -1;
                }
            }
        }
        private void GetPrefabAssets(string input)
        {
            string trueDirectory = Application.dataPath;
            //Clicking Get Prefabs with no input will open folder dialogue to choose from.
            if (string.IsNullOrEmpty(input))
            {
                trueDirectory = EditorUtility.OpenFolderPanel("Get Prefabs", Application.dataPath, "");
                _inputFilePath = trueDirectory.Replace(Application.dataPath, "Assets");
                input = _inputFilePath;
                EditorGUI.FocusTextInControl(null);
            }
            //If user copied the path to their prefabs folder or typed it out.
            else
            {
                trueDirectory = trueDirectory.Replace("Assets", input);
            }
            //Use full directory to see if the folder exists.
            if (Directory.Exists($"{trueDirectory}"))
            {
                if (prefabAssets == null)
                {
                    prefabAssets = new List<GameObject>();
                }
                if (prefabAssets.Count > 0)
                {
                    prefabAssets.Clear();
                }
                //Find Assets starts the search from the "Assets" folder in the project.
                string[] prefabGUID = AssetDatabase.FindAssets("t:gameObject", new[] { $"{input}" });
                string[] prefabPaths = new string[prefabGUID.Length];
                for (int i = 0; i < prefabGUID.Length; i++)
                {
                    prefabPaths[i] = AssetDatabase.GUIDToAssetPath(prefabGUID[i]);
                }
                GameObject prefabItem;
                for (int i = 0; i < prefabPaths.Length; i++)
                {
                    prefabItem = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPaths[i], typeof(GameObject));
                    prefabAssets.Add(prefabItem);
                }
                //Gets array of GUIContent to represent the assets.
                //by getting their prefab name and preivew image.
                if (prefabAssets.Count > 0)
                {
                    prefabDisplays = new GUIContent[prefabAssets.Count];
                    GUIContent prefabDisplay;
                    string prefabName;
                    Texture2D iconTexture;
                    GameObject prefab;
                    for (int i = 0; i < prefabAssets.Count; i++)
                    {
                        prefab = prefabAssets[i];
                        iconTexture = AssetPreview.GetAssetPreview(prefab);
                        prefabName = prefab.name;
                        prefabDisplay = new GUIContent(prefabName, iconTexture, prefabName);
                        prefabDisplays[i] = prefabDisplay;
                    }
                }
            }
            else
            {
                Debug.LogWarning($"Could not find Directory: {trueDirectory}");
            }

        }
        //For alligning the prefab selection icons to the current window size.
        private int GetDisplayColumns(Rect rect, float contentWidth)
        {
            int columns = 1;
            float truePanelWdith = rect.width - rect.x;
            if (truePanelWdith < contentWidth)
            {
                columns = 1;
            }
            else
            {
                columns = (int)truePanelWdith / (int)contentWidth;
            }
            return columns;
        }
        public GameObject ChosenPrefab
        {
            get
            {
                if (prefabAssets != null)
                {
                    if (prefabAssets.Count > 0 && _selectionIndex >= 0)
                    {
                        return prefabAssets[_selectionIndex];
                    }
                }
                return null;
            }
        }
        //Invokes event when the selection index has changed.
        private int selectionIndex
        {
            set
            {
                int before = _selectionIndex;
                _selectionIndex = value;
                if (_selectionIndex != before)
                {
                    OnSelectionChanged?.Invoke();
                }
            }
        }
    }
}

