using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

namespace NazosiTeyze
{
    // Teyzenin hedefe gitmesini, animasyonlarini ve objelerin hareketini kontrol eder
    public class NazT_TeyzeArrival : MonoBehaviour
    {
        [Header("Ana objeler")]
        public Transform teyzeRoot;                 // Hareket edecek parent obje
        public Transform targetPos;                 // Gidilecek hedef

        [Header("Yuruyen ayaklar")]
        public List<Transform> ayaklar;             // Yukari asagi hareket edecek ayaklar

        [Header("Tekerlekler")]
        public List<Transform> tekerlekler;         // Dönecek tekerlekler

        [Header("Anahtar (ziplayan")]
        public Transform anahtar;                    // Jump animasyonu yapacak obje

        [Header("Kafa (sallanacak)")]
        public Transform kafa;                      // Kafa objesi (surekli sallanacak)

        [Header("Hareket Ayarlari")]
        public float moveDuration = 1.5f;
        public float ayakStepHeight = 0.1f;
        public float ayakStepDuration = 0.25f;
        public float tekerlekDonusHizi = 360f; // derece/sn

        [Header("Kafa animasyon")]
        public float kafaDonusAcisi = 10f;
        public float kafaDonusSuresi = 1f;

        [Header("Anahtar ziplama")]
        public float anahtarJumpHeight = 0.2f;
        public float anahtarJumpDuration = 0.4f;

        private Vector3 startPos;
        private Quaternion kafaStartRot;
        private bool isMoving = false;
        private List<Vector3> ayakStartPos = new List<Vector3>();

        void Start()
        {
            if (teyzeRoot != null)
                startPos = teyzeRoot.localPosition;

            foreach (var a in ayaklar)
            {
                if (a != null)
                    ayakStartPos.Add(a.localPosition);
            }

            if (kafa != null)
                kafaStartRot = kafa.localRotation;
        }

        void OnMouseDown()
        {
            if (!Input.GetMouseButtonDown(0) || isMoving || teyzeRoot == null || targetPos == null)
                return;

            isMoving = true;

            // Yurumeye basla
            teyzeRoot.DOLocalMove(targetPos.localPosition, moveDuration).SetEase(Ease.OutSine).OnComplete(() =>
            {
                // Varinca tekerlekleri durdur
                foreach (var t in tekerlekler)
                {
                    if (t != null)
                        DOTween.Kill(t);
                }

                // Ayak animasyonlarini durdur
                foreach (var a in ayaklar)
                {
                    if (a != null)
                        DOTween.Kill(a);
                }

                // Kafa sallanmasi baslar
                StartKafaSallama();

                // Anahtar ziplamasi devam eder
                StartAnahtarJump();
            });

            // Ayaklar yurur
            StartAyakYurume();

            // Tekerlekler doner
            foreach (var t in tekerlekler)
            {
                if (t != null)
                {
                    t.DOLocalRotate(new Vector3(0f, 0f, 360f), 1f, RotateMode.FastBeyond360)
                     .SetEase(Ease.Linear)
                     .SetLoops(-1, LoopType.Restart);
                }
            }
        }

        void StartAyakYurume()
        {
            for (int i = 0; i < ayaklar.Count; i++)
            {
                var ayak = ayaklar[i];
                if (ayak == null) continue;

                float offset = i % 2 == 0 ? 0f : ayakStepDuration / 2f; // sag-sol ayak sirasina gore

                ayak.DOLocalMoveY(ayak.localPosition.y + ayakStepHeight, ayakStepDuration)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetDelay(offset);
            }
        }

        void StartKafaSallama()
        {
            if (kafa == null) return;

            kafa.DOLocalRotate(new Vector3(0f, 0f, kafaDonusAcisi), kafaDonusSuresi)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }

        void StartAnahtarJump()
        {
            if (anahtar == null) return;

            anahtar.DOLocalMoveY(anahtar.localPosition.y + anahtarJumpHeight, anahtarJumpDuration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }

        void OnDisable()
        {
            // root
            if (teyzeRoot != null)
            {
                DOTween.Kill(teyzeRoot);
                teyzeRoot.localPosition = startPos;
            }

            // ayaklar
            for (int i = 0; i < ayaklar.Count; i++)
            {
                if (ayaklar[i] != null)
                {
                    DOTween.Kill(ayaklar[i]);
                    ayaklar[i].localPosition = ayakStartPos[i];
                }
            }

            // tekerler
            foreach (var t in tekerlekler)
            {
                if (t != null)
                {
                    DOTween.Kill(t);
                    t.localRotation = Quaternion.identity;
                }
            }

            // anahtar
            if (anahtar != null)
            {
                DOTween.Kill(anahtar);
                anahtar.localPosition = new Vector3(
                    anahtar.localPosition.x,
                    startPos.y,
                    anahtar.localPosition.z);
            }

            // kafa
            if (kafa != null)
            {
                DOTween.Kill(kafa);
                kafa.localRotation = kafaStartRot;
            }

            isMoving = false;
        }
    }
}
