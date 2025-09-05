using UnityEngine;

public class FallingBlock : MonoBehaviour
{
    public float fallSpeed;
    public FirewallBreak firewallBreak;

    void Update()
    {
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Als het blokje de grond raakt, is de speler af
        if (other.CompareTag("Ground"))
        {
            firewallBreak.GameOver();
            Destroy(gameObject);
        }
    }
}