using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomVerticalLayout : MonoBehaviour
{
    public float spacing = 10f; // Jarak antar child
    public float padding = 10f; // Padding atas dan bawah
    public bool alignToTop = true; // Tata letak mulai dari atas

    public List<Transform> children = new List<Transform>();

    private float totalHeight; // Total tinggi tata letak

    void Start()
    {
        RebuildLayout(); // Bangun tata letak saat Start
        ResetScrollPosition(); // Atur posisi scroll ke atas
    }

    void OnTransformChildrenChanged()
    {
        RebuildLayout(); // Rebuild tata letak jika ada perubahan child
        ResetScrollPosition(); // Perbarui posisi scroll
    }

    public void RebuildLayout()
    {
        float currentY = alignToTop ? -padding : 0f; // Posisi Y awal
        totalHeight = padding; // Inisialisasi tinggi total dengan padding

        for (int i = 0; i < transform.childCount; i++)
        {
            children.Add(transform.GetChild(i));
        }

        children.Reverse();

        for (int i = 0; i < children.Count; i++)
        {
            Transform child = children[i];

            // Abaikan child yang tidak aktif
            if (!child.gameObject.activeSelf) continue;

            // Hitung tinggi child
            float childHeight = GetChildHeight(child);

            // Atur posisi child
            child.localPosition = new Vector3(0, currentY, 0);

            // Update posisi Y untuk child berikutnya
            currentY -= childHeight + spacing;

            // Tambahkan tinggi child ke total tinggi
            totalHeight += childHeight + spacing;
        }

        totalHeight -= spacing; // Hapus spacing terakhir
        totalHeight += padding; // Tambahkan padding bawah

        // Perbarui collider parent
        UpdateParentCollider();
    }

    private float GetChildHeight(Transform child)
    {
        // Coba dapatkan tinggi dari SpriteRenderer
        SpriteRenderer spriteRenderer = child.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            return spriteRenderer.bounds.size.y;
        }

        // Jika tidak ada SpriteRenderer, fallback ke skala Y
        return child.localScale.y;
    }

    private void UpdateParentCollider()
    {
        // Perbarui BoxCollider2D untuk mencakup semua child
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider != null)
        {
            Vector2 size = boxCollider.size;
            size.y = totalHeight;
            boxCollider.size = size;

            // Atur offset collider agar sesuai dengan tata letak
            Vector2 offset = boxCollider.offset;
            offset.y = -totalHeight / 2f;
            boxCollider.offset = offset;
        }
    }

    public void ResetScrollPosition()
    {
        // Atur posisi parent ke bagian atas
        Vector3 startPosition = transform.position;
        startPosition.y = totalHeight / 2f; // Mulai dari atas tata letak
        transform.position = startPosition;
    }
}
