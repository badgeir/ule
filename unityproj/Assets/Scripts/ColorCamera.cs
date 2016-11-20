using UnityEngine;
using System.Collections;

public class ColorCamera : VirtualCamera 
{

    public RenderTexture mRenderTexture;

    private int mDimX, mDimY;

    void Start()
    {
        mDimX = mRenderTexture.width;
        mDimY = mRenderTexture.height;
    }

    public override byte[] GetImageBytes()
    {
        GetComponent<Camera>().Render();
        RenderTexture.active = mRenderTexture;
        Texture2D tex = new Texture2D(mDimX, mDimY, TextureFormat.RGB24, false);

        tex.ReadPixels(new Rect(0, 0, mDimX, mDimY), 0, 0);
        tex.Apply();
        RenderTexture.active = null;

        byte[] bytes = tex.EncodeToPNG();
        return bytes;
    }
}