using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shuriken : MonoBehaviour
{
    [SerializeField]
    private CircleCollider2D KillCollider;

    [SerializeField]
    private BoxCollider2D PlatformCollider;

    [SerializeField]
    private Rigidbody2D rigidBody;

    [SerializeField]
    private float velocity = 5f;

    [NonSerialized]
    public bool GoLeft = false;

    private void Update()
    {
        if (GoLeft)
            rigidBody.velocity = new Vector2(-velocity, 0f);
        else
            rigidBody.velocity = new Vector2(velocity, 0f);
    }
}
