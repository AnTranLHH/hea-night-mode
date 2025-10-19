using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace UIPanelLib
{
    public class AddressableUIPanel : BaseUIPanel
    {
        [SerializeField]
        private GameObject _loadingPanel;
        [SerializeField]
        private AssetReference _uiPanelAddress;
        [SerializeField][Tooltip("After closing 'lifeTime' seconds, the panel will be released automatically")]
        private float _lifeTime = 60f;

        private UIPanel _panelInstance;
        private float _currentLifeTime = 0f;

        public override void Init(UIController controller)
        {
            _uiController = controller;
        }

        public override void Open()
        {
            Async_Open().Forget();
        }

        public override async UniTask Async_Open()
        {
            CancelTrackTimeLife();

            _panelStatus = PanelStatus.IsOpening;
            transform.SetAsLastSibling();

            if (_panelInstance == null)
            {
                _loadingPanel.SetActive(true);
                _panelInstance = (await _uiPanelAddress.InstantiateAsync(transform, false)).GetComponent<UIPanel>();
                _uiController.RegisterAddressableUIPanel(_panelInstance, this);
                _loadingPanel.SetActive(false);
            }

            await _panelInstance.Async_Open();
            _panelStatus = PanelStatus.Opened;
        }

        public override void Close()
        {
            Async_Close().Forget();
        }

        public override async UniTask Async_Close()
        {
            if (_panelInstance == null)
            {
                return;
            }

            _panelStatus = PanelStatus.IsClosing;
            await _panelInstance.Async_Close();
            transform.SetAsFirstSibling();
            _panelStatus = PanelStatus.Closed;
        }

        public override void SetClickBlockerVisible(bool visible)
        {
            if (_panelInstance == null)
            {
                return;
            }
            _panelInstance.SetClickBlockerVisible(visible);
        }

        internal async UniTaskVoid PostCloseProcess()
        {
            if (_panelInstance == null)
            {
                return;
            }

            _panelStatus = PanelStatus.IsClosing;
            await UniTask.WaitUntil(() => _panelInstance.Status == PanelStatus.Closed);
            _panelStatus = PanelStatus.Closed;

            StartTrackLifeTime().Forget();
        }

        private CancellationTokenSource _updateTokenSource;
        private async UniTaskVoid StartTrackLifeTime()
        {
            _updateTokenSource?.Cancel();
            _updateTokenSource = new();
            
            _currentLifeTime = _lifeTime;
            while (_currentLifeTime > 0)
            {
                _currentLifeTime -= Time.deltaTime;
                await UniTask.Yield(cancellationToken: _updateTokenSource.Token);
            }

            ReleasePanelInstance();
        }

        private void CancelTrackTimeLife()
        {
            _updateTokenSource?.Cancel();
        }

        private void ReleasePanelInstance()
        {
            _uiPanelAddress.ReleaseInstance(_panelInstance.gameObject);
            _panelInstance = null;
            _updateTokenSource?.Cancel();
        }
    }
}

