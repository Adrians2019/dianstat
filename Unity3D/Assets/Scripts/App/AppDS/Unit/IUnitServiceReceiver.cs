using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnitServiceReceiver
{
    void LastUnitDS_CB(string unitLatestDS);
    void SaveUnit_CB(string uJSON);
    void TextureU_CB(Texture2D unitTexture, string utUrl);
}
