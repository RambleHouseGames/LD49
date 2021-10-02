using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    [SerializeField]
    private Collider2D collider;

    private List<Collider2D> collidedColliders = new List<Collider2D>();

    public bool IsGrounded()
    {
        return collidedColliders.Count > 0;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "ground" && !collidedColliders.Contains(collision.collider))
        {
            bool isHit = collidedColliders.Count == 0;
            collidedColliders.Add(collision.collider);
            if (isHit)
            { 
                GlobalSignalManager.Inst.FireSignal(new CharacterHitGroundSignal(collision.GetContact(0).point));
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.collider.tag == "ground")
        {
            Debug.Assert(collidedColliders.Contains(collision.collider));
            collidedColliders.Remove(collision.collider);
            if (collidedColliders.Count == 0)
                GlobalSignalManager.Inst.FireSignal(new CharacterLeftGroundSignal());
        }
    }
}
