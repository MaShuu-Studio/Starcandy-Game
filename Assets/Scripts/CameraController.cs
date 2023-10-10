using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get { return instance; } }
    private static CameraController instance;

    private Camera cam;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        cam = Camera.main;
    }

    public Vector3 ScreenToWorldPoint(Vector3 screenPoint)
    {
        return cam.ScreenToWorldPoint(screenPoint);
    }
}
