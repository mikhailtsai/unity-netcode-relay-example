using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float smoothSpeed = 3f;

    [HideInInspector]
    public Transform target;

    private Vector3 offset = new Vector3(0, 5f, -10f);

    void LateUpdate()
    {
        if (target == null) return;

        transform.position = target.position + offset;
    }
}
