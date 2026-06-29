using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBerjalan : MonoBehaviour
{
    [Header("Pengaturan Patroli NPC")]
    public float kecepatan = 2f;
    public float jarakPatroli = 4f;

    private Vector2 posisiAwal;
    private int arahMaju = 1;
    private bool balik = false;

    void Start()
    {
        // Mencatat koordinat awal tempat NPC ditaruh di map
        posisiAwal = transform.position;
    }

    void Update()
    {
        // Hitung batas patroli kanan dan kiri
        float batasKanan = posisiAwal.x + jarakPatroli;
        float batasKiri = posisiAwal.x - jarakPatroli;

        // Logika balik arah saat menyentuh batas jarak
        if (transform.position.x >= batasKanan && arahMaju == 1)
        {
            arahMaju = -1;
            Flip();
        }
        else if (transform.position.x <= batasKiri && arahMaju == -1)
        {
            arahMaju = 1;
            Flip();
        }

        // Pergerakan murni menggunakan koordinat posisi (Anti-Terbang!)
        transform.Translate(Vector2.right * arahMaju * kecepatan * Time.deltaTime);
    }

    void Flip()
    {
        balik = !balik;
        Vector3 skala = transform.localScale;
        skala.x *= -1;
        transform.localScale = skala;
    }
}
