using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    private float health;
    public float maxHealth = 100f;
    public float chipSpeed = 2f;
    private float lerpTimer;
    public Image frontHealthBar;
    public Image backHealthBar;

    public float damageAmount = 10f; // Damage per interval
    public float healAmount = 15f;   // Healing amount
    public float damageInterval = 1f; // Damage every 1 second while inside hazard zone

    private bool isInHazard = false; // Track if player is inside hazard zone
    private float hazardTimer = 0f;  // Timer to track damage interval

    public DamageScreenEffect damageEffect; // Reference to the script

    void Start()
    {
        health = maxHealth;
    }

    void Update()
    {
        health = Mathf.Clamp(health, 0, maxHealth);
        UpdateHealthUI();

        // Continuously damage player if inside hazard zone
        if (isInHazard)
        {
            hazardTimer += Time.deltaTime;
            if (hazardTimer >= damageInterval)
            {
                TakeDamage(damageAmount);
                hazardTimer = 0f; // Reset timer
            }
        }
    }

    public void UpdateHealthUI()
    {
        float fillF = frontHealthBar.fillAmount;
        float fillB = backHealthBar.fillAmount;
        float hFraction = health / maxHealth; // Normalize health value

        if (fillB > hFraction)
        {
            frontHealthBar.fillAmount = hFraction;
            backHealthBar.color = Color.red;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete *= percentComplete;
            backHealthBar.fillAmount = Mathf.Lerp(fillB, hFraction, percentComplete);
        }
        if (fillF < hFraction)
        {
            backHealthBar.color = Color.green;
            backHealthBar.fillAmount = hFraction;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete *= percentComplete;
            frontHealthBar.fillAmount = Mathf.Lerp(fillF, hFraction, percentComplete);
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        lerpTimer = 0f;

        if (damageEffect != null)
        {
            damageEffect.ShowDamageEffect(); // Show the bloody screen effect
        }

        Debug.Log("Player took damage! Current health: " + health);
    }

    public void RestoreHealth(float healAmount)
    {
        health += healAmount;
        lerpTimer = 0f;
        Debug.Log("Player healed! Current health: " + health);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hazard"))  // Start continuous damage
        {
            isInHazard = true;
            hazardTimer = 0f; // Reset timer when entering
        }
        else if (other.CompareTag("HealthPickup")) // Heal and remove object
        {
            RestoreHealth(healAmount);
            Destroy(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Hazard")) // Stop damage when leaving hazard
        {
            isInHazard = false;
        }
    }
}
