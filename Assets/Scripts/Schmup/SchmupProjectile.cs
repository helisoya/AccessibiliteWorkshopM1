using UnityEngine;

public class SchmupProjectile : MonoBehaviour
{
    [SerializeField] private bool isPlayer;
    [SerializeField] private Vector3 direction;
    [SerializeField] private float speed;
    private bool destroyed = false;

    [Header("Audio")]
    [SerializeField] private AudioClip hitEnnemyClip;

    void Update()
    {
        if (destroyed) return;

        transform.position += direction * Time.deltaTime * speed;
        if (transform.position.y >= 5.5f || transform.position.y <= -5.5f)
        {
            destroyed = true;
            Destroy(gameObject);
        }

    }


    void OnTriggerEnter2D(Collider2D collider)
    {
        bool destroyAtEnd = false;
        if (collider.tag == "Player" && !isPlayer)
        {
            // Shot player
            destroyAtEnd = true;
            ((SchmupManager)MiniGame.instance).TakeDamage();
        }
        else if (collider.tag == "Ennemy" && isPlayer)
        {
            // Shot Ennemy
            GameManager.instance.Play3DSFX(hitEnnemyClip, transform.position);
            destroyAtEnd = true;
            Destroy(collider.attachedRigidbody.gameObject);
        }

        if (destroyAtEnd)
        {
            destroyed = true;
            Destroy(gameObject);
        }
    }
}
