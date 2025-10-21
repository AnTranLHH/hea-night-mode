using System;
using SoSystem;
using UnityEngine;
using UnityEngine.UI;

public class VisibleMask : MonoBehaviour
{
    [SerializeField]
    private Vector3Variable _playerPosition;
    [SerializeField]
    private FloatVariable _flashDuration;
    [SerializeField]
    private FloatVariable _maxFlashDuration;
    [SerializeField]
    private Vector2 _flashRadiusRange;
    [SerializeField]
    private Camera _mainCam;
    [SerializeField]
    private Camera _uiCam;
    [SerializeField]
    private Image _flashLightMask;

    private void OnEnable()
    {
        _flashDuration.OnValueChange += AdjustFlashRadius;
        AdjustFlashRadius(_flashDuration.Value);
    }

    private void OnDisable()
    {
        _flashDuration.OnValueChange -= AdjustFlashRadius;
    }

    private void Update()
    {
        Vector3 playerScreenPos = _mainCam.WorldToScreenPoint(_playerPosition.Value);
        Vector3 maskPos = _uiCam.ScreenToWorldPoint(playerScreenPos);
        maskPos.z = 3f;
        _flashLightMask.transform.position = maskPos;
    }

    private void AdjustFlashRadius(float duration)
    {
        float scale = MathUtils.Remap(duration, 0f, _maxFlashDuration.Value, _flashRadiusRange.x, _flashRadiusRange.y);
        _flashLightMask.transform.localScale = new(scale, scale, 1f);
    }

}
