using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropObject : MonoBehaviour
{
    private int level;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private CircleCollider2D trigger;
    private bool active;

    private void Start()
    {
        level = 1;
        tag = "Drop Object";
        ActiveObject(false);
    }

    public void ActiveObject(bool b)
    {
        if (b) _rigidbody.bodyType = RigidbodyType2D.Dynamic;
        else _rigidbody.bodyType = RigidbodyType2D.Static;

        active = b;
        Naming();
    }

    public void Naming()
    {
        gameObject.name = (active ? "T" : "F") + level.ToString();
    }

    public void Upgrade()
    {
        level++;
        Naming();
        transform.localScale *= 1.2f;
        StartCoroutine(Upgrading());
    }

    // TriggerEnter를 작동시키기 위해 1프레임을 사이에 두고 trigger를 오프, 온 함.
    private IEnumerator Upgrading()
    {
        trigger.enabled = false;
        yield return null;
        trigger.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Drop Object"
            && active && collision.name[0] == 'T')
        {
            if (collision.gameObject.name == gameObject.name)
            {
                Upgrade();
                Destroy(collision.gameObject);
            }
        }
    }
}
