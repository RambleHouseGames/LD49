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
    private float hitDelay = 1f;

    private Vector2 hitVelocity = new Vector2(4f, 5f);

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

    [SerializeField]
    private bool amGrounded = false;

    private float hitTimer = 0f;
    private int remainingLives = 3;

    private bool amDead = false;

    private void Start()
    {
        GlobalSignalManager.Inst.AddListener<PlayerDiedSignal>(onPlayerDied);
    }

    // Update is called once per frame
    void Update()
    {
        if (hitTimer > 0f)
            hitTimer -= Time.deltaTime;
        else
        {

            Collider2D ground = GetGround();
            if (amGrounded && ground == null)
            {
                transform.SetParent(pagoda.transform);
                amGrounded = false;
            }
            else if (!amGrounded && ground != null)
            {
                Transform newParent = ground.transform;
                if (newParent.tag == "Shuriken")
                    newParent = newParent.parent;
                transform.SetParent(newParent);
                amGrounded = true;
            }

            myRigidbody.velocity = new Vector2(moveSpeed * Input.GetAxis("Horizontal"), myRigidbody.velocity.y);
            if (amGrounded && Input.GetButtonDown("Jump"))
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
    }

    private void onPlayerDied(GlobalSignal signal)
    {
        myAnimator.SetTrigger("Die");
        myRigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
        amDead = true;
        transform.position = new Vector3(transform.position.x, transform.position.y, -3f);
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

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (amDead)
            return;

        if (collider.tag == "Fireball" || collider.tag == "Enemy")
        {
            if (hitTimer <= 0f)
            {
                remainingLives--;
                GlobalSignalManager.Inst.FireSignal(new PlayerGotHitSignal(remainingLives));
                if (remainingLives <= 0)
                    GlobalSignalManager.Inst.FireSignal(new PlayerDiedSignal());
            }

            hitTimer = hitDelay;
            myAnimator.SetTrigger("Hit");
            if (collider.transform.position.x < transform.position.x)
                myRigidbody.velocity = new Vector2(hitVelocity.x, hitVelocity.y);
            else
                myRigidbody.velocity = new Vector2(-hitVelocity.x, hitVelocity.y);
        }
    }

    private Collider2D GetGround()
    {
        RaycastHit2D hit = Physics2D.Raycast(myCollider.bounds.center, Vector2.down, myCollider.bounds.extents.y + .1f, groundLayerMask);
        return hit.collider;
    }
}
