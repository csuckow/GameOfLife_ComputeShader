using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Screencapture : MonoBehaviour
{
    public void SaveScreenshot()
    {
        ScreenCapture.CaptureScreenshot("Screencapture/" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png");
    }
}
