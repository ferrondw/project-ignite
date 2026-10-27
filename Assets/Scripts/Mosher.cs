using UnityEngine;
using Yakanashe.Yautl;

public class Mosher : MonoBehaviour
{
    [SerializeField] private Datamosh mosh;
    
    private float _baseEntropy;

    private void Start()
    {
        _baseEntropy = mosh.Entropy;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Mosh()
    {
        mosh.Sequence = 1;
        EntropyTo(mosh, -0.5f, 2f).OnComplete(() =>
        {
            mosh.Sequence = 0;
            mosh.Entropy = _baseEntropy;
        });
    }
    
    public static ITween EntropyTo(Datamosh mosh, float to, float duration, EaseType ease = EaseType.InOutSine)
    {
        var tween = new Tween<float>(mosh.transform, () => mosh.Entropy, v => mosh.Entropy = v, to, duration, ease, Mathf.Lerp);
        TweenRunner.Instance.Run(tween);
        return tween;
    }
}
