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
        if (Inst == null)
            Inst = this;
        else
            Destroy(gameObject);
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
public class CharacterHitGroundSignal : GlobalSignal
{
    public Vector2 point { get; private set; }

    public CharacterHitGroundSignal(Vector2 point)
    {
        this.point = point;
    }
}
public class CharacterLeftGroundSignal : GlobalSignal { }
public class ThrowAnimationEndedSignal : GlobalSignal { }
public class ThrowAnimationReleasePointSignal : GlobalSignal { }
public class PlayerGotHitSignal : GlobalSignal
{
    public int remainingLives { get; private set; }

    public PlayerGotHitSignal(int remainingLives)
    {
        this.remainingLives = remainingLives;
    }
}
public class PlayerDiedSignal : GlobalSignal { }
public class FinishedFadeOutSignal : GlobalSignal { }
public class FinishedFadeInSignal : GlobalSignal { }
public class StartButtonPressedSignal : GlobalSignal { }
public class StateStartedSignal : GlobalSignal
{
    public GlobalState StartingState { get; private set; }

    public StateStartedSignal(GlobalState startingState)
    {
        this.StartingState = startingState;
    }

}

public class TextFinishedSignal : GlobalSignal { }
public class CameraPanFinishedSignal : GlobalSignal { }
public class BossFinishedJumpDownSignal : GlobalSignal { }
public class BossFinishedSmashingSignal : GlobalSignal { }
public class DeathAnimationFinishedSignal : GlobalSignal { }