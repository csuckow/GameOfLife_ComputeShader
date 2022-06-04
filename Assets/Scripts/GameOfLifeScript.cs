using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GameOfLifeScript : MonoBehaviour
{

    public Texture2D initialTexture;
    public int framesPerIteration = 10;

    private ComputeShader computeShader;

    private RenderTexture outputTexture;
    private RenderTexture inputTexture;
    private int width;
    private int height;

    private void OnRenderImage(RenderTexture source, RenderTexture destination) {
        if (Time.frameCount % framesPerIteration == 0) {
            //GetComponent<Screencapture>().SaveScreenshot();
            Render(destination);
        }
    }

    private void Render(RenderTexture destination) {
        InitializeRenderTextures();
        LoadComputeShader();

        computeShader.SetTexture(0, "_InputTexture", inputTexture);
        computeShader.SetTexture(0, "_OutputTexture", outputTexture);
        computeShader.SetFloat("_Width", width);
        computeShader.SetFloat("_Height", height);
        int threadGroupsX = Mathf.CeilToInt(width / 8.0f);
        int threadGroupsY = Mathf.CeilToInt(height / 8.0f);
        computeShader.Dispatch(0, threadGroupsX, threadGroupsY, 1);

        Graphics.Blit(outputTexture, destination);
        Graphics.Blit(outputTexture, inputTexture);
    }

    private void InitializeRenderTextures() {
        if (inputTexture == null || inputTexture.width != width || inputTexture.height != height) {

            //Texture2D initialTexture = LoadPNG(initialImagePath);
            width = initialTexture.width;
            height = initialTexture.height;

            // Release render texture if we already have one
            if (inputTexture != null) {
                inputTexture.Release();
            }
            // Get a render target
            inputTexture = new RenderTexture(width, height, 0,
                RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            inputTexture.enableRandomWrite = true;
            inputTexture.Create();

            // Release render texture if we already have one
            if (outputTexture != null) {
                outputTexture.Release();
            }
            // Get a render target
            outputTexture = new RenderTexture(width, height, 0,
                RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            outputTexture.enableRandomWrite = true;
            outputTexture.Create();

            // copy loaded image to graphics card
            Graphics.Blit(initialTexture, inputTexture);
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
