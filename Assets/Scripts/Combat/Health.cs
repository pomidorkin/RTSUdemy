using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 100;

    // [SyncVar(hook = "funcName" )] - a function can be assigned to the hook,
    // which gets called each time when the value of the sync var is updated.

    //Server will change the variable and sync it to the clients.
    // Whenever the variable is updated, a function HandleHealthUpdated get callsed
    // and that function raises the ClientOnHealthUpdated event.
    [SyncVar(hook = nameof(HandleHealthUpdated))]
    private int currentHealth;

    public event Action ServerOnDie;
    public event Action<int, int> ClientOnHealthUpdated;

    #region Server

    public override void OnStartServer()
    {
        currentHealth = maxHealth;

        UnitBase.ServerOnPlayerDie += ServeHandlePlayerDie;
    }

    public override void OnStopServer()
    {
        UnitBase.ServerOnPlayerDie -= ServeHandlePlayerDie;
    }

    private void ServeHandlePlayerDie(int connectionId)
    {
        if(connectionToClient.connectionId != connectionId) { return; }

        // It`s a way to destroy an object when player dies
        // In other words, everything that has a Health script will die if it base dies
        DealDamage(currentHealth);
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

    // This function get called when the SyncVar is changed
    private void HandleHealthUpdated(int oldHealth, int newHealth)
    {
        ClientOnHealthUpdated?.Invoke(oldHealth, newHealth);
    }

    #endregion

}
