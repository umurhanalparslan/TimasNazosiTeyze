using UnityEngine;
using DG.Tweening;

namespace NazosiTeyze
{
    public class NazT_NazoSimpleEntry : MonoBehaviour
    {
        public Transform nazoObject;               // Tek sprite objesi (her sey dahil)
        public Transform targetPos;                // Gidecegi hedef
        public SpriteRenderer[] torTexts;          // Blink yapan yazilar

        public float moveDuration = 1.2f;

        [Header("Motor efekti")]
        public float shakeAmount = 0.03f;
        public float shakeDuration = 0.15f;

        [Header("Blink ayarlari")]
        public float blinkIn = 0.1f;
        public float blinkOut = 0.1f;

        private Vector3 nazoStartPos;
        private bool isStarted = false;
        private Sequence motorSequence;

        void Start()
        {
            if (nazoObject != null)
                nazoStartPos = nazoObject.localPosition;

            foreach (var t in torTexts)
            {
                if (t != null)
                {
                    Color c = t.color;
                    c.a = 0f;
                    t.color = c;
                }
            }
        }

        void OnMouseDown()
        {
            if (!Input.GetMouseButtonDown(0) || isStarted || targetPos == null || nazoObject == null)
                return;

            isStarted = true;

            nazoObject.DOLocalMove(targetPos.localPosition, moveDuration)
                .SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    StartTorBlink();
                    StartMotorEffect();
                });
        }

        void StartTorBlink()
        {
            if (torTexts == null || torTexts.Length == 0) return;

            Sequence seq = DOTween.Sequence();

            foreach (var t in torTexts)
            {
                if (t == null) continue;

                seq.Append(t.DOFade(1f, blinkIn))
                   .AppendInterval(0.05f)
                   .Append(t.DOFade(0f, blinkOut))
                   .AppendInterval(0.05f);
            }

            seq.SetLoops(-1, LoopType.Restart);
        }

        void StartMotorEffect()
        {
            if (nazoObject == null) return;

            float baseX = nazoObject.localPosition.x;

            motorSequence = DOTween.Sequence();
            motorSequence.Append(nazoObject.DOLocalMoveX(baseX + shakeAmount, shakeDuration).SetEase(Ease.InOutSine))
                         .Append(nazoObject.DOLocalMoveX(baseX - shakeAmount, shakeDuration).SetEase(Ease.InOutSine));
            motorSequence.SetLoops(-1, LoopType.Yoyo);
        }

        void OnDisable()
        {
            if (motorSequence != null)
                motorSequence.Kill();

            if (nazoObject != null)
            {
                DOTween.Kill(nazoObject);
                nazoObject.DOKill(); // ekstra guvenlik
                nazoObject.localPosition = nazoStartPos;
            }

            foreach (var t in torTexts)
            {
                if (t != null)
                {
                    DOTween.Kill(t.transform);
                    Color c = t.color;
                    c.a = 0f;
                    t.color = c;
                }
            }

            isStarted = false;
        }
    }
}
