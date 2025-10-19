using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SOMessage", menuName = "ScriptableObjectSystem/SOMessage")]
public class SOMessage : ScriptableObject
{
    public delegate void SOMessageAction(params object[] args);
    /// <summary>
    /// Recommend to call this method once per life time of object
    /// </summary>
    public void Listen(SOMessageAction action, MonoBehaviour listeningObject, SOMessageBroadcaster targetBroadcaster)
    {
        if (targetBroadcaster == null)
        {
            targetBroadcaster = listeningObject.GetComponentInParent<SOMessageBroadcaster>();
        }
        if (targetBroadcaster == null)
        {
            Debug.LogWarning($"You are listening a message that will never be sent from any broadcaster", listeningObject);
            return;
        }

        targetBroadcaster.SetUpMessageAction(this, listeningObject, action);
    }
}
