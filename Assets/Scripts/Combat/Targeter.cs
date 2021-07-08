using Mirror;
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
