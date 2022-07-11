using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace Mahrq
{
    /// <summary>
    /// Editor tool that places prefabs on the scene, from a selection of prefabs designated by the user.
    /// </summary>
    public class PrefabPlacementEditor : EditorWindow
    {
        public static PrefabPlacementEditor editorWindow;
        private static string editorPrefsKey = "PREFAB_PLACEMENT";
        private SerializedObject serializedObject;

        private PrefabSelectionPanel _prefabSelectionPanel;
        private SerializedProperty prefabSelectionPanel;
        private PlacementDetailsPanel _placementDetailsPanel;
        private SerializedProperty placementDetailsPanel;

        private bool axisToggle = false;
        private bool isHoldingPrefab = false;
        private bool gridPlacementMode = false;
        private GameObject heldPrefab;
        //Starts the window.
        [MenuItem("Mahrq/Level Editor/Prefab Placement/Editor", priority = 0)]
        static void StartWindow()
        {
            editorWindow = GetWindow<PrefabPlacementEditor>();
            editorWindow.titleContent = new GUIContent("Prefab Placement");
            editorWindow.Show();
        }
        //Clears editor pref key
        [MenuItem("Mahrq/Level Editor/Prefab Placement/Delete Pref Key", priority = 11)]
        static void ClearPrefKey()
        {
            if (EditorPrefs.HasKey(editorPrefsKey))
            {
                if (EditorUtility.DisplayDialog("Removing " + editorPrefsKey + "?",
                    "Are you sure you want to " +
                    "delete the editor key " +
                    editorPrefsKey + "?, This action cant be undone",
                    "Yes",
                    "No"))
                    EditorPrefs.DeleteKey(editorPrefsKey);
            }
            else
            {
                EditorUtility.DisplayDialog("Could not find " + editorPrefsKey,
                    "Seems that " + editorPrefsKey +
                    " does not exists or it has been deleted already, " +
                    "check that you have typed correctly the name of the key.",
                    "Ok");
            }
        }
        private void OnEnable()
        {
            if (EditorPrefs.HasKey(editorPrefsKey))
            {
                string jsonData = EditorPrefs.GetString(editorPrefsKey, JsonUtility.ToJson(this, false));
                JsonUtility.FromJsonOverwrite(jsonData, this);
            }
            editorWindow = this;
            serializedObject = serializedObject ?? new SerializedObject(this);
            InitialiseSkin();
            _prefabSelectionPanel = _prefabSelectionPanel ?? new PrefabSelectionPanel(new Rect(300, 0, position.width, position.height), editorSkin);
            prefabSelectionPanel = prefabSelectionPanel ?? serializedObject.FindProperty("_prefabSelectionPanel");
            _placementDetailsPanel = _placementDetailsPanel ?? new PlacementDetailsPanel(new Rect(0, 0, 300f, position.height), editorSkin);
            placementDetailsPanel = placementDetailsPanel ?? serializedObject.FindProperty("_placementDetailsPanel");
            _placementDetailsPanel.EventSetUp();

            PlacementDetailsPanel.OnPlacementModeChanged += OnPlacementModeChangedCallback;
        }
        private void OnDisable()
        {
            PlacementDetailsPanel.OnPlacementModeChanged -= OnPlacementModeChangedCallback;
            _placementDetailsPanel.EventCleanUp();
            SceneView.duringSceneGui -= DuringSceneGUICallback;
            string jsonData = JsonUtility.ToJson(this, false);
            EditorPrefs.SetString(editorPrefsKey, jsonData);
        }
        private void OnGUI()
        {
            GUI.skin = editorSkin;
            serializedObject.Update();

            _placementDetailsPanel.panelRect.height = position.height;
            _placementDetailsPanel.Draw(Event.current);
            _prefabSelectionPanel.panelRect.width = position.width;
            _prefabSelectionPanel.panelRect.height = position.height;
            _prefabSelectionPanel.Draw(Event.current);


            ProcessDetailsPanelEvents(Event.current);
            ProcessPrefabPanelEvents(Event.current);
            serializedObject.ApplyModifiedProperties();

            if (GUI.changed == true)
            {
                Repaint();
            }
        }
        private void OnFocus()
        {
            SceneView.duringSceneGui -= DuringSceneGUICallback;
            SceneView.duringSceneGui += DuringSceneGUICallback;

        }
        private void DuringSceneGUICallback(SceneView sceneView)
        {
            if (_placementDetailsPanel.inPlacementMode)
            {
                ProcessOnSceneEvents(Event.current);
                //Raycast from mouse position to scene canvas.
                Ray guiRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                //Gets world position ignoring y axis.
                Vector3 rayPosition = guiRay.origin - guiRay.direction * (guiRay.origin.y / guiRay.direction.y);

                Vector3 current = new Vector3(rayPosition.x,rayPosition.y + _placementDetailsPanel.ySpawnPos,rayPosition.z);
                //Sets spawn position depending on the grid placement toggle. Rounding just returns a vector with no floating numbers.
                _placementDetailsPanel.spawnPosition = gridPlacementMode ? RoundToGrid(current) : current;
                if (isHoldingPrefab)
                {
                    //Update the position and rotation of the prefab until it is released.
                    if (heldPrefab != null)
                    {
                        heldPrefab.transform.position = _placementDetailsPanel.spawnPosition;
                        heldPrefab.transform.rotation = Quaternion.Euler(_placementDetailsPanel.spawnRotation);
                    }
                    else
                    {
                        isHoldingPrefab = false;
                    }
                }
                else
                {
                    //Add a check if it is still placement mode, since the state of this bool can be changed on the input thread.
                    //Meaning this block would still run despite the status of placement mode has changed.
                    if (!_placementDetailsPanel.inPlacementMode)
                    {
                        return;
                    }
                    if (_prefabSelectionPanel.ChosenPrefab != null)
                    {
                        //Check if parent object exists otherwise just spawn the object with no parent.
                        if (_placementDetailsPanel.parentHolder != null)
                        {
                            heldPrefab = (GameObject)Instantiate(_prefabSelectionPanel.ChosenPrefab, _placementDetailsPanel.parentHolder.transform, instantiateInWorldSpace: true);
                        }
                        else
                        {
                            heldPrefab = Instantiate(_prefabSelectionPanel.ChosenPrefab);
                        }
                        //Make an undo entry when the prefab is sucessfully created.
                        if (heldPrefab != null)
                        {
                            Undo.RegisterCreatedObjectUndo(heldPrefab, $"Created {heldPrefab.name}");
                        }
                        isHoldingPrefab = true;
                    }
                }
                this.Repaint();
                sceneView.Repaint();
            }
        }
        /// <summary>
        /// Releases the held prefab while in placement mode.
        /// </summary>
        private void ReleaseHeldPrefab(bool destroy = false)
        {
            //Destroying option usually used for when the user cancels plcement mode.
            //Destroy the already held and instatiated object.
            if (destroy)
            {
                DestroyImmediate(heldPrefab);
            }
            heldPrefab = null;
            isHoldingPrefab = false;
        }
        //Rounds the given vector to have no floating points.
        private Vector3Int RoundToGrid(Vector3 input)
        {
            return new Vector3Int(Mathf.RoundToInt(input.x), Mathf.RoundToInt(input.y), Mathf.RoundToInt(input.z));
        }
        private void ProcessOnSceneEvents(Event e)
        {
            switch (e.type)
            {
                case EventType.MouseDown:
                    //Left click releases the held prefab.
                    if (e.button == 0)
                    {
                        ReleaseHeldPrefab();
                        e.Use();
                    }
                    break;
                case EventType.MouseUp:
                    break;
                case EventType.MouseMove:
                    break;
                case EventType.MouseDrag:
                    break;
                case EventType.KeyDown:
                    switch (e.keyCode)
                    {
                            //Up
                        case KeyCode.W:
                            //Increase y position.
                            if (!axisToggle)
                            {
                                _placementDetailsPanel.ySpawnPos += _placementDetailsPanel.ySpawnIncrement;
                                e.Use();
                            }
                            break;
                            //Left
                        case KeyCode.A:
                            if (axisToggle)
                            {
                                //Toggles affected x axis.
                                _placementDetailsPanel.ToggleAxis(e.keyCode);
                            }
                            else
                            {
                                //Adjusts the spawn rotation by the amount.
                                _placementDetailsPanel.ModifySpawnRotation(increase: false);
                            }
                            e.Use();
                            break;
                            //Down
                        case KeyCode.S:
                            if (axisToggle)
                            {
                                //Toggles affected y axis.
                                _placementDetailsPanel.ToggleAxis(e.keyCode);
                            }
                            else
                            {
                                //Decrease y position.
                                _placementDetailsPanel.ySpawnPos -= _placementDetailsPanel.ySpawnIncrement;
                            }
                            e.Use();
                            break;
                            //Right
                        case KeyCode.D:
                            if (axisToggle)
                            {
                                //Togglex affected z axis.
                                _placementDetailsPanel.ToggleAxis(e.keyCode);
                            }
                            else
                            {
                                //Adjust the spawn rotation by the amount.
                                _placementDetailsPanel.ModifySpawnRotation(increase: true);
                            }
                            e.Use();
                            break;
                        case KeyCode.F:
                            //Reset all rotation modifications and height levels.
                            _placementDetailsPanel.ResetModifications();
                            e.Use();
                            break;
                        case KeyCode.LeftShift:
                            //Toggle on axis.
                            axisToggle = true;
                            e.Use();
                            break;
                        case KeyCode.Escape:
                            //Exit/Cancel Placement mode.
                            ReleaseHeldPrefab(destroy: true);
                            _placementDetailsPanel.ExitPlacementMode();
                            e.Use();
                            break;
                        case KeyCode.LeftControl:
                            //Toggle on grid snap.
                            gridPlacementMode = true;
                            e.Use();
                            break;
                        default:
                            break;
                    }
                    break;
                case EventType.KeyUp:
                    switch (e.keyCode)
                    {
                        case KeyCode.LeftShift:
                            //Toggle off axis.
                            axisToggle = false;
                            e.Use();
                            break;
                        case KeyCode.LeftControl:
                            //Toggle off grid snap.
                            gridPlacementMode = false;
                            e.Use();
                            break;
                        default:
                            break;
                    }
                    break;
                case EventType.Layout:
                    break;
                default:
                    break;
            }
        }
        private void ProcessPrefabPanelEvents(Event e)
        {
            if (_prefabSelectionPanel != null)
            {
                if (_prefabSelectionPanel.ProcessEvents(e))
                {
                    GUI.changed = true;
                }
            }
        }
        private void ProcessDetailsPanelEvents(Event e)
        {
            if (_placementDetailsPanel != null)
            {
                if (_placementDetailsPanel.ProcessEvents(e))
                {
                    GUI.changed = true;
                }
            }
        }
        private void OnPlacementModeChangedCallback()
        {
            if (heldPrefab != null)
            {
                ReleaseHeldPrefab(destroy: true);
            }
        }
        [SerializeField]
        private GUISkin editorSkin;
        private void InitialiseSkin()
        {
            editorSkin = (GUISkin)EditorGUIUtility.Load("EditorSkins/PrefabPlacementEditor.guiskin");
        }

        public bool CanPlacePrefab
        {
            get { return _prefabSelectionPanel.ChosenPrefab != null; }
        }
    }
}

