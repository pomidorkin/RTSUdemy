using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBase : NetworkBehaviour
{
    [SerializeField] Health health = null;

    public static event Action<int> ServerOnPlayerDie;
    public static event Action<UnitBase> ServerOnBaseSpawned;
    public static event Action<UnitBase> ServerOnBaseDespawned;

    #region Server
    public override void OnStartServer()
    {
        health.ServerOnDie += ServerHandleDeath;
        ServerOnBaseSpawned?.Invoke(this);
    }

    public override void OnStopServer()
    {
        ServerOnBaseDespawned?.Invoke(this);
        health.ServerOnDie -= ServerHandleDeath;
    }

    [Server]
    private void ServerHandleDeath()
    {
        ServerOnPlayerDie?.Invoke(connectionToClient.connectionId);
        NetworkManager.Destroy(gameObject);
    }
    #endregion

    #region CLient
    #endregion
}
