using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    public float floatSpeed = 30f;
    public float fadeDuration = 1f;

    private TextMeshProUGUI tmp;
    private float timer = 0f;

    void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        timer += Time.deltaTime;

        // Beweeg omhoog
        transform.localPosition += Vector3.up * floatSpeed * Time.deltaTime;

        // Fade out
        if (tmp != null)
        {
            float alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            tmp.color = new Color(tmp.color.r, tmp.color.g, tmp.color.b, alpha);
        }

        // Vernietig na fade
        if (timer >= fadeDuration)
        {
            Destroy(gameObject);
        }
    }
}
