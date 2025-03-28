using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class Settings
{
    [System.Serializable]
    public class GameInputs
    {
        public KeyCode left = KeyCode.LeftArrow;
        public KeyCode right = KeyCode.RightArrow;
        public KeyCode up = KeyCode.UpArrow;
        public KeyCode down = KeyCode.DownArrow;
        public KeyCode action = KeyCode.Space;
        public KeyCode cancel = KeyCode.Z;
        public KeyCode pause = KeyCode.Escape;
    }

    public GameInputs inputs;
    public int width;
    public int height;
    public bool fullscreen;
    public float timeScale;

    public float sliderBGM;
    public float sliderSFX;
    public bool subtitles;
    public bool cognitiveMode;
    public bool schmupNoDefil;
    public bool highContrast;
    public bool blackWhite;

    public Settings()
    {
        inputs = new GameInputs();
        fullscreen = true;
        sliderBGM = 0;
        sliderSFX = 0;
        timeScale = 1f;
        subtitles = true;
        cognitiveMode = false;
        schmupNoDefil = false;
        blackWhite = false;
        highContrast = false;
    }
}
