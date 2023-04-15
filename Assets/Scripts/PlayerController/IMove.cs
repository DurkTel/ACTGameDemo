using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMove
{
    Transform rootTransform { get; set; }
    float deltaTtime { get; set; }

    float gravity { get; set; }

    MoveType moveType { get; set; }

    void Move();

    void Move(Vector3 direction, float speed);

    void Move(Vector3 target, float time, float delay);

    void MoveCompensation(Vector3 direction = default);

    void Rotate();

    void Rotate(Vector3 direction, float speed);

    void Rotate(Quaternion target, float time, float delay);

    void RotateCompensation();

    void Stop(bool value);

    void EnableGravity(bool value);

    float GetGravityAcceleration();

    bool IsGrounded();

    bool IsFalled();

    Vector2 GetRelativeMove(Vector3 move);

    void SetGravityAccelerationByHeight(float height);

}
