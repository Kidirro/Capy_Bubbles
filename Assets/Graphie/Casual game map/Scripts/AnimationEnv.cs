using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimationEnv : MonoBehaviour
{
    public SpriteRenderer spriteRenderer; // SpriteRenderer yang menampilkan animasi
    public Sprite[] frames;               // Array frame dari spritesheet
    public float frameRate = 0.1f;        // Waktu antara setiap frame (kecepatan animasi)

    public float delay = 0;
    private int currentFrame;
    private float timer;

    void Start()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        // Pastikan kita mulai dari frame pertama
        currentFrame = 0;
        timer = 0;

        StartCoroutine(StartAnim());
    }

    IEnumerator StartAnim()
    {
        while (true)
        {
            if (frames.Length == 0) break;

            // Tambahkan waktu ke timer
            timer += Time.deltaTime;

            // Jika timer mencapai frameRate, pindah ke frame berikutnya
            if (timer >= frameRate)
            {
                timer = 0f; // Reset timer
                currentFrame++; // Pindah ke frame berikutnya

                // Jika frame mencapai akhir array, mulai kembali dari awal
                if (currentFrame >= frames.Length)
                {
                    currentFrame = 0;
                    yield return new WaitForSecondsRealtime(delay);
                }

                // Ganti sprite dari SpriteRenderer dengan frame saat ini
                spriteRenderer.sprite = frames[currentFrame];
            }

            yield return null;
        }
    }

    // Fungsi opsional untuk memulai animasi
    public void StartAnimation()
    {
        currentFrame = 0;
        timer = 0;
        enabled = true;
    }

    // Fungsi opsional untuk menghentikan animasi
    public void StopAnimation()
    {
        enabled = false;
    }
}
