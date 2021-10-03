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

    private float startY;
    private float paralaxStartY;

    private void Start()
    {
        startY = transform.position.y;
        paralaxStartY = paralax.transform.localPosition.y;
    }

    private void Update()
    {
        if (character.transform.position.y > (transform.position.y + maximumGain))
            transform.position = new Vector3(transform.position.x, character.transform.position.y - maximumGain, transform.position.z);
        if (character.transform.position.y < (transform.position.y - maximumGain))
            transform.position = new Vector3(transform.position.x, character.transform.position.y + maximumGain, transform.position.z);
        if (transform.position.y < startY)
            transform.position = new Vector3(transform.position.x, startY, transform.position.z);

        float cameraYDelta = transform.position.y - startY;
        float paralaxDelta = cameraYDelta * paralaxSpeed;

        paralax.transform.localPosition = new Vector3(paralax.transform.localPosition.x, paralaxStartY - paralaxDelta, paralax.transform.localPosition.z);
    }
}
