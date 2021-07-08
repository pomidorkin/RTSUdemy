using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 100;

    // [SyncVar(hook = "funcName" )] - a function can be assigned to the hook,
    // which gets called each time when the value of the sync var is updated
    [SyncVar]
    private int currentHealth;

    public event Action ServerOnDie;

    #region Server

    public override void OnStartServer()
    {
        currentHealth = maxHealth;
    }

    [Server]
    public void DealDamage(int damageAmount)
    {
        if(currentHealth <= 0) { return; }

        currentHealth = Mathf.Max(currentHealth - damageAmount, 0);

        if(currentHealth != 0) { return; }

        // Question mark stops event from throwing exceptions if no one is listening to it
        ServerOnDie?.Invoke();

        Debug.Log("WE died");
    }

    #endregion

    #region Client

    //...

    #endregion

}
