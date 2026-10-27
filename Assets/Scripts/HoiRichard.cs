using Yakanashe.Yautl;
using UnityEngine;

public class HoiRichard : MonoBehaviour
{
    [SerializeField] private float endScale = 40f;
    [SerializeField] private float duration = 60f;

    private void Start()
    {
        transform.ScaleTo(Vector3.one * endScale, duration, EaseType.Linear);
    }
}
