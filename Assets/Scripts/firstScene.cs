using UnityEngine;

public class firstScene : MonoBehaviour
{
    [SerializeField] private SpriteRenderer popcornLeft;
    [SerializeField] private SpriteRenderer popcornRight;
    [SerializeField] private SpriteRenderer cokeLeft;
    [SerializeField] private SpriteRenderer cokeRight;
    [SerializeField] private SpriteRenderer cgvLogo;
    [SerializeField] private SpriteRenderer textSal;
    [SerializeField] private SpriteRenderer textMok;
    [SerializeField] private SpriteRenderer textJi;
    [SerializeField] private SpriteRenderer jaeminHead;

    void Start()
    {
        // MainScript.clock.SetOffset(0d);

        MainScript.clock.Schedule(MainScript.BEAT_SECOND * 1, () => cokeLeft.enabled = true);
        MainScript.clock.Schedule(MainScript.BEAT_SECOND * 2, () => cokeRight.enabled = true);
        MainScript.clock.Schedule(MainScript.BEAT_SECOND * 3, () => popcornLeft.enabled = true);
        MainScript.clock.Schedule(MainScript.BEAT_SECOND * 4, () => popcornRight.enabled = true);

        MainScript.clock.Schedule(MainScript.BEAT_SECOND * 5, () => cgvLogo.enabled = textSal.enabled = true);

        MainScript.clock.Schedule(MainScript.BEAT_SECOND * 6, () => { textSal.enabled = false; textMok.enabled = true; });
        MainScript.clock.Schedule(MainScript.BEAT_SECOND * 7, () => { textMok.enabled = false; textJi.enabled = true; });
        MainScript.clock.Schedule(MainScript.BEAT_SECOND * 8, () => { textJi.enabled = false; jaeminHead.enabled = true; });
    }
}
