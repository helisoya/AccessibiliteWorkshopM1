using UnityEngine;
using UnityEngine.SceneManagement;

public class MiniGame : MonoBehaviour
{
    public static MiniGame instance;
    [SerializeField] protected string nextScene;
    protected bool inGame = false;

    protected void Awake()
    {
        instance = this;
        inGame = false;
    }

    public virtual void OnStart()
    {
        inGame = true;
    }

    public virtual void OnEnd()
    {
        inGame = false;
        GameManager.instance.PlayBGM(null);
        SceneManager.LoadScene(nextScene);
    }

    protected virtual void OnUpdate()
    {

    }

    protected virtual void OnFixedUpdate()
    {

    }

    public virtual void UpdateInputs()
    {

    }

    protected void Update()
    {
        if (!GameGUI.instance.pauseOpen && inGame)
        {
            OnUpdate();
        }
    }

    protected void FixedUpdate()
    {
        if (!GameGUI.instance.pauseOpen && inGame)
        {
            OnFixedUpdate();
        }
    }
}
