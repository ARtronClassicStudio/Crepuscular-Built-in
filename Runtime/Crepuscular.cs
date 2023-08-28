using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView,AddComponentMenu("Crepuscular")]
public sealed class Crepuscular : MonoBehaviour
{
    public bool useColorDirectional = true;
    public Color color = Color.white;
    public Quality quality = Quality.High;
    [Range(0, 1)]
    public float density = 1;
    [Range(0,1)]
    public float weight = 1;
    [Range(0,1)]
    public float exposure = 0.5f;
    [Range(0,10)]
    public float illuminationDecay = 1;

    public enum Quality
    {
        Ultra,
        High,
        Medium,
        Low
    }
    Camera render;
    Material m_Material;
    Light[] lightData;
    
    const string kShaderName = "Hidden/Crepuscular";

    private void Awake()
    {
        if (Shader.Find(kShaderName) != null)
        {
            render = Camera.main;
            lightData = FindObjectsOfType<Light>();
            m_Material = new Material(Shader.Find(kShaderName));
        }
        else
            Debug.LogError($"Unable to find shader '{kShaderName}'. Post Process Volume Crepuscular is unable to load.");
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (m_Material == null)
            return;

        if (enabled)
        {

            switch (quality)
            {
                case Quality.Ultra: m_Material.SetFloat(Shader.PropertyToID("_NumSamples"), 1024); break;
                case Quality.High: m_Material.SetFloat(Shader.PropertyToID("_NumSamples"), 300); break;
                case Quality.Medium: m_Material.SetFloat(Shader.PropertyToID("_NumSamples"), 150); break;
                case Quality.Low: m_Material.SetFloat(Shader.PropertyToID("_NumSamples"), 50); break;
            }

            m_Material.SetFloat(Shader.PropertyToID("_Density"), density);
            m_Material.SetFloat(Shader.PropertyToID("_Weight"),weight);
            m_Material.SetFloat(Shader.PropertyToID("_Decay"), 1);
            m_Material.SetFloat(Shader.PropertyToID("_Exposure"), exposure);
            m_Material.SetFloat(Shader.PropertyToID("_IlluminationDecay"), illuminationDecay);
            if (!useColorDirectional)
            {
                m_Material.SetColor(Shader.PropertyToID("_ColorRay"), color);
            }

            foreach (var l in lightData)
            {
                m_Material.SetVector(Shader.PropertyToID("_LightPos"), render.WorldToViewportPoint(render.transform.position - l.transform.forward));
                if (useColorDirectional)
                {
                    m_Material.SetColor(Shader.PropertyToID("_ColorRay"), l.color);
                }
            } 
        }

       // m_Material.SetTexture("_MainTex", source);
       Graphics.Blit(source, destination, m_Material, 0);
    }
}
