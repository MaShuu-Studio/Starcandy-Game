using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverLine : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Drop Object")
        {
            // Trigger�� ������ ������ �� ���ʿ� �ִٸ� ���ӿ�����. 
            if (Spawner.Instance.CheckGameOver(collision.attachedRigidbody.position.y - collision.transform.localScale.y))
            {
                GameController.Instance.GameOver();
            }
        }
    }
}
