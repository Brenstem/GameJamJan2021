using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DanesUnityLibrary;

public class HitBoxController : MonoBehaviour
{
    [Header("Hitbox Settings")]
    [SerializeField] private float damage;
    [SerializeField] private float lifeTime;
    [SerializeField] private Vector3 hitBoxExtents;
    [SerializeField] private LayerMask targetLayer;

    [Header("Debug")]
    [SerializeField] private bool debug;

    private Timer hitBoxTimer;

    private void Start()
    {
        hitBoxTimer = new Timer(0.01f);
    }

    private void Update()
    {
        print(hitBoxTimer.Expired);

        if (!hitBoxTimer.Expired)
        {
            debug = true;

            Collider[] hits = Physics.OverlapBox(transform.position, hitBoxExtents);

            foreach (Collider col in hits)
            {
                if (col.GetComponent<Health>() == true)
                {
                    col.GetComponent<Health>().Damage(damage);
                }
            }
        }
        else if (hitBoxTimer.Expired)
        {
            debug = false;
        }
    }

    public void ExposeHitBox()
    {
        hitBoxTimer = new Timer(lifeTime);
    }

    public void OnDrawGizmos()
    {
        if (debug)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, hitBoxExtents);
        }
    }
}