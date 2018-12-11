using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IImgProxy
{
    void ReqLastUrls();
    void ReqTexture(string url, ImgProxy.TextureCB_CallerID callerId);
    void PrepareImgUpload(bool toSet);
    void ReqDeleteImgByDateKey(string imgDateKey);
}
