using UnityEngine;

public class PlatformerDoor : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Player") ((PlatformerManager)MiniGame.instance).TryExit();
    }
}
