using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 5f;

    [SerializeField]
    private SpriteRenderer myRenderer;

    [NonSerialized]
    public bool goLeft = false;

    private void Update()
    {
        float newX = transform.position.x + (moveSpeed * Time.deltaTime);
        if (goLeft)
        {
            myRenderer.flipX = true;
            newX = transform.position.x - (moveSpeed * Time.deltaTime);
        }
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Shuriken")
            Destroy(collision.gameObject);

        if(collision.tag != "Enemy")
            Destroy(gameObject);
    }
}
