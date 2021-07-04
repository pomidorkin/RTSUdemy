using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class UnitMovement : NetworkBehaviour
{
    [SerializeField] private NavMeshAgent agent = null;
    private Camera mainCamera;

    #region Server

    // Movement is done via NavigationMesh (moving player using mouse click)
    [Command]
    private void CmdMove(Vector3 position)
    {
        // Returns true or false if the position is valid
        if (!NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas)) { return; }
        agent.SetDestination(hit.position);
    }

    #endregion

    #region Client

    // OnStartAuthority() is called only for us, whereas if we use the Start() method,
    // all players will get the camera for all instances of the player
    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        mainCamera = Camera.main;
    }

    // Since the Update() method is called on the client and on the server,
    // we need to use the [ClientCallback] attribute. It prevents the server from running Update().
    // And in order to run this method only on our client, we need to check the authority
    [ClientCallback]
    private void Update()
    {
        if (!hasAuthority) { return; }
        if (!Mouse.current.rightButton.wasPressedThisFrame) { return; }

        // Raycasting
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if(!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity)){ return; }
        // At this point we know that we are a client with authority and we have clicked on a NavMesh
        CmdMove(hit.point);

    }

    #endregion

}
