using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropObject : Poolable
{
    private int level;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private CircleCollider2D trigger;
    private bool active;
    public bool Dropping { get; private set; }

    public override void MakePrefab(string name)
    {
        tag = "Drop Object";
        ActiveObject(false);
    }

    public void SetLevel(int level, Sprite sprite, float size)
    {
        this.level = level;
        spriteRenderer.sprite = sprite;
        transform.localScale = Vector3.one * size;
        ActiveObject(false);

        Naming();
    }

    public void ActiveObject(bool b)
    {
        _rigidbody.simulated = b;
        Dropping = b;

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
        GameController.Instance.AddScore(level);
        SoundController.Instance.AddSfx("POP");
        level++;
        // ���� �ϼ� �� �ı�
        if (level > 11)
        {
            ActiveObject(false);
            StopAllCoroutines();
            Spawner.Instance.ReturnObject(this);
            return;
        }
        Naming();
        spriteRenderer.sprite = Spawner.Instance.Sprites[level - 1];
        transform.localScale = Vector3.one * Spawner.Instance.ObjectSizes[level - 1];

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
        if (active)
        {
            if (Dropping && (collision.tag == "Drop Object" || collision.tag == "Box"))
            {
                Dropping = false;
                Spawner.Instance.CreateObject();
            }

            if (collision.tag == "Drop Object" && collision.name[0] == 'T')
            {
                if (collision.gameObject.name == gameObject.name)
                {
                    Upgrade();

                    DropObject drop = collision.GetComponent<DropObject>();
                    Spawner.Instance.ReturnObject(drop);
                }
            }
        }
    }
}
