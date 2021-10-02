using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character2 : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 5f;

    [SerializeField]
    private float jumpSpeed = 10f;

    [SerializeField]
    private SpriteRenderer myRenderer;

    [SerializeField]
    private Animator myAnimator;

    [SerializeField]
    private Rigidbody2D myRigidbody;

    [SerializeField]
    private Collider2D myCollider;

    [SerializeField]
    private LayerMask groundLayerMask;

    [SerializeField]
    private Shuriken shurikenPrefab;

    [SerializeField]
    private float shurikenSpawnOffset = 1f;

    [SerializeField]
    private GameObject pagoda;

    private bool amGrounded = false;

    // Update is called once per frame
    void Update()
    {
        Collider2D ground = GetGround();
        if(amGrounded && ground == null)
        {
            transform.SetParent(pagoda.transform);
            amGrounded = false;
        }
        else if(!amGrounded && ground != null)
        {
            transform.SetParent(ground.transform);
            amGrounded = true;
        }

        myRigidbody.velocity = new Vector2(moveSpeed * Input.GetAxis("Horizontal"), myRigidbody.velocity.y);
        if(amGrounded && Input.GetButtonDown("Jump"))
            myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, jumpSpeed);

        if (myRenderer.flipX && Input.GetAxis("Horizontal") > 0)
            myRenderer.flipX = false;
        else if (!myRenderer.flipX && Input.GetAxis("Horizontal") < 0)
            myRenderer.flipX = true;

        myAnimator.SetBool("Grounded", amGrounded);
        myAnimator.SetBool("LateralMovement", Mathf.Abs(Input.GetAxis("Horizontal")) > .1f);

        if (Input.GetButtonDown("Fire1"))
            myAnimator.SetTrigger("Throw");
    }

    public void OnThrowAnimationReleasePoint()
    {
        if (myRenderer.flipX)
        {
            Vector3 spawnPosition = new Vector3(transform.position.x - shurikenSpawnOffset, transform.position.y, transform.position.z);
            Shuriken newShuriken = Instantiate(shurikenPrefab, spawnPosition, Quaternion.identity);
            newShuriken.GoLeft = true;
        }
        else
        {
            Vector3 spawnPosition = new Vector3(transform.position.x + shurikenSpawnOffset, transform.position.y, transform.position.z);
            Shuriken newShuriken = Instantiate(shurikenPrefab, spawnPosition, Quaternion.identity);
            newShuriken.GoLeft = false;
        }
    }

    private Collider2D GetGround()
    {
        RaycastHit2D hit = Physics2D.Raycast(myCollider.bounds.center, Vector2.down, myCollider.bounds.extents.y + .1f, groundLayerMask);
        return hit.collider;
    }
}
