using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace Mahrq
{
    [Serializable]
    public enum ConnectionStyle
    {
        Bezier,
        Line
    }
    /// <summary>
    /// A connection is the line that is rendered to signify that two nodes have been connected.
    /// </summary>
    [Serializable]
    public class NodeConnection
    {
        [SerializeReference]
        private NodeConnectionPoint _inPoint;
        [SerializeReference]
        private NodeConnectionPoint _outPoint;
        [SerializeField]
        private ConnectionStyle connectionStyle;
        public event Action<NodeConnection> OnClickRemoveConnection;
        public NodeConnectionPoint inPoint => _inPoint;
        public NodeConnectionPoint outPoint => _outPoint;
        public NodeConnection(NodeConnectionPoint inPoint, NodeConnectionPoint outPoint, ConnectionStyle connectionStyle)
        {
            this._inPoint = inPoint;
            this._outPoint = outPoint;
            this.connectionStyle = connectionStyle;
        }

        public void DrawConnection()
        {
            switch (connectionStyle)
            {
                case ConnectionStyle.Bezier:
                    Handles.DrawBezier(
                    _inPoint.rect.center,
                    _outPoint.rect.center,
                    _inPoint.rect.center + Vector2.left * 50f,
                    _outPoint.rect.center + Vector2.left * 50f,
                    Color.magenta,
                    null,
                    2f);
                    break;
                case ConnectionStyle.Line:
                    Handles.color = Color.magenta;
                    Handles.DrawLine(_inPoint.rect.center, _outPoint.rect.center);
                    break;
                default:
                    break;
            }
            //A square button in the middle of the line when clicked will remove the connection;
            if (Handles.Button((_inPoint.rect.center + _outPoint.rect.center) * 0.5f, Quaternion.identity, 8f,8f, Handles.RectangleHandleCap))  
            {
                OnClickRemoveConnection?.Invoke(this);
            }
        }

    }
}

