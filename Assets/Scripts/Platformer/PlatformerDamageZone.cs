using Unity.Mathematics.Geometry;
using UnityEngine;

public class PlatformerDamageZone : MonoBehaviour
{
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            ((PlatformerManager)MiniGame.instance).TakeDamage();
        }
    }
}
