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
    public AudioClip openBottleSound;
    public AudioClip drinkSound;
    [Range(0f, 1f)] public float audioVolume = 1f;

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
            // Start het bierdrinkproces
            StartCoroutine(DrinkBeerSequence());
        }
    }

    // Coroutine die alles afhandelt: geluid, heal, respawn, verwijderen
    private IEnumerator DrinkBeerSequence()
    {
        // Speel openen fles geluid
        if (openBottleSound != null)
            audioSource.PlayOneShot(openBottleSound, audioVolume);

        // Start dronken effect
        player.DrinkBeer();

        // Heal speler
        if (playerHealth != null)
            playerHealth.Heal(healAmount);

        // Speel drink geluid 4 keer hoorbaar
        if (drinkSound != null)
        {
            for (int i = 0; i < 4; i++)
            {
                audioSource.PlayOneShot(drinkSound, audioVolume);
                yield return new WaitForSeconds(drinkSound.length * 0.9f); // wacht bijna tot het geluid klaar is
            }
        }

        // Verwijder UI prompt
        if (promptText != null)
            promptText.text = "";

        // Respawn bier na 10 seconden
        if (BeerSpawnManager.Instance != null)
            BeerSpawnManager.Instance.RespawnBeer(transform.position, transform.rotation, 10f);

        // Verwijder het bierobject pas NA het afspelen van alle geluiden
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerBeerSystem p = other.GetComponent<PlayerBeerSystem>();
        PlayerHealth h = other.GetComponentInChildren<PlayerHealth>();

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
