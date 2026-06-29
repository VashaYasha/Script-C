using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusuhTerbang : MonoBehaviour
{
    [Header("Pengaturan Jarak & Kecepatan")]
    public float kecepatan = 3f;
    public float jarakDeteksi = 7f;   // 🔴 TAMBAHAN: Jarak mulai mengejar player
    public float jarakSerang = 1.2f;   // Jarak dekat untuk mulai menggigit

    [Header("Atribut Serangan Musuh")]
    public int dayaSerang = 1;
    public float jedaSerangan = 1.5f;  // Jeda waktu antar gigitan (detik)

    [Header("Atribut Kesehatan Musuh")]
    public int nyawaMusuh = 1;         // 🔴 TAMBAHAN: Nyawa kelelawar

    private Transform targetPlayer;
    private Animator anim;
    private float waktuSeranganBerikutnya;
    private bool balik = false;

    void Start()
    {
        anim = GetComponent<Animator>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            targetPlayer = playerObj.transform;
        }
    }

    void Update()
    {
        if (targetPlayer == null) return;

        // Hitung jarak antara Bat dan Kucing
        float jarakKePlayer = Vector2.Distance(transform.position, targetPlayer.position);

        // 🔴 LOGIKA 1: Bat DIAM jika player di luar jarak deteksi
        if (jarakKePlayer > jarakDeteksi)
        {
            if (anim != null) anim.SetBool("isFlying", false);
            return;
        }

        // Atur arah hadap (Flip) otomatis ke arah player
        if (targetPlayer.position.x > transform.position.x && balik) Flip();
        else if (targetPlayer.position.x < transform.position.x && !balik) Flip();

        // 🔴 LOGIKA 2: Mengejar vs Menyerang di jarak tertentu
        if (jarakKePlayer > jarakSerang)
        {
            // Terbang mengejar player jika berada di antara jarakDeteksi dan jarakSerang
            transform.position = Vector2.MoveTowards(transform.position, targetPlayer.position, kecepatan * Time.deltaTime);
            if (anim != null) anim.SetBool("isFlying", true);
        }
        else
        {
            // Berhenti terbang mengejar jika sudah masuk jarak serang yang sangat dekat
            if (anim != null) anim.SetBool("isFlying", false);

            // Menyerang berkala sesuai jeda waktu
            if (Time.time >= waktuSeranganBerikutnya)
            {
                SerangPlayer();
                waktuSeranganBerikutnya = Time.time + jedaSerangan;
            }
        }
    }

    void SerangPlayer()
    {
        if (anim != null) anim.SetTrigger("attack");

        Gerak playerScript = targetPlayer.GetComponent<Gerak>();
        if (playerScript != null)
        {
            playerScript.nyawa -= dayaSerang;
        }
    }

    // 🔴 LOGIKA 3: Fungsi yang dipanggil saat kucing nge-dash menabrak Bat
    public void TerkenaSerangan(int damage)
    {
        nyawaMusuh -= damage;
        if (nyawaMusuh <= 0)
        {
            // Tambahkan efek hancur/mati di sini jika ada
            Destroy(gameObject);
        }
    }

    void Flip()
    {
        balik = !balik;
        Vector3 skala = transform.localScale;
        skala.x *= -1;
        transform.localScale = skala;
    }

    // Menggambar radius lingkaran di Scene View untuk patokan desainer
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, jarakDeteksi);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, jarakSerang);
    }
}
