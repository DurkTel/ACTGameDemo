using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMove
{
    Transform rootTransform { get; set; }
    void Move();

    void Move(Vector3 direction, float speed);

    void Move(Vector3 target, float time, float delay = 0f);

    void Rotate();

    void Rotate(Vector3 direction, float speed);

    void Rotate(Quaternion target, float time, float delay = 0f);

    void Stop();

    void EnableGravity(bool value);

    float GetGravityAcceleration();

    bool IsGrounded();

    bool IsFalled();

    Vector2 GetRelativeMove(Vector3 move);

    void SetGravityAcceleration(float height);

}