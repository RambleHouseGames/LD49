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
    public override void Start()
    {
        SceneManager.LoadSceneAsync("MenuScene", LoadSceneMode.Additive);
    }
}
