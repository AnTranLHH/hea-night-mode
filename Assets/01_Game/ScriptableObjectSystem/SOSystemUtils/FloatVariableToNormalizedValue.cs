using System;
using System.Collections;
using System.Collections.Generic;
using SoSystem;
using UnityEngine;

public class FloatVariableToNormalizedValue : MonoBehaviour
{
    [SerializeField]
    protected FloatVariable _refFloat;
    protected float _normalizedValue;
    public event Action<float> OnNormalizedValueChange;

    private float _maxValue = 1f;

    public void SetMaxValue(float maxValue)
    {
        _maxValue = maxValue;
    }

    protected virtual void DoOnEnable()
    {

    }

    protected virtual void DoOnDisable()
    {

    }

    private void UpdateNormalizedValue(float newValue)
    {
        _maxValue = Mathf.Max(_maxValue, newValue);
        _normalizedValue = newValue / _maxValue;
        OnNormalizedValueChange?.Invoke(_normalizedValue);
    }

    private void OnEnable()
    {
        _refFloat.OnValueChange += UpdateNormalizedValue;
        UpdateNormalizedValue(_refFloat.Value);
        DoOnEnable();
    }

    private void OnDisable()
    {
        _refFloat.OnValueChange -= UpdateNormalizedValue;
        DoOnDisable();
    }
}
