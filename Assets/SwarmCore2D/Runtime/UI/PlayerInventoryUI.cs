using UnityEngine;
using System.Collections.Generic;

public class PlayerInventoryUI : MonoBehaviour
{
    [Header("Referencias")]
    public RectTransform container;
    public GameObject slotPrefab;

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

        foreach (var weapon in weaponController.weapons)
        {
            if (weapon == null) continue;

            GameObject slot = Instantiate(slotPrefab, container);

            SpriteRenderer icon = slot.GetComponentInChildren<SpriteRenderer>();
            if (icon != null && weapon.weaponIcon != null)
                icon.sprite = weapon.weaponIcon;

            activeSlots.Add(slot);
        }
    }

    void ClearSlots()
    {
        foreach (var slot in activeSlots)
            if (slot != null) Destroy(slot);

        activeSlots.Clear();
    }
}
