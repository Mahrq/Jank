using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace Mahrq
{ 
    [Serializable]
    public class PanelTemplate
    {
        public Rect panelRect;
        [SerializeReference]
        private GUIStyle style;
        private Vector2 scrollPos;

        [SerializeField]
        private string _exampleText;
        [SerializeField]
        private int _exampleInt;
        [SerializeField]
        private float _exampleFloat;
        [SerializeField]
        private Rect _nodeCanvasRect;
        public bool expandCanvas;
        public PanelTemplate(Rect rect, GUIStyle style = null)
        {
            this.panelRect = rect;
            this.style = style;
            InitialiseSkin();
        }
        public void Draw(Event e)
        {
            GUI.Box(panelRect, "", style);
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(panelRect.width));
            EditorGUILayout.BeginVertical();
            _exampleText = EditorGUILayout.TextField("Example Text", _exampleText);
            EditorGUILayout.Space(2);
            _exampleInt = EditorGUILayout.IntField("Example Int", _exampleInt);
            EditorGUILayout.Space(2);
            _exampleFloat = EditorGUILayout.FloatField("Example Float", _exampleFloat);
            EditorGUILayout.Space(2);
            _nodeCanvasRect = EditorGUILayout.RectField("Node Canvas Rect", _nodeCanvasRect);
            EditorGUILayout.Space(2);
            expandCanvas = EditorGUILayout.Toggle("Expand canvas to window", expandCanvas);
            EditorGUILayout.Space(6);
            if (GUILayout.Button("Set Node Canvas Rect"))
            {
                NodeEditorTemplate.window.nodeCanvas.canvasRect = new Rect(_nodeCanvasRect);
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }

        public bool ProcessEvents(Event e)
        {
            switch (e.type)
            {
                case EventType.MouseDown:
                    break;
                case EventType.MouseUp:
                    break;
                case EventType.MouseMove:
                    break;
                case EventType.MouseDrag:
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
        /// <summary>
        /// built in resource -     https://gist.github.com/Geri-Borbas/bb9d77b6444524857391a0e4822ca6c0
        /// </summary>
        private void InitialiseSkin()
        {
            if (style == null)
            {
                style = new GUIStyle();
                style.normal.background = (Texture2D)EditorGUIUtility.Load("builtin skins/darkskin/images/objectpickerbackground.png");
            }
        }
    }
}

