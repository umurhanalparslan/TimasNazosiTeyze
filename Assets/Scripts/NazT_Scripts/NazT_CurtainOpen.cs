using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

// perde acma, fade out ve el sallama islemlerini yapar
namespace NazosiTeyze
{
    public class NazT_CurtainOpen : MonoBehaviour
    {
        [Header("Acik hale gelecek objeler")]
        public List<GameObject> openObjects;

        [Header("Fade olacak objeler")]
        public List<SpriteRenderer> fadeOutObjects;

        [Header("Sallanacak eller")]
        public List<Transform> shakingHands;

        [Header("Fade ayarlari")]
        public float fadeDuration = 0.4f;

        [Header("El sallanma")]
        public float shakeAmount = 10f;
        public float shakeDuration = 0.5f;

        // orijinal alpha ve rotasyon degerlerini tutmak icin
        private Dictionary<Transform, Quaternion> originalRotations = new Dictionary<Transform, Quaternion>();
        private Dictionary<SpriteRenderer, float> originalAlphas = new Dictionary<SpriteRenderer, float>();
        private bool isTriggered = false;

        void Start()
        {
            // Tum acilacak objeleri sahne basinda kapat
            foreach (var go in openObjects)
            {
                if (go != null)
                    go.SetActive(false);
            }

            // Ellerin baslangic rotasyonlarini kaydet
            foreach (var t in shakingHands)
            {
                if (t != null && !originalRotations.ContainsKey(t))
                    originalRotations.Add(t, t.localRotation);
            }

            // Sprite'larin baslangic alpha degerlerini kaydet
            foreach (var s in fadeOutObjects)
            {
                if (s != null && !originalAlphas.ContainsKey(s))
                    originalAlphas.Add(s, s.color.a);
            }
        }

        void OnMouseDown()
        {
            // Tiklama kontrolu + sadece bir kere calissin
            if (!Input.GetMouseButtonDown(0) || isTriggered)
                return;

            isTriggered = true;

            // Tum openObjects listesindekileri aktif hale getir
            foreach (var go in openObjects)
            {
                if (go != null)
                    go.SetActive(true);
            }

            // Fade out listesine fade uygula
            foreach (var s in fadeOutObjects)
            {
                if (s != null)
                    s.DOFade(0f, fadeDuration).SetEase(Ease.InOutQuad);
            }

            // Elleri salla
            foreach (var t in shakingHands)
            {
                if (t != null)
                {
                    t.DOLocalRotate(new Vector3(0f, 0f, shakeAmount), shakeDuration)
                     .SetEase(Ease.InOutSine)
                     .SetLoops(-1, LoopType.Yoyo);
                }
            }
        }

        void OnDisable()
        {
            // Tum acilan objeleri kapat
            foreach (var go in openObjects)
            {
                if (go != null)
                    go.SetActive(false);
            }

            // Sprite'lari eski alpha'ya dondur
            foreach (var s in fadeOutObjects)
            {
                if (s != null && originalAlphas.ContainsKey(s))
                {
                    DOTween.Kill(s.transform);
                    var c = s.color;
                    c.a = originalAlphas[s];
                    s.color = c;
                }
            }

            // Ellerin rotasyonunu sifirla
            foreach (var t in shakingHands)
            {
                if (t != null && originalRotations.ContainsKey(t))
                {
                    DOTween.Kill(t);
                    t.localRotation = originalRotations[t];
                }
            }

            // Tekrar kullanilabilir hale getir
            isTriggered = false;
        }
    }
}
