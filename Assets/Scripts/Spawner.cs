using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public static Spawner Instance { get { return instance; } }
    private static Spawner instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    [SerializeField] private DropObject objPrefab;
    [SerializeField] private LineRenderer line;

    [SerializeField] private float boundary;
    [SerializeField] private float height;

    private Vector3 linePos;

    private Pool<DropObject> pool;
    private List<DropObject> objects;
    private DropObject curObject;

    // Update is called once per frame
    private void Start()
    {
        objPrefab.MakePrefab("");

        GameObject go = new GameObject("Pool");
        go.transform.SetParent(transform);
        pool = go.AddComponent<DropObjectPool>();
        pool.Init(objPrefab);

        objects = new List<DropObject>();
        int layerMask = 1 << LayerMask.NameToLayer("Box");
        RaycastHit2D hit = Physics2D.Raycast(objPrefab.transform.position, Vector2.down, 20f, layerMask);
        objPrefab.gameObject.SetActive(false);

        CreateObject();
        linePos = new Vector3(curObject.transform.position.x, hit.point.y);
    }

    void Update()
    {
        Vector3 pos = CameraController.Instance.ScreenToWorldPoint(Input.mousePosition);
        if (pos.x > boundary) pos.x = boundary;
        else if (pos.x < -boundary) pos.x = -boundary;
        pos.y = height;
        pos.z = 0;
        curObject.transform.position = pos;

        line.SetPosition(0, pos);
        linePos.x = pos.x;
        line.SetPosition(1, linePos);

        if (Input.GetMouseButtonDown(0))
        {
            SoundController.Instance.AddSfx("DROP");
            curObject.ActiveObject(true);
            CreateObject();
        }
    }

    public void CreateObject()
    {
        curObject = pool.Pop();
        objects.Add(curObject);
    }
}
