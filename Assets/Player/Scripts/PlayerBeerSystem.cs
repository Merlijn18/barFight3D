using UnityEngine;

public class PlayerBeerSystem : MonoBehaviour
{
    [Header("Beer Settings")]
    public float drunkness = 0f;
    public float maxDrunkness = 1f;
    public float effectDuration = 25f; // langer effect (van 15s naar 25s)
    public float buildUpTime = 5f;

    [Header("Camera Effects")]
    public Camera mainCam;
    public Material blurMaterial;

    private float timer = 0f;
    private bool isDrunk = false;

    [Header("Wobble Strength")]
    public float rollStrength = 5f;
    public float pitchStrength = 2f;
    public float yawStrength = 2f;

    private void Update()
    {
        if (isDrunk)
        {
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
        else
        {
            ApplyEffects(); // reset effecten
        }
    }

    public void DrinkBeer()
    {
        isDrunk = true;
        timer = 0f;

        // Eventueel extra tijd toevoegen bij meerdere biertjes
        // effectDuration += 5f;
    }

    private void ApplyEffects()
    {
        if (mainCam != null)
        {
            float roll = Mathf.Sin(Time.time * 3f) * rollStrength * drunkness;
            float pitch = Mathf.Cos(Time.time * 2.5f) * pitchStrength * drunkness;
            float yaw = Mathf.Sin(Time.time * 2f) * yawStrength * drunkness;

            Vector3 currentEuler = mainCam.transform.localEulerAngles;
            mainCam.transform.localRotation = Quaternion.Euler(
                currentEuler.x + pitch,
                currentEuler.y + yaw,
                roll
            );
        }

        if (blurMaterial != null)
        {
            // Blur sterkte groeit met drunkness
            float blurStrength = Mathf.Lerp(0f, 1f, drunkness); // 1 = maximaal wazig
            blurMaterial.SetFloat("_Strength", blurStrength);
        }
    }
}
