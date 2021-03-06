﻿using UnityEngine;
using System.Collections;
using System;
using SimpleJSON;

using ULE;

public class ColorCamera : Sensor 
{
    public string mName;

    private Camera mCamera;
    public RenderTexture mRenderTexture;

    private int mDimX, mDimY;

    void Start()
    {
        mCamera = GetComponent<Camera>();
        mCamera.enabled = true;

        mDimX = mRenderTexture.width;
        mDimY = mRenderTexture.height;

        GameObject.Find("Agent").GetComponent<ReinforcementAgent>().AddSensor(this);
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

    public override string SampleJson()
    {
        mCamera.targetTexture = mRenderTexture;
        RenderTexture.active = mRenderTexture;
        mCamera.Render();
        Texture2D tex = new Texture2D(mDimX, mDimY, TextureFormat.RGB24, false);

        tex.ReadPixels(new Rect(0, 0, mDimX, mDimY), 0, 0);
        tex.Apply();
        RenderTexture.active = null;
        mCamera.targetTexture = null;

        byte[] bytes = tex.EncodeToPNG();
        string image = Convert.ToBase64String(bytes);
        string json = string.Format("{{\"name\": \"{0}\", \"value\": \"{1}\"}}", mName, image);
        return json;
    }
}