using System;
using DG.Tweening;
using SoSystem;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Vacuumable : MonoBehaviour
{
    [Header("Reference - Read")]
    [SerializeField]
    private Vector3Variable _holeTop;
    [SerializeField]
    private Vector3Variable _holeBot;
    [SerializeField]
    private IntegerVariable _playerLevel;

    public UnityEvent OnVacuumed;

    [Header("Configs")]
    [SerializeField]
    private int _level;
    [SerializeField]
    private Collider _collider;
    [SerializeField]
    private Transform _target;
    [SerializeField]
    private int _score;
    [SerializeField]
    private float _vacuumeTime;
    [SerializeField]
    private VacuumeType _vacuumeType;

    private Tweener _vacuumeTween;

    public int GetScore()
    {
        return _score;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (_playerLevel.Value < _level)
        {
            return;
        }

        BeVacuumed();
    }

    private void BeVacuumed()
    {
        _collider.enabled = false;

        if (_vacuumeType == VacuumeType.Ghost)
        {
            return;
        }

        Span<Vector3> path = stackalloc Vector3[3];
        path[0] = _target.transform.position;
        path[1] = _holeTop.Value;
        path[2] = _holeBot.Value;
        _target.transform.DOPath(path.ToArray(), _vacuumeTime, PathType.CatmullRom, PathMode.Full3D, 5);
        _target.transform.DOScale(0f, 1f);

        OnVacuumed.Invoke();
    }
}

public enum VacuumeType
{
    Regular = 0,
    Ghost = 1
}
