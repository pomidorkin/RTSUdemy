using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitProjectile : NetworkBehaviour
{

    [SerializeField] private Rigidbody rb = null;
    [SerializeField] private float launchForce = 10f;
    [SerializeField] private float destroyAfterSeconds = 5f;

    void Start()
    {
        rb.velocity = transform.forward * launchForce;
    }

    public override void OnStartServer()
    {
        // Invoke allows us to call a particular method after some time has passed
        Invoke(nameof(DestroySelf), destroyAfterSeconds);
    }

    [Server]
    private void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }
}
