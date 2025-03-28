using UnityEngine;

public class PlatformerMovingPlatform : MonoBehaviour
{
    [SerializeField] private Transform platform;
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private bool circle;
    [SerializeField] private float speed;
    private int side;
    private int currentIdx;


    void Start()
    {
        side = 1;
        currentIdx = 0;
        platform.position = waypoints[currentIdx].position;
        NextTarget();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        if (waypoints.Length < 2) return;

        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
        }

        if (circle)
        {
            Gizmos.DrawLine(waypoints[0].position, waypoints[waypoints.Length - 1].position);
        }
    }

    void NextTarget()
    {
        if (currentIdx == waypoints.Length - 1)
        {
            if (circle) currentIdx = 0;
            else
            {
                side = -1;
                currentIdx--;
            }
        }
        else if (side == -1 && !circle && currentIdx == 0)
        {
            currentIdx++;
            side = 1;
        }
        else
        {
            currentIdx += side;
        }
    }


    void Update()
    {
        if (platform.position == waypoints[currentIdx].position)
        {
            NextTarget();
        }
        else
        {
            platform.position = Vector3.MoveTowards(platform.position, waypoints[currentIdx].position, speed * Time.deltaTime);
        }
    }
}
