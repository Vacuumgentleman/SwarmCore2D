using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 pos = target.position;
        pos.z = -10;

        transform.position = pos;
    }
}