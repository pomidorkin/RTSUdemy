using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class UnitMovement : NetworkBehaviour
{
    [SerializeField] private NavMeshAgent agent = null;
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private float chaseRange = 10f;
    // private Camera mainCamera;

    #region Server

    public override void OnStartServer()
    {
        GameoverHandler.ServerOnGameover += ServerHandleGameover;
    }

    public override void OnStopServer()
    {
        GameoverHandler.ServerOnGameover -= ServerHandleGameover;
    }

    [Server]
    private void ServerHandleGameover()
    {
        agent.ResetPath();
    }



    // The Server tag does not allow the code to be run on a client, but it sends warnings.
    // ServerCallback doest the exact same thing, but does not print any warnings saying that
    // a client is trying to run this code
    [ServerCallback]
    private void Update()
    {
        Targetable target = targeter.GetTarget();
        // Chasing logic
        if (target != null)
        {
            // Checking the distance between the object and it target
            if ((target.transform.position - transform.position).sqrMagnitude > chaseRange * chaseRange) 
            {
                //chase
                agent.SetDestination(target.transform.position);
            } else if (agent.hasPath)
            {
                // Stop chasing
                agent.ResetPath();
            }
            return;
        }

        // If there`s no target, we do the normal logic for click and move

        if (agent.hasPath) { return; }

        if(agent.remainingDistance > agent.stoppingDistance) { return; } // Checking if the object has reached it`s target yet

        agent.ResetPath(); // Clearing the path, so that the object will not try to get to its target continuously
    }

    // Movement is done via NavigationMesh (moving player using mouse click)
    [Command]
    public void CmdMove(Vector3 position)
    {
        targeter.ClearTarget();
        // Returns true or false if the position is valid
        if (!NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas)) { return; }
        agent.SetDestination(hit.position);
    }

    #endregion

    //#region Client

    //// OnStartAuthority() is called only for us, whereas if we use the Start() method,
    //// all players will get the camera for all instances of the player
    //public override void OnStartAuthority()
    //{
    //    base.OnStartAuthority();
    //    mainCamera = Camera.main;
    //}

    //// Since the Update() method is called on the client and on the server,
    //// we need to use the [ClientCallback] attribute. It prevents the server from running Update().
    //// And in order to run this method only on our client, we need to check the authority
    //[ClientCallback]
    //private void Update()
    //{
    //    if (!hasAuthority) { return; }
    //    if (!Mouse.current.rightButton.wasPressedThisFrame) { return; }

    //    // Raycasting
    //    Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

    //    if(!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity)){ return; }
    //    // At this point we know that we are a client with authority and we have clicked on a NavMesh
    //    CmdMove(hit.point);

    //}

    //#endregion

}
