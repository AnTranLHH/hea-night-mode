﻿using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UIPanelLib
{
    [RequireComponent(typeof(UITransition))]
    public class UIElement : MonoBehaviour
    {
        [SerializeField]
        private float _showDelay;
        [SerializeField]
        private float _hideDelay;

        private bool _useFixedTimeScale = true;
        private UITransition _transition;

        public void Init(bool useFixedTimeScale = true)
        {
            _useFixedTimeScale = useFixedTimeScale;

            _transition = GetComponent<UITransition>();
            _transition.Init();
        }

        public async UniTask Async_Show()
        {
            _transition.PreshowSetSup();
            await UniTask.NextFrame(); // Ensure the preshow setup is complete before showing
            await UniTask.Delay(System.TimeSpan.FromSeconds(_showDelay), ignoreTimeScale: _useFixedTimeScale);
            await _transition.RunShowTransition();
        }

        public async UniTask Async_Hide()
        {
            await UniTask.Delay(System.TimeSpan.FromSeconds(_hideDelay), ignoreTimeScale: _useFixedTimeScale);
            await _transition.RunHideTransition();
        }

        internal void ResetTransition()
        {
            _transition.ResetAnimatedProps();
        }
    }

}
