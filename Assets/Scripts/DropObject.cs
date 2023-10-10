using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropObject : MonoBehaviour
{
    [SerializeField] private int level;
    [SerializeField] private CircleCollider2D trigger;

    private void Start()
    {
        gameObject.name = level.ToString();
        tag = "Drop Object";
    }

    public void Upgrade()
    {
        level++;
        gameObject.name = level.ToString();
        transform.localScale *= 1.2f;
        StartCoroutine(Upgrading());
    }

    // TriggerEnter�� �۵���Ű�� ���� 1�������� ���̿� �ΰ� trigger�� ����, �� ��.
    private IEnumerator Upgrading()
    {
        trigger.enabled = false;
        yield return null;
        trigger.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Drop Object")
        {
            if (collision.gameObject.name == gameObject.name)
            {
                Upgrade();
                Destroy(collision.gameObject);
            }
        }
    }
}
