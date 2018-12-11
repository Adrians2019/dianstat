using System.Collections.Generic;
using UnityEngine;

public class RegionInitEvt {}

public class NoUrlImgEvt { }

public class AppImgCurUrlImgEvt {
    public string _appUrl;
    public Texture2D _appTexture;
}

public class ReqRegionDSSaveEvt {
    public Region regionDS = new Region();
}

public class RegionUpdatedEvt {
    public Region newRegion = new Region();
}

public class RegionsDownloaded {
    public string url = "";
}