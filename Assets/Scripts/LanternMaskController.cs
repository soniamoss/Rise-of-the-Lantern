using UnityEngine;
using UnityEngine.UI;

public class LanternMaskController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform player;
    [SerializeField] Camera cam;
    [SerializeField] Image darkMaskImage;

    [Header("Battery")]
    [SerializeField] float maxEnergy = 1f;
    [SerializeField] float drainPerSecond = 0.05f;
    [SerializeField] float minRadius = 0.15f;
    [SerializeField] float maxRadius = 0.40f;
    [SerializeField] float lowThreshold = 0.2f;

    [Header("Flicker")]
    [SerializeField] float flickerSpeed = 8f;
    [SerializeField] float maxFlickerShrink = 0.05f;

    float energy;
    Material mat;

    void Awake()
    {
        if (!darkMaskImage) darkMaskImage = GetComponent<Image>();
        mat = Instantiate(darkMaskImage.material);
        darkMaskImage.material = mat;
        energy = maxEnergy;
    }

    void Update()
    {
        if (!player || !cam || !mat) return;

        // drain battery
        energy -= drainPerSecond * Time.deltaTime;
        energy = Mathf.Clamp01(energy);

        // radius from energy
        float radius = Mathf.Lerp(minRadius, maxRadius, energy);

        // low-battery flicker
        if (energy <= lowThreshold)
        {
            float t = Mathf.PerlinNoise(Time.time * flickerSpeed, 0);
            float shrink = Mathf.Lerp(0, maxFlickerShrink, 1 - (energy / lowThreshold));
            radius -= t * shrink;
        }

        // center on player (viewport 0..1)
        Vector3 vp = cam.WorldToViewportPoint(player.position);
        vp.x = Mathf.Clamp01(vp.x);
        vp.y = Mathf.Clamp01(vp.y);

        mat.SetVector("_Center", new Vector4(vp.x, vp.y, 0, 0));
        mat.SetFloat("_Radius", radius);
        mat.SetFloat("_Feather", 0.18f);

        // optional: handle total blackout later (game over)
        // if (energy <= 0f) {...}
    }

    public void Recharge(float amount)   // amount in 0..1
    {
        energy = Mathf.Clamp01(energy + amount);
    }

    public void RestoreOnRespawn()
    {
        energy = Mathf.Max(energy, 0.5f);
    }
}
