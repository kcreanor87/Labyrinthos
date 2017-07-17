using UnityEngine;

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
public class CameraGlobalRT : MonoBehaviour
{
    public Camera Camera0;
    public Camera Camera1;
    // Add more cameras or use a list instead

    protected Camera m_Camera;
    protected RenderTexture m_RenderTexture;

    void OnEnable()
    {
        m_Camera = GetComponent<Camera>();
    }

    void Start()
    {
        if (Camera0 == null || Camera1 == null)
            return;

        Camera0.enabled = false;
        Camera1.enabled = false;
    }

    void OnPreRender()
    {
        if (Camera0 == null || Camera1 == null)
            return;

        m_RenderTexture = RenderTexture.GetTemporary(m_Camera.pixelWidth, m_Camera.pixelHeight);

        Camera0.targetTexture = m_RenderTexture;
        Camera0.Render();
        Camera0.targetTexture = null;

        Camera1.targetTexture = m_RenderTexture;
        Camera1.Render();
        Camera1.targetTexture = null;
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (Camera0 == null || Camera1 == null)
            Graphics.Blit(source, destination);
        else
            Graphics.Blit(m_RenderTexture, destination);

        RenderTexture.ReleaseTemporary(m_RenderTexture);
    }
}
