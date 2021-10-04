using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [SerializeField]
    private Animator myAnimator;

    [SerializeField]
    private SpeechBubble speechBubble;

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
        else if (stateStartedSignal.StartingState.GetType() == typeof(BossTalk1State))
        {
            speechBubble.SetVisible(true);
            speechBubble.SetMessage("This pagoda smells like cats!!");
        }
        else if (stateStartedSignal.StartingState.GetType() == typeof(BossTalk2State))
        {
            speechBubble.SetVisible(true);
            speechBubble.SetMessage("Lets smash it boys!!");
        }
        if (stateStartedSignal.StartingState.GetType() == typeof(SmashState))
        {
            speechBubble.SetVisible(false);
            myAnimator.SetTrigger("Smash");
        }
    }
}
