using System;
using UnityEngine;

public class AspectRatioUtility : MonoBehaviour
{
    [SerializeField] float targetAspectRatio = 16f / 9f;
    
    private Camera camera;

    private void Awake()
    {
        camera = Camera.main;;
    }

    private void Update()
    {
        Adjust();
    }

    private void Adjust()
    {
        float windowAspectRatio = (float)Screen.width / (float)Screen.height;
        float scaleHeight = windowAspectRatio/targetAspectRatio;
        if (scaleHeight < 1f)
        {
            Rect rect = camera.rect;
            rect.width = 1f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1f - scaleHeight) / 2f;
            
            camera.rect = rect;
        }
        else
        {
            float scaleWidth = 1f/scaleHeight;
            Rect rect = camera.rect;
            
            rect.width = scaleWidth;
            rect.height = 1f;
            rect.x = (1f - scaleWidth) / 2f;
            rect.y = 0;
            
            camera.rect = rect;
        }
    }
}