using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatVariableToSlider : FloatVariableToNormalizedValue
{
    [SerializeField]
    private Slider _slider;

    private void UpdateSlider(float newValue)
    {
        _slider.value = newValue;
    }

    protected override void DoOnEnable()
    {
        OnNormalizedValueChange += UpdateSlider;
        base.DoOnEnable();
    }

    protected override void DoOnDisable()
    {
        OnNormalizedValueChange -= UpdateSlider;
        base.DoOnDisable();
    }  
}
