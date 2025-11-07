using Unity.Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    [SerializeField] CinemachineCamera cineCam;

    [Range(40, 100)]
    float lensFOV;

    [SerializeField] float zoomSpeed;

	private void Start()
	{
        lensFOV = cineCam.Lens.FieldOfView;
	}

	void Update()
    {
        if (Input.GetKey(KeyCode.Equals)) UpdateLensFOV(-Time.deltaTime * zoomSpeed);
        if (Input.GetKey(KeyCode.Minus)) UpdateLensFOV(Time.deltaTime * zoomSpeed);
    }

    void UpdateLensFOV(float changeBy)
    {
        lensFOV += changeBy;
        cineCam.Lens.FieldOfView = lensFOV;
	}
}
