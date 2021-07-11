using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameoverHandler : NetworkBehaviour
{
    public static event Action ServerOnGameover;
    public static event Action<string> ClientOnGameOver;

    private List<UnitBase> bases = new List<UnitBase>();

    #region Server
    public override void OnStartServer()
    {
        UnitBase.ServerOnBaseSpawned += ServerHandleBaseSpawned;
        UnitBase.ServerOnBaseDespawned += ServerHandleBaseDespawned;
    }

    public override void OnStopServer()
    {
        UnitBase.ServerOnBaseSpawned -= ServerHandleBaseSpawned;
        UnitBase.ServerOnBaseDespawned -= ServerHandleBaseDespawned;
    }

    [Server]
    private void ServerHandleBaseSpawned(UnitBase unitBase)
    {
        bases.Add(unitBase);
    }

    [Server]
    private void ServerHandleBaseDespawned(UnitBase unitBase)
    {
        bases.Remove(unitBase);

        if(bases.Count != 1) { return; }

        // We have a lisr of bases and when only one player is left (the winner) whe get the
        // first (and the only one) base fron the bases list and get it`s clinet`s connection id
        int playerId = bases[0].connectionToClient.connectionId;

        RpcGameOver($"Player: {playerId}");

        ServerOnGameover?.Invoke();
    }
    #endregion

    #region Client

    [ClientRpc]
    private void RpcGameOver(string winner)
    {
        ClientOnGameOver?.Invoke(winner);
    }

    #endregion
}
