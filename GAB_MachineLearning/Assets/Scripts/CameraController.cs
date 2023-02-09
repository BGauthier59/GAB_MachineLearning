using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    [SerializeField] private Vector3 localPositionToMove = new Vector3(0, 5, -15);
    [SerializeField] private Vector3 localPositionToLook = new Vector3(0, -1, 5);

    [SerializeField] private float moveSpeed = .2f;
    [SerializeField] private float rotateSpeed = .1f;

    private Vector3 wantedPosition;
    private Quaternion wantedRotation;

    void Update()
    {
        wantedPosition = target.TransformPoint(localPositionToMove);
        wantedPosition.y = target.position.y + localPositionToMove.y;
        transform.position = Vector3.Lerp(transform.position, wantedPosition, moveSpeed);

        wantedRotation = Quaternion.LookRotation(target.TransformPoint(localPositionToLook) - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, wantedRotation, rotateSpeed);
    }
}
