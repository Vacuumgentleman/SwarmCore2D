using UnityEngine;

public class EnemyDatabase : MonoBehaviour
{
    public EnemyData[] enemies;

    public static EnemyDatabase Instance;

    void Awake()
    {
        Instance = this;
    }

    public EnemyData Get(int id)
    {
        if (id < 0 || id >= enemies.Length)
            return null;

        return enemies[id];
    }
}