using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnitProvider {
    float GetCurrentUnitMagnitude();
    bool UnitUrlImgAllAvailable();
    Texture2D GetTexture2D();
    string GetCurrentUrl();
    UnitLatest GetUnitLatestDS();
    void ReqSaveUnitJSON(string uJSON);
    void ReNewUnitLatestDS();
    void QueryUnit();
}
