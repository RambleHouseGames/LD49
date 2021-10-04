using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pagoda : MonoBehaviour
{
    [SerializeField]
    private int width = 10;

    [SerializeField]
    private GameObject tilePrefab;

    [SerializeField]
    private List<PagodaRow> rows;

    private float waveProgress = 0f;
    private float waveAmplitude = 1f;
    private float waveSpeed = 1f;
    private bool shouldWave = false;

    private void Start()
    {
        GlobalSignalManager.Inst.AddListener<StateStartedSignal>(onStateStarted);
    }

    private void onStateStarted(GlobalSignal signal)
    {
        StateStartedSignal stateStartedSignal = (StateStartedSignal)signal;
        GlobalState startingState = stateStartedSignal.StartingState;

        if (stateStartedSignal.StartingState.GetType() == typeof(SmashState))
        {
            shouldWave = true;
        }
        else if (stateStartedSignal.StartingState.GetType() == typeof(PlayState))
        {
            shouldWave = true;
        }
    }

        private void Update()
    {
        if(shouldWave)
            waveProgress += Time.deltaTime * waveSpeed;
        for (int i = 0; i < rows.Count; i++)
        {
            float offset = waveProgress - (i / waveAmplitude);
            if(offset > 0)
                rows[i].transform.position = new Vector3((Mathf.Sin(offset)) * waveAmplitude, rows[i].transform.position.y, rows[i].transform.position.z);
        }
    }
}