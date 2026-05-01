using UnityEngine;

public class MinimapFollow : MonoBehaviour
{
    public Transform target;
    public float height = 30f;

    void LateUpdate()
    {
        if (target == null) return;

        transform.position = new Vector3(
            target.position.x,
            target.position.y + height,
            target.position.z
        );
    }
}