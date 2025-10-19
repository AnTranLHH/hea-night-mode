using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SOMessageBroadcaster : MonoBehaviour
{
    [SerializeField]
    private bool _logWhenBroadcastMessage = false;
    public string editorOnly_messageToBroadcast;
    public int editorOnly_messageIndexToBroadcast;
    [SerializeField]
    private List<SOMessage> _managedMessages;
    private Dictionary<SOMessage, List<(MonoBehaviour, SOMessage.SOMessageAction)>> _messageAndActionDict = new();

    [Header("Inspec")]
    [SerializeField]
    private List<ListenerInfo> _listeners = new();

    [Serializable]
    private class ListenerInfo
    {
        public MonoBehaviour listener;
        public SOMessage interestMessage;
        public string responseAction;
    }

    public void BroadcastSOMessage(SOMessage message, params object[] args)
    {
        if (message == null)
        {
            return;
        }
        _messageAndActionDict.TryGetValue(message, out var actions);
        if (actions == null)
        {
            Debug.LogWarning($"message {message.name} does not register any action on this broadcaster", this);
            return;
        }
        
        for (int i = 0; i < actions.Count; i++)
        {
            MonoBehaviour associatedObj = actions[i].Item1;
            if (associatedObj == null || !associatedObj.enabled)
            {
                continue;
            }
            if (_logWhenBroadcastMessage)
            {
                Debug.Log($"Message [{message.name}] has been sent to [{associatedObj.gameObject.name}]");
            }

            if (args == null || args.Length == 0)
            {
                actions[i].Item2.Invoke();
                continue;
            }
            actions[i].Item2.Invoke(args);
        }
    }

    public void BroadcastSOMessage(SOMessage message)
    {
        BroadcastSOMessage(message, null);
    }

    public void BroadcastAndIncludeMessage(SOMessage message)
    {
        BroadcastSOMessage(message, message);
    }

    public void SetUpMessageAction(SOMessage message, MonoBehaviour associatedObject, SOMessage.SOMessageAction action)
    {
        _messageAndActionDict.TryGetValue(message, out var actions);
        if (actions == null)
        {
            actions = new();
            _messageAndActionDict.Add(message, actions);
        }

        var messageAndAction = (associatedObject, action);
        if (actions.Contains(messageAndAction))
        {
            return;
        }

        actions.Add(messageAndAction);
        _listeners.Add(new() { listener = associatedObject, interestMessage = message, responseAction = action.Method.Name });
    }

#if UNITY_EDITOR
    public void EditorOnly_AddMessageFromListener(SOMessage message)
    {
        if (_managedMessages.Contains(message))
        {
            return;
        }

        _managedMessages.Add(message);
    }

    public void EditorOnly_BroadcastAllMessage()
    {
        for (int i = 0; i < _managedMessages.Count; i++)
        {
            BroadcastSOMessage(_managedMessages[i]);
        }
    }

    public void EditorOnly_BroadcastMessageByName(string message)
    {
        for (int i = 0; i < _managedMessages.Count; i++)
        {
            if (_managedMessages[i].name == message)
            {
                BroadcastSOMessage(_managedMessages[i]);
                return;
            }
        }
    }

    public void EditorOnly_BroadcastMessageByIndex(int index)
    {
        BroadcastSOMessage(_managedMessages[index % _managedMessages.Count]);
    }
#endif
}
