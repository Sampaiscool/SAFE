using UnityEngine;

public class PlayerCatcher : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Transform[] columnPositions; // Array van GameObjects die de kolommen vertegenwoordigen
    private int currentColumnIndex = 0;

    void Update()
    {
        // Alleen beweging toestaan als de FirewallMinigame actief is
        if (GameManager.Instance.CurrentControl == AppNames.FirewallMinigame)
        {
            // Controleer of er op A of D gedrukt wordt
            if (Input.GetKeyDown(KeyCode.A))
            {
                // Ga naar de vorige kolom, maar zorg dat je niet onder 0 komt
                currentColumnIndex = Mathf.Max(0, currentColumnIndex - 1);
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                // Ga naar de volgende kolom, maar blijf binnen de grenzen van de array
                currentColumnIndex = Mathf.Min(columnPositions.Length - 1, currentColumnIndex + 1);
            }

            // Verplaats de speler soepel naar de positie van de gekozen kolom
            Vector3 targetPosition = new Vector3(columnPositions[currentColumnIndex].position.x, transform.position.y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("FallingBlock"))
        {
            // Roep de BlockCaught-functie aan in de FirewallBreak-manager
            FindObjectOfType<FirewallBreak>().BlockCaught();
            // Vernietig het blokje na het vangen
            Destroy(other.gameObject);
        }
    }
}