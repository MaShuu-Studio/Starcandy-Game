using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get { return instance; } }
    private static CameraController instance;

    private Camera cam;
    public float Ratio { get { return ratio; } }
    private float ratio;

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
        ratio = (float)Screen.height / Screen.width;
        if (ratio < 1) ratio = 1 / ratio;
    }

    public void SetCameraSize(bool isPortrait)
    {
        /* 박스의 사이즈는 6.5 * 8의 사이즈임.
         * 
         * 랜드스케이프 기준으로
         * 카메라의 기본 사이즈는 5, 위치는 0.5임. 
         * 따라서 카메라의 기본 세로 범위는 10.
         * 박스 기준 화면의 아래는 0.5, 위는 1.5만큼 남음.
         * 이는 바뀌지 않으며 고정값임.
         * 
         * 이를 Portrait으로 변환시킬 때 세로가 10이면 가로는 5 등, 화면의 비율에 따라 달라지게 됨.
         * 가로는 6.5를 커버해야하므로 대략 7정도의 사이즈가 나오면 좋음.
         * size * 2 = h
         * ratio = h / w = height / width
         * h = w * ratio = size * 2
         * size = w * ratio / 2
         * 
         * 만약에 1440 * 2960의 사이즈라고 가정.
         * ratio는 2가 됨.
         * w는 7로 고정할 것이므로 size = 7 * 2 / 2 = 7
         * h는 14가 되므로 박스의 위치를 하단 0.5 위로 맞춘다고 가정하면
         * 카메라의 위치는 h - 8 / 2 - 0.5 = size - 4 - 0.5 = 2.5
         */

        float size, pos;
        if (isPortrait)
        {
            size = 7 * ratio / 2;
            pos = size - 4.5f;
        }
        else
        {
            size = 5;
            pos = 0.5f;
        }
        cam.transform.position = new Vector3(0, pos, -10);
        cam.orthographicSize = size;
    }

    public Vector3 ScreenToWorldPoint(Vector3 screenPoint)
    {
        return cam.ScreenToWorldPoint(screenPoint);
    }
}
