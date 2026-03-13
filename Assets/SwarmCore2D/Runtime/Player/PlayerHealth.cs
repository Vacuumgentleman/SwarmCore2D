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

    [Header("Death")]
    public ParticleSystem deathParticles;

    float invulTimer;
    Color originalColor;

    void Start()
    {
        currentHealth = maxHealth;

        if (sprite != null)
            originalColor = sprite.color;
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

        if (currentHealth <= 0f)
            Die();
    }

    void Die()
    {
        if (deathParticles != null)
            Instantiate(deathParticles, transform.position, Quaternion.identity);

        gameObject.SetActive(false);
    }
}