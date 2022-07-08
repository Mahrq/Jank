using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace Mahrq
{
    /// <summary>
    /// Resource -          https://gram.gs/gramlog/creating-node-based-editor-unity/
    /// 
    /// A node canvas is the area in which you are able to draw nodes and process their events.
    /// Making this into it's own class with a customisable rect makes this a modular item and doesn't
    /// have to make the entire editor window the canvas.
    /// </summary>
    [Serializable]
    public class NodeCanvas
    {
        public Rect canvasRect;
        [SerializeReference]
        private List<EditorNode> nodesToDraw;
        [SerializeReference]
        private List<NodeConnection> connectionsToDraw;
        private NodeConnectionPoint selectedInPoint;
        private NodeConnectionPoint selectedOutPoint;
        [SerializeField]
        private GUIStyle style;

        private bool isDraggingCanvas = false;
        private Vector2 dragCanvasDelta;
        private Vector2 gridOffset;
        public NodeCanvas(Rect rect, GUIStyle style = null)
        {
            this.style = style;
            canvasRect = rect;
        }
        public void EventSetUp()
        {
            if (nodesToDraw == null)
            {
                nodesToDraw = new List<EditorNode>();
            }
            if (connectionsToDraw == null)
            {
                connectionsToDraw = new List<NodeConnection>();
            }
            EventCleanUp();
            for (int i = 0; i < nodesToDraw.Count; i++)
            {
                nodesToDraw[i].OnRemoveNode += OnRemoveNodeCallback;
                nodesToDraw[i].inPoint.OnClickConnectionPoint += OnClickConnectionPointCallback;
                nodesToDraw[i].outPoint.OnClickConnectionPoint += OnClickConnectionPointCallback;
            }
            for (int i = 0; i < connectionsToDraw.Count; i++)
            {
                connectionsToDraw[i].OnClickRemoveConnection += OnClickRemoveConnectionCallback;
            }
        }
        public void EventCleanUp()
        {
            if (nodesToDraw != null)
            {
                for (int i = 0; i < nodesToDraw.Count; i++)
                {
                    nodesToDraw[i].OnRemoveNode -= OnRemoveNodeCallback;
                    nodesToDraw[i].inPoint.OnClickConnectionPoint -= OnClickConnectionPointCallback;
                    nodesToDraw[i].outPoint.OnClickConnectionPoint -= OnClickConnectionPointCallback;
                }
            }
            if (connectionsToDraw != null)
            {
                for (int i = 0; i < connectionsToDraw.Count; i++)
                {
                    connectionsToDraw[i].OnClickRemoveConnection -= OnClickRemoveConnectionCallback;
                }
            }
        }
        #region GUI Draw
        public void Draw(Event e)
        {
            if (style != null)
            {
                GUI.Box(canvasRect, "", style);
            }
            else
            {
                GUI.Box(canvasRect, "");
            }
            //Smaller Inner grid
            DrawGrid(20f, 0.1f, Color.magenta, canvasRect);
            //Big Grid
            DrawGrid(100f, 0.2f, Color.grey, canvasRect);
            DrawNodes();
            DrawConnections();
            //Draws the connection line when a connection point has been clicked but not fully connected.
            DrawConnectionLine(e);
        }
        private void DrawNodes()
        {
            if (nodesToDraw != null)
            {
                for (int i = 0; i < nodesToDraw.Count; i++)
                {
                    nodesToDraw[i].DrawNode();
                }
            }
        }
        private void DrawConnections()
        {
            if (connectionsToDraw != null)
            {
                for (int i = 0; i < connectionsToDraw.Count; i++)
                {
                    connectionsToDraw[i].DrawConnection();
                }
            }
        }
        //Draws connection line when either connection point is clicked from point to mouse position.
        //Indication that you are trying to make a connection.
        public void DrawConnectionLine(Event e)
        {
            //Draw connection line from in point
            if (selectedInPoint != null && selectedOutPoint == null)
            {
                Handles.DrawBezier(
                    selectedInPoint.rect.center,
                    e.mousePosition,
                    selectedInPoint.rect.center + Vector2.left * 50f,
                    e.mousePosition - Vector2.left * 50f,
                    Color.white,
                    null,
                    2f);
                GUI.changed = true;
            }
            //Draw connection line from out point
            if (selectedOutPoint != null && selectedInPoint == null)
            {
                Handles.DrawBezier(
                selectedOutPoint.rect.center,
                e.mousePosition,
                selectedOutPoint.rect.center + Vector2.left * 50f,
                e.mousePosition - Vector2.left * 50f,
                Color.white,
                null,
                2f);
                GUI.changed = true;
            }
        }
        //Draws grid lines based on the given rect.
        private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor, Rect gridBounds)
        {
            int widthDivision = Mathf.CeilToInt(gridBounds.width / gridSpacing);
            int heightDivision = Mathf.CeilToInt(gridBounds.height / gridSpacing);

            Handles.BeginGUI();
            Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

            //Also shifts the grid when canvas dragging.
            gridOffset += dragCanvasDelta * 0.5f;
            Vector3 newOffset = new Vector3(gridOffset.x % gridSpacing, gridOffset.y % gridSpacing, 0);
            //Draws the veritcal lines of the grid.
            for (int i = 0; i < widthDivision; i++)
            {
                Handles.DrawLine(new Vector3((gridSpacing * i) + gridBounds.x, -gridSpacing + gridBounds.y, 0f) + newOffset,
                    new Vector3(gridSpacing * i + gridBounds.x, gridBounds.height + gridBounds.y, 0) + newOffset);
            }
            //Draws the horizontal lines of the grid.
            for (int i = 0; i < heightDivision; i++)
            {
                Handles.DrawLine(new Vector3(-gridSpacing + gridBounds.x, (gridSpacing * i) + gridBounds.y, 0f) + newOffset,
                    new Vector3(gridBounds.width + gridBounds.x, (gridSpacing * i) + gridBounds.y, 0) + newOffset);
            }
            Handles.EndGUI();
        }
        //Menu on right click to create a node.
        private void DrawContextMenu(Vector2 mousePosition)
        {
            GenericMenu contextMenu = new GenericMenu();
            contextMenu.AddItem(new GUIContent("Add Blue Node"), false, () => ContextMenuCreateNode(mousePosition, EditorNodeType.Blue));
            contextMenu.AddItem(new GUIContent("Add Teal Node"), false, () => ContextMenuCreateNode(mousePosition, EditorNodeType.Teal));
            contextMenu.AddItem(new GUIContent("Add Green Node"), false, () => ContextMenuCreateNode(mousePosition, EditorNodeType.Green));
            contextMenu.AddItem(new GUIContent("Add Yellow Node"), false, () => ContextMenuCreateNode(mousePosition, EditorNodeType.Yellow));
            contextMenu.AddItem(new GUIContent("Add Orange Node"), false, () => ContextMenuCreateNode(mousePosition, EditorNodeType.Orange));
            contextMenu.AddItem(new GUIContent("Add Red Node"), false, () => ContextMenuCreateNode(mousePosition, EditorNodeType.Red));
            contextMenu.ShowAsContext();
        }
        private void ContextMenuCreateNode(Vector2 mousePosition, EditorNodeType type)
        {
            if (nodesToDraw == null)
            {
                nodesToDraw = new List<EditorNode>();
            }
            EditorNode node = new EditorNode(mousePosition, 150, 70, canvasRect, type);
            nodesToDraw.Add(node);
            EventSetUp();
        }
        #endregion
        private void CreateConnection()
        {
            if (connectionsToDraw == null)
            {
                connectionsToDraw = new List<NodeConnection>();
            }
            connectionsToDraw.Add(new NodeConnection(selectedInPoint, selectedOutPoint, ConnectionStyle.Bezier));
            EventSetUp();
        }
        private void ClearConnectionSelection()
        {
            selectedInPoint = null;
            selectedOutPoint = null;
        }
        private void OnDragCanvas(Vector2 delta, List<EditorNode> nodes)
        {
            dragCanvasDelta = delta;
            if (nodes != null)
            {
                for (int i = 0; i < nodes.Count; i++)
                {
                    nodes[i].DragNode(delta);
                }
            }
            GUI.changed = true;
        }
        public bool ProcessEvents(Event e)
        {
            dragCanvasDelta = Vector2.zero;
            //only process node events when the mouse is in the canvas
            if (canvasRect.Contains(e.mousePosition))
            {
                ProcessNodeEvents(e);
            }
            switch (e.type)
            {
                case EventType.MouseDown:
                    //only respond to events when the mouse is in the canvas
                    if (canvasRect.Contains(e.mousePosition))
                    {
                        //Left click
                        if (e.button == 0)
                        {
                            ClearConnectionSelection();
                            e.Use();
                            return true;
                        }
                        //Right click
                        if (e.button == 1)
                        {
                            //while having a connection point selected, right click will clear that selection.
                            if (selectedInPoint != null || selectedOutPoint != null)
                            {
                                ClearConnectionSelection();
                            }
                            else
                            {
                                DrawContextMenu(e.mousePosition);
                            }
                            e.Use();
                            return true;
                        }
                        //Middle click
                        if (e.button == 2)
                        {
                            //enable canvas dragging when middle clicking on the canvas
                            isDraggingCanvas = true;
                            e.Use();
                            return true;
                        }
                    }
                    break;
                case EventType.MouseUp:
                    //Disables canvas dragging when middle click is released anywhere.
                    if (e.button == 2)
                    {
                        isDraggingCanvas = false;
                        e.Use();
                        return true;
                    }
                    break;
                case EventType.MouseMove:
                    break;
                case EventType.MouseDrag:
                    //Holding middle click while moving the mouse drags the canvas.
                    if (isDraggingCanvas)
                    {
                        if (e.button == 2)
                        {
                            OnDragCanvas(e.delta, nodesToDraw);
                            e.Use();
                            return true;
                        }
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
        private void ProcessNodeEvents(Event e)
        {
            if (nodesToDraw != null)
            {
                bool guiChanged = false;
                //Traverses the node list backwards, because the last node is drawn at the top, so it should process the events first.
                for (int i = nodesToDraw.Count - 1; i >= 0; i--)
                {
                    guiChanged = nodesToDraw[i].ProcessEvents(e);
                    if (guiChanged)
                    {
                        GUI.changed = true;
                    }
                }
            }
        }
        #region Callbacks
        //Sender NodeConnectionPoint.cs
        private void OnClickConnectionPointCallback(NodeConnectionPoint point)
        {
            switch (point.type)
            {
                case ConnectionType.In:
                    selectedInPoint = point;
                    if (selectedOutPoint != null)
                    {
                        //Check if the connection is not pointing to the same node.
                        if (selectedInPoint.node != selectedOutPoint.node)
                        {
                            CreateConnection();
                            ClearConnectionSelection();
                        }
                        else
                        {
                            ClearConnectionSelection();
                        }
                    }
                    break;
                case ConnectionType.Out:
                    selectedOutPoint = point;
                    if (selectedInPoint != null)
                    {
                        if (selectedOutPoint.node != selectedInPoint.node)
                        {
                            CreateConnection();
                            ClearConnectionSelection();
                        }
                        else
                        {
                            ClearConnectionSelection();
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        //Sender EditorNode.cs
        private void OnRemoveNodeCallback(EditorNode node)
        {
            //Check for connections
            if (connectionsToDraw != null)
            {
                List<NodeConnection> connectionsToRemove = new List<NodeConnection>();
                //Check if any of those connections are linked to the target node
                for (int i = 0; i < connectionsToDraw.Count; i++)
                {
                    if (connectionsToDraw[i].inPoint == node.inPoint || connectionsToDraw[i].outPoint == node.outPoint)
                    {
                        connectionsToRemove.Add(connectionsToDraw[i]);
                    }
                }
                //Remove the connections linked to the target node
                if (connectionsToRemove.Count > 0)
                {
                    for (int i = 0; i < connectionsToRemove.Count; i++)
                    {
                        connectionsToDraw.Remove(connectionsToRemove[i]);
                    }
                    connectionsToRemove = null;
                }
            }
            //Finally remove the node
            nodesToDraw.Remove(node);
            EventSetUp();
        }
        //Sender NodeConnection.cs
        private void OnClickRemoveConnectionCallback(NodeConnection connection)
        {
            if (connectionsToDraw != null)
            {
                if (connectionsToDraw.Count > 0)
                {
                    connectionsToDraw.Remove(connection);
                    EventSetUp();
                }
            }
        }
        #endregion
    }
}
