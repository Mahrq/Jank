using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace Mahrq
{
    /// <summary>
    /// This class displays the spawning information and relevant fields and properties of the prefab to be spawned.
    /// </summary>
    [Serializable]
    public class PlacementDetailsPanel : PanelTemplate
    {
        public GameObject parentHolder;
        public Vector3 spawnPosition;
        public Vector3 spawnRotation;
        public float ySpawnIncrement = 1f;
        public float ySpawnPos = 0f;
        public float spawnRotationIncrement = 45f;
        private bool _inPlacementMode = false;
        [SerializeField]
        private Axis affectedSpawnAxis = Axis.Y;

        public static event Action OnPlacementModeChanged;

        public PlacementDetailsPanel(Rect rect, GUISkin skin)
        {
            this.panelRect = rect;
            this.skin = skin;
        }
        public void EventSetUp()
        {
            PrefabSelectionPanel.OnSelectionChanged += ExitPlacementMode;
        }
        public void EventCleanUp()
        {
            PrefabSelectionPanel.OnSelectionChanged -= ExitPlacementMode;
        }
        public override void Draw(Event e)
        {
            GUILayout.BeginArea(panelRect);
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(panelRect.width));
            EditorGUILayout.BeginVertical();
            EditorGUILayout.Space(8);
            TogglePlacementMode();
            EditorGUILayout.Space(12);
            //Draws a box on the panel to signify that the user is in placement mode.
            if (inPlacementMode)
            {
                GUI.Box(panelRect, "", skin.box);
                GUI_SpawnDetails();
            }
            else
            {
                parentHolder = (GameObject)EditorGUILayout.ObjectField("Parent Holder", parentHolder, typeof(GameObject), allowSceneObjects: true);
                EditorGUILayout.Space(4);
                affectedSpawnAxis = (Axis)EditorGUILayout.EnumFlagsField("Rotation Axis", affectedSpawnAxis);
                EditorGUILayout.Space(4);
                spawnRotationIncrement = EditorGUILayout.FloatField("Axis Rotation Increment", spawnRotationIncrement);
                EditorGUILayout.Space(4);
                ySpawnIncrement = EditorGUILayout.FloatField("Y Increment", ySpawnIncrement);
                EditorGUILayout.Space(12);
                GUI_ControlsHelpBox();
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
            GUILayout.EndArea();
        }
        public void ToggleAxis(KeyCode keyCode)
        {
            switch (keyCode)
            {
                case KeyCode.A:
                    affectedSpawnAxis ^= Axis.X;
                    break;
                case KeyCode.S:
                    affectedSpawnAxis ^= Axis.Y;
                    break;
                case KeyCode.D:
                    affectedSpawnAxis ^= Axis.Z;
                    break;
                default:
                    break;
            }
        }
        public void ModifySpawnRotation(bool increase)
        {
            float amount = increase ? spawnRotationIncrement : -spawnRotationIncrement;
            //Checks if any of the flags are on, then applies the amount of rotation to the spawn rotation.
            if ((affectedSpawnAxis & Axis.X) == Axis.X)
            {
                spawnRotation.x += amount;
            }
            if ((affectedSpawnAxis & Axis.Y) == Axis.Y)
            {
                spawnRotation.y += amount;
            }
            if ((affectedSpawnAxis & Axis.Z) == Axis.Z)
            {
                spawnRotation.z += amount;
            }
        }
        public void ResetModifications()
        {
            ySpawnPos = 0f;
            spawnRotation.x = 0f;
            spawnRotation.y = 0f;
            spawnRotation.z = 0f;
        }
        private void GUI_SpawnDetails()
        {
            EditorGUILayout.LabelField("Affected Rotation Axis", skin.customStyles[4]);
            EditorGUILayout.LabelField($"{affectedSpawnAxis}", skin.customStyles[4]);
            EditorGUILayout.Space(4);
            EditorGUILayout.LabelField("Spawn Location", skin.customStyles[4]);
            EditorGUILayout.LabelField($"{spawnPosition}", skin.customStyles[4]);
            EditorGUILayout.Space(4);
            EditorGUILayout.LabelField("Spawn Rotation", skin.customStyles[4]);                
            EditorGUILayout.LabelField($"{spawnRotation}", skin.customStyles[4]);
        }
        private void GUI_ControlsHelpBox()
        {
            string message = "Placement Mode Controls:" +
                "\n\nW: +y" +
                "\nS: -y" +
                "\nA: -rotation" +
                "\nD: +rotation" +
                "\nEsc: Exit Placement Mode" +
                "\n\n-Shift Modifier-" +
                "\nA: Toggle x" +
                "\nS: Toggle y" +
                "\nD: Toggle z" +
                "\n\n-Hold Modifier-" +
                "\nCtrl: Grid Snap" +
                "\n\n-Mouse-" +
                "\nLeft Click: place prefab";
            EditorGUILayout.HelpBox(message, MessageType.None, true);
        }
        private void TogglePlacementMode()
        {
            if (!PrefabPlacementEditor.editorWindow.CanPlacePrefab)
            {
                inPlacementMode = false;
            }
            string placementLabel = inPlacementMode ? "Exit Placement Mode" : "Enter Placement Mode";
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(placementLabel))
            {
                if (PrefabPlacementEditor.editorWindow.CanPlacePrefab)
                {
                    inPlacementMode = !inPlacementMode;
                    if (inPlacementMode)
                    {
                        SceneView.FocusWindowIfItsOpen<SceneView>();
                    }
                }
            }
            if (inPlacementMode)
            {
                EditorGUILayout.Space(4);
                if (GUILayout.Button("Reset Modifications"))
                {
                    ResetModifications();
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        public void ExitPlacementMode()
        {
            inPlacementMode = false;
        }
        //Invokes event when the placement mode has changed.
        public bool inPlacementMode
        {
            get
            {
                return _inPlacementMode;
            }
            private set
            {
                bool before = _inPlacementMode;
                _inPlacementMode = value;
                if (before != _inPlacementMode)
                {
                    OnPlacementModeChanged?.Invoke();
                }
            }
        }
        [Flags]

        private enum Axis
        {
            None = 0,
            X = 1,
            Y = 1 << 1,
            Z = 1 << 2
        }
    }
}

