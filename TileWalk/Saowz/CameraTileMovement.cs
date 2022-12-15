using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Camera for tracking the player in bird's eye view
/// </summary>
public class CameraTileMovement : MonoBehaviour
{
    [SerializeField]
    private Transform _playerTransform;
    [SerializeField]
    [Range(1f, 20f)]
    private float _height = 13f;
    [SerializeField]
    [Range(1f, 10f)]
    private float _moveSpeed = 8.5f;
    private Transform _transform;
    private Vector3 _targetPosition;

    private void Awake()
    {
        _transform = this.GetComponent<Transform>();
        _transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));
    }

    private void LateUpdate()
    {
        _targetPosition = new Vector3(_playerTransform.position.x, _height, _playerTransform.position.z);

        _transform.position = Vector3.MoveTowards(_transform.position, _targetPosition, _moveSpeed * Time.deltaTime);
    }
}
