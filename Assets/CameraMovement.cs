using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Stickman stickman;

    [SerializeField] private Camera cam;
    [SerializeField] private Transform target;
    [SerializeField] private float distanceToTarget = 3;
    //
    public float zoomSpeed = 10f;
    public float minZoomFOV = 10f;
    public float maxZoomFOV = 60f;
    //
    private Vector3 previousPosition;

    private void Update()
    {
        stickman = GameObject.Find("Stickman").GetComponent<Stickman>();
        cam=stickman.camera;
        
        if (Input.GetMouseButtonDown(0))
        {
            previousPosition = cam.ScreenToViewportPoint(Input.mousePosition);
        }
        else if (Input.GetMouseButton(0))
        {
            Vector3 newPosition = cam.ScreenToViewportPoint(Input.mousePosition);
            Vector3 direction = previousPosition - newPosition;

            float rotationAroundYAxis = -direction.x * 180; // camera moves horizontally
            float rotationAroundXAxis = direction.y * 180; // camera moves vertically

            cam.transform.position = target.position;

            cam.transform.Rotate(new Vector3(1, 0, 0), rotationAroundXAxis);
            cam.transform.Rotate(new Vector3(0, 1, 0), rotationAroundYAxis, Space.World);

            cam.transform.Translate(new Vector3(0, 0, -distanceToTarget));

            previousPosition = newPosition;
        }
        float zoomDelta = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        float cursorPosition = Input.mousePosition.y / Screen.height;

        Vector3 zoomPivot = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cam.nearClipPlane));
        // Zoom in
        if (zoomDelta > 0f && cam.fieldOfView > minZoomFOV)
        {
            cam.fieldOfView -= zoomDelta;
            Vector3 newCameraPosition = zoomPivot + (cam.transform.position - zoomPivot) / (1f + zoomDelta);
            cam.transform.position = newCameraPosition;
        }
        // Zoom out
        else if (zoomDelta < 0f && cam.fieldOfView < maxZoomFOV)
        {
            cam.fieldOfView -= zoomDelta;
            Vector3 newCameraPosition = zoomPivot + (cam.transform.position - zoomPivot) * (1f + Mathf.Abs(zoomDelta));
            cam.transform.position = newCameraPosition;
        }
    }
}