using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private CarController _carController;

    private void Update()
    {
        _carController.horizontalInput = Input.GetAxis("Horizontal");
        _carController.verticalInput = Input.GetAxis("Vertical");
    }
}
