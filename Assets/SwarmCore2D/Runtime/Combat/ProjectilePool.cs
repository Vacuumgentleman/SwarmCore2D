using UnityEngine;
using System.Collections.Generic;

public class ProjectilePool : MonoBehaviour
{
    public static ProjectilePool Instance;

    public Projectile projectilePrefab;
    public int initialSize = 32;

    Queue<Projectile> pool = new Queue<Projectile>();

    void Awake()
    {
        Instance = this;

        for (int i = 0; i < initialSize; i++)
            CreateNew();
    }

    Projectile CreateNew()
    {
        Projectile p = Instantiate(projectilePrefab, transform);
        p.gameObject.SetActive(false);
        p.pool = this;

        pool.Enqueue(p);
        return p;
    }

    public Projectile GetProjectile()
    {
        if (pool.Count == 0)
            CreateNew();

        Projectile p = pool.Dequeue();
        p.gameObject.SetActive(true);

        return p;
    }

    public void ReturnProjectile(Projectile p)
    {
        p.gameObject.SetActive(false);
        pool.Enqueue(p);
    }
}