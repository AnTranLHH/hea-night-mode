using UnityEngine;
using UnityEngine.UI;

public class FloatVariableToFillImage : FloatVariableToNormalizedValue
{
    [SerializeField]
    private Image _fillImage;

    private void UpdateFillImage(float newValue)
    {
        _fillImage.fillAmount = newValue;
    }

    protected override void DoOnEnable()
    {
        OnNormalizedValueChange += UpdateFillImage;
        base.DoOnEnable();
    }

    protected override void DoOnDisable()
    {
        OnNormalizedValueChange -= UpdateFillImage;
        base.DoOnDisable();
    }
}
