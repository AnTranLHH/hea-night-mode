using System;
using SoSystem;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Reference - Read")]
    [SerializeField]
    private Vector3Variable _direction;
    [SerializeField]
    private FloatVariable _playerSpeed;

    [Header("Configs")]
    [SerializeField]
    private Transform _player;


    private void UpdateMovement(Vector3 newDir)
    {
        _player.position += newDir * _playerSpeed.Value;
    }

    private void OnEnable()
    {
        _direction.OnValueChange += UpdateMovement;
    }

    private void OnDisable()
    {
        _direction.OnValueChange -= UpdateMovement;
    }
}
