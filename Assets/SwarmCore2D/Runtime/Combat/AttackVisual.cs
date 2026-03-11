using UnityEngine;

public class AttackVisual : MonoBehaviour
{
    SpriteRenderer sr;

    Sprite[] frames;
    float frameRate;

    int frame;
    float timer;

    public void Init(Sprite[] frames, float frameRate)
    {
        this.frames = frames;
        this.frameRate = frameRate;

        sr = GetComponent<SpriteRenderer>();

        frame = 0;
        timer = 0;

        if (frames.Length > 0)
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
                Destroy(gameObject);
                return;
            }

            sr.sprite = frames[frame];
        }
    }
}