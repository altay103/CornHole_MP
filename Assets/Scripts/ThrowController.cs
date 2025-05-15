using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowController : MonoBehaviour
{
    // Start is called before the first frame update
    bool isCharging = false;
    public GameObject projectilePrefab;

    public Transform throwPoint;
    public float throwForce = 10f;
    public float maxChargeTime = 2f;
    float chargeTime = 0f;
    GameObject projectile;
    Texture2D chargeBarTexture;

    void Awake()
    {
        chargeBarTexture = new Texture2D(1, 1);
    }

    void Start()
    {
        CreateProjectile();
    }

    // Update is called once per frame
    void Update()
    {
        // PC: Space tuşu
        if (Input.GetKey(KeyCode.Space))
        {
            isCharging = true;
            ChargeThrow();
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            isCharging = false;
            ReleaseThrow();
            chargeTime = 0f;
        }

        // Mobil: Ekrana uzun basma
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                isCharging = true;
                ChargeThrow();
            }
            if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isCharging = false;
                ReleaseThrow();
                chargeTime = 0f;
            }
            if (isCharging && touch.phase == TouchPhase.Stationary)
            {
                ChargeThrow();
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                isCharging = false;
                ReleaseThrow();
                chargeTime = 0f;
            }
        }
    }
    void ChargeThrow()
    {
        // Charge süresini arttır
        if (chargeTime < maxChargeTime)
            chargeTime += Time.deltaTime;
    }
    void ReleaseThrow()
    {
        if (projectilePrefab != null && throwPoint != null)
        {
            projectile.transform.SetParent(null);
            projectile.GetComponent<Rigidbody>().isKinematic = false;
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                float force = throwForce * Mathf.Lerp(0.3f, 1f, chargeTime / maxChargeTime); // Minimum %30 güç
                rb.AddForce(throwPoint.forward * force, ForceMode.Impulse);
            }
            Invoke("CreateProjectile", 0.5f);
        }
    }
    void CreateProjectile()
    {
        if (projectilePrefab != null && throwPoint != null)
        {
            projectile = Instantiate(projectilePrefab, throwPoint.position, throwPoint.rotation, throwPoint);
            projectile.GetComponent<Rigidbody>().isKinematic = true;
        }
    }
    void OnGUI()
    {
        if (isCharging)
        {
            float barWidth = 30f;
            float barHeight = 200f;
            float fill = Mathf.Clamp01(chargeTime / maxChargeTime);
            float x = Screen.width - barWidth - 30f; // Sağdan 30px içeride
            float y = (Screen.height - barHeight) / 2f; // Dikey ortala
            // Bar arka planı
            GUI.Box(new Rect(x, y, barWidth, barHeight), "");
            // Doluluk oranı (aşağıdan yukarıya)
            float fillHeight = barHeight * fill;
            float fillY = y + (barHeight - fillHeight);
            // Renk geçişi: yeşilden kırmızıya
            Color barColor = Color.Lerp(Color.green, Color.red, fill);
            chargeBarTexture.SetPixel(0, 0, barColor);
            chargeBarTexture.Apply();
            GUI.DrawTexture(new Rect(x, fillY, barWidth, fillHeight), chargeBarTexture);
            // Yüzdeyi barın üstüne yaz
            GUI.Label(new Rect(x - 20, y - 25, barWidth + 40, 20), $"{(fill * 100f):0}%", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 20 });
        }
    }
}
