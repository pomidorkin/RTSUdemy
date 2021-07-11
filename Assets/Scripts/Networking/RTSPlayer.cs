using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSPlayer : NetworkBehaviour
{
    [SerializeField] private List<Unit> myUnits = new List<Unit>();

    public List<Unit> GetUnits()
    {
        return myUnits;
    }

    #region Server
    public override void OnStartServer()
    {
        Unit.ServerOnUnitSpawned += HandleUnitSpawned;
        Unit.ServerOnUnitDespawned += HandleUnitDespawned;
    }

    public override void OnStopServer()
    {
        Unit.ServerOnUnitSpawned -= HandleUnitSpawned;
        Unit.ServerOnUnitDespawned -= HandleUnitDespawned;
    }
    
    // Since this script is on a plyaer, all players will have this scrips. We don`t want all players
    // to have same Lists of every unit in the game. We want each list to have units for that player
    private void HandleUnitSpawned(Unit unit)
    {
        // We are checking it the player who owns this unit is the playes who owns this script
        if(unit.connectionToClient.connectionId != connectionToClient.connectionId) { return; }

        myUnits.Add(unit);
    }

    private void HandleUnitDespawned(Unit unit)
    {
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId) { return; }

        myUnits.Remove(unit);
    }

    #endregion

    #region Client

    public override void OnStartAuthority()
    {
        if (NetworkServer.active) { return; } // Checking if we are the server, if true - return
        Unit.AuthorityOnUnitSpawned += AuthorityHandleUnitSpawned;
        Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;
    }

    public override void OnStopClient()
    {
        if (!isClientOnly || !hasAuthority) { return; }
        Unit.AuthorityOnUnitSpawned -= AuthorityHandleUnitSpawned;
        Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
    }

    private void AuthorityHandleUnitSpawned(Unit unit)
    {
        myUnits.Add(unit);
    }

    private void AuthorityHandleUnitDespawned(Unit unit)
    {
        myUnits.Remove(unit);
    }

    #endregion
}
