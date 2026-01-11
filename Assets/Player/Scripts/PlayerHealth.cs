using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    [SerializeField] private int currentHealth;

    [Header("UI")]
    [SerializeField] private Image healthImage;
    [SerializeField] private RectTransform healthRect;

    [Header("Health Sprites")]
    public Sprite health100;
    public Sprite health75;
    public Sprite health50;
    public Sprite health25;
    public Sprite health0;

    [Header("Visual Effects")]
    [SerializeField] private float fadeSpeed = 10f;
    [SerializeField] private float shakeStrength = 5f;
    [SerializeField] private float shakeDuration = 0.1f;

    [Header("Idle Movement")]
    [SerializeField] private float floatAmplitude = 2f;
    [SerializeField] private float floatSpeed = 2f;

    [Header("Audio")]
    public AudioClip damageSound;
    public AudioClip heartbeatSound;
    [Range(0f, 1f)] public float damageVolume = 1f;
    [Range(0f, 1f)] public float heartbeatVolume = 0.6f;

    [Header("Heartbeat Settings")]
    [SerializeField] private int heartbeatStartHealth = 30;
    [SerializeField] private float minHeartbeatDelay = 0.4f;
    [SerializeField] private float maxHeartbeatDelay = 1.2f;

    private AudioSource audioSource;
    private AudioSource heartbeatSource;

    private Sprite targetSprite;
    private Coroutine fadeCoroutine;
    private Coroutine heartbeatCoroutine;
    private Vector2 basePosition;

    public int CurrentHealth => currentHealth;

    private void Awake()
    {
        // Damage audio
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;

        // Heartbeat audio (aparte source)
        heartbeatSource = gameObject.AddComponent<AudioSource>();
        heartbeatSource.playOnAwake = false;
        heartbeatSource.loop = false;
        heartbeatSource.spatialBlend = 0f;
        heartbeatSource.volume = heartbeatVolume;
    }

    private void Start()
    {
        Time.timeScale = 1f;
        currentHealth = maxHealth;

        if (healthRect != null)
            basePosition = healthRect.anchoredPosition;

        UpdateHealthUI(true);
    }

    private void Update()
    {
        // Subtiele idle float
        if (healthRect == null) return;

        float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        healthRect.anchoredPosition = basePosition + new Vector2(0, yOffset);
    }

    // ================= DAMAGE =================
    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthUI(false);

        if (damageSound != null)
            audioSource.PlayOneShot(damageSound, damageVolume);

        StartCoroutine(ShakeHealth());
        UpdateHeartbeat();

        if (currentHealth <= 0)
            Die();
    }

    // ================= HEAL =================
    public void Heal(int amount)
    {
        if (currentHealth <= 0) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthUI(false);
        UpdateHeartbeat();
    }

    // ================= UI =================
    private void UpdateHealthUI(bool instant)
    {
        if (healthImage == null) return;

        if (currentHealth > 75)
            targetSprite = health100;
        else if (currentHealth > 50)
            targetSprite = health75;
        else if (currentHealth > 25)
            targetSprite = health50;
        else if (currentHealth > 0)
            targetSprite = health25;
        else
            targetSprite = health0;

        float t = (float)currentHealth / maxHealth;
        Color baseColor = Color.Lerp(Color.red, Color.white, t);
        healthImage.color = new Color(baseColor.r, baseColor.g, baseColor.b, healthImage.color.a);

        if (instant)
        {
            healthImage.sprite = targetSprite;
            healthImage.color = new Color(
                healthImage.color.r,
                healthImage.color.g,
                healthImage.color.b,
                1f
            );
            return;
        }

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeToSprite(targetSprite));
    }

    private IEnumerator FadeToSprite(Sprite newSprite)
    {
        while (healthImage.color.a > 0)
        {
            healthImage.color -= new Color(0, 0, 0, Time.deltaTime * fadeSpeed);
            yield return null;
        }

        healthImage.sprite = newSprite;

        while (healthImage.color.a < 1)
        {
            healthImage.color += new Color(0, 0, 0, Time.deltaTime * fadeSpeed);
            yield return null;
        }
    }

    // ================= SHAKE =================
    private IEnumerator ShakeHealth()
    {
        if (healthRect == null) yield break;

        Vector3 originalPos = basePosition;
        Vector3 originalScale = healthRect.localScale;

        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            healthRect.anchoredPosition =
                originalPos + (Vector3)Random.insideUnitCircle * shakeStrength;

            float scale = 1f + Mathf.Sin(elapsed * 40f) * 0.05f;
            healthRect.localScale = originalScale * scale;

            elapsed += Time.deltaTime;
            yield return null;
        }

        healthRect.anchoredPosition = originalPos;
        healthRect.localScale = originalScale;
    }

    // ================= HEARTBEAT =================
    private void UpdateHeartbeat()
    {
        if (currentHealth <= 0 || heartbeatSound == null)
        {
            StopHeartbeat();
            return;
        }

        if (currentHealth <= heartbeatStartHealth)
        {
            if (heartbeatCoroutine == null)
                heartbeatCoroutine = StartCoroutine(HeartbeatLoop());
        }
        else
        {
            StopHeartbeat();
        }
    }

    private IEnumerator HeartbeatLoop()
    {
        while (currentHealth > 0 && currentHealth <= heartbeatStartHealth)
        {
            heartbeatSource.pitch = Mathf.Lerp(1f, 2f, 1f - ((float)currentHealth / heartbeatStartHealth));
            heartbeatSource.PlayOneShot(heartbeatSound, heartbeatVolume *10);

            float t = (float)currentHealth / heartbeatStartHealth;
            float delay = Mathf.Lerp(minHeartbeatDelay, maxHeartbeatDelay, t);

            yield return new WaitForSeconds(delay);
        }

        heartbeatCoroutine = null;
    }

    private void StopHeartbeat()
    {
        if (heartbeatCoroutine != null)
        {
            StopCoroutine(heartbeatCoroutine);
            heartbeatCoroutine = null;
        }
    }

    // ================= DEATH =================
    private void Die()
    {
        StopHeartbeat();
        Debug.Log("PLAYER IS DEAD");
        SceneManager.LoadScene("DeathMenu");
    }
}
