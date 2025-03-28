using UnityEngine;

public class PlatformerCoin : MonoBehaviour
{
    private bool destroyed = false;

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (!destroyed & collider.transform.tag == "Player")
        {
            destroyed = true;
            ((PlatformerManager)MiniGame.instance).TakeCoin();
            Destroy(gameObject);
        }
    }
}
