using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalStateMachine : MonoBehaviour
{
    public static GlobalStateMachine Inst;
    private GlobalState currentState;

    private void Awake()
    {
        Inst = this;
    }

    public void Start()
    {
        currentState = new MenuState();
        currentState.Start();
    }

    // Update is called once per frame
    void Update()
    {
        GlobalState nextState = currentState.Update();
        if(nextState != currentState)
        {
            currentState.End();
            currentState = nextState;
            currentState.Start();
            GlobalSignalManager.Inst.FireSignal(new StateStartedSignal(currentState));
        }
    }
}

public class GlobalState
{
    public virtual void Start() { }
    public virtual GlobalState Update()
    {
        return this;
    }
    public virtual void End() { }
}

public class MenuState : GlobalState
{
    private GlobalState nextState;

    public MenuState()
    {
        nextState = this;
    }

    public override void Start()
    {
        SceneManager.LoadSceneAsync("MenuScene", LoadSceneMode.Additive);
        GlobalSignalManager.Inst.AddListener<StartButtonPressedSignal>(onStartButtonPressed);
    }

    public override GlobalState Update()
    {
        return nextState;
    }

    private void onStartButtonPressed(GlobalSignal signal)
    {
        GlobalSignalManager.Inst.RemoveListener<StartButtonPressedSignal>(onStartButtonPressed);
        nextState = new IntroState();
    }

    public override void End()
    {
        SceneManager.UnloadSceneAsync("MenuScene");
    }
}

public class IntroState : GlobalState
{
    private GlobalState nextState;

    public IntroState()
    {
        nextState = this;
    }

    public override void Start()
    {
        SceneManager.LoadSceneAsync("MainScene", LoadSceneMode.Additive);
        GlobalSignalManager.Inst.AddListener<FinishedFadeInSignal>(onFinishedFadeIn);
    }

    public override GlobalState Update()
    {
        return nextState;
    }

    public override void End()
    {
        GlobalSignalManager.Inst.RemoveListener<FinishedFadeInSignal>(onFinishedFadeIn);
    }

    private void onFinishedFadeIn(GlobalSignal signal)
    {
        nextState = new CatTalk1State();
    }
}

public class CatTalk1State : GlobalState
{
    private bool textFinished = false;
    private GlobalState nextState;

    public CatTalk1State()
    {
        nextState = this;
    }

    public override void Start()
    {
        GlobalSignalManager.Inst.AddListener<TextFinishedSignal>(onTextFinished);
    }

    public override GlobalState Update()
    {
        if (Input.GetButtonDown("Jump") && textFinished)
            nextState = new CatTalk2State();

        return nextState;
    }

    public override void End()
    {
        GlobalSignalManager.Inst.AddListener<TextFinishedSignal>(onTextFinished);
    }

    private void onTextFinished(GlobalSignal signal)
    {
        textFinished = true;
    }
}

public class CatTalk2State : GlobalState
{
    private bool textFinished = false;
    private GlobalState nextState;

    public CatTalk2State()
    {
        nextState = this;
    }

    public override void Start()
    {
        GlobalSignalManager.Inst.AddListener<TextFinishedSignal>(onTextFinished);
    }

    public override GlobalState Update()
    {
        if (Input.GetButtonDown("Jump") && textFinished)
            nextState = new PanUpState();

        return nextState;
    }

    public override void End()
    {
        GlobalSignalManager.Inst.AddListener<TextFinishedSignal>(onTextFinished);
    }

    private void onTextFinished(GlobalSignal signal)
    {
        textFinished = true;
    }
}

public class PanUpState : GlobalState
{
    private GlobalState nextState;

    public PanUpState()
    {
        nextState = this;
    }

    public override void Start()
    {
        GlobalSignalManager.Inst.AddListener<CameraPanFinishedSignal>(onCameraPanFinished);
    }

    public override GlobalState Update()
    {
        return nextState;
    }

    public override void End()
    {
        GlobalSignalManager.Inst.AddListener<CameraPanFinishedSignal>(onCameraPanFinished);
    }

    private void onCameraPanFinished(GlobalSignal signal)
    {
        nextState = new JumpDownState();
    }
}

public class JumpDownState : GlobalState
{
    private GlobalState nextState;

    public JumpDownState()
    {
        nextState = this;
    }

    public override void Start()
    {
        GlobalSignalManager.Inst.AddListener<BossFinishedJumpDownSignal>(onBossFinishedJumpDown);
    }

    public override GlobalState Update()
    {
        return nextState;
    }

    public override void End()
    {
        GlobalSignalManager.Inst.AddListener<BossFinishedJumpDownSignal>(onBossFinishedJumpDown);
    }

    private void onBossFinishedJumpDown(GlobalSignal signal)
    {
        Debug.Log("Boss Talk");
    }
}