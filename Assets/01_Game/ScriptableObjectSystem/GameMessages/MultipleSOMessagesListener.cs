using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MultipleSOMessagesListener : MonoBehaviour
{
    [SerializeField]
    private SOMessageBroadcaster _broadcaster;
    [SerializeField]
    private MessageAndAction[] _messageAndActionPairs;

    private void Start()
    {
        for (int i = 0; i < _messageAndActionPairs.Length; i++)
        {
            _messageAndActionPairs[i].Init(this, _broadcaster);
        }
    }

    [System.Serializable]
    public class MessageAndAction
    {
        public SOMessage[] messages;
        public UnityEvent action;
        private void HandleMessage(object[] args)
        {
            action.Invoke();
        }

        public void Init(MonoBehaviour holdGameObject, SOMessageBroadcaster broadcaster)
        {
            for (int i = 0; i < messages.Length; i++)
            {
                messages[i].Listen(HandleMessage, holdGameObject, broadcaster);
            }
        }
    }
}
