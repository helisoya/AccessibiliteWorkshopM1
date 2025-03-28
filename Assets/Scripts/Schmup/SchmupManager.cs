using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SchmupManager : MiniGame
{
    [Header("Schmup")]

    [Header("Player")]
    [SerializeField] private Rigidbody2D playerRb;
    [SerializeField] private float playerSpeed;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private float fireCooldown = 0.5f;
    [SerializeField] private GameObject bulletPrefab;
    private float lastFireTime;

    [Header("Health")]
    [SerializeField] private int life = 3;
    [SerializeField] private TextMeshProUGUI lifeText;
    [SerializeField] private float invincibilityTime = 1f;
    private float lastDamageTime;

    [Header("Background")]
    [SerializeField] private Renderer backgroundRenderer;
    [SerializeField] private float backgroundScrollSpeed = 1f;
    private float backgroundScrollValue;


    [Header("Inputs")]
    [SerializeField] private TextMeshProUGUI inputLeftText;
    [SerializeField] private TextMeshProUGUI inputRightText;
    [SerializeField] private TextMeshProUGUI inputActionText;
    [SerializeField] private TextMeshProUGUI inputOptionsText;

    [Header("Audio")]
    [SerializeField] private AudioClip musicClip;
    [SerializeField] private AudioClip playerDamageClip;
    [SerializeField] private AudioClip playerShootClip;

    public override void OnStart()
    {
        base.OnStart();
        GameManager.instance.PlayBGM(musicClip);
        lifeText.text = "Lives : " + life;
    }

    public override void UpdateInputs()
    {
        inputLeftText.text = GameManager.instance.GetSettings().inputs.left.ToString();
        inputRightText.text = GameManager.instance.GetSettings().inputs.right.ToString();
        inputActionText.text = GameManager.instance.GetSettings().inputs.action.ToString();
        inputOptionsText.text = GameManager.instance.GetSettings().inputs.pause.ToString();
    }

    public void TakeDamage()
    {
        GameManager.instance.PlaySFX(playerDamageClip);
        if (GameManager.instance.GetSettings().cognitiveMode || Time.time - lastDamageTime < invincibilityTime) return;
        lastDamageTime = Time.time;
        life--;
        lifeText.text = "Lives : " + life;
        playerAnimator.SetTrigger("Damage");
        if (life == 0) SceneManager.LoadScene("Schmup");
    }

    protected override void OnUpdate()
    {
        // Background

        if (!GameManager.instance.GetSettings().schmupNoDefil)
        {
            backgroundScrollValue += Time.deltaTime * backgroundScrollSpeed;
            while (backgroundScrollValue >= 1f) backgroundScrollValue -= 1f;
            backgroundRenderer.material.mainTextureOffset = new Vector2(0, backgroundScrollValue);
        }

        // Player movements

        playerRb.linearVelocity = new Vector2(playerSpeed * GameManager.instance.GetHorizontalInput(), 0);
        playerRb.position = new Vector2(playerRb.position.x, -3.67f);
        playerAnimator.SetFloat("Direction", (GameManager.instance.GetHorizontalInput() + 1f) / 2f);

        if (GameManager.instance.GetActionInput() && Time.time - lastFireTime >= fireCooldown)
        {
            lastFireTime = Time.time;
            GameManager.instance.Play3DSFX(playerShootClip, playerRb.position);
            // Instantiate Bullet
            // Who cares about pools anyway : )
            Instantiate(bulletPrefab, playerRb.position, Quaternion.identity);
        }
    }
}
