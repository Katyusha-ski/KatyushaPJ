using System.Collections.Generic;
using UnityEngine;

public class UsagiShopTrigger : MonoBehaviour
{
    [SerializeField] private SequencePlayer sequencePlayer;
    [SerializeField] private GameObject shopUIActiveButton;

    private readonly HashSet<Collider2D> playerColliders = new();

    private void Awake()
    {
        if (sequencePlayer == null)
            sequencePlayer = GetComponent<SequencePlayer>();

        SetShopButtonActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsPlayer(other)) return;

        playerColliders.Add(other);
        SetShopButtonActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!IsPlayer(other)) return;

        playerColliders.Remove(other);
        if (playerColliders.Count == 0)
            SetShopButtonActive(false);
    }

    private static bool IsPlayer(Collider2D other)
    {
        return other.CompareTag("Player") ||
               other.attachedRigidbody != null && other.attachedRigidbody.CompareTag("Player") ||
               other.transform.root.CompareTag("Player");
    }

    private void SetShopButtonActive(bool isActive)
    {
        if (shopUIActiveButton != null)
            shopUIActiveButton.SetActive(isActive);
    }
}
