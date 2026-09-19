using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public abstract class Entity : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] protected float moveSpeed = 5f;
    [SerializeField] protected float rotateSpeed = 15f;

    public float MoveSpeed => moveSpeed;
    public float RotateSpeed => rotateSpeed;
    public CharacterController Controller { get; private set; }
    public Animator Animator { get; private set; }

    protected virtual void Awake()
    {
        Controller = GetComponent<CharacterController>();
        Animator = GetComponent<Animator>();
    }

    protected virtual void Update()
    {
        if (Controller != null && Controller.enabled)
        {
            Controller.Move(Vector3.down * 9.81f * Time.deltaTime);
        }
    }
}