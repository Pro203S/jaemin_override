using UnityEngine;

public class MainScript : MonoBehaviour
{
    public static double BEAT_SECOND = 60d / 204d;

    [SerializeField] private MVClock mvClock;

    public static MVClock clock { get; private set; }

    void Awake()
    {
        clock = mvClock;
    }

    void Start()
    {
        mvClock.Play();
    }
}
