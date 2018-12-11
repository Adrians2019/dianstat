using System.Collections.Generic;
using UnityEngine;

public class QueryUnitTextureEvt
{
    public string url = "";
}

public class QueryTextureEvt
{
    public string url = "";
}

public class UrlsUpdateEvt
{
    public string lastReqUrl = "";
    public Texture2D urlTexture = null;
    public Dictionary<string, string> urlsDict = null;
    public string LastUploadedImg = "";
}

