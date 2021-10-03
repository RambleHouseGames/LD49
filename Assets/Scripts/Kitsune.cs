using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kitsune : MonoBehaviour
{
    [SerializeField]
    private BadShuriken badShurikenPrefab;

    [SerializeField]
    private List<WayPoint> waypoints;

    [SerializeField]
    private float moveSpeed = 1f;

    private int currentWaypoint = 0;
    private bool goingForward = true;

    private void Start()
    {
        transform.position = waypoints[currentWaypoint].transform.position;
    }

    private void Update()
    {
        
    }

    public void MoveTowardNextWayPoint()
    {

        //transform.position = Vector3.MoveTowards(transform.position, )
    }
}

public abstract class KitsuneState
{
    protected Kitsune kitsune;

    public KitsuneState (Kitsune kitsune)
    {
        this.kitsune = kitsune;
    }

    public virtual void Start() { }
    public virtual KitsuneState Update () { return this; }
    public virtual void End() { }
}

public class KitsuneWalkState : KitsuneState
{
    public KitsuneWalkState(Kitsune kitsune) : base (kitsune) {}

    public override KitsuneState Update()
    {
        kitsune.MoveTowardNextWayPoint();
        return this;
    }
}