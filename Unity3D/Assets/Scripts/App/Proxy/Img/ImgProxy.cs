using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using System.Runtime.InteropServices;

public class ImgProxy : MonoBehaviour, IImgProxy
{
    public enum TextureCB_CallerID { APP_IMG, APP_UNIT }

    [DllImport("__Internal")]
    private static extern void JS_ReqLastUrls();

    [DllImport("__Internal")]
    private static extern void JS_PrepareImgUpload();

    [DllImport("__Internal")]
    private static extern void JS_UnPrepareImgUpload();

    [DllImport("__Internal")]
    private static extern void JS_ReqDeleteImgByDateKey(string imgDateKey);

    [Inject]
    IUnitServiceReceiver _unitSrvReceiver;

    [Inject]
    IImgServiceReceiver _imgSrvReceiver;

    void Start() {
    }

    public virtual void ReqTexture(string url, TextureCB_CallerID callerId) {
        // TODO: error handling
        if (!string.IsNullOrEmpty(url)) {
            StartCoroutine(OutputRoutine(url, callerId));
        }
    }

    private IEnumerator OutputRoutine(string url, TextureCB_CallerID callerId)
    {
        var loader = new WWW(url);
        yield return loader;
        Texture2D _texture;
        _texture = new Texture2D(loader.texture.width, loader.texture.height);
        _texture = loader.texture;
        if (callerId == TextureCB_CallerID.APP_IMG)
        {
            _imgSrvReceiver.TextureCB(_texture, url);
        }
        else if (callerId == TextureCB_CallerID.APP_UNIT) {
            _unitSrvReceiver.TextureU_CB(_texture, url);
        }
    }

    public virtual void ReqLastUrls()
    {
        JS_ReqLastUrls();
    }

    public virtual void LastUrlsCB(string urls)// urls: JSON array
    {
        _imgSrvReceiver.LastUrlsCB(urls);
    }

    public virtual void PrepareImgUpload(bool toSet) {
        if (toSet)
            JS_PrepareImgUpload();
        else
            JS_UnPrepareImgUpload();
    }

    public void ReqDeleteImgByDateKey(string imgDateKey) {
        JS_ReqDeleteImgByDateKey(imgDateKey);
    }
}
