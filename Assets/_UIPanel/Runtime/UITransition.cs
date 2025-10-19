using System;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using UnityEngine.Playables;

namespace UIPanelLib
{
    public class UITransition : MonoBehaviour
    {
        [SerializeField]
        private TransitionSetup[] _showTransition;
        [SerializeField]
        private TransitionSetup[] _hideTransition;

        [SerializeField]
        private UnityEvent _onStartShow;
        [SerializeField]
        private UnityEvent _onFinishShow;
        [SerializeField]
        private UnityEvent _onStartHide;
        [SerializeField]
        private UnityEvent _onFinishHide;

        private CanvasGroup _canvasGroup;
        private PlayableDirector _timelinePlayer;
        private bool _useFixedTimeScale = true;

        public void Init()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _timelinePlayer = GetComponent<PlayableDirector>();
        }

        public void PreshowSetSup()
        {
            for (int i = 0; i < _showTransition.Length; i++)
            {
                PreshowSetUp(_showTransition[i]);
            }
        }

        private UniTask[] _transitionTasks;
        public async UniTask RunShowTransition()
        {
            _onStartShow.Invoke();
            _transitionTasks ??= new UniTask[_showTransition.Length];
            for (int i = 0; i < _showTransition.Length; i++)
            {
                var transition = _showTransition[i];
                float startAfter = transition.startAfterSecond;
                await UniTask.WaitForSeconds(startAfter, _useFixedTimeScale);
                _transitionTasks[i] = RunTranstion(transition);
            }

            await UniTask.WhenAll(_transitionTasks);
            _onFinishShow.Invoke();
        }

        private UniTask[] _hideTransitionTasks;
        public async UniTask RunHideTransition()
        {
            _onStartHide.Invoke();
            _hideTransitionTasks ??= new UniTask[_hideTransition.Length];
            for (int i = 0; i < _hideTransition.Length; i++)
            {
                var transition = _hideTransition[i];
                float startAfter = transition.startAfterSecond;
                await UniTask.WaitForSeconds(startAfter, _useFixedTimeScale);
                _hideTransitionTasks[i] = RunTranstion(transition);
            }

            await UniTask.WhenAll(_hideTransitionTasks);
            _onFinishHide.Invoke();
        }

        private void PreshowSetUp(TransitionSetup setup)
        {
            switch (setup.transitionType)
            {
                case TransitionType.Fade:
                    {
                        if (_canvasGroup == null)
                        {
                            Debug.LogWarning("In order to use Fade Transition, add CanvasGroup Component to this object", this);
                            return;
                        }

                        _canvasGroup.alpha = setup.from.x;
                        break;
                    }
                case TransitionType.Move:
                    {
                        transform.localPosition = setup.from;
                        break;
                    }
                case TransitionType.Zoom:
                    {
                        transform.localScale = setup.from;
                        break;
                    }
                case TransitionType.Timeline:
                    {
                        if (_timelinePlayer == null)
                        {
                            Debug.LogError("Please attach PlayableDirector to use Timeline transition", this);
                            return;
                        }
                        _timelinePlayer.time = 0f;
                        _timelinePlayer.Pause();
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        public async UniTask RunTranstion(TransitionSetup setup)
        {
            switch (setup.transitionType)
            {
                case TransitionType.Fade:
                    {
                        if (_canvasGroup == null)
                        {
                            Debug.LogWarning("In order to use Fade Transition, add CanvasGroup Component to this object", this);
                            return;
                        }

                        _canvasGroup.DOKill();
                        await _canvasGroup.DOFade(setup.to.x, setup.duration).SetEase(setup.ease).SetUpdate(_useFixedTimeScale);
                        break;
                    }
                case TransitionType.Move:
                    {
                        transform.DOKill();
                        await transform.DOLocalMove(setup.to, setup.duration).SetEase(setup.ease).SetUpdate(_useFixedTimeScale);
                        break;
                    }
                case TransitionType.Zoom:
                    {
                        transform.DOKill();
                        await transform.DOScale(setup.to.x, setup.duration).SetEase(setup.ease).SetUpdate(_useFixedTimeScale);
                        break;
                    }
                case TransitionType.Timeline:
                    {
                        _timelineStopped = false;
                        _timelinePlayer.timeUpdateMode = _useFixedTimeScale ? DirectorUpdateMode.UnscaledGameTime : DirectorUpdateMode.GameTime;
                        _timelinePlayer.stopped += NotifyFinishTimeline;
                        _timelinePlayer.Resume();
                        await UniTask.WaitUntil(() => _timelineStopped);
                        
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        internal void ResetAnimatedProps()
        {
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 1f;
            }
            if (_timelinePlayer != null)
            {
                _timelinePlayer.time = 0f;
                _timelinePlayer.Stop();
            }
            transform.localPosition = Vector3.zero;
            transform.localScale = Vector3.one;
        }

        private bool _timelineStopped = false;
        private void NotifyFinishTimeline(PlayableDirector timeline)
        {
            _timelineStopped = true;
            timeline.stopped -= NotifyFinishTimeline;
        }
    }
    

    public enum TransitionType
    {
        None = 0,
        Move = 1,
        Zoom = 2,
        Fade = 3,
        Timeline = 4
    }

    [System.Serializable]
    public struct TransitionSetup
    {
        public float startAfterSecond;
        public TransitionType transitionType;
        public Vector3 from;
        public Vector3 to;
        public float duration;
        public Ease ease;
    }

}