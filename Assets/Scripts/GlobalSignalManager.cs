using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalSignalManager : MonoBehaviour
{
    public static GlobalSignalManager Inst;

    private Dictionary<Type, Action<GlobalSignal>> listeners = new Dictionary<Type, Action<GlobalSignal>>();

    private void Awake()
    {
        Inst = this;
    }

    public void AddListener<T>(Action<GlobalSignal> callback) where T : GlobalSignal
    {
        if (listeners.ContainsKey(typeof(T)))
            listeners[typeof(T)] += callback;
        else
            listeners.Add(typeof(T), callback);
    }

    public void RemoveListener<T>(Action<GlobalSignal> callback) where T : GlobalSignal
    {
        listeners[typeof(T)] -= callback;
        if (listeners[typeof(T)] == null)
            listeners.Remove(typeof(T));
    }

    public void FireSignal(GlobalSignal signal)
    {
        if(listeners.ContainsKey(signal.GetType()))
            listeners[signal.GetType()](signal);
    }
}

public class GlobalSignal { }
public class CharacterHitGroundSignal : GlobalSignal { }
public class CharacterLeftGroundSignal : GlobalSignal { }
public class ThrowAnimationEndedSignal : GlobalSignal { }
public class ThrowAnimationReleasePointSignal : GlobalSignal { }