using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IREnvFSM
{
    void ReqStateChange(REnvState nextState);
    REnvState GetCurrentStateName();
    StateEntity GetCurrentStateEntity();
}
