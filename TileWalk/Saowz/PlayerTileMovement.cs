using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Player movement where movement is restricted to a given tile.
/// 
/// Requires tiles set up with WalkableTile scripts.
/// </summary>
public class PlayerTileMovement : MonoBehaviour
{
    [SerializeField]
    private Transform _tileScannerTransform;
    [SerializeField]
    [Range(1, 10)]
    private int _tileSpacing = 4; // use to match the spacing of your own tiles.
    [SerializeField]
    private WalkableTile.Type _canWalkOn = WalkableTile.Type.Grass;
    private Transform _transform;
    private Vector3 _inputVector;
    private float _hInput;
    private float _vInput;
    [SerializeField]
    private bool _movementInProgress = false;
    private Vector3 _tileToMoveToPosition;

    [SerializeField]
    private float _movementSpeed = 10f;

    private void Awake()
    {
        _transform = this.GetComponent<Transform>();
        //Tile scanner will always be 1 tile worth ahead of the player.
        _tileScannerTransform.localPosition = new Vector3(0, 2, _tileSpacing);
    }

    private void Update()
    {
        _hInput = Input.GetAxisRaw("Horizontal");
        _vInput = Input.GetAxisRaw("Vertical");

        if (_movementInProgress)
        {
            //While movement is in progress, move the player towards the next tile.
            _transform.position = Vector3.MoveTowards(_transform.position, _tileToMoveToPosition, _movementSpeed * Time.deltaTime);
            //If the player is practically at the tile position, snap it at the location then finish the movement sequence.
            if (Vector3.Distance(_transform.position, _tileToMoveToPosition) <= 0.001f)
            {
                _transform.position = _tileToMoveToPosition;
                _movementInProgress = false;
            }
        }
        else
        {
            //Only allow input while the player is not currently moving.
            if (_hInput != 0 || _vInput != 0)
            {
                _movementInProgress = true;
                if (_hInput != 0 && _vInput != 0)
                {
                    /*Priority given to vertical movement when both input pressed.
                    Invert comment for opposite effect or comment out both if no movement should happen when both pressed.*/
                    _hInput = 0;
                    //_vInput = 0;
                }
                _inputVector = new Vector3(_hInput, 0, _vInput);
                //turns the player towards the direction pressed.
                _transform.forward = _inputVector;
                if (!ScanTile(_canWalkOn, ref _tileToMoveToPosition))
                {
                    _movementInProgress = false;
                }
            }
        }
    }
    /// <summary>
    /// Fires a raycast from the scanner down towards the tiles.
    /// Returns true if the tile matches the filter.
    /// </summary>
    private bool ScanTile(WalkableTile.Type filter, ref Vector3 tilePosition) 
    {
        Ray ray = new Ray(_tileScannerTransform.position, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 10f, LayerMask.GetMask("WalkableTile")))
        {
            WalkableTile tile;
            if (hit.transform.TryGetComponent<WalkableTile>(out tile))
            {
                if ((filter & tile.type) != 0)
                {
                    tilePosition = tile.position;
                    tilePosition.y = 0f;

                    Debug.Log($"Scanned Tile: {tile.type} Position: {tilePosition}");
                    return true;
                }
                else
                {
                    Debug.LogWarning("Cant Walk Here");
                }
            }
        }
        return false;
    }
}
