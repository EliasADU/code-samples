using UnityEngine;
using Sirenix.OdinInspector;

public class Radar : MonoBehaviour
{
    [FoldoutGroup("Options")]
    [SerializeField]
    float range;

    [FoldoutGroup("Options")]
    [SerializeField]
    float frequency;

    [FoldoutGroup("Options")]
    [SerializeField]
    float spread;

    [FoldoutGroup("Options")]
    [SerializeField]
    int raysPerPing;

    [FoldoutGroup("Options")]
    [SerializeField]
    float maxRandomDelay;

    [FoldoutGroup("Options")]
    [SerializeField]
    public bool isStrongRadar;

    [FoldoutGroup("Options")]
    [SerializeField]
    float strongIncrementAmount;

    [FoldoutGroup("Requirements")]
    [SerializeField]
    Transform source;

    [FoldoutGroup("Requirements")]
    [SerializeField]
    Transform direction;

    [FoldoutGroup("Requirements")]
    [SerializeField]
    GameObject blip;

    [FoldoutGroup("Requirements")]
    [SerializeField]
    GameObject line;

    [HideInInspector]
    public bool Active {
        get {
            return active;
        }
        set {
            active = value;
            time = 0;
        }
    }

    bool active;
    float time;
    float strongRadarIncrement = 0;

    void Update()
    {
        if (active)
        {
            ConditionallyPing();
        }
    }

    void ConditionallyPing()
    {
        if (time == 0)
        {
            for (int i = 0; i < raysPerPing; i++)
            {
                RadarPing();
            }
        }

        time += Time.deltaTime;
        if (time >= frequency)
        {
            if (isStrongRadar)
                time = 0;
            else
            {
                // 50% chance of delaying by a random amount
                int doDelay = Random.Range(0, 2);
                float randomDelay = 0;
                if (doDelay == 1)
                    randomDelay = Random.Range(0, maxRandomDelay);

                time = -randomDelay;
            }
        }
    }

    void RadarPing()
    {
        Vector2 ray = (direction.position - transform.position).normalized;
        if (isStrongRadar)
        {
            ray = ray.Rotate(strongRadarIncrement);
            strongRadarIncrement += strongIncrementAmount;
            if (strongRadarIncrement == 360)
                strongRadarIncrement = 0;
        }
        else
            ray = ray.RandomRotate(spread).normalized * range;

        RaycastHit2D hit = Physics2D.Raycast(source.position, ray.normalized, range);
        if(hit){
            if (isStrongRadar)
                Instantiate(blip, hit.point, Quaternion.identity);
            else
            {
                GameObject l = Instantiate(line, gameObject.transform.position, Quaternion.identity);
                LineRenderer lr = l.GetComponent<LineRenderer>();

                Instantiate(blip, hit.point, Quaternion.identity);
                lr.SetPositions(new Vector3[] {
                    transform.position,
                    hit.point
                }); ;
            }
        }
    }
}
