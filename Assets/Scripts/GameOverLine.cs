using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverLine : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Drop Object")
        {
            // Trigger가 밖으로 나갔을 때 위쪽에 있다면 게임오버임. 
            if (Spawner.Instance.CheckGameOver(collision.attachedRigidbody.position.y - collision.transform.localScale.y))
            {
                GameController.Instance.GameOver();
            }
        }
    }
}
