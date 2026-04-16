using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerInventoryUI : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("RectTransform del panel de inventario en la esquina")]
    public RectTransform container;

    [Header("Slot Config")]
    [Tooltip("Prefab del slot: debe tener un Image como componente raiz o hijo directo para el icono")]
    public GameObject slotPrefab;
    public float slotSize = 40f;
    public float slotSpacing = 4f;

    PlayerWeaponController weaponController;
    List<GameObject> activeSlots = new List<GameObject>();

    void Start()
    {
        weaponController = FindFirstObjectByType<PlayerWeaponController>();

        if (weaponController == null)
        {
            Debug.LogError("PlayerInventoryUI: PlayerWeaponController no encontrado");
            return;
        }

        weaponController.OnWeaponsChanged += Refresh;

        Refresh();
    }

    void OnDestroy()
    {
        if (weaponController != null)
            weaponController.OnWeaponsChanged -= Refresh;
    }

    public void Refresh()
    {
        ClearSlots();

        if (weaponController == null || container == null || slotPrefab == null)
            return;

        int count = weaponController.weapons.Count;

        if (count == 0)
            return;

        float cellSize = slotSize + slotSpacing;
        float containerWidth = container.rect.width;

        int columns = Mathf.Max(1, Mathf.FloorToInt((containerWidth + slotSpacing) / cellSize));

        for (int i = 0; i < count; i++)
        {
            var weapon = weaponController.weapons[i];

            if (weapon == null)
                continue;

            int col = i % columns;
            int row = i / columns;

            float xPos = col * cellSize + slotSize * 0.5f;
            float yPos = -(row * cellSize + slotSize * 0.5f);

            GameObject slot = Instantiate(slotPrefab, container);
            RectTransform slotRect = slot.GetComponent<RectTransform>();

            slotRect.anchorMin = new Vector2(0f, 1f);
            slotRect.anchorMax = new Vector2(0f, 1f);
            slotRect.pivot = new Vector2(0.5f, 0.5f);
            slotRect.sizeDelta = new Vector2(slotSize, slotSize);
            slotRect.anchoredPosition = new Vector2(xPos, yPos);

            Image icon = slot.GetComponentInChildren<Image>();
            if (icon != null && weapon.weaponIcon != null)
                icon.sprite = weapon.weaponIcon;

            activeSlots.Add(slot);
        }
    }

    void ClearSlots()
    {
        foreach (var slot in activeSlots)
        {
            if (slot != null)
                Destroy(slot);
        }

        activeSlots.Clear();
    }
}