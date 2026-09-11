using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float lifeTime = 0.5f;

    private float timer;
    private TextMeshPro textMesh;
    private Color originalColor;

    void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
        originalColor = textMesh.color;
    }

    public void Setup(int damageAmount)
    {
        textMesh.text = "-" + damageAmount.ToString();
        textMesh.color = originalColor;
        timer = lifeTime;
    }

    void Update()
    {
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);
        timer -= Time.deltaTime;

        float fadeAlpha = timer / lifeTime;
        textMesh.color = new Color(originalColor.r, originalColor.g, originalColor.b, fadeAlpha);

        if (timer <= 0) DamageTextPool.Instance.ReturnToPool(gameObject);
    }
}