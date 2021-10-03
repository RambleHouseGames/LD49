using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kitsune : MonoBehaviour
{
    [SerializeField]
    private Fireball fireBallPrefab;

    [SerializeField]
    private List<WayPoint> waypoints;

    [SerializeField]
    private float moveSpeed = 1f;

    private float fireballCooldown = 1f;

    [SerializeField]
    private GameObject eyeline;

    [SerializeField]
    private LayerMask GroundAndCharacterLayerMask;

    [SerializeField]
    private Animator myAnimator;

    [SerializeField]
    private Rigidbody2D myRigidbody;

    private int currentWaypoint = 0;
    private bool goingForward = true;

    private float pauseTimer = 0f;
    private float fireballTimer = 0f;

    private bool amDead = false;
    private float deathTimer = 100f;
    private Vector2 deathVelocity = new Vector2(5f, 5f);

    private void Start()
    {
        transform.position = waypoints[currentWaypoint].transform.position;
    }

    private void Update()
    {
        if(amDead)
        {
            myRigidbody.constraints = RigidbodyConstraints2D.None;
            transform.SetParent(null);
            transform.position = new Vector3(transform.position.x + (deathVelocity.x * Time.deltaTime), transform.position.y + (deathVelocity.y * Time.deltaTime), -2f);
            transform.Rotate(new Vector3(0f, 0f, 360f * Time.deltaTime));
            deathTimer -= Time.deltaTime;
            if (deathTimer < 0f)
                Destroy(gameObject);
        }
        else if (fireballTimer > 0f)
            fireballTimer -= Time.deltaTime;
        else
        {
            myAnimator.SetBool("FiringRight", false);
            myAnimator.SetBool("FiringLeft", false);
            Collider2D rightCollider = ScanRight();
            Collider2D leftCollider = ScanLeft();

            if (rightCollider != null && rightCollider.tag == "Character")
                ThrowFireBallRight();
            else if (leftCollider != null && leftCollider.tag == "Character")
                ThrowFireBallLeft();
            else if (pauseTimer > 0)
                pauseTimer -= Time.deltaTime;
            else
                MoveTowardNextWayPoint();
        }
    }

    private void ThrowFireBallRight()
    {
        myAnimator.SetBool("FiringRight", true);
        fireballTimer = fireballCooldown;
        Fireball newFireball = Instantiate(fireBallPrefab, eyeline.transform.position, Quaternion.identity);
        newFireball.goLeft = false;
    }

    private void ThrowFireBallLeft()
    {
        myAnimator.SetBool("FiringLeft", true);
        fireballTimer = fireballCooldown;
        Fireball newFireball = Instantiate(fireBallPrefab, eyeline.transform.position, Quaternion.identity);
        newFireball.goLeft = true;
    }

    private void MoveTowardNextWayPoint()
    {
        int nextWayPoint = currentWaypoint;
        if(waypoints.Count > 1)
        {
            if(goingForward)
            {
                nextWayPoint++;
                if(nextWayPoint > waypoints.Count - 1)
                {
                    goingForward = false;
                    nextWayPoint = waypoints.Count - 2;
                }
            }
            else
            {
                nextWayPoint--;
                if(nextWayPoint < 0)
                {
                    goingForward = true;
                    nextWayPoint = 1;
                }
            }
        }
        if (Vector3.Distance(transform.position, waypoints[nextWayPoint].transform.position) < moveSpeed * Time.deltaTime)
        {
            myAnimator.SetBool("Walking", false);
            transform.position = waypoints[nextWayPoint].transform.position;
            currentWaypoint = nextWayPoint;
            pauseTimer = waypoints[currentWaypoint].pauseDuration;
        }
        else
        {
            myAnimator.SetBool("Walking", true);
            transform.position = Vector3.MoveTowards(transform.position, waypoints[nextWayPoint].transform.position, moveSpeed * Time.deltaTime);
        }
    }

    private Collider2D ScanRight()
    {
        RaycastHit2D hit = Physics2D.Raycast(eyeline.transform.position, Vector2.right, 1000f, GroundAndCharacterLayerMask);
        Debug.DrawRay(eyeline.transform.position, Vector2.right * 1000f, Color.blue);
        return hit.collider;
    }

    private Collider2D ScanLeft()
    {
        RaycastHit2D hit = Physics2D.Raycast(eyeline.transform.position, Vector2.left, 1000, GroundAndCharacterLayerMask);
        Debug.DrawRay(eyeline.transform.position, Vector2.left * 1000f, Color.red);
        return hit.collider;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Shuriken" && collision.GetType() == typeof(CircleCollider2D))
        {
            if (collision.transform.position.x > transform.position.x)
                deathVelocity = new Vector2(-deathVelocity.x, deathVelocity.y);
            amDead = true;
            Destroy(collision.gameObject);
        }
    }
}