using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IImgProvider {
    bool CurrentImgUrlBothExist();
    string GetCurrentUrl();
    Texture2D GetTexture2D();
    void ReNewAppImgDS();// drop current and make a new empty DS
    Dictionary<string, string> GetUrlDict();
    void ReqDeleteImgByDateKey(string imgDateKey);
    void DeleteFromAppImgDSByKey(string imgDateKey);
}