using UnityEngine;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource prefabSFX3D;
    [SerializeField] private Volume volumeBlackWhite;
    [SerializeField] private Volume volumeHighContrast;
    public static GameManager instance;
    private Settings settings;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            settings = new Settings();
            settings.width = Screen.currentResolution.width;
            settings.height = Screen.currentResolution.height;
            settings.fullscreen = Screen.fullScreen;
            DontDestroyOnLoad(gameObject);

        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpdateVolume()
    {
        volumeBlackWhite.weight = settings.blackWhite ? 1f : 0f;
        volumeHighContrast.weight = settings.highContrast ? 1f : 0f;
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void Play3DSFX(AudioClip clip, Vector3 position)
    {
        AudioSource source = Instantiate(prefabSFX3D, position, Quaternion.identity);
        source.clip = clip;
        source.Play();
        Destroy(source.gameObject, clip.length);
    }

    public void PlayBGM(AudioClip clip)
    {
        bgmSource.clip = clip;
        bgmSource.Stop();
        bgmSource.Play();
    }

    public Settings GetSettings()
    {
        return settings;
    }

    public int GetHorizontalDownInput()
    {
        return (Input.GetKeyDown(settings.inputs.right) ? 1 : 0) - (Input.GetKeyDown(settings.inputs.left) ? 1 : 0);
    }

    public int GetVerticalDownInput()
    {
        return (Input.GetKeyDown(settings.inputs.down) ? 1 : 0) - (Input.GetKeyDown(settings.inputs.up) ? 1 : 0);
    }

    public int GetHorizontalInput()
    {
        return (Input.GetKey(settings.inputs.right) ? 1 : 0) - (Input.GetKey(settings.inputs.left) ? 1 : 0);
    }

    public int GetVerticalInput()
    {
        return (Input.GetKey(settings.inputs.down) ? 1 : 0) - (Input.GetKey(settings.inputs.up) ? 1 : 0);
    }

    public bool GetActionDownInput()
    {
        return Input.GetKeyDown(settings.inputs.action);
    }

    public bool GetActionInput()
    {
        return Input.GetKey(settings.inputs.action);
    }

    public bool GetCancelDownInput()
    {
        return Input.GetKeyDown(settings.inputs.cancel);
    }

    public bool GetPauseDownInput()
    {
        return Input.GetKeyDown(settings.inputs.pause);
    }
}
