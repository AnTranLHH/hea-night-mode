using System;
using DG.Tweening;
using SoSystem;
using UnityEngine;

public class Vacuumable : MonoBehaviour
{
    [Header("Reference - Read")]
    [SerializeField]
    private Vector3Variable _holeTop;
    [SerializeField]
    private Vector3Variable _holeBot;

    [Header("Configs")]
    [SerializeField]
    private Collider _collider;
    [SerializeField]
    private Transform _target;
    [SerializeField]
    private int _store;
    [SerializeField]
    private float _vacuumeTime;


    public int GetScore()
    {
        return _store;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        BeVacuumed();
    }
    
    private void BeVacuumed()
    {
        _collider.enabled = false;

        Span<Vector3> path = stackalloc Vector3[3];
        path[0] = _target.transform.position;
        path[1] = _holeTop.Value;
        path[2] = _holeBot.Value;
        _target.transform.DOPath(path.ToArray(), _vacuumeTime, PathType.CatmullRom, PathMode.Full3D, 5);
        _target.transform.DOScale(0f, 1f);
    }
}
