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

    [SerializeField]
    private float fireballCooldown = 3f;

    [SerializeField]
    private GameObject eyeline;

    [SerializeField]
    private LayerMask GroundAndCharacterLayerMask;

    private int currentWaypoint = 0;
    private bool goingForward = true;

    private float pauseTimer = 0f;
    private float fireballTimer = 0f;

    private void Start()
    {
        transform.position = waypoints[currentWaypoint].transform.position;
    }

    private void Update()
    {
        if (fireballTimer > 0f)
            fireballTimer -= Time.deltaTime;
        else
        {
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
        fireballTimer = fireballCooldown;
        Fireball newFireball = Instantiate(fireBallPrefab, eyeline.transform.position, Quaternion.identity);
        newFireball.goLeft = false;
    }

    private void ThrowFireBallLeft()
    {
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
        if(Vector3.Distance(transform.position, waypoints[nextWayPoint].transform.position) < moveSpeed * Time.deltaTime)
        {
            transform.position = waypoints[nextWayPoint].transform.position;
            currentWaypoint = nextWayPoint;
            pauseTimer = waypoints[currentWaypoint].pauseDuration;
        }
        else
            transform.position = Vector3.MoveTowards(transform.position, waypoints[nextWayPoint].transform.position, moveSpeed * Time.deltaTime);
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
}