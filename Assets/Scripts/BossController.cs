using UnityEngine;
using System.Collections;

public class Boss : MonoBehaviour
{
    public int health = 100; // Boss's health
    public float flashDuration = 0.1f; // How long the boss turns red when hit
    public GameObject hitEffect; // Particle effect to play when the boss is hit

    private SpriteRenderer spriteRenderer; // Reference to the boss's sprite renderer
    private Color originalColor; // Store the boss's original color

    void Start()
    {
        // Get the SpriteRenderer component
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Store the boss's original color
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage; // Reduce health by the damage amount
        Debug.Log("Boss took " + damage + " damage! Remaining health: " + health);

        // Flash red when hit
        if (spriteRenderer != null)
        {
            StartCoroutine(FlashRed());
        }

        // Play hit effect
        if (hitEffect != null)
        {
            Instantiate(hitEffect, transform.position, Quaternion.identity);
        }

        if (health <= 0)
        {
            Die(); // Call the Die method if health drops to 0 or below
        }
    }

    IEnumerator FlashRed()
    {
        // Change the boss's color to red
        spriteRenderer.color = Color.red;

        // Wait for the flash duration
        yield return new WaitForSeconds(flashDuration);

        // Revert to the original color
        spriteRenderer.color = originalColor;
    }

    void Die()
    {
        Debug.Log("Boss defeated!");
        Destroy(gameObject); // Destroy the boss object
    }
}