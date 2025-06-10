using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform target;

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    void LateUpdate()
    {
        if (target != null)
        {
            transform.position = new Vector3(
                target.position.x,
                target.position.y,
                transform.position.z);
        }
    }
}
