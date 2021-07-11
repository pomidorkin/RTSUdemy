using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targeter : NetworkBehaviour
{

    private Targetable taget;

    public Targetable GetTarget()
    {
        return taget;
    }

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
        ClearTarget();
    }

    [Command]
    public void CmdSetTarger(GameObject targetGameObject)
    {
        if(!targetGameObject.TryGetComponent<Targetable>(out Targetable target)) { return; }
        this.taget = target;
    }

    [Server]
    public void ClearTarget()
    {
        taget = null;
    }

    #endregion

    #region Client

    #endregion

}
