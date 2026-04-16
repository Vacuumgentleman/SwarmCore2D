using UnityEngine;

public class AttackVisual : MonoBehaviour
{
    SpriteRenderer sr;

    Sprite[] frames;
    float frameRate;
    bool loop;

    int frame;
    float timer;

    public void Init(Sprite[] frames, float frameRate, bool loop = false)
    {
        this.frames = frames;
        this.frameRate = frameRate;
        this.loop = loop;

        sr = GetComponent<SpriteRenderer>();

        frame = 0;
        timer = 0;

        if (frames != null && frames.Length > 0)
            sr.sprite = frames[0];
    }

    void Update()
    {
        if (frames == null || frames.Length == 0)
            return;

        timer += Time.deltaTime;

        if (timer >= 1f / frameRate)
        {
            timer = 0;
            frame++;

            if (frame >= frames.Length)
            {
                if (loop)
                {
                    frame = 0;
                }
                else
                {
                    Destroy(gameObject);
                    return;
                }
            }

            sr.sprite = frames[frame];
        }
    }
}
