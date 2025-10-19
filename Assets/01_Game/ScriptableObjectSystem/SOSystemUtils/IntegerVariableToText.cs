using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks.Linq;
using Cysharp.Threading.Tasks;
using System.Threading;
using SoSystem;

public class IntegerVariableToText : MonoBehaviour
{
    [SerializeField]
    private IntegerVariable _intVariable;
    [SerializeField]
    private TMP_Text _textMesh;
    [SerializeField]
    private bool _continuousChange = false;
    [SerializeField]
    private bool _dontRefreshText = false;
    [SerializeField]
    private int _unitChangePerSec = 5;
    [SerializeField]
    private float _continuousChangeCapTime = 1f;
    [SerializeField]
    private float _updateDelay = 0f;
    [SerializeField]
    private string _format = "{0}";
    [SerializeField]
    private bool _freezeUpdateValue = false;
    public bool FreezeUpdateValue
    {
        get => _freezeUpdateValue;
        set
        {
            _freezeUpdateValue = value;
        }
    }

    private CancellationTokenSource _updateToken;

    private int _lastValue = 0;

    private void OnEnable()
    {
        UpdateValue(_intVariable.Value);
        if (_dontRefreshText)
        {
            return;
        }
        _intVariable.OnValueChange += UpdateValue;
    }

    private void OnDisable()
    {
        _updateToken?.Cancel();
        _intVariable.OnValueChange -= UpdateValue;
    }

    private void UpdateValue(int newValue)
    {
        if (_continuousChange)
        {
            ContinuousUpdateValue(newValue).Forget();
            return;
        }
        _textMesh.text = string.Format(_format, newValue.ToString());
    }

    private async UniTaskVoid ContinuousUpdateValue(int newValue)
    {
        if (_updateDelay != 0f)
        {
            await UniTask.Delay(System.TimeSpan.FromSeconds(_updateDelay));
        }

        int diff = newValue - _lastValue;

        float expectedTime = (float)(Mathf.Abs(diff)) / _unitChangePerSec;

        float changePerSec = _unitChangePerSec * Mathf.Sign(diff);
        if (expectedTime > _continuousChangeCapTime)
        {
            changePerSec = diff / _continuousChangeCapTime;
            expectedTime = _continuousChangeCapTime;
        }

        float bufferValue = _lastValue;
        _lastValue = newValue;

        _updateToken?.Cancel();
        _updateToken = new CancellationTokenSource();

        await foreach (var _ in UniTaskAsyncEnumerable.EveryUpdate().WithCancellation(_updateToken.Token))
        {
            float change = _freezeUpdateValue ? 0f : changePerSec * Time.deltaTime;
            bufferValue += change;
            bufferValue = diff < 0 ? Mathf.Max(bufferValue, newValue) : Mathf.Min(bufferValue, newValue);
            if (expectedTime <= 0f)
            {
                _textMesh.text = string.Format(_format, newValue.ToString());
                return;
            }
            _textMesh.text = string.Format(_format, ((int)bufferValue).ToString());
            if (_freezeUpdateValue)
            {
                continue;
            }
            expectedTime -= Time.deltaTime;
        }
    }
}
