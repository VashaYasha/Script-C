using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Diperlukan untuk mengontrol TextMeshPro UI
using UnityEngine.SceneManagement; // Diperlukan untuk merestart scene

public class Gerak : MonoBehaviour
{
    [Header("Pergerakan & Lompat")]
    public int kecepatan;
    public int kekuatanLompat;
    public int pindah;
    public bool balik;

    [Header("Dash Attack")]
    public float kekuatanDash = 20f;
    public float durasiDash = 0.2f;
    private bool sedangDash = false;

    [Header("Atribut Player")]
    public int nyawa;
    public int score;
    private bool isDead = false;
    private bool isWin = false;

    [Header("Sistem Checkpoint & Jurang")]
    public float batasJatuhY = -10f;          // 🔴 Batas koordinat Y jika kucing jatuh tak berujung
    public TextMeshProUGUI teksCheckpointVisual; // 🔴 Tarik objek TeksCheckpointVisual ke sini
    private static Vector2 posisiCheckpoint;  // Static agar posisinya tersimpan saat scene di-reload
    private static bool sudahPunyaCheckpoint = false;
    private static int nyawaSaatCheckpoint;   // Menyimpan jumlah nyawa saat menyentuh checkpoint
    private static int scoreSaatCheckpoint;   // Menyimpan jumlah koin saat menyentuh checkpoint

    [Header("Sistem Koin Objektif")]
    public int koinDibutuhkan = 10;
    public TextMeshProUGUI teksKoinObjektif;

    [Header("UI Visual (HUD)")]
    public TextMeshProUGUI teksNyawaVisual;
    public GameObject[] objekHatiVisual;

    [Header("UI Game Over & Win")]
    public GameObject panelGameOverVisual;
    public GameObject panelWinVisual;

    Rigidbody2D rb;
    Animator anim;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        if (panelGameOverVisual != null) panelGameOverVisual.SetActive(false);
        if (panelWinVisual != null) panelWinVisual.SetActive(false);
        if (teksCheckpointVisual != null) teksCheckpointVisual.gameObject.SetActive(false);

        // 🔴 JIKA sudah punya checkpoint sebelumnya, pindahkan posisi kucing ke koordinat tersebut
        if (sudahPunyaCheckpoint)
        {
            transform.position = posisiCheckpoint;
            nyawa = nyawaSaatCheckpoint;
            score = scoreSaatCheckpoint;
        }

        UpdateTeksKoin();
    }

    void Update()
    {
        // 🔴 DETEKSI JATUH JURANG: Jika posisi Y kucing lebih rendah dari batas jatuh, otomatis mati
        if (transform.position.y < batasJatuhY && !isDead && !isWin)
        {
            nyawa = 0; // Paksa nyawa menjadi 0 agar memicu fungsi mati
        }

        // 1. Sinkronisasi Teks Angka Nyawa di Layar
        if (teksNyawaVisual != null)
        {
            teksNyawaVisual.text = "X " + nyawa.ToString();
        }

        // 2. Sinkronisasi Ikon Hati Visual
        if (objekHatiVisual != null && objekHatiVisual.Length > 0)
        {
            for (int i = 0; i < objekHatiVisual.Length; i++)
            {
                if (i < nyawa) objekHatiVisual[i].SetActive(true);
                else objekHatiVisual[i].SetActive(false);
            }
        }

        // 3. Logika Kematian Player
        if (nyawa <= 0)
        {
            if (!isDead && !isWin)
            {
                isDead = true;
                rb.velocity = Vector2.zero;
                rb.simulated = false;
                if (anim != null) anim.SetTrigger("dead");
                if (panelGameOverVisual != null) panelGameOverVisual.SetActive(true);
            }
            return;
        }

        if (isWin) return;

        // 4. Deteksi Input Dash (Tombol Shift Kiri)
        if (Input.GetKeyDown(KeyCode.LeftShift) && !sedangDash)
        {
            StartCoroutine(EksekusiDash());
        }

        if (sedangDash) return;

        // 5. Pergerakan Horizontal (A / D)
        if (Input.GetKey(KeyCode.D))
        {
            rb.velocity = new Vector2(kecepatan, rb.velocity.y);
            pindah = 1;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            rb.velocity = new Vector2(-kecepatan, rb.velocity.y);
            pindah = -1;
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            pindah = 0;
        }

        // 6. Pergerakan Melompat (W)
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (rb.velocity.y == 0)
            {
                rb.AddForce(new Vector2(0, kekuatanLompat), ForceMode2D.Impulse);
                if (anim != null) anim.SetBool("isJumping", true);
            }
        }

        if (rb.velocity.y == 0 && anim != null)
        {
            anim.SetBool("isJumping", false);
        }

        // 7. Logika Balik Badan Otomatis
        if (pindah > 0 && balik) balikBadan();
        else if (pindah < 0 && !balik) balikBadan();

        if (anim != null) anim.SetFloat("Speed", Mathf.Abs(pindah));
    }

    void balikBadan()
    {
        balik = !balik;
        Vector3 karakter = transform.localScale;
        karakter.x *= -1;
        transform.localScale = karakter;
    }

    IEnumerator EksekusiDash()
    {
        sedangDash = true;
        if (anim != null) anim.SetTrigger("attack");

        float arahDash = 0f;
        if (Input.GetKey(KeyCode.A)) arahDash = -1f;
        else if (Input.GetKey(KeyCode.D)) arahDash = 1f;
        else arahDash = balik ? -1f : 1f;

        rb.velocity = new Vector2(arahDash * kekuatanDash, rb.velocity.y);
        float gravitasiAwal = rb.gravityScale;
        rb.gravityScale = 0f;

        yield return new WaitForSeconds(durasiDash);

        rb.gravityScale = gravitasiAwal;
        rb.velocity = new Vector2(0, rb.velocity.y);
        sedangDash = false;
    }

    public void AmbilKoin()
    {
        if (koinDibutuhkan > 0)
        {
            koinDibutuhkan--;
            score++;
            UpdateTeksKoin();

            if (koinDibutuhkan <= 0)
            {
                isWin = true;
                rb.velocity = Vector2.zero;

                // 🔴 MENANG: Hapus seluruh data checkpoint agar permainan diulang murni dari awal
                sudahPunyaCheckpoint = false;

                if (panelWinVisual != null)
                {
                    panelWinVisual.SetActive(true);
                }
            }
        }
    }

    void UpdateTeksKoin()
    {
        if (teksKoinObjektif != null)
        {
            teksKoinObjektif.text = "OBJECTIVES:\nFind x" + koinDibutuhkan + " Coins";
        }
    }

    // 🔴 LOGIKA TABRAKAN: Membaca sensor checkpoint & serangan dash ke kelelawar
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. Deteksi Menyentuh Checkpoint
        if (collision.CompareTag("Checkpoint"))
        {
            posisiCheckpoint = collision.transform.position; // Simpan posisi tiang/bendera
            nyawaSaatCheckpoint = nyawa;                     // Simpan status nyawa saat itu
            scoreSaatCheckpoint = score;                     // Simpan status koin saat itu
            sudahPunyaCheckpoint = true;

            // Jalankan tulisan "CHECKPOINT" berkedip di layar
            StartCoroutine(TampilkanTeksCheckpoint());

            // Matikan collider tiang agar tidak bisa dipicu berulang-ulang
            collision.enabled = false;
        }

        // 2. Deteksi Dash ke Musuh
        if (collision.CompareTag("Enemy"))
        {
            MusuhTerbang bat = collision.GetComponent<MusuhTerbang>();
            if (bat != null && sedangDash)
            {
                bat.TerkenaSerangan(1);
            }
        }
    }

    // Coroutine untuk memunculkan teks checkpoint selama 2 detik lalu hilang
    IEnumerator TampilkanTeksCheckpoint()
    {
        if (teksCheckpointVisual != null)
        {
            teksCheckpointVisual.gameObject.SetActive(true);
            yield return new WaitForSeconds(2f);
            teksCheckpointVisual.gameObject.SetActive(false);
        }
    }

    public void KlikMainLagi()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
