using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Attach to tile objects to access their properties.
/// </summary>
public class WalkableTile : MonoBehaviour
{
    [SerializeField]
    private Type _type = Type.Grass;
    public Type type => _type;
    public Vector3 position => _transform.position;
    private Transform _t;

    [System.Flags]
    public enum Type
    {
        None = 0,
        Grass = 1,
        Water = 1 << 1
    }

    private Transform _transform
    {
        get 
        {
            if (_t == null)
            {
                _t = this.GetComponent<Transform>();
            }
            return _t;
        }
    }


}
