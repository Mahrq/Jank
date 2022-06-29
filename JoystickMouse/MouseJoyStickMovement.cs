#define ROTATION_MOVEMENT
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Mouse with joystick like behaviour meaning it uses the centre of the screen and the mouse position to
/// determine the directional input to move a transform.
/// </summary>
public class MouseJoyStickMovement : MonoBehaviour
{
    private Transform _transform;
    private Vector3 _mousePosition = Vector3.zero;
    private Vector3 _mouseDirection = Vector3.zero;
    private Vector3 _movementDirection = Vector3.zero;
    private Vector3 _joystickCenter = Vector3.zero;
    private Vector3 _newRotation = Vector3.zero;
    private float _accelerator = 1f;

    [Range(0.1f, 200f)]
    public float _moveSpeed = 10f;
    [Range(1, 10f)]
    public float _acceleratorModifier = 2f;
    public bool _acceleratorAffectsRotation = false;
    [Range(0.1f, 10)]
    public float _rotationSpeed = 5f;
    private void Awake()
    {
        _transform = this.GetComponent<Transform>();
        _joystickCenter = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f);
    }
    private void Update()
    {
        //Get mouse position.
        _mousePosition = Input.mousePosition;
        //Get direction relative to center of the screen.
        _mouseDirection = _mousePosition - _joystickCenter;
        //Acceleration value calculated by comparing how far it is from the center.
        _accelerator = GetAcceleratorValue(_mouseDirection, _acceleratorModifier);
        //Normalize to get a non scaled direction.
        _mouseDirection.Normalize();
#if ROTATION_MOVEMENT
        //Movement will always be going forward, the mouse position determines the rotation value.
        _movementDirection = _transform.forward * _moveSpeed * _accelerator * Time.deltaTime;
        //Optional choice if the accelerator will affect the rotation speed.
        _accelerator = _acceleratorAffectsRotation ? _accelerator : 1f;
        //Compares the forward direction to the input direction and tries to match it.
        _newRotation = Vector3.RotateTowards(_transform.forward, new Vector3(_mouseDirection.x, 0, _mouseDirection.y), _rotationSpeed * _accelerator * Time.deltaTime, 0f);
        //Set the new rotation value.
        _transform.rotation = Quaternion.LookRotation(_newRotation);
        //Set the new forward value.
        _transform.position += _movementDirection;
#else
        //Convert mouse direction to world direction
        _movementDirection = new Vector3(_mouseDirection.x, 0, _mouseDirection.y);
        //Scale the direction
        _movementDirection *= _moveSpeed * _accelerator * Time.deltaTime;
        //Move in that direction
        _transform.Translate(_movementDirection);
#endif
    }
    /// <summary>
    /// Calculates the the acceleration value based on how far the mouse position is from the screen.
    /// </summary>
    private float GetAcceleratorValue(Vector3 direction, float modifier)
    {
        //Current direction
        float current = Vector3.Magnitude(direction);
        //The maxed based on the normalized direction times the screen height.
        //This will make the joystick more square like as most screens have more width.
        float max = Vector3.Magnitude(direction.normalized * Screen.height);
        //Get a percentage how far along the current direction is from the max.
        float result = GetPercentage(current, 0, max);
        //the result will always be between 0 and 1 so give it a modifier to make it bigger
        //since this value will be used to modify speed.
        result *= modifier;
        return result;
    }
    private float GetPercentage(float current, float min, float max)
    {
        float result = (current - min) / (max - min);
		result = Mathf.Clamp(result, min, max);
        return result;
    }
}
