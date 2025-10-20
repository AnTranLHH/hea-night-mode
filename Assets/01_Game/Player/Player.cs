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
    [SerializeField]
    private IntegerVariable _score;
    [SerializeField]
    private IntegerVariable _level;
    [SerializeField]
    private Vector3Variable _holeTop;
    [SerializeField]
    private Vector3Variable _holeBot;

    [Header("Events out")]
    [SerializeField]
    private GameEvent _onLevelUp;

    [Header("Configs")]
    [SerializeField]
    private Transform _player;
    [SerializeField]
    private int[] _scoreMileStone;
    [SerializeField]
    private Transform _top;
    [SerializeField]
    private Transform _bot;

    private void Update()
    {
        _player.position += _direction.Value * _playerSpeed.Value * Time.deltaTime;
        _flashLightDuration.Value = Mathf.Max(0f, _flashLightDuration.Value - Time.deltaTime);
        _holeBot.Value = _bot.transform.position;
        _holeTop.Value = _top.transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        var vacuumable = other.gameObject.GetComponent<Vacuumable>();
        UpdateScore(vacuumable);

        var battery = other.gameObject.GetComponent<Battery>();
        if (battery == null)
        {
            return;
        }

        _flashLightDuration.Value = Mathf.Min(_batteryMaxDuration.Value, _flashLightDuration.Value + battery.GetBatteryDuration());
    }
    
    private void UpdateScore(Vacuumable vacuumable)
    {
        if (vacuumable == null)
        {
            return;
        }

        _score.Value += vacuumable.GetScore();
        for (int i = _scoreMileStone.Length - 1; 0 <= i; i--)
        {
            if (_score.Value >= _scoreMileStone[i]) 
            {
                _level.Value = i + 1;
                _onLevelUp.Raise();
                break;
            }
        }
    }
}
