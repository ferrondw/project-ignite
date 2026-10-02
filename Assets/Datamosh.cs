using UnityEngine;

[RequireComponent(typeof(Camera))]
public class Datamosh : MonoBehaviour
{
    #region Public properties and methods

    /// Size of compression macroblock.
    public int blockSize = 16;

    /// Entropy coefficient. The larger value makes the stronger noise.
    [Range(0, 1)] public float Entropy = 0.7f;

    /// Contrast of stripe-shaped noise.
    [Range(0.5f, 4)] public float NoiseContrast = 1.4f;

    /// Scale factor for velocity vectors.
    [Range(0, 2)] public float VelocityScale = 0.9f;

    /// Amount of random displacement. 
    [Range(0, 2)] public float Diffusion = 0.8f;

    /// Enables/disables the effect
    [Range(0, 2)] public int Sequence;

    /// Start glitching.
    public void Glitch()
    {
        Sequence = 1;
    }

    /// Stop glitching.
    public void Reset()
    {
        Sequence = 0;
    }

    #endregion

    #region Private properties

    [SerializeField] Shader _shader;

    private Material _material;

    private RenderTexture _workBuffer; // working buffer
    private RenderTexture _dispBuffer; // displacement buffer

    private int _lastFrame;

    private RenderTexture NewWorkBuffer(RenderTexture source)
    {
        return RenderTexture.GetTemporary(source.width, source.height);
    }

    private RenderTexture NewDisplayBuffer(RenderTexture source)
    {
        var rt = RenderTexture.GetTemporary(
            source.width / blockSize,
            source.height / blockSize,
            0, RenderTextureFormat.ARGBHalf
        );
        rt.filterMode = FilterMode.Point;
        return rt;
    }

    private void ReleaseBuffer(RenderTexture buffer)
    {
        if (buffer != null) RenderTexture.ReleaseTemporary(buffer);
    }

    #endregion

    #region MonoBehaviour functions

    private void OnEnable()
    {
        _material = new Material(Shader.Find("Hidden/Kino/Datamosh"))
        {
            hideFlags = HideFlags.DontSave
        };

        GetComponent<Camera>().depthTextureMode |=
            DepthTextureMode.Depth | DepthTextureMode.MotionVectors;

        Sequence = 0;
    }

    private void OnDisable()
    {
        ReleaseBuffer(_workBuffer);
        _workBuffer = null;

        ReleaseBuffer(_dispBuffer);
        _dispBuffer = null;

        DestroyImmediate(_material);
        _material = null;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        _material.SetFloat("_BlockSize", blockSize);
        _material.SetFloat("_Quality", 1 - Entropy);
        _material.SetFloat("_Contrast", NoiseContrast);
        _material.SetFloat("_Velocity", VelocityScale);
        _material.SetFloat("_Diffusion", Diffusion);

        if (Sequence == 0)
        {
            // Step 0: no effect, just keep the last frame.

            // Update the working buffer with the current frame.
            ReleaseBuffer(_workBuffer);
            _workBuffer = NewWorkBuffer(source);
            Graphics.Blit(source, _workBuffer);

            // Blit without effect.
            Graphics.Blit(source, destination);
        }
        else if (Sequence == 1)
        {
            // Step 1: start effect, no moshing.

            // Initialize the displacement buffer.
            ReleaseBuffer(_dispBuffer);
            _dispBuffer = NewDisplayBuffer(source);
            Graphics.Blit(null, _dispBuffer, _material, 0);

            // Simply blit the working buffer because motion vectors
            // might not be ready (because of camera switching).
            Graphics.Blit(_workBuffer, destination);

            Sequence++;
        }
        else
        {
            // Step 2: apply effect.

            if (Time.frameCount != _lastFrame)
            {
                // Update the displaceent buffer.
                var newDisp = NewDisplayBuffer(source);
                Graphics.Blit(_dispBuffer, newDisp, _material, 1);
                ReleaseBuffer(_dispBuffer);
                _dispBuffer = newDisp;

                // Moshing!
                var newWork = NewWorkBuffer(source);
                _material.SetTexture("_WorkTex", _workBuffer);
                _material.SetTexture("_DispTex", _dispBuffer);
                Graphics.Blit(source, newWork, _material, 2);
                ReleaseBuffer(_workBuffer);
                _workBuffer = newWork;

                _lastFrame = Time.frameCount;
            }

            // Blit the result.
            Graphics.Blit(_workBuffer, destination);
        }
    }

    #endregion
}