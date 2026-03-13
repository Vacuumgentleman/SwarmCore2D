using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Invulnerability")]
    public float invulnerabilityTime = 0.5f;

    [Header("Effects")]
    public SpriteRenderer sprite;
    public Color hitColor = Color.red;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip damageSound;

    [Header("Death")]
    public ParticleSystem deathParticles;

    [Header("Scene Transition")]
    public ScreenFadeToMenu screenFade;

    float invulTimer;
    Color originalColor;

    void Start()
    {
        currentHealth = maxHealth;

        if (sprite != null)
            originalColor = sprite.color;

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (invulTimer > 0f)
        {
            invulTimer -= Time.deltaTime;

            if (sprite != null)
                sprite.color = hitColor;
        }
        else
        {
            if (sprite != null)
                sprite.color = originalColor;
        }
    }

    public void Damage(float dmg)
    {
        if (invulTimer > 0f)
            return;

        currentHealth -= dmg;

        invulTimer = invulnerabilityTime;

        PlayDamageSound();

        if (currentHealth <= 0f)
            Die();
    }

    void PlayDamageSound()
    {
        if (audioSource == null)
            return;

        if (damageSound == null)
            return;

        audioSource.PlayOneShot(damageSound);
    }

    void Die()
    {
        if (deathParticles != null)
            Instantiate(deathParticles, transform.position, Quaternion.identity);

        if (screenFade != null)
            screenFade.FadeToMenu();

        gameObject.SetActive(false);
    }
}