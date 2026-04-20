using UnityEngine;
using UnityEngine.InputSystem;
using SwarmCore2D.Core;
using SwarmCore2D.Drops;

public class ChestController : MonoBehaviour
{
    public ChestData data;

    Cainos.PixelArtPlatformer_VillageProps.Chest chest;
    Transform playerTransform;
    GameObject interactPrompt;
    bool opened;

    void Awake()
    {
        chest = GetComponent<Cainos.PixelArtPlatformer_VillageProps.Chest>();
        interactPrompt = transform.Find("InteractPrompt")?.gameObject;
        if (interactPrompt != null) interactPrompt.SetActive(false);
    }

    void Start()
    {
        var player = FindFirstObjectByType<PlayerController>();
        if (player != null) playerTransform = player.transform;
    }

    void Update()
    {
        if (opened || SwarmTime.Paused || chest == null || playerTransform == null || data == null)
        {
            if (interactPrompt != null) interactPrompt.SetActive(false);
            return;
        }

        float dist = Vector2.Distance(transform.position, playerTransform.position);
        bool inRange = dist <= data.interactRadius;

        if (interactPrompt != null) interactPrompt.SetActive(inRange);

        if (inRange && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            OpenChest();
    }

    void OpenChest()
    {
        opened = true;
        chest.Open();
        if (interactPrompt != null) interactPrompt.SetActive(false);

        if (DropSystem.Instance != null && data.lootTable != null)
        {
            uint seed = (uint)(SwarmTime.Tick ^ (uint)(GetInstanceID() * 2654435761u));
            DropSystem.Instance.SpawnFromTable(transform.position, data.lootTable, seed);
        }
    }
}
