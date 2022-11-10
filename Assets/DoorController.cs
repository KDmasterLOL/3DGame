using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Animator))]
public class DoorController : MonoBehaviour
{
    private Animator _animator;


    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Use()
    {
        _animator.SetTrigger("Use");
    }

    private void OnTriggerEnter(Collider collider)
    {
        collider.CompareTag(Tags.Player);
        Player.Use += Use;
    }
    private void OnTriggerExit(Collider collider)
    {
        collider.CompareTag(Tags.Player);
        Player.Use -= Use;
    }
}
