using UnityEngine;

public interface IInteractable
{
    void Interact(Transform whoActivatedMe);

    string GetDisplayName();
}
