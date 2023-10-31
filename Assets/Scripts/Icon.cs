using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class Icon : MonoBehaviour
{
    [SerializeField] private Image image;
    private int index;

    public void SetIcon(Sprite sprite, int index)
    {
        image.sprite = sprite;
        this.index = index;
        GetComponent<Button>().onClick.AddListener(() => SpriteManager.Instance.SetSpriteIndex(this.index));
        gameObject.SetActive(true);
    }
}
