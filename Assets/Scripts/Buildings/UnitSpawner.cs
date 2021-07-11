using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField] private Health health = null;
    [SerializeField] private GameObject unitPrefab = null;
    [SerializeField] private Transform unitSpawnPoint;

    // Unity will call this function for me whenever I click this GameObject
    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button != PointerEventData.InputButton.Left) { return; }
        if (!hasAuthority) { return; }
        CmdSpawnUnit();
    }

    #region Server

    public override void OnStartServer()
    {
        health.ServerOnDie += HandleServerOnDie;
    }

    public override void OnStopServer()
    {
        health.ServerOnDie -= HandleServerOnDie;
    }

    [Server]
    private void HandleServerOnDie()
    {
        //NetworkServer.Destroy(gameObject);
    }

    [Command]
    private void CmdSpawnUnit()
    {
        // Istantiating an object, but it is just on the server and there`s nothing networked about it rn
        GameObject unitInstance = Instantiate(unitPrefab, unitSpawnPoint.position, unitSpawnPoint.rotation);
        // Here we spawn the object on the network, in other words, we spawn it on all clients.
        // connectionToClient is the owner connection, if we do not pass it it`s just a server object
        // connectionOnClient exists in the NetworkBehaviour. So, because this spawner belongs to me,
        // the unit that spawns also belongs to me. The server here is gaying: "Give authority to the connectionToClient"
        NetworkServer.Spawn(unitInstance, connectionToClient);
    }

    #endregion

    #region Client

    #endregion
}
