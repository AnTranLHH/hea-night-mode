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
    [SerializeField]
    private FloatVariable _batteryMaxDuration;

    [Header("Reference - Write")]
    [SerializeField]
    private FloatVariable _flashLightDuration;

    [Header("Configs")]
    [SerializeField]
    private Transform _player;

    private void Update()
    {
        _player.position += _direction.Value * _playerSpeed.Value * Time.deltaTime;
        _flashLightDuration.Value = Mathf.Max(0f, _flashLightDuration.Value - Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        var battery = other.gameObject.GetComponent<Battery>();
        if (battery == null)
        {
            return;
        }

        _flashLightDuration.Value = Mathf.Min(_batteryMaxDuration.Value, _flashLightDuration.Value + battery.GetBatteryDuration());
    }
}
