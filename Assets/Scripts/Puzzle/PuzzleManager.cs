using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleManager : MiniGame
{
    [Header("Puzzle")]
    [SerializeField] private Animator[] buttonAnimators;
    [SerializeField] private float gameLength = 120.0f;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;

    [Header("Inputs")]
    [SerializeField] private TextMeshProUGUI inputLeftText;
    [SerializeField] private TextMeshProUGUI inputRightText;
    [SerializeField] private TextMeshProUGUI inputActionText;
    [SerializeField] private TextMeshProUGUI inputOptionsText;

    [Header("Easy mode")]
    [SerializeField] private Image prefabEasyImg;
    [SerializeField] private Transform easyModeRoot;
    [SerializeField] private Sprite[] easyModeSprites;

    [Header("Audio")]
    [SerializeField] private AudioClip[] buttonClips;
    [SerializeField] private AudioClip goodClip;
    [SerializeField] private AudioClip badClip;

    private int currentIdx;
    private List<int> currentSequence;
    private int currentIdxOnSequence;
    private Coroutine routineSequence;
    private int highScore;
    private int currentScore;

    public override void OnStart()
    {
        base.OnStart();
        currentSequence = new List<int>();
        AddToSequence(true);
    }

    private void ClearEasyModeDisplay()
    {
        foreach (Transform child in easyModeRoot) Destroy(child.gameObject);
    }

    private IEnumerator RoutineSequence()
    {
        ClearEasyModeDisplay();
        yield return new WaitForSeconds(1f);

        foreach (int idx in currentSequence)
        {
            Instantiate(prefabEasyImg, easyModeRoot).sprite = easyModeSprites[idx];
            GameManager.instance.PlaySFX(buttonClips[idx]);
            buttonAnimators[idx].SetTrigger("Show");
            yield return new WaitForSeconds(1.5f);
        }

        buttonAnimators[currentIdx].SetBool("Selected", true);
        routineSequence = null;
    }

    public void AddToSequence(bool resetSequence = false)
    {
        if (resetSequence)
        {
            currentSequence.Clear();
        }

        currentIdxOnSequence = 0;

        buttonAnimators[currentIdx].SetBool("Selected", false);
        currentIdx = 0;

        currentSequence.Add(Random.Range(0, buttonAnimators.Length));

        if (routineSequence != null)
        {
            StopCoroutine(routineSequence);
        }

        routineSequence = StartCoroutine(RoutineSequence());
    }

    private void IncrementIndex(int value)
    {
        buttonAnimators[currentIdx].SetBool("Selected", false);
        currentIdx = (currentIdx + value + buttonAnimators.Length) % buttonAnimators.Length;
        buttonAnimators[currentIdx].SetBool("Selected", true);
    }

    public override void UpdateInputs()
    {
        inputLeftText.text = GameManager.instance.GetSettings().inputs.left.ToString();
        inputRightText.text = GameManager.instance.GetSettings().inputs.right.ToString();
        inputActionText.text = GameManager.instance.GetSettings().inputs.action.ToString();
        inputOptionsText.text = GameManager.instance.GetSettings().inputs.pause.ToString();
    }

    private void CheckIdx()
    {
        GameManager.instance.PlaySFX(buttonClips[currentIdx]);
        buttonAnimators[currentIdx].SetTrigger("Use");

        if (currentSequence[currentIdxOnSequence] == currentIdx)
        {

            currentIdxOnSequence++;
            if (currentIdxOnSequence == currentSequence.Count)
            {
                GameManager.instance.PlaySFX(goodClip);
                // Guessed entire sequence

                currentScore++;
                if (currentScore > highScore) highScore = currentScore;

                scoreText.text = "Score : " + currentScore;
                highScoreText.text = "High Score : " + highScore;

                AddToSequence(false);
            }
            else
            {
                // Continue sequence
            }
        }
        else
        {
            GameManager.instance.PlaySFX(badClip);
            // Failed successfully
            AddToSequence(true);
            currentScore = 0;
            scoreText.text = "Score : " + currentScore;
        }
    }

    protected override void OnUpdate()
    {
        easyModeRoot.gameObject.SetActive(GameManager.instance.GetSettings().cognitiveMode);

        if (routineSequence == null)
        {
            int value = GameManager.instance.GetHorizontalDownInput();
            if (value != 0)
            {
                IncrementIndex(value);
            }

            if (GameManager.instance.GetActionDownInput())
            {
                CheckIdx();
            }
        }

        gameLength = Mathf.Clamp(gameLength - Time.deltaTime, 0f, float.MaxValue);
        timerText.text = "Time remaining : " + (int)gameLength + "s";
        if (gameLength <= 0.0f)
        {
            OnEnd();
        }
    }
}
