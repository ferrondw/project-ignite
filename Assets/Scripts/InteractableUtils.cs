using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yakanashe.Yautl;

public class InteractableUtils : MonoBehaviour
{
    public void TweenDown(Transform target)
    {
        target.MoveTo(target.position + new Vector3(0, -5, 0), 3, EaseType.Linear);
    }

    public void Spin(Transform target)
    {
        target.RotateTo(Quaternion.Euler(-90, 0, 360),2f, EaseType.Linear).OnComplete(() =>
        {
            target.RotateTo(Quaternion.Euler(-90, 0, 180), 2f, EaseType.Linear);
        });
    }

    public void FlipRotation(Transform target)
    {
        target.Rotate(target.up, 180);
    }
}
