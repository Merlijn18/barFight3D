using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;
using TMPro;


[RequireComponent(typeof(Collider))]
public class BeerPickup : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI promptText;

    [Header("Health")]
    public int healAmount = 50;

    [Header("Audio")]
    public AudioClip openBottleSound;
    public AudioClip drinkSound;
    [Range(0f, 1f)] public float audioVolume = 1f;

    private bool inRange;
    private PlayerBeerSystem player;
    private PlayerHealth playerHealth;
    private AudioSource audioSource;

    // Input System
    //private PlayerInputActions inputActions;
    public InputActionReference interactAction;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 0f;
        audioSource.volume = audioVolume;

    }
    private void OnEnable()
    {
        if (interactAction != null)
            interactAction.action.performed += OnInteract;
    }

    private void OnDisable()
    {
        if (interactAction != null)
            interactAction.action.performed -= OnInteract;
    }


    private void OnInteract(InputAction.CallbackContext context)
    {
        if (inRange && player != null)
        {
            // Start het bierdrinkproces
            StartCoroutine(DrinkBeerSequence());
        }
    }

    // Coroutine die alles afhandelt: geluid, heal, respawn, verwijderen
    private IEnumerator DrinkBeerSequence()
    {
        // Zet mesh en collider uit zodat het bier meteen "weg" is
        GetComponent<Collider>().enabled = false;

        foreach (var r in GetComponentsInChildren<Renderer>())
            r.enabled = false;

        // UI weg
        if (promptText != null)
            promptText.gameObject.SetActive(false);

        if (openBottleSound != null)
            audioSource.PlayOneShot(openBottleSound, audioVolume);

        // Effecten
        player.DrinkBeer();

        if (playerHealth != null)
            playerHealth.Heal(healAmount);

        // Drinkgeluid
        if (drinkSound != null)
        {
            for (int i = 0; i < 4; i++)
            {
                audioSource.PlayOneShot(drinkSound, audioVolume);
                yield return new WaitForSeconds(drinkSound.length * 0.9f);
            }
        }

        // Respawn
        if (BeerSpawnManager.Instance != null)
            BeerSpawnManager.Instance.RespawnBeer(transform.position, transform.rotation, 10f);

        Destroy(gameObject);
    }


    private void OnTriggerEnter(Collider other)
    {
        player = other.GetComponent<PlayerBeerSystem>();
        playerHealth = other.GetComponentInChildren<PlayerHealth>();

        if (player != null)
        {
            inRange = true;
            //player = p;
            //playerHealth = h;

            if (promptText != null)
                promptText.gameObject.SetActive(true); // alleen "E"
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerBeerSystem>() != null)
        {
            inRange = false;
            player = null;
            playerHealth = null;

            if (promptText != null)
                promptText.gameObject.SetActive(false);
        }
    }
}
