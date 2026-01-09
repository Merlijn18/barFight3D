using UnityEngine;

public class PlayerBeerSystem : MonoBehaviour
{
    [Header("Beer Settings")]
    public float drunkness = 0f;
    public float maxDrunkness = 1f;
    public float drunknessPerBeer = 0.2f; // 🔥 meer dronken per biertje
    public float effectDuration = 25f;
    public float buildUpTime = 5f;

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

    public void DrinkBeer()
    {
        isDrunk = true;
        timer = 0f;

        // 🍺 Stapel dronkenschap
        targetDrunkness += drunknessPerBeer;
        targetDrunkness = Mathf.Clamp(targetDrunkness, 0f, maxDrunkness);
    }

    private void Update()
    {
        if (!isDrunk) return;

        timer += Time.deltaTime;

        // Opbouw naar huidige targetDrunkness
        if (timer <= buildUpTime)
        {
            drunkness = Mathf.Lerp(drunkness, targetDrunkness, Time.deltaTime);
        }
        // Afbouw
        else if (timer >= effectDuration)
        {
            drunkness = Mathf.Lerp(drunkness, 0f, Time.deltaTime);
            targetDrunkness = drunkness;

            if (drunkness <= 0.01f)
            {
                drunkness = 0f;
                targetDrunkness = 0f;
                isDrunk = false;
            }
        }

        ApplyEffects();
    }

    private void ApplyEffects()
    {
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

        if (blurMaterial != null)
        {
            blurMaterial.SetFloat("_Strength", drunkness);
        }
    }
}
