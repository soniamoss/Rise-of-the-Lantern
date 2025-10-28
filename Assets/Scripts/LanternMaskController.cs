using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class LanternMaskController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform player;          // Player (parent)
    [SerializeField] Camera cam;                // Main Camera
    [SerializeField] Image darkMaskImage;       // UI Image with ScreenRadialMask material

    [Header("Battery Drain")]
    [SerializeField] float maxEnergy = 1f;
    [SerializeField] float drainPerSecond = 0.05f;
    [SerializeField] float minRadius = 0.15f;
    [SerializeField] float maxRadius = 0.40f;
    [SerializeField] float lowThreshold = 0.2f;

    [Header("Flicker")]
    [SerializeField] float flickerSpeed = 8f;
    [SerializeField] float maxFlickerShrink = 0.05f;

    [Header("Depletion Behavior")]
    [SerializeField] bool killOnEmpty = true;
    [SerializeField] float deathFadeTime = 0.35f;

    [Header("Endgame / Brighten")]
    [SerializeField] bool fullBright = false;     // when true, overlay is disabled
    [SerializeField] float endgameRadius = 1.05f; // how big the circle grows to
    [SerializeField] float brightenTime = 1.5f;   // seconds to expand
    bool animatingBrighten = false;

    // runtime
    float energy;
    Material mat;
    bool depletedFired;

    void Awake()
    {
        if (!player)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }
        if (!cam) cam = Camera.main;
        if (!darkMaskImage) darkMaskImage = GetComponent<Image>();

        if (darkMaskImage && darkMaskImage.material != null)
        {
            mat = Instantiate(darkMaskImage.material); 
            darkMaskImage.material = mat;
        }

        energy = maxEnergy;
        depletedFired = false;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Update()
    {
        if (!player || !cam || !mat) return;

        // If we’re already full bright, keep overlay off and skip logic
        if (fullBright || (GameManager.I != null && GameManager.I.worldLit))
        {
            if (darkMaskImage) darkMaskImage.enabled = false;
            return;
        }

        // Drain
        energy -= drainPerSecond * Time.deltaTime;
        energy = Mathf.Clamp01(energy);

        // Radius based on energy
        float radius = Mathf.Lerp(minRadius, maxRadius, energy);

        // Low battery flicker
        if (energy <= lowThreshold && energy > 0f)
        {
            float t = Mathf.PerlinNoise(Time.time * flickerSpeed, 0);
            float shrink = Mathf.Lerp(0, maxFlickerShrink, 1 - (energy / lowThreshold));
            radius -= t * shrink;
        }

        
        Vector3 vp = cam.WorldToViewportPoint(player.position);
        vp.x = Mathf.Clamp01(vp.x);
        vp.y = Mathf.Clamp01(vp.y);
        mat.SetVector("_Center", new Vector4(vp.x, vp.y, 0, 0));

        mat.SetFloat("_Radius", radius);
        mat.SetFloat("_Feather", 0.18f);

        // Death on empty
        if (killOnEmpty && !depletedFired && energy <= 0f)
        {
            depletedFired = true;
            StartCoroutine(DoLanternDeath());
        }
    }

    IEnumerator DoLanternDeath()
    {
        if (DeathOverlay.I != null)
            yield return DeathOverlay.I.FadeInAndHold(deathFadeTime, 0.6f, "Lantern died! Restarting...");
        else
            yield return new WaitForSeconds(deathFadeTime);

        if (GameManager.I != null)
        {
            GameManager.I.lives = 0;
            GameManager.I.LoseLife();
        }
    }

    // Called by GameManager on scene load
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // If we’ve already lit the world in a prior scene, stay bright everywhere
        if (GameManager.I != null && GameManager.I.worldLit)
        {
            SetFullBright(true);
            return;
        }

        if (scene.name == "Level_04")
        {
            // Animate: expand the light, then switch to full bright forever for this run
            StartCoroutine(BrightenThenLockFull());
        }
        else
        {
            // Reset to normal for earlier levels (when starting a new run)
            fullBright = false;
            if (darkMaskImage) darkMaskImage.enabled = true;
            depletedFired = false;
            energy = maxEnergy;
        }
    }

    IEnumerator BrightenThenLockFull()
    {
        if (animatingBrighten || mat == null) yield break;
        animatingBrighten = true;

        if (darkMaskImage && !darkMaskImage.enabled) darkMaskImage.enabled = true;

        // read current radius
        float startRadius = Mathf.Lerp(minRadius, maxRadius, energy);
        float t = 0f;
        while (t < brightenTime)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / brightenTime);
            float r = Mathf.Lerp(startRadius, endgameRadius, k);
            mat.SetFloat("_Radius", r);
            mat.SetFloat("_Feather", Mathf.Lerp(0.18f, 0.35f, k));
            yield return null;
        }

        // Lock full bright for the rest of the run
        SetFullBright(true);
        if (GameManager.I != null) GameManager.I.worldLit = true;

        animatingBrighten = false;
    }

    // Public helpers
    public void Recharge(float amount) => energy = Mathf.Clamp01(energy + amount);

    public void RestoreOnRespawn()
    {
        energy = Mathf.Max(energy, 0.5f);
        depletedFired = false;
    }

    public void SetPlayer(Transform t) => player = t;

    public void SetFullBright(bool on)
    {
        fullBright = on;
        if (darkMaskImage) darkMaskImage.enabled = !on;
        if (on) drainPerSecond = 0f; // stop draining once lit
    }
}
