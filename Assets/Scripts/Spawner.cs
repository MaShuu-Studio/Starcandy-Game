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

    [SerializeField] private GameObject box;
    [SerializeField] private GameOverLine gameOverLine;
    [SerializeField] private LineRenderer line;
    private Vector3 linePos;

    [SerializeField] private float boundary;
    [SerializeField] private float height;

    private float objBoundary;

    public Sprite[] Sprites { get { return sprites; } }
    private Sprite[] sprites;

    public float[] ObjectSizes { get { return objectSizes; } }
    private float[] objectSizes;

    [SerializeField] private DropObject objPrefab;
    private Pool<DropObject> pool;
    private List<DropObject> objects;
    private DropObject curObject;
    private int nextLevel;
    private bool ready;
    private bool isLoad = false;

    // Update is called once per frame
    public void Init()
    {
        objectSizes = new float[]
        {
            0.43f,
            0.56f,
            0.73f,
            0.92f,
            1.18f,
            1.52f,
            1.75f,
            2.1f,
            2.37f,
            2.93f,
            3.45f
        };

        objPrefab.MakePrefab("");

        GameObject go = new GameObject("Pool");
        go.transform.SetParent(transform);
        pool = go.AddComponent<DropObjectPool>();
        pool.Init(objPrefab);

        objects = new List<DropObject>();
        int layerMask = 1 << LayerMask.NameToLayer("Box");
        RaycastHit2D hit = Physics2D.Raycast(objPrefab.transform.position, Vector2.down, 20f, layerMask);
        linePos = new Vector3(0, hit.point.y);
        objPrefab.gameObject.SetActive(false);
        line.gameObject.SetActive(false);
        box.SetActive(false);
        isLoad = true;
    }

    public void StartGame()
    {
        while (objects.Count > 0)
        {
            ReturnObject(objects[0]);
        }

        sprites = SpriteManager.Instance.GetSprites;
        UIController.Instance.SetGrade();
        box.SetActive(true);

        ready = false;
        nextLevel = Random.Range(0, 5);
        CreateObject();
    }

    public void StopGame()
    {
        ready = false;
        line.gameObject.SetActive(false);
    }

    public void Title()
    {
        StopGame();
        while (objects.Count > 0)
        {
            ReturnObject(objects[0]);
        }
        box.SetActive(false);
    }

    public bool CheckGameOver(float posY)
    {
        return posY > gameOverLine.transform.position.y;
    }

    void Update()
    {
        if (isLoad == false) return;

        if (GameController.Instance.Pause == false && ready)
        {
            Vector3 pos = GetPointPos();
            curObject.transform.position = pos;

            line.SetPosition(0, pos);
            linePos.x = pos.x;
            line.SetPosition(1, linePos);

            if (Input.GetMouseButtonUp(0)) //Android
            //if (Input.GetMouseButtonDown(0)) // PC
            {
                ready = false;
                SoundController.Instance.AddSfx("DROP");
                curObject.ActiveObject(true);
                line.gameObject.SetActive(false);
            }
        }
    }

    private Vector3 GetPointPos()
    {
        Vector3 pos = CameraController.Instance.ScreenToWorldPoint(Input.mousePosition);
        if (pos.x > objBoundary) pos.x = objBoundary;
        else if (pos.x < -objBoundary) pos.x = -objBoundary;
        pos.y = height;
        pos.z = 0;

        return pos;
    }

    public void CreateObject()
    {
        if (ready) return;

        curObject = pool.Pop();
        curObject.SetLevel(nextLevel + 1, sprites[nextLevel], objectSizes[nextLevel]);

        objBoundary = boundary - curObject.transform.localScale.x / 2;
        curObject.transform.position = GetPointPos();

        nextLevel = Random.Range(0, 5);
        UIController.Instance.SetNext(sprites[nextLevel], objectSizes[nextLevel]);

        objects.Add(curObject);

        line.gameObject.SetActive(true);
        ready = true;
    }

    public void ReturnObject(DropObject dropObject)
    {
        if (objects.Contains(dropObject) == false) return;

        objects.Remove(dropObject);
        pool.Push(dropObject);

        if (dropObject.Dropping) CreateObject();
        dropObject.ActiveObject(false);
    }
}
