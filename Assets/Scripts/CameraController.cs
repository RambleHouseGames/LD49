using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private GameObject character;

    [SerializeField]
    private float maximumGain = 3f;

    private void Update()
    {
        if (character.transform.position.y > (transform.position.y + maximumGain))
            transform.position = new Vector3(transform.position.x, character.transform.position.y - maximumGain, transform.position.z);
        if (character.transform.position.y < (transform.position.y - maximumGain))
            transform.position = new Vector3(transform.position.x, character.transform.position.y + maximumGain, transform.position.z);
    }
}
