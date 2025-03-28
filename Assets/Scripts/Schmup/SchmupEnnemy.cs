using UnityEngine;

public class SchmupEnnemy : MonoBehaviour
{
    [SerializeField] private GameObject sign;
    [SerializeField] private float speed;
    [SerializeField] private Vector3 direction;
    [SerializeField] private bool canFire;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float fireCooldown;
    private float lastFireTime;
    private bool destroyed = false;
    private bool onScreen = false;

    [Header("Audio")]
    [SerializeField] private AudioClip shootClip;

    void Update()
    {
        if (destroyed || GameGUI.instance.pauseOpen) return;

        transform.position += direction * Time.deltaTime * speed;

        if (!onScreen && transform.position.y <= 5.5f)
        {
            onScreen = true;
            lastFireTime = Time.time;
            if (sign) Destroy(sign);
        }

        if (canFire && Time.time - lastFireTime >= fireCooldown && onScreen)
        {
            lastFireTime = Time.time;
            GameManager.instance.Play3DSFX(shootClip, transform.position);
            Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        }

        if (transform.position.y <= -6f)
        {
            destroyed = true;
            Destroy(gameObject);
        }
    }


    void OnCollisionEnter2D(Collision2D collision2D)
    {
        if (collision2D.transform.tag == "Player")
        {
            // Collided with player
            ((SchmupManager)MiniGame.instance).TakeDamage();
            destroyed = true;
            Destroy(gameObject);
        }
    }
}
