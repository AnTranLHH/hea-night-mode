using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class GhostMovement : MonoBehaviour
{
    [SerializeField]
    private Vector2 _horizontalBound;
    [SerializeField]
    private Vector2 _verticalBound;
    [SerializeField]
    private Transform _target;
    [SerializeField]
    private Vector2 _standByDuration;
    [SerializeField]
    private float _speed;
    [SerializeField]
    private float _floor;

    private CancellationTokenSource _cts;
    private Tweener _moveTween;
    private Tweener _rotateTween;

    public void StopMovementRoutine()
    {
        _moveTween?.Kill();
        _rotateTween?.Kill();
        _cts?.Cancel();
    }

    private void OnEnable()
    {
        Async_StartMovementRoutine().Forget();
    }
    
    private async UniTaskVoid Async_StartMovementRoutine()
    {
        _cts?.Cancel();
        _cts = new CancellationTokenSource();
        while (true)
        {
            Vector3 des = new();
            des.y = _floor;
            des.x = Random.Range(_horizontalBound.x, _horizontalBound.y);
            des.z = Random.Range(_verticalBound.x, _verticalBound.y);
            float distance = Vector3.Distance(_target.position, des);
            float time = distance / _speed;
            Vector3 direction = (des - _target.position).normalized;
            Quaternion rot = Quaternion.LookRotation(direction);
            Vector3 euler = rot.eulerAngles;
            _rotateTween = _target.DORotate(euler, 0.2f).SetEase(Ease.OutSine);
            _moveTween = _target.DOMove(des, time).SetEase(Ease.InOutSine);
            await _moveTween;
            float standByTime = Random.Range(_standByDuration.x, _standByDuration.y);
            await UniTask.WaitForSeconds(standByTime, cancellationToken: _cts.Token);
        }
    }
}
