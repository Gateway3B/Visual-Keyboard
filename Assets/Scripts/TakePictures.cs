using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakePictures : MonoBehaviour
{
    public Transform target;
    public float dist, height, maxHeight;
    private List<Vector2> pos;
    private int number;
    private float initHeight, side, maxSide;
    private Camera myCamera;
    private static TakePictures instance;
    private bool takeScreenshotOnNextFrame;
    void Start()
    {
        pos = new List<Vector2>();
        instance = this;
        myCamera = gameObject.GetComponent<Camera>();
        number = 0;
        side = Mathf.Abs(this.transform.position.x);
        maxSide = -side;
        this.transform.position = new Vector3(side+dist, height, 0);
        initHeight = height;
        
    }
    void Update()
    {
        Vector3 rotateAround = new Vector3(side, 0, 0);
        this.transform.LookAt(rotateAround, Vector3.up);
        this.transform.RotateAround(rotateAround, Vector3.up, 10);

        if(this.transform.eulerAngles.y > 90 && this.transform.eulerAngles.y < 250)
        {
            if(height == maxHeight)
            {
                height = initHeight;
                side += Mathf.Abs(maxSide)/maxSide;
                if(Mathf.Abs(side) >= Mathf.Abs(maxSide))
                {
                    enabled = false;
                }
                this.transform.position = new Vector3(side+dist, height, 0);                
            }
            else
            {
                Debug.Log("Flipped");
                height += 1;
                this.transform.position = new Vector3(side+dist, height, 0);
            }
        }
        else
        {
            Vector2 temp = myCamera.WorldToScreenPoint(target.position);
            if(temp[0] > 0 && temp[1] > 0 && temp[0] < 1024 && temp[1] < 512)
            {
                pos.Add(temp);
                Debug.Log(pos[pos.Count-1]);
                takeScreenshot(Screen.width, Screen.height);
            }
        }

    }

    private void OnPostRender()
    {
        if(takeScreenshotOnNextFrame)
        {
            takeScreenshotOnNextFrame = false;
            RenderTexture renderTexture = myCamera.targetTexture;
            Texture2D renderResult = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
            Rect rect = new Rect(0, 0, renderTexture.width, renderTexture.height);
            renderResult.ReadPixels(rect, 0, 0);

            byte[] byteArray = renderResult.EncodeToPNG();
            System.IO.File.WriteAllBytes(Application.dataPath + "/Screenshots/" + number + pos[number] + ".png", byteArray);
            Debug.Log("Saved: " + number + pos[number] + ".png");
            number++;

            RenderTexture.ReleaseTemporary(renderTexture);
            myCamera.targetTexture = null;
        }
    }

    private void TakeScreenshot(int width, int height)
    {
        myCamera.targetTexture = RenderTexture.GetTemporary(width, height, 16);
        takeScreenshotOnNextFrame = true;
    }

    public static void takeScreenshot(int width, int height)
    {
        instance.TakeScreenshot(width, height);
    }
}
