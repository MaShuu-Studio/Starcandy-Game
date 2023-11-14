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
        /* �ڽ��� ������� 6.5 * 8�� ��������.
         * 
         * ���彺������ ��������
         * ī�޶��� �⺻ ������� 5, ��ġ�� 0.5��. 
         * ���� ī�޶��� �⺻ ���� ������ 10.
         * �ڽ� ���� ȭ���� �Ʒ��� 0.5, ���� 1.5��ŭ ����.
         * �̴� �ٲ��� ������ ��������.
         * 
         * �̸� Portrait���� ��ȯ��ų �� ���ΰ� 10�̸� ���δ� 5 ��, ȭ���� ������ ���� �޶����� ��.
         * ���δ� 6.5�� Ŀ���ؾ��ϹǷ� �뷫 7������ ����� ������ ����.
         * size * 2 = h
         * ratio = h / w = height / width
         * h = w * ratio = size * 2
         * size = w * ratio / 2
         * 
         * ���࿡ 1440 * 2960�� �������� ����.
         * ratio�� 2�� ��.
         * w�� 7�� ������ ���̹Ƿ� size = 7 * 2 / 2 = 7
         * h�� 14�� �ǹǷ� �ڽ��� ��ġ�� �ϴ� 0.5 ���� ����ٰ� �����ϸ�
         * ī�޶��� ��ġ�� h - 8 / 2 - 0.5 = size - 4 - 0.5 = 2.5
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
