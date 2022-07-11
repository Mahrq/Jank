using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace Mahrq
{
    [Serializable]
    public class PanelTemplate
    {
        [SerializeReference]
        public Rect panelRect;
        [SerializeReference]
        protected GUISkin skin;
        [SerializeReference]
        protected GUIStyle style;
        protected Vector2 scrollPos;

        public PanelTemplate()
        {

        }
        public PanelTemplate(Rect rect, GUIStyle style = null)
        {
            this.panelRect = rect;
            this.style = style;
            InitialiseSkin();
        }
        public PanelTemplate (Rect rect, GUISkin skin = null)
        {
            this.panelRect = rect;
            this.skin = skin;
            this.style = skin.box;
            InitialiseSkin();
        }
        public virtual void Draw(Event e)
        {
            GUI.Box(panelRect, "", style);
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(panelRect.width));
            EditorGUILayout.BeginVertical();

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }

        public virtual bool ProcessEvents(Event e)
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
        protected virtual void InitialiseSkin()
        {
            if (style == null)
            {
                style = new GUIStyle();
                style.normal.background = (Texture2D)EditorGUIUtility.Load("builtin skins/darkskin/images/objectpickerbackground.png");
            }
        }
    }
}

