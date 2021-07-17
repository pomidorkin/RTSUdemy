using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSPlayer : NetworkBehaviour
{
    private List<Unit> myUnits = new List<Unit>();
    private List<Building> myBuildings = new List<Building>();

    public List<Unit> GetUnits()
    {
        return myUnits;
    }

    public List<Building> GetBuildings()
    {
        return myBuildings;
    }

    #region Server
    public override void OnStartServer()
    {
        Unit.ServerOnUnitSpawned += HandleUnitSpawned;
        Unit.ServerOnUnitDespawned += HandleUnitDespawned;
        Building.ServerOnBuildingSpawned += HandleBuildingSpawned;
        Building.ServerOnBuildingDespawned += HandleBuildingDespawned;
    }

    public override void OnStopServer()
    {
        Unit.ServerOnUnitSpawned -= HandleUnitSpawned;
        Unit.ServerOnUnitDespawned -= HandleUnitDespawned;
        Building.ServerOnBuildingSpawned -= HandleBuildingSpawned;
        Building.ServerOnBuildingDespawned -= HandleBuildingDespawned;
    }


    private void HandleBuildingSpawned(Building building)
    {
        // Checking if the builnging`s owner id is the same with this player`s id
        if (building.connectionToClient.connectionId != connectionToClient.connectionId) { return; }

        myBuildings.Add(building);
    }

    private void HandleBuildingDespawned(Building building)
    {
        if (building.connectionToClient.connectionId != connectionToClient.connectionId) { return; }

        myBuildings.Remove(building);
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
        Building.AuthorityOnBuildingSpawned += AuthorityHandleBuildingSpawned;
        Building.AuthorityOnBuildingDespawned += AuthorityHandleBuildingDespawned;
    }

    public override void OnStopClient()
    {
        if (!isClientOnly || !hasAuthority) { return; }
        Unit.AuthorityOnUnitSpawned -= AuthorityHandleUnitSpawned;
        Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
        Building.AuthorityOnBuildingSpawned -= AuthorityHandleBuildingSpawned;
        Building.AuthorityOnBuildingDespawned -= AuthorityHandleBuildingDespawned;
    }

    private void AuthorityHandleUnitSpawned(Unit unit)
    {
        myUnits.Add(unit);
    }

    private void AuthorityHandleUnitDespawned(Unit unit)
    {
        myUnits.Remove(unit);
    }

    private void AuthorityHandleBuildingSpawned(Building building)
    {
        myBuildings.Add(building);
    }

    private void AuthorityHandleBuildingDespawned(Building building)
    {
        myBuildings.Remove(building);
    }

    #endregion
}
