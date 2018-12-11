using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IImgServiceReceiver {
    void LastUrlsCB(string urls);
    void TextureCB(Texture2D uTexture, string url);
}
