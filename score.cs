using Unity.VisualScripting;
using UnityEngine;

public class score : MonoBehaviour
{
    Gerak komponenGerak;

    // Start is called before the first frame update
    void Start()
    {
        // Mencari objek bernama "kucing" dan mengambil komponen script Gerak-nya
        GameObject targetKucing = GameObject.Find("kucing");
        if (targetKucing != null)
        {
            komponenGerak = targetKucing.GetComponent<Gerak>();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Memeriksa apakah yang menabrak koin adalah Player
        if (other.transform.tag == "Player")
        {
            if (komponenGerak != null)
            {
                // 🛠️ PANGGIL FUNGSI BARU: Mengurangi sisa koin objektif dan cek menang
                komponenGerak.AmbilKoin();
            }

            // Menghancurkan koin dari peta setelah diambil
            Destroy(gameObject);
        }
    }
}
