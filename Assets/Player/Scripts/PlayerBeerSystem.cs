using UnityEngine;

public class PlayerBeerSystem : MonoBehaviour
{
    [Header("Beer Settings")]
    public float drunkness = 0f;
    public float maxDrunkness = 1f;
    public float effectDuration = 25f; // Langer effect
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

    public void DrinkBeer()
    {
        isDrunk = true;
        timer = 0f;
    }

    private void Update()
    {
        if (!isDrunk) return;

        timer += Time.deltaTime;

        // Opbouw
        if (timer <= buildUpTime)
            drunkness = Mathf.Lerp(0, maxDrunkness, timer / buildUpTime);
        // Max
        else if (timer > buildUpTime && timer < (effectDuration - buildUpTime))
            drunkness = maxDrunkness;
        // Afbouw
        else if (timer >= (effectDuration - buildUpTime) && timer <= effectDuration)
            drunkness = Mathf.Lerp(0, maxDrunkness, (effectDuration - timer) / buildUpTime);
        else
        {
            drunkness = 0;
            timer = 0;
            isDrunk = false;
        }

        ApplyEffects();
    }

    private void ApplyEffects()
    {
        if (mainCam != null)
        {
            // Extra wobble: sin + cos voor pitch, yaw en roll
            float roll = Mathf.Sin(Time.time * 3f) * rollStrength * drunkness;
            float pitch = Mathf.Cos(Time.time * 2.5f) * pitchStrength * drunkness;
            float yaw = Mathf.Sin(Time.time * 2f) * yawStrength * drunkness;

            // Combineer met bestaande camera rotatie
            mainCam.transform.localRotation = Quaternion.Euler(
                pitch,
                mainCam.transform.localEulerAngles.y + yaw,
                roll
            );
        }

        if (blurMaterial != null)
        {
            float blurStrength = Mathf.Lerp(0f, 1f, drunkness);
            blurMaterial.SetFloat("_Strength", blurStrength);
        }
    }
}
