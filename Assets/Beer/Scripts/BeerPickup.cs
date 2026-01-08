using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class BeerPickup : MonoBehaviour
{
    [Header("UI")]
    public Text promptText;

    [Header("Health")]
    public int healAmount = 50;

    [Header("Audio")]
    public AudioClip openBottleSound;  // geluid fles openen
    public AudioClip drinkSound;       // geluid drinken
    [Range(0f, 1f)] public float audioVolume = 1f;       // standaard volume
    [Range(0.9f, 1.1f)] public float pitchMin = 0.95f;   // minimale pitch drinkgeluid
    [Range(0.9f, 1.1f)] public float pitchMax = 1.05f;   // maximale pitch drinkgeluid
    [Range(1f, 2f)] public float openBottleVolumeMultiplier = 1.5f; // harder openen
    [Range(1f, 2f)] public float drinkVolumeMultiplier = 1.5f;      // harder drinken

    private bool inRange = false;
    private PlayerBeerSystem player;
    private PlayerHealth playerHealth;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.spatialBlend = 0f; // 2D geluid
        audioSource.volume = audioVolume;
        audioSource.playOnAwake = false;
    }

    private void Reset()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.isTrigger = true;
    }

    private void Update()
    {
        if (inRange && player != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            StartCoroutine(DrinkBeerSequence());
        }
    }

    private IEnumerator DrinkBeerSequence()
    {
        // Fles openen, harder geluid
        if (openBottleSound != null)
        {
            audioSource.pitch = 1f;
            audioSource.PlayOneShot(openBottleSound, Mathf.Clamp01(audioVolume * openBottleVolumeMultiplier));
        }

        yield return new WaitForSeconds(0.5f);

        // Drink 4 keer met pitch variatie en harder geluid
        for (int i = 0; i < 4; i++)
        {
            if (drinkSound != null)
            {
                audioSource.pitch = Random.Range(pitchMin, pitchMax);
                audioSource.PlayOneShot(drinkSound, Mathf.Clamp01(audioVolume * drinkVolumeMultiplier));
            }
            yield return new WaitForSeconds(0.3f);
        }

        // Voeg HP toe
        if (playerHealth != null)
            playerHealth.Heal(healAmount);

        if (player != null)
            player.DrinkBeer();

        // Respawn bier
        if (BeerSpawnManager.Instance != null)
            BeerSpawnManager.Instance.RespawnBeer(transform.position, transform.rotation, 10f);

        // UI prompt verwijderen
        if (promptText != null)
            promptText.text = "";

        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerBeerSystem p = other.GetComponent<PlayerBeerSystem>();
        PlayerHealth h = other.GetComponent<PlayerHealth>();

        if (p != null)
        {
            inRange = true;
            player = p;
            playerHealth = h;

            if (promptText != null)
                promptText.text = $"Druk [E] om bier te drinken (+{healAmount} HP)";
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerBeerSystem p = other.GetComponent<PlayerBeerSystem>();
        if (p != null)
        {
            inRange = false;
            player = null;
            playerHealth = null;

            if (promptText != null)
                promptText.text = "";
        }
    }
}
