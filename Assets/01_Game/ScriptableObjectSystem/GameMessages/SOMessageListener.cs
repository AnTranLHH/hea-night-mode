using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SOMessageListener : MonoBehaviour
{
    [SerializeField]
    private SOMessageBroadcaster _targetBroadcaster;
    [SerializeField]
    private SOMessage[] _listenedMessages;
    public UnityEvent _onReceivedMessage;

    private void Start()
    {
        if (_targetBroadcaster == null)
        {
            _targetBroadcaster = GetComponentInParent<SOMessageBroadcaster>();
        }
        for (int i = 0; i < _listenedMessages.Length; i++)
        {
            _listenedMessages[i].Listen(MessageHandler, this, _targetBroadcaster);
        }
    }

    private void MessageHandler(object[] args)
    {
        _onReceivedMessage.Invoke();
    }

    private void OnValidate()
    {
        if (_targetBroadcaster == null)
        {
            return;
        }

        if (_listenedMessages == null)
        {
            return;
        }

        for (int i = 0; i < _listenedMessages.Length; i++)
        {
            var message = _listenedMessages[i];
            if (message == null)
            {
                continue;
            }

#if UNITY_EDITOR
            _targetBroadcaster.EditorOnly_AddMessageFromListener(_listenedMessages[i]);
#endif
        }
    }
}
