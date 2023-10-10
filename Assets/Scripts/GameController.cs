using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private DropObject prefab;
    [SerializeField] private Camera cam;
    // Update is called once per frame
    private void Start()
    {
        prefab.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 pos = cam.ScreenToWorldPoint(Input.mousePosition);

            if (pos.x > 2.23f) pos.x = 2.23f;
            else if (pos.x < -2.23f) pos.x = -2.23f;
            pos.y = 2.7f;
            pos.z = 0;

            DropObject obj = Instantiate(prefab);
            obj.transform.position = pos;
            obj.gameObject.SetActive(true);
        }
    }
}
