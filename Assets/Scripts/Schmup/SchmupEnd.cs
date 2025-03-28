using UnityEngine;

public class SchmupEnd : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Vector3 direction;
    private bool activated = false;

    void Update()
    {
        if (GameGUI.instance.pauseOpen) return;


        transform.position += direction * Time.deltaTime * speed;
    }


    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player" && !activated)
        {
            activated = true;
            MiniGame.instance.OnEnd();
        }
    }
}
