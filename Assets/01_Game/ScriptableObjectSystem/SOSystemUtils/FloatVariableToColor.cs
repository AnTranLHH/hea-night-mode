using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using SoSystem;
using UnityEngine;
using UnityEngine.UI;

public class FloatVariableToColor : MonoBehaviour
{
    [SerializeField]
    private FloatVariable _refFloat;
    [SerializeField]
    private Color[] _colors;
    [SerializeField]
    private Image _outputColorImage;

    private float _maxValue = 1f;
    private int _currentColorIndex = 0;

    private Tween _updateColorTween;

    public void SetMaxValue(float max)
    {
        _maxValue = max;
    }

    private void UpdateColor(float newValue)
    {
        _maxValue = Mathf.Max(_maxValue, newValue);
        float normalizedValue = newValue / _maxValue;
        int newColorIndex = 0;
        for (int i = 0; i < _colors.Length; i++)
        {
            if ((float)i / _colors.Length <= normalizedValue)
            {
                newColorIndex = i;
            }
        }
        if (newColorIndex == _currentColorIndex)
        {
            return;
        }
        _updateColorTween?.Kill();
        Color from = _colors[_currentColorIndex];
        Color tweenColor = from;
        Color to = _colors[newColorIndex];
        _currentColorIndex = newColorIndex;
        _updateColorTween = DOTween.To(() => tweenColor, val => tweenColor = val, to, 3f).SetEase(Ease.OutElastic).OnUpdate(() =>
        {
            _outputColorImage.color = tweenColor;
        });
    }

    private void OnEnable()
    {
        _currentColorIndex = 0;
        _refFloat.OnValueChange += UpdateColor;
        UpdateColor(_refFloat.Value);
    }

    private void OnDisable()
    {
        _refFloat.OnValueChange -= UpdateColor;
    }
}
