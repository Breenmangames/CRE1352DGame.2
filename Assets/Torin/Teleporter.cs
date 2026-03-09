using UnityEngine;

public class Teleporter : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform destination;

    public void Interact()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null && destination != null)
            player.transform.position = destination.position;
    }

    public Transform GetDestination()
    {
        return destination;
    }
}