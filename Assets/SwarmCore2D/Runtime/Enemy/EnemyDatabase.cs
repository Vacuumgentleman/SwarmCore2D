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

    public int IndexOf(EnemyData data)
    {
        if (data == null || enemies == null) return 0;
        for (int i = 0; i < enemies.Length; i++)
            if (enemies[i] == data) return i;
        return 0;
    }
}