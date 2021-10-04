using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [SerializeField]
    private Animator myAnimator;

    private void Start()
    {
        GlobalSignalManager.Inst.AddListener<StateStartedSignal>(onStateStarted);
    }

    public void OnFinishedJumpDown()
    {
        GlobalSignalManager.Inst.FireSignal(new BossFinishedJumpDownSignal());
    }

    private void onStateStarted(GlobalSignal signal)
    {
        StateStartedSignal stateStartedSignal = (StateStartedSignal)signal;
        GlobalState startingState = stateStartedSignal.StartingState;

        if (stateStartedSignal.StartingState.GetType() == typeof(JumpDownState))
        {
            myAnimator.SetTrigger("JumpDown");
        }
    }
}
