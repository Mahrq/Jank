using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace Mahrq
{
    [Serializable]
    public class EditorNode
    {
        [SerializeField]
        private EditorNodeType _type;
        [SerializeField]
        private Rect _rect;
        [SerializeField]
        private Rect canvasBounds;
        [SerializeField]
        private string title;
        private bool isDragged;
        private bool isSelected;

        [SerializeReference]
        private GUIStyle currentStyle;
        [SerializeReference]
        private GUIStyle defaultStyle;
        [SerializeReference]
        private GUIStyle selectedStyle;

        [SerializeReference]
        private NodeConnectionPoint _inPoint;
        [SerializeReference]
        private NodeConnectionPoint _outPoint;
        [SerializeReference]
        private GUIStyle inPointStyle;
        [SerializeReference]
        private GUIStyle outPointStyle;

        public EditorNodeType type => _type;
        public event Action<EditorNode> OnRemoveNode;
        public Rect rect => _rect;
        public NodeConnectionPoint inPoint => _inPoint;
        public NodeConnectionPoint outPoint => _outPoint;
        public EditorNode(Vector2 position, float width, float height, Rect canvasBounds, EditorNodeType type)
        {
            this._rect = new Rect(position.x, position.y, width, height);
            this.canvasBounds = canvasBounds;
            this._type = type;
            InitialiseSkin(type);
            this._inPoint = new NodeConnectionPoint(this, ConnectionType.In, inPointStyle, canvasBounds);
            this._outPoint = new NodeConnectionPoint(this, ConnectionType.Out, outPointStyle, canvasBounds);
        }
        //For canvas dragging, moves all nodes.
        public void DragNode(Vector2 deltaPosition)
        {
            _rect.position += deltaPosition;
        }
        //For single node dragging, center the node to the mouse position
        public void DragNode(Event e)
        {
            Vector2 mouseCenter = e.mousePosition;
            mouseCenter.x -= _rect.width * 0.5f;
            mouseCenter.y -= _rect.height * 0.5f;
            _rect.position = mouseCenter;
        }
        public void DrawNode()
        {
            _inPoint.DrawConnectionPoint();
            _outPoint.DrawConnectionPoint();
            GUI.Box(_rect, title, currentStyle);
        }
        private void DrawContextMenu()
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Remove Node"), false, ContextMenuRemoveNode);
            menu.ShowAsContext();
        }
        private void ContextMenuRemoveNode()
        {
            OnRemoveNode?.Invoke(this);
        }
        public bool ProcessEvents(Event e)
        {
            switch (e.type)
            {
                case EventType.MouseDown:
                    //Left click
                    if (e.button == 0)
                    {
                        //Become the selected node when the cursor is on the node when the button is clicked.
                        if (_rect.Contains(e.mousePosition))
                        {
                            isDragged = true;
                            isSelected = true;
                            currentStyle = selectedStyle;
                        }
                        else
                        {
                            isSelected = false;
                            currentStyle = defaultStyle;
                        }
                        GUI.changed = true;
                    }
                    //Right click
                    if (e.button == 1)
                    {
                        //Brings up remove node context menu if the node is selected.
                        if (isSelected && _rect.Contains(e.mousePosition))
                        {
                            DrawContextMenu();
                            e.Use();
                        }
                    }
                    break;
                case EventType.MouseUp:
                    if (e.button == 0)
                    {
                        isDragged = false;
                    }
                    break;
                case EventType.MouseMove:
                    break;
                case EventType.MouseDrag:
                    if (e.button == 0 && isDragged)
                    {
                        DragNode(e);
                        e.Use();
                        return true;
                    }
                    break;
                case EventType.KeyDown:
                    //Delete Selected node when pressing delete key.
                    if (isSelected)
                    {
                        if (e.keyCode == KeyCode.Delete)
                        {
                            OnRemoveNode?.Invoke(this);
                            e.Use();
                        }
                    }
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
        /// template -              builtin skins/darkskin/images/node1.png
        ///                         builtin skins/darkskin/images/node1 on.png                      
        /// node1 -                 blue
        /// node2 -                 teal
        /// node3 -                 green
        /// node4 -                 yellow
        /// node5 -                 orange
        /// node6 -                 red
        /// </summary>
        private void InitialiseSkin(EditorNodeType type)
        {
            defaultStyle = new GUIStyle();
            selectedStyle = new GUIStyle();
            RectOffset nodeBorder = new RectOffset(10, 10, 10, 10);
            RectOffset pointBorder = new RectOffset(4, 4, 12, 12);
            switch (type)
            {
                case EditorNodeType.Blue:
                    defaultStyle.normal.background = (Texture2D)EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png");
                    defaultStyle.border = nodeBorder;
                    selectedStyle.normal.background = (Texture2D)EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png");
                    selectedStyle.border = nodeBorder;
                    break;
                case EditorNodeType.Teal:
                    defaultStyle.normal.background = (Texture2D)EditorGUIUtility.Load("builtin skins/darkskin/images/node2.png");
                    defaultStyle.border = nodeBorder;
                    selectedStyle.normal.background = (Texture2D)EditorGUIUtility.Load("builtin skins/darkskin/images/node2 on.png");
                    selectedStyle.border = nodeBorder;
                    break;
                case EditorNodeType.Green:
                    defaultStyle.normal.background = (Texture2D)EditorGUIUtility.Load("builtin skins/darkskin/images/node3.png");
                    defaultStyle.border = nodeBorder;
                    selectedStyle.normal.background = (Texture2D)EditorGUIUtility.Load("builtin skins/darkskin/images/node3 on.png");
                    selectedStyle.border = nodeBorder;
                    break;
                case EditorNodeType.Yellow:
                    defaultStyle.normal.background = (Texture2D)EditorGUIUtility.Load("builtin skins/darkskin/images/node4.png");
                    defaultStyle.border = nodeBorder;
                    selectedStyle.normal.background = (Texture2D)EditorGUIUtility.Load("builtin skins/darkskin/images/node4 on.png");
                    selectedStyle.border = nodeBorder;
                    break;
                case EditorNodeType.Orange:
                    defaultStyle.normal.background = (Texture2D)EditorGUIUtility.Load("builtin skins/darkskin/images/node5.png");
                    defaultStyle.border = nodeBorder;
                    selectedStyle.normal.background = (Texture2D)EditorGUIUtility.Load("builtin skins/darkskin/images/node5 on.png");
                    selectedStyle.border = nodeBorder;
                    break;
                case EditorNodeType.Red:
                    defaultStyle.normal.background = (Texture2D)EditorGUIUtility.Load("builtin skins/darkskin/images/node6.png");
                    defaultStyle.border = nodeBorder;
                    selectedStyle.normal.background = (Texture2D)EditorGUIUtility.Load("builtin skins/darkskin/images/node6 on.png");
                    selectedStyle.border = nodeBorder;
                    break;
                default:
                    break;
            }
            currentStyle = defaultStyle;

            inPointStyle = new GUIStyle();
            inPointStyle.normal.background = (Texture2D)EditorGUIUtility.Load("builtin skins/darkskin/images/btn left.png");
            inPointStyle.active.background = (Texture2D)EditorGUIUtility.Load("builtin skins/darkskin/images/btn left on.png");
            inPointStyle.border = pointBorder;

            outPointStyle = new GUIStyle();
            outPointStyle.normal.background = (Texture2D)EditorGUIUtility.Load("builtin skins/darkskin/images/btn right.png");
            outPointStyle.active.background = (Texture2D)EditorGUIUtility.Load("builtin skins/darkskin/images/btn right on.png");
            outPointStyle.border = pointBorder;
        }

    }
    [Serializable]
    public enum EditorNodeType
    {
        Blue,
        Teal,
        Green,
        Yellow,
        Orange,
        Red
    }
}

