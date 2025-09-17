using UnityEngine;
using DG.Tweening;

namespace NazosiTeyze
{
    // Kedi kolu bardaga vurunca bardagin donerek hedefe dusmesini saglar
    public class NazT_CatCupHit : MonoBehaviour
    {
        [Header("Kedi kolu")]
        public Transform catArm;

        [Header("Bardak objesi")]
        public Transform cup;

        [Header("Bardagin dusus hedefi")]
        public Transform targetPos;

        [Header("Animasyon ayarlari")]
        public float armRotateAngle = -25f; // kol donme acisi
        public float armRotateDuration = 0.3f;
        public float cupFallDuration = 0.8f;
        public float cupRotateAmount = 360f; // bardak donme
        public float cupScalePunch = 0.2f;   // bounce scale

        private Quaternion armStartRot;
        private Vector3 cupStartPos;
        private Quaternion cupStartRot;
        private Vector3 cupStartScale;

        private bool triggered = false;

        void Start()
        {
            if (catArm != null) armStartRot = catArm.localRotation;
            if (cup != null)
            {
                cupStartPos = cup.localPosition;
                cupStartRot = cup.localRotation;
                cupStartScale = cup.localScale;
            }
        }

        void OnMouseDown()
        {
            if (!Input.GetMouseButtonDown(0)) return;
            if (triggered) return;

            triggered = true;

            if (catArm != null)
            {
                // Kedi kolunu dondur
                catArm.DOLocalRotate(new Vector3(0, 0, armRotateAngle), armRotateDuration)
                    .SetEase(Ease.OutQuad)
                    .OnComplete(() =>
                    {
                        // Bardak duserken donsun + scale bounce
                        if (cup != null && targetPos != null)
                        {
                            Sequence cupSeq = DOTween.Sequence();

                            cupSeq.Append(cup.DOLocalMove(targetPos.localPosition, cupFallDuration)
                                .SetEase(Ease.InOutQuad));

                            cupSeq.Join(cup.DOLocalRotate(
                                new Vector3(0, 0, cupRotateAmount), cupFallDuration, RotateMode.FastBeyond360)
                                .SetEase(Ease.OutQuad));

                            cupSeq.Join(cup.DOPunchScale(Vector3.one * cupScalePunch, 0.4f, 6, 0.8f));
                        }

                        // Kol tekrar eski rotasyona donsun
                        catArm.DOLocalRotateQuaternion(armStartRot, armRotateDuration).SetEase(Ease.OutQuad);
                    });
            }
        }

        void OnDisable()
        {
            DOTween.Kill(transform);

            if (catArm != null)
                catArm.localRotation = armStartRot;

            if (cup != null)
            {
                cup.localPosition = cupStartPos;
                cup.localRotation = cupStartRot;
                cup.localScale = cupStartScale;
            }

            triggered = false;
        }
    }
}
