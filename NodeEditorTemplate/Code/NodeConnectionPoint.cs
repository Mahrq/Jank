using System;
using UnityEngine;
namespace Mahrq
{
    [Serializable]
    public enum ConnectionType
    {
        In,
        Out
    }
    /// <summary>
    /// Connection points are the small buttons on the edges of nodes that allow them to, when clicked, draw the origin of connection lines.
    /// An in type connection point must pair with an out connection point to fully connect the nodes.
    /// </summary>
    [Serializable]
    public class NodeConnectionPoint
    {
        [SerializeField]
        private Rect _rect;
        [SerializeField]
        private ConnectionType _type;
        [SerializeReference]
        private EditorNode _node;
        [SerializeReference]
        private GUIStyle style;
        [SerializeReference]
        private Rect canvasBounds;
        public event Action<NodeConnectionPoint> OnClickConnectionPoint;
        public Rect rect => _rect;
        public EditorNode node => _node;
        public ConnectionType type => _type;
        public NodeConnectionPoint(EditorNode node, ConnectionType type, GUIStyle style, Rect canvasBounds)
        {
            this._node = node;
            this._type = type;
            this.style = style;
            this.canvasBounds = canvasBounds;
            this._rect = new Rect(0, 0, 10, 20);
        }
        public void DrawConnectionPoint()
        {
            //Position the connection point to the middle of the node.
            _rect.y = _node.rect.y + (_node.rect.height * 0.5f) - _rect.height * 0.5f;
            //Then based on the type, off set them to the left for out and to the right for in
            switch (_type)
            {
                case ConnectionType.In:
                    _rect.x = _node.rect.x - _rect.width + 8f;
                    break;
                case ConnectionType.Out:
                    _rect.x = _node.rect.x + _node.rect.width - 8f;
                    break;
                default:
                    break;
            }
            if (GUI.Button(_rect, "", style))
            {
                if (canvasBounds.Contains(new Vector2(_rect.x, _rect.y)))
                {
                    OnClickConnectionPoint?.Invoke(this);
                }
            }
        }
    }
}

