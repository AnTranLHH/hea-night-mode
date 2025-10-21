using SoSystem;
using UnityEngine;

public class FlashLight : MonoBehaviour
{
    [Header("Reference - Read")]
    [SerializeField]
    private FloatVariable _flashLightDuration;
    [SerializeField]
    private FloatVariable _flashLightMaxDuration;

    [Header("Reference - Write")]
    [SerializeField]
    private FloatVariable _flashLightRadius;

    [Header("Configs")]
    [SerializeField]
    private Vector2 _flashLightRange = new(2f, 10f);
    [SerializeField]
    private GameObject _flashLight;

    void OnEnable()
    {
        _flashLightDuration.OnValueChange += AdjustFlashRadius;
    }

    void OnDisable()
    {
        _flashLightDuration.OnValueChange -= AdjustFlashRadius;
    }

    private void AdjustFlashRadius(float duration)
    {
        float scale = MathUtils.Remap(duration, 0f, _flashLightMaxDuration.Value, _flashLightRange.x, _flashLightRange.y);
        _flashLightRadius.Value = scale;
        _flashLight.transform.localScale = new(scale, scale, 1f);
    }
}
