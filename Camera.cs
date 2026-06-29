using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class camera : MonoBehaviour
{
    // Target yang akan diikuti (drag Object Player kamu ke sini nanti)
    public Transform kucing;

    // Kecepatan kamera mengikuti player (makin kecil makin halus/lambat)
    public float smoothSpeed = 0.125f;

    // Jarak aman antara kamera dan player (X, Y, Z)
    // Untuk game 2D, biasanya nilai Z diatur ke -10 agar kamera tidak menempel di player
    public Vector3 offset = new Vector3(0f, 0f, -10f);

    void LateUpdate()
    {
        if (kucing != null)
        {
            Vector3 desiredPosition = kucing.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

            // Cukup gunakan baris ini untuk memindahkan kamera:
            transform.position = smoothedPosition;

            // HAPUS ATAU JANGAN GUNAKAN BARIS DI BAWAH INI:
            // transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        }
    }
}
