using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FloatingTextSpawner : MonoBehaviour
{
    public static FloatingTextSpawner Instance;

    [Header("Settings")]
    public float floatSpeed  = 1.5f;
    public float duration    = 1.2f;
    public float fontSize    = 3f;
    public int   poolSize    = 10;

    class FloatingText
    {
        public GameObject go;
        public TextMeshPro tmp;
        public Color       baseColor;
        public float       timer;
        public bool        active;
    }

    readonly List<FloatingText> pool = new List<FloatingText>();

    void Awake()
    {
        Instance = this;

        for (int i = 0; i < poolSize; i++)
        {
            var go  = new GameObject("FloatingText");
            go.transform.SetParent(transform);
            var tmp = go.AddComponent<TextMeshPro>();
            tmp.alignment    = TextAlignmentOptions.Center;
            tmp.fontSize     = fontSize;
            tmp.sortingOrder = 100;
            go.SetActive(false);
            pool.Add(new FloatingText { go = go, tmp = tmp });
        }
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    void Update()
    {
        foreach (var ft in pool)
        {
            if (!ft.active) continue;

            ft.timer += Time.deltaTime;
            ft.go.transform.position += Vector3.up * floatSpeed * Time.deltaTime;

            float alpha = Mathf.Max(0f, 1f - ft.timer / duration);
            ft.tmp.color = new Color(ft.baseColor.r, ft.baseColor.g, ft.baseColor.b, alpha);

            if (ft.timer >= duration)
            {
                ft.active = false;
                ft.go.SetActive(false);
            }
        }
    }

    public void Spawn(Vector2 position, string text, Color color)
    {
        foreach (var ft in pool)
        {
            if (ft.active) continue;

            ft.go.transform.position = new Vector3(position.x, position.y, -2f);
            ft.tmp.text  = text;
            ft.tmp.color = color;
            ft.baseColor = color;
            ft.timer     = 0f;
            ft.active    = true;
            ft.go.SetActive(true);
            return;
        }
    }
}
