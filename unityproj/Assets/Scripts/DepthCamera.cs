using UnityEngine;
using System.Collections;
using System.IO;
using System.Xml.Serialization;

public class DepthCamera : VirtualCamera 
{
    public RenderTexture mRenderTexture;
	public Material mDepthMaterial;

    private int mDimX, mDimY;

	void Start () {
		Camera cam = GetComponent<Camera> ();
        cam.depthTextureMode = DepthTextureMode.Depth;
        cam.depth = 24;

        mDimX = mRenderTexture.width;
        mDimY = mRenderTexture.height;
    }

	public void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, mDepthMaterial);
	}

    public override byte[] GetImageBytes()
    {
        //GetComponent<Camera>().Render();
        RenderTexture.active = mRenderTexture;
        Texture2D tex = new Texture2D(mDimX, mDimY, TextureFormat.ARGB32, false);

        tex.ReadPixels(new Rect(0, 0, mDimX, mDimY), 0, 0);
        tex.Apply();
        RenderTexture.active = null;

        byte[] bytes = tex.EncodeToPNG();
        return bytes;
    }
}
