using SoSystem;
using UnityEngine;

public class GhostRender : MonoBehaviour
{
    [Header("Reference - Read")]
    [SerializeField]
    private Vector3Variable _playerPosition;
    [SerializeField]
    private FloatVariable _flashLightRadius;

    [Header("Configs")]
    [SerializeField]
    private SkinnedMeshRenderer[] _renderers;
    [SerializeField]
    private Transform _target;

    private bool _visible = false;
    private bool Visible
    {
        get { return _visible; }
        set
        {
            if (_visible == value)
            {
                return;
            }
            _visible = value;
            for (int i = 0; i < _renderers.Length; i++)
            {
                _renderers[i].enabled = value;
            }
        }
    }

    private void OnEnable()
    {
        _flashLightRadius.OnValueChange += AdjustFlashRadius;
        AdjustFlashRadius(_flashLightRadius.Value);
    }

    private void OnDisable()
    {
        _flashLightRadius.OnValueChange -= AdjustFlashRadius;
    }

    private void AdjustFlashRadius(float radius)
    {
        float toPlayerDistance = Vector3.Distance(_playerPosition.Value, _target.position);
        Visible = toPlayerDistance < radius;
    }
}
