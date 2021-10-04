using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private GameObject character;

    [SerializeField]
    private float maximumGain = 3f;

    [SerializeField]
    private GameObject paralax;

    [SerializeField]
    private float paralaxSpeed = .1f;

    [SerializeField]
    private float enemyY = 10f;

    [SerializeField]
    private float panSpeed = 3f;

    private float startY;
    private float paralaxStartY;

    private float targetY;

    private bool isCutScene = true;

    private void Start()
    {
        startY = transform.position.y;
        targetY = startY;
        paralaxStartY = paralax.transform.localPosition.y;

        GlobalSignalManager.Inst.AddListener<StateStartedSignal>(onStateStarted);
    }

    private void onStateStarted(GlobalSignal signal)
    {
        StateStartedSignal stateStartedSignal = (StateStartedSignal)signal;

        if(stateStartedSignal.StartingState.GetType() == typeof(PanUpState))
        {
            targetY = enemyY;
        }
        else if (stateStartedSignal.StartingState.GetType() == typeof(PanDownState))
        {
            targetY = startY;
        }
        else if (stateStartedSignal.StartingState.GetType() == typeof(PlayState))
        {
            isCutScene = false;
        }
    }

    private void Update()
    {
        if (isCutScene)
        {
            if(Mathf.Abs(transform.position.y - targetY) < panSpeed * Time.deltaTime)
            {
                transform.position = new Vector3(transform.position.x, targetY, transform.position.z);
                GlobalSignalManager.Inst.FireSignal(new CameraPanFinishedSignal());
            }
            else
            {
                float newY = transform.position.y + (panSpeed * Time.deltaTime);
                if(targetY < transform.position.y)
                    newY = transform.position.y - (panSpeed * Time.deltaTime);
                transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            }
        }
        else
        {
            if (character.transform.position.y > (transform.position.y + maximumGain))
                transform.position = new Vector3(transform.position.x, character.transform.position.y - maximumGain, transform.position.z);
            if (character.transform.position.y < (transform.position.y - maximumGain))
                transform.position = new Vector3(transform.position.x, character.transform.position.y + maximumGain, transform.position.z);
            if (transform.position.y < startY)
                transform.position = new Vector3(transform.position.x, startY, transform.position.z);
        }

        float cameraYDelta = transform.position.y - startY;
        float paralaxDelta = cameraYDelta * paralaxSpeed;

        paralax.transform.localPosition = new Vector3(paralax.transform.localPosition.x, paralaxStartY - paralaxDelta, paralax.transform.localPosition.z);
    }
}
