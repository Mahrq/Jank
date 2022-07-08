using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace Mahrq
{
    public class NodeEditorTemplate : EditorWindow
    {
        public static NodeEditorTemplate window;
        private static string editorPrefsKey = "NODE_EDITOR_TEMPLATE";
        [SerializeField]
        private SerializedObject serializedObject;

        [SerializeReference]
        private NodeCanvas _nodeCanvas;
        private SerializedProperty _spNodeCanvas;
        [SerializeReference]
        private PanelTemplate _panelTemplate;
        private SerializedProperty _spPanelTemplate;
        public NodeCanvas nodeCanvas => _nodeCanvas;
        public PanelTemplate panelTemplate => _panelTemplate;
        [MenuItem("Mahrq/Node Editor/Example Template/Editor")]
        static void StartWindow()
        {
            window = GetWindow<NodeEditorTemplate>();
            window.titleContent = new GUIContent("Node Editor Template");
            window.Show();
        }
        //Menu item to clear editor pref key if you want to reset the data.
        [MenuItem("Mahrq/Node Editor/Example Template/Delete EditorPrefs Key")]
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
            window = this;
            //Load serialized data if the editor has any.
            if (EditorPrefs.HasKey(editorPrefsKey))
            {
                string loadedJson = EditorPrefs.GetString(editorPrefsKey, JsonUtility.ToJson(this, false));
                JsonUtility.FromJsonOverwrite(loadedJson, this);
            }
            serializedObject = serializedObject == null ? new SerializedObject(this) : serializedObject;
            //Create the node canvas and panel if it hasn't already been loaded from the json data.
            _nodeCanvas = _nodeCanvas == null ? new NodeCanvas(new Rect(300, 0, position.width, position.height)) : _nodeCanvas;
            _panelTemplate = _panelTemplate == null ? new PanelTemplate(new Rect(0, 0, 300, position.height)) : _panelTemplate;
            //Create serialized properties of the canvas and panel.
            _spNodeCanvas = _spNodeCanvas == null ? serializedObject.FindProperty("_nodeCanvas") : _spNodeCanvas;
            _spPanelTemplate = _spPanelTemplate == null ? serializedObject.FindProperty("_panelTemplate") : _spPanelTemplate;

            _nodeCanvas.EventSetUp();
        }
        private void OnDisable()
        {
            _nodeCanvas.EventCleanUp();
            //Save any items that are tagged to be serialized into json data, then store them into the editor prefs key.
            string savedJson = JsonUtility.ToJson(this, false);
            EditorPrefs.SetString(editorPrefsKey, savedJson);
        }
        private void OnGUI()
        {
            serializedObject.Update();

            if (_panelTemplate.expandCanvas)
            {
                _nodeCanvas.canvasRect.width = position.width;
                _nodeCanvas.canvasRect.height = position.height;
            }
            _nodeCanvas.Draw(Event.current);

            _panelTemplate.panelRect.height = position.height;
            _panelTemplate.Draw(Event.current);

            ProcessPanelEvents(Event.current);
            ProcessCanvasEvents(Event.current);

            serializedObject.ApplyModifiedProperties();
            if (GUI.changed)
            {
                window.Repaint();
            }
        }
        private void ProcessCanvasEvents(Event e)
        {
            if (_nodeCanvas != null)
            {
                if (_nodeCanvas.ProcessEvents(e))
                {
                    GUI.changed = true;
                }
            }
        }
        private void ProcessPanelEvents(Event e)
        {
            if (_panelTemplate != null)
            {
                if (_panelTemplate.ProcessEvents(e))
                {
                    GUI.changed = true;
                }
            }
        }
    }
}

