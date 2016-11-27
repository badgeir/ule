using UnityEngine;
using System.Collections;
using System;
using SimpleJSON;

public class ColorCamera : Sensor 
{
    public string mName;
    public RenderTexture mRenderTexture;

    private int mDimX, mDimY;

    string image;

    void Start()
    {
        mDimX = mRenderTexture.width;
        mDimY = mRenderTexture.height;


        GetComponent<Camera>().Render();
        RenderTexture.active = mRenderTexture;
        Texture2D tex = new Texture2D(mDimX, mDimY, TextureFormat.RGB24, false);

        tex.ReadPixels(new Rect(0, 0, mDimX, mDimY), 0, 0);
        tex.Apply();
        RenderTexture.active = null;

        byte[] bytes = tex.EncodeToPNG();
        image = Convert.ToBase64String(bytes);
    }

    public override string name()
    {
        return mName;
    }

    public override JSONNode JsonDescription()
    {
        JSONClass json = new JSONClass();
        json["name"] = mName;
        json["type"] = "camera";
        json["width"].AsInt = mDimX;
        json["heigth"].AsInt = mDimY;
        json["channels"].AsInt = 3;
        return json;
    }

    public override JSONNode SampleJson()
    {
        GetComponent<Camera>().Render();
        RenderTexture.active = mRenderTexture;
        Texture2D tex = new Texture2D(mDimX, mDimY, TextureFormat.RGB24, false);

        tex.ReadPixels(new Rect(0, 0, mDimX, mDimY), 0, 0);
        tex.Apply();
        RenderTexture.active = null;

        byte[] bytes = tex.EncodeToPNG();

        JSONClass json = new JSONClass();
        json["name"] = mName;
        json["value"] = image;

        return json;
    }
}