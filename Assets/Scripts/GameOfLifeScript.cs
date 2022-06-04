using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GameOfLifeScript : MonoBehaviour
{

    public Texture2D initialTexture;
    public int framesPerIteration = 10;

    private ComputeShader computeShader;

    private RenderTexture renderTexture;
    private int width;
    private int height;

    private void OnRenderImage(RenderTexture source, RenderTexture destination) {
        if (Time.frameCount % framesPerIteration == 0) {
            Render(destination);
        }
    }

    private void Render(RenderTexture destination) {
        InitializeRenderTexture();
        LoadComputeShader();

        computeShader.SetTexture(0, "_RenderTexture", renderTexture);
        computeShader.SetFloat("_Width", width);
        computeShader.SetFloat("_Height", height);
        int threadGroupsX = Mathf.CeilToInt(width / 8.0f);
        int threadGroupsY = Mathf.CeilToInt(height / 8.0f);
        computeShader.Dispatch(0, threadGroupsX, threadGroupsY, 1);

        Graphics.Blit(renderTexture, destination);
    }

    private void InitializeRenderTexture() {
        if (renderTexture == null || renderTexture.width != width || renderTexture.height != height) {

            //Texture2D initialTexture = LoadPNG(initialImagePath);
            width = initialTexture.width;
            height = initialTexture.height;

            // Release render texture if we already have one
            if (renderTexture != null)
                renderTexture.Release();
            // Get a render target
            renderTexture = new RenderTexture(width, height, 0,
                RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            renderTexture.enableRandomWrite = true;
            renderTexture.Create();

            // copy loaded image to graphics card
            Graphics.Blit(initialTexture, renderTexture);
        }
    }

    private void LoadComputeShader() {
        if (computeShader == null) {
            computeShader = (ComputeShader)AssetDatabase.LoadAssetAtPath("Assets/Shaders/GameOfLifeComputeShader.compute", typeof(ComputeShader));
        }
    }

    // loading image files to texture2d (from https://gist.github.com/openroomxyz/bb22a79fcae656e257d6153b867ad437)
    private static Texture2D LoadPNG(string filePath) {
        Texture2D tex = null;
        byte[] fileData;

        if (System.IO.File.Exists(filePath)) {
            fileData = System.IO.File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData);
        } else {
            throw new System.Exception("File not found!");
        }
        return tex;
    }
}
