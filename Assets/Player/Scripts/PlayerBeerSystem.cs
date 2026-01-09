using UnityEngine;

public class PlayerBeerSystem : MonoBehaviour
{
    [Header("Beer Settings")]
    public float drunkness = 0f;
    public float maxDrunkness = 1f;
    public float drunknessPerBeer = 0.2f;

    [Header("Drunk Effect Timing")]
    public float effectDuration = 25f;
    public float buildUpTime = 5f;

    [Header("Vomit Settings")]
    public ParticleSystem vomitParticles;

    [Header("Camera Effects")]
    public Camera mainCam;
    public Material blurMaterial;

    [Header("Wobble Strength")]
    public float rollStrength = 5f;
    public float pitchStrength = 2f;
    public float yawStrength = 2f;

    private float timer = 0f;
    private bool isDrunk = false;
    private float targetDrunkness = 0f;
    private int beerCount = 0;

    // 🍺 Wordt aangeroepen als speler een bier drinkt
    public void DrinkBeer()
    {
        beerCount++;

        isDrunk = true;
        timer = 0f;

        // Stapel dronkenschap
        targetDrunkness += drunknessPerBeer;
        targetDrunkness = Mathf.Clamp(targetDrunkness, 0f, maxDrunkness);

        // 🤢 Elke 5e bier → kotsen
        if (beerCount % 2 == 0)
        {
            Vomit();
        }
    }

    private void Update()
    {
        if (!isDrunk) return;

        timer += Time.deltaTime;

        // Opbouw naar target drunkness
        if (timer <= buildUpTime)
        {
            drunkness = Mathf.Lerp(drunkness, targetDrunkness, Time.deltaTime * 5f);
        }
        // Afbouw na effectDuration
        else if (timer >= effectDuration)
        {
            drunkness = Mathf.Lerp(drunkness, 0f, Time.deltaTime);

            if (drunkness <= 0.01f)
            {
                drunkness = 0f;
                targetDrunkness = 0f;
                timer = 0f;
                isDrunk = false;
            }
        }

        ApplyEffects();
    }

    private void ApplyEffects()
    {
        // 🎥 Camera wobble
        if (mainCam != null)
        {
            float roll = Mathf.Sin(Time.time * 3f) * rollStrength * drunkness;
            float pitch = Mathf.Cos(Time.time * 2.5f) * pitchStrength * drunkness;
            float yaw = Mathf.Sin(Time.time * 2f) * yawStrength * drunkness;

            Vector3 currentRotation = mainCam.transform.localEulerAngles;

            mainCam.transform.localRotation = Quaternion.Euler(
                currentRotation.x + pitch,
                currentRotation.y + yaw,
                roll
            );
        }

        // 🌫️ Blur effect
        if (blurMaterial != null)
        {
            blurMaterial.SetFloat("_Strength", drunkness);
        }
    }

    // 🤮 Particle kots
    private void Vomit()
    {
        if (vomitParticles == null) return;

        vomitParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        vomitParticles.Play();
    }
}
