using DG.Tweening;
using SoSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Joystick : MonoBehaviour, IPointerUpHandler, IDragHandler, IPointerDownHandler
{
    [Header("Reference - Write")]
    [SerializeField]
    private Vector3Variable _direction;

    [Header("Configs")]
    [SerializeField]
    private Camera _uiCamera;
    [SerializeField]
    private Canvas _rootCanvas;
    [SerializeField]
    private CanvasGroup _joystickGroup;
    [SerializeField]
    private Image _joystickControl;
    [SerializeField]
    private Image _outerDragIndicator;
    [SerializeField]
    private Transform _center;
    [SerializeField]
    private float _minDragToActive;
    [SerializeField]
    private float _innerDragRadius;

    private float _sqrThreshold;
    private float _innerDragThresholdSqr;
    private Tweener _backControlTween;
    private Tweener _centerAlphaTween;

    private void OnEnable()
    {
        _sqrThreshold = _minDragToActive * _minDragToActive;
        _innerDragThresholdSqr = _innerDragRadius * _innerDragRadius;
    }

    public void OnDrag(PointerEventData eventData)
    {   
        Vector3 dragPosition = _uiCamera.ScreenToWorldPoint(eventData.position);
        dragPosition.z = _rootCanvas.planeDistance;
        float dragDistance = Vector3.SqrMagnitude(dragPosition - _center.position);
        if (dragDistance < _sqrThreshold)
        {
            return;
        }

        Vector3 uiDirection = dragPosition - _center.position;
        Vector3 worldDirection = new(uiDirection.x, 0f, uiDirection.y);
        _direction.Value = worldDirection.normalized;

        if (dragDistance < _innerDragThresholdSqr)
        {
            _outerDragIndicator.enabled = false;
            _joystickControl.transform.position = dragPosition;
            return;
        }

        _outerDragIndicator.enabled = true;
        _outerDragIndicator.transform.position = dragPosition;
        _joystickControl.transform.position = _center.position + uiDirection.normalized * _innerDragRadius;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Vector3 centerPos = _uiCamera.ScreenToWorldPoint(eventData.position);
        centerPos.z = _rootCanvas.planeDistance;
        _center.position = centerPos;
        _joystickControl.transform.position = centerPos;

        _centerAlphaTween?.Kill();
        _centerAlphaTween = _joystickGroup.DOFade(1f, 0.2f).SetEase(Ease.OutSine);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _backControlTween?.Kill();
        _backControlTween = _joystickControl.transform.DOMove(_center.position, 0.2f).SetEase(Ease.OutSine);
        _centerAlphaTween?.Kill();
        _centerAlphaTween = _joystickGroup.DOFade(0f, 0.2f).SetEase(Ease.OutSine);
        _outerDragIndicator.enabled = false;
        _direction.Value = Vector3.zero;
    }
}
