using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class GameGUI : MonoBehaviour
{
    [Header("Rules Menu")]
    [SerializeField] protected GameObject rulesMenuRoot;

    [Header("Game's UI")]
    [SerializeField] protected GameObject gameRoot;

    [Header("Pause Menu")]
    [SerializeField] protected GameObject[] pages;
    [SerializeField] protected GameObject pauseMenuRoot;

    [Space]
    [SerializeField] protected TextMeshProUGUI resolutionText;
    [SerializeField] protected Toggle fullScreenToggle;
    [SerializeField] protected Toggle blackWhiteToggle;
    [SerializeField] protected Toggle highContrastToggle;

    [SerializeField] protected Slider bgmSlider;
    [SerializeField] protected Slider sfxSlider;
    [SerializeField] protected Toggle subtitleSlider;

    [Space]
    [SerializeField] protected Slider timeScaleSlider;
    [SerializeField] protected Toggle cognitiveToggle;
    [SerializeField] protected Toggle schmupDefilToggle;

    [Space]
    [SerializeField] private GameObject rebindUI;
    [SerializeField] protected TextMeshProUGUI leftButton;
    [SerializeField] protected TextMeshProUGUI rightButton;
    [SerializeField] protected TextMeshProUGUI upButton;
    [SerializeField] protected TextMeshProUGUI downButton;
    [SerializeField] protected TextMeshProUGUI actionButton;
    [SerializeField] protected TextMeshProUGUI cancelButton;
    [SerializeField] protected TextMeshProUGUI pauseButton;

    [Header("Event System")]
    [SerializeField] protected GameObject onOpenObj;
    [SerializeField] protected GameObject onRulesObj;
    [SerializeField] protected Selectable[] onDownObjs;
    [SerializeField] protected Button[] buttons;
    [SerializeField] private AccessEventInputSystem eventInputSystem;

    [Header("Components")]
    [SerializeField] protected AudioMixer audioMixer;

    [Header("Sounds")]
    [SerializeField] private AudioClip actionClip;
    [SerializeField] private AudioClip startClip;

    protected Coroutine routineRebind;
    protected int currentPage;
    protected Resolution[] resolutions;
    protected int currentResIdx;
    protected bool selectingResolution;
    protected bool minigameStarted;
    public bool pauseOpen { get { return pauseMenuRoot.activeInHierarchy || rulesMenuRoot.activeInHierarchy; } }
    public static GameGUI instance;

    void Awake()
    {
        instance = this;
        currentPage = 0;
        selectingResolution = false;
        minigameStarted = false;
    }

    void Start()
    {
        gameRoot.SetActive(false);
        pauseMenuRoot.SetActive(false);
        OpenRulesMenu();
        ChangePage(0, false);
        UpdatePauseValues();
    }

    public void OpenRulesMenu()
    {
        rulesMenuRoot.SetActive(true);
        Time.timeScale = 0;
        EventSystem.current.SetSelectedGameObject(onRulesObj);
    }

    public void Event_StartGame()
    {
        GameManager.instance.PlaySFX(startClip);
        gameRoot.SetActive(true);
        rulesMenuRoot.SetActive(false);
        minigameStarted = true;

        MiniGame.instance.UpdateInputs();
        Time.timeScale = GameManager.instance.GetSettings().timeScale;
        EventSystem.current.SetSelectedGameObject(null);

        MiniGame.instance.OnStart();
    }

    public void Event_ChangePage(int idx)
    {
        ChangePage(idx, true);
    }

    private void ChangePage(int idx, bool playSFX = true)
    {
        if (playSFX) GameManager.instance.PlaySFX(actionClip);
        pages[currentPage].SetActive(false);
        currentPage = idx;
        pages[currentPage].SetActive(true);

        foreach (Button button in buttons)
        {
            Navigation navigation = button.navigation;
            navigation.selectOnDown = onDownObjs[idx];
            button.navigation = navigation;
        }
    }

    void UpdatePauseValues()
    {
        leftButton.text = GameManager.instance.GetSettings().inputs.left.ToString();
        rightButton.text = GameManager.instance.GetSettings().inputs.right.ToString();
        upButton.text = GameManager.instance.GetSettings().inputs.up.ToString();
        downButton.text = GameManager.instance.GetSettings().inputs.down.ToString();
        cancelButton.text = GameManager.instance.GetSettings().inputs.cancel.ToString();
        actionButton.text = GameManager.instance.GetSettings().inputs.action.ToString();
        pauseButton.text = GameManager.instance.GetSettings().inputs.pause.ToString();


        sfxSlider.SetValueWithoutNotify(GameManager.instance.GetSettings().sliderSFX);
        bgmSlider.SetValueWithoutNotify(GameManager.instance.GetSettings().sliderBGM);
        timeScaleSlider.SetValueWithoutNotify(GameManager.instance.GetSettings().timeScale);

        fullScreenToggle.SetIsOnWithoutNotify(GameManager.instance.GetSettings().fullscreen);
        subtitleSlider.SetIsOnWithoutNotify(GameManager.instance.GetSettings().subtitles);
        cognitiveToggle.SetIsOnWithoutNotify(GameManager.instance.GetSettings().cognitiveMode);
        schmupDefilToggle.SetIsOnWithoutNotify(!GameManager.instance.GetSettings().schmupNoDefil);
        blackWhiteToggle.SetIsOnWithoutNotify(GameManager.instance.GetSettings().blackWhite);
        highContrastToggle.SetIsOnWithoutNotify(GameManager.instance.GetSettings().highContrast);

        currentResIdx = 0;
        Resolution currentRes = Screen.currentResolution;
        resolutions = Screen.resolutions;
        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].width == currentRes.width && currentRes.height == resolutions[i].height)
            {
                currentResIdx = i;
                break;
            }
        }

        resolutionText.text = resolutions[currentResIdx].width + "x" + resolutions[currentResIdx].height;
    }

    public void OpenPause()
    {
        GameManager.instance.PlaySFX(actionClip);
        gameRoot.SetActive(false);
        rulesMenuRoot.SetActive(false);
        pauseMenuRoot.SetActive(true);
        Time.timeScale = 0;
        EventSystem.current.SetSelectedGameObject(onOpenObj);
        ChangePage(0, false);
    }

    public void ClosePause()
    {
        GameManager.instance.PlaySFX(actionClip);
        pauseMenuRoot.SetActive(false);
        if (!minigameStarted)
        {
            OpenRulesMenu();
        }
        else
        {
            gameRoot.SetActive(true);
            MiniGame.instance.UpdateInputs();
            Time.timeScale = GameManager.instance.GetSettings().timeScale;
        }
    }

    public void Event_EditFullScreen(bool fullscreen)
    {
        GameManager.instance.PlaySFX(actionClip);
        Screen.fullScreen = fullscreen;
        GameManager.instance.GetSettings().fullscreen = fullscreen;
    }

    public void Event_EditSubtitles(bool subtitles)
    {
        GameManager.instance.PlaySFX(actionClip);
        GameManager.instance.GetSettings().subtitles = subtitles;
    }

    public void Event_EditCognitive(bool cognitive)
    {
        GameManager.instance.PlaySFX(actionClip);
        GameManager.instance.GetSettings().cognitiveMode = cognitive;
    }

    public void Event_EditSchmupDefil(bool defil)
    {
        GameManager.instance.PlaySFX(actionClip);
        GameManager.instance.GetSettings().schmupNoDefil = !defil;
    }

    public void Event_EditBlackWhite(bool blackWhite)
    {
        GameManager.instance.PlaySFX(actionClip);
        GameManager.instance.GetSettings().blackWhite = blackWhite;
        GameManager.instance.UpdateVolume();
    }

    public void Event_EditContrast(bool contrast)
    {
        GameManager.instance.PlaySFX(actionClip);
        GameManager.instance.GetSettings().highContrast = contrast;
        GameManager.instance.UpdateVolume();
    }

    public void Event_EnterResolution()
    {
        selectingResolution = true;
    }

    public void Event_ExitResolution()
    {
        selectingResolution = false;
    }

    public void Event_EditTimeScale(float newScale)
    {
        GameManager.instance.PlaySFX(actionClip);
        GameManager.instance.GetSettings().timeScale = newScale;
    }

    public void Event_EditSFXVolume(float newVolume)
    {
        GameManager.instance.PlaySFX(actionClip);
        audioMixer.SetFloat("SFX", newVolume);
        GameManager.instance.GetSettings().sliderSFX = newVolume;
    }

    public void Event_EditBGMVolume(float newVolume)
    {
        GameManager.instance.PlaySFX(actionClip);
        audioMixer.SetFloat("BGM", newVolume);
        GameManager.instance.GetSettings().sliderBGM = newVolume;
    }

    public void Event_EditLeftKey()
    {
        if (routineRebind != null) return;
        routineRebind = StartCoroutine(RoutineRebind(0, leftButton));
    }

    public void Event_EditRightKey()
    {
        if (routineRebind != null) return;
        routineRebind = StartCoroutine(RoutineRebind(1, rightButton));
    }

    public void Event_EditUpKey()
    {
        if (routineRebind != null) return;
        routineRebind = StartCoroutine(RoutineRebind(2, upButton));
    }

    public void Event_EditDownKey()
    {
        if (routineRebind != null) return;
        routineRebind = StartCoroutine(RoutineRebind(3, downButton));
    }

    public void Event_EditCancelKey()
    {
        if (routineRebind != null) return;
        routineRebind = StartCoroutine(RoutineRebind(5, cancelButton));
    }

    public void Event_EditActionKey()
    {
        if (routineRebind != null) return;
        routineRebind = StartCoroutine(RoutineRebind(4, actionButton));
    }

    public void Event_EditPauseKey()
    {
        if (routineRebind != null) return;
        routineRebind = StartCoroutine(RoutineRebind(6, pauseButton));
    }

    public void Event_RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Event_SkipLevel()
    {
        MiniGame.instance.OnEnd();
    }

    protected IEnumerator RoutineRebind(int idxKey, TextMeshProUGUI buttonText)
    {
        bool check = true;
        KeyCode selectedKey = KeyCode.Backspace;

        GameManager.instance.PlaySFX(actionClip);
        rebindUI.SetActive(true);
        //eventInputSystem.DeactivateModule();

        yield return new WaitForSecondsRealtime(0.5f);

        while (check)
        {
            foreach (KeyCode keyCode in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(keyCode))
                {
                    selectedKey = keyCode;
                    check = false;
                    break;
                }
            }
            yield return null;
        }

        if (idxKey == 0) GameManager.instance.GetSettings().inputs.left = selectedKey;
        else if (idxKey == 1) GameManager.instance.GetSettings().inputs.right = selectedKey;
        else if (idxKey == 2) GameManager.instance.GetSettings().inputs.up = selectedKey;
        else if (idxKey == 3) GameManager.instance.GetSettings().inputs.down = selectedKey;
        else if (idxKey == 4) GameManager.instance.GetSettings().inputs.action = selectedKey;
        else if (idxKey == 5) GameManager.instance.GetSettings().inputs.cancel = selectedKey;
        else if (idxKey == 6) GameManager.instance.GetSettings().inputs.pause = selectedKey;

        buttonText.text = selectedKey.ToString();

        GameManager.instance.PlaySFX(actionClip);
        rebindUI.SetActive(false);
        //eventInputSystem.ActivateModule();

        routineRebind = null;
    }

    public void IncrementResolution(int amount)
    {
        GameManager.instance.PlaySFX(actionClip);
        currentResIdx = (currentResIdx + amount + resolutions.Length) % resolutions.Length;
        Screen.SetResolution(resolutions[currentResIdx].width, resolutions[currentResIdx].height, GameManager.instance.GetSettings().fullscreen);
        resolutionText.text = resolutions[currentResIdx].width + "x" + resolutions[currentResIdx].height;
    }

    void Update()
    {
        if (routineRebind != null) return;

        if (GameManager.instance.GetPauseDownInput())
        {
            if (pauseOpen) ClosePause();
            else OpenPause();
        }

        if (GameManager.instance.GetCancelDownInput() && pauseMenuRoot.activeInHierarchy)
        {
            ClosePause();
        }

        if (pauseOpen && selectingResolution)
        {
            int value = GameManager.instance.GetHorizontalDownInput();
            if (value != 0)
            {
                IncrementResolution(value);
            }
        }
    }
}
