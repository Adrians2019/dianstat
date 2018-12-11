using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using System.Runtime.InteropServices;

public class UnitProxy : MonoBehaviour, IUnitProxy
{

    [DllImport("__Internal")]
    private static extern void JS_ReqLastUnitDS();

    [DllImport("__Internal")]
    private static extern void JS_ReqSaveUnitJSON(string uJSON);

    [Inject]
    IUnitServiceReceiver _srvReceiver;

    public virtual void ReqLastUnitDS()
    {
        JS_ReqLastUnitDS();
    }

    public void LastUnitDS_CB(string jsonUnitDS)
    {
        _srvReceiver.LastUnitDS_CB(jsonUnitDS);
    }

    public void SaveUnit_CB(string uJSON)
    {
        _srvReceiver.SaveUnit_CB(uJSON);
    }

    public void ReqSaveUnitJSON(string uJSON) {
        JS_ReqSaveUnitJSON(uJSON);
    }
}
