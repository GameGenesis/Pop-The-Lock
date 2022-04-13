using System;
using UnityEngine;

public class LockPin : MonoBehaviour
{
    public event Action<bool> onLockPinCollision;

    public float CurrentVelocity { get; set; }
    public float CurrentAngle { get; set; }

    private void Update()
    {
        transform.RotateAround(Vector3.zero, Vector3.back, CurrentVelocity * Time.deltaTime);
        CurrentAngle = Vector2.Angle(Vector2.up, transform.localPosition);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        onLockPinCollision?.Invoke(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        onLockPinCollision?.Invoke(false);
    }
}
