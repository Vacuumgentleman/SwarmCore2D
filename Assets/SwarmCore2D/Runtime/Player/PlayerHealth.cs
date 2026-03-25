using UnityEngine;
using System;

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

    public event Action OnHealthChanged;

    float invulTimer;
    Color originalColor;

    void Start()
    {
        currentHealth = maxHealth;

        if (screenFade == null)
            screenFade = FindFirstObjectByType<ScreenFadeToMenu>();

        if (sprite != null)
            originalColor = sprite.color;

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        OnHealthChanged?.Invoke();
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

        currentHealth = Mathf.Max(currentHealth, 0f);

        invulTimer = invulnerabilityTime;

        PlayDamageSound();

        OnHealthChanged?.Invoke();

        if (currentHealth <= 0f)
            Die();
    }

    public float GetHealthPercent()
    {
        return currentHealth / maxHealth;
    }

    void PlayDamageSound()
    {
        if (audioSource == null || damageSound == null)
            return;

        audioSource.PlayOneShot(damageSound);
    }

    void Die()
    {

        if (deathParticles != null)
            Instantiate(deathParticles, transform.position, Quaternion.identity);

        if (screenFade != null)
        {
            screenFade.FadeToMenu();
        }

        gameObject.SetActive(false);
    }
}