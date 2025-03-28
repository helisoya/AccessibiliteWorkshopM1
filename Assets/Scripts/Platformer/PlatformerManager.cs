using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlatformerManager : MiniGame
{
    [Header("Platformer")]

    [Header("Player")]
    [SerializeField] private Rigidbody2D playerRb;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private float playerSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float groundCheckLength;
    [SerializeField] private SpriteRenderer playerRenderer;
    private float playerInput;

    [Header("Camera")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float cameraSpeed;

    [Header("Score")]
    [SerializeField] private int amountOfCoins;
    [SerializeField] private TextMeshProUGUI coinsText;
    private int coinsCollected = 0;

    [Header("Door")]
    [SerializeField] private SpriteRenderer doorRenderer;
    [SerializeField] private Sprite doorOpenSprite;
    [SerializeField] private GameObject[] doorCoins;

    [Header("Audio")]
    [SerializeField] private AudioClip musicClip;
    [SerializeField] private AudioSource walkSource;
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioClip coinClip;


    [Header("Inputs")]
    [SerializeField] private TextMeshProUGUI inputLeftText;
    [SerializeField] private TextMeshProUGUI inputRightText;
    [SerializeField] private TextMeshProUGUI inputActionText;
    [SerializeField] private TextMeshProUGUI inputOptionsText;

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundCheckLength);
    }

    public override void UpdateInputs()
    {
        inputLeftText.text = GameManager.instance.GetSettings().inputs.left.ToString();
        inputRightText.text = GameManager.instance.GetSettings().inputs.right.ToString();
        inputActionText.text = GameManager.instance.GetSettings().inputs.action.ToString();
        inputOptionsText.text = GameManager.instance.GetSettings().inputs.pause.ToString();
    }

    public override void OnStart()
    {
        base.OnStart();
        GameManager.instance.PlayBGM(musicClip);
        coinsText.text = "Coins : " + coinsCollected + "/" + amountOfCoins;
    }

    public void TakeCoin()
    {
        doorCoins[coinsCollected].SetActive(true);
        coinsCollected++;
        GameManager.instance.PlaySFX(coinClip);
        coinsText.text = "Coins : " + coinsCollected + "/" + amountOfCoins;
        if (coinsCollected == amountOfCoins)
        {
            doorRenderer.sprite = doorOpenSprite;
            coinsText.text = "Find the exit !";
        }
    }

    public void TryExit()
    {
        if (coinsCollected == amountOfCoins) OnEnd();
    }

    protected override void OnUpdate()
    {
        // Player movements
        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector3.down, groundCheckLength, groundMask);

        playerRb.transform.SetParent(hit ? hit.transform : null);

        playerInput = GameManager.instance.GetHorizontalInput() * playerSpeed;

        playerAnimator.SetBool("Moving", playerInput != 0);
        playerAnimator.SetBool("OnGround", hit);

        walkSource.gameObject.SetActive(hit && playerInput != 0);

        if (playerInput != 0) playerRenderer.flipX = playerInput < 0;

        if (hit && GameManager.instance.GetActionDownInput())
        {
            GameManager.instance.PlaySFX(jumpClip);
            playerAnimator.SetTrigger("Jump");
            playerRb.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
        }


        // Camera follow
        Vector3 follow = playerRb.position;
        follow.z = -10f;

        cameraTransform.transform.position = Vector3.MoveTowards(cameraTransform.position, follow, cameraSpeed * Time.deltaTime);
    }

    public void TakeDamage()
    {
        playerAnimator.SetTrigger("Damage");
        playerRb.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
    }

    protected override void OnFixedUpdate()
    {
        playerRb.linearVelocityX = playerInput;
    }
}
