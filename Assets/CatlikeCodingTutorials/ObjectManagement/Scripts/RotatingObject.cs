using UnityEngine;

public class RotatingObject : PersistentObject {
    [SerializeField] private Vector3 angularVelocity;

    private void FixedUpdate() {
        transform.Rotate(angularVelocity * Time.deltaTime);
    }
}