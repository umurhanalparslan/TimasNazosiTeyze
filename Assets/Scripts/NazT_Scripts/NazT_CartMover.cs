using UnityEngine;
using DG.Tweening;

namespace NazosiTeyze
{
    public class NazT_CartMover : MonoBehaviour
    {
        public Transform cartToMove;                   // Hareket edecek obje (teyze vs)
        public Transform targetPos;                    // Gidecegi pozisyon
        public SpriteRenderer trailSprite;             // Alt iz sprite
        public SpriteRenderer[] torTexts;              // TOR yazilari
        public Transform wheelToRotate;                // Donen teker
        public float moveDuration = 1.2f;              // Hareket suresi
        public float fadeDuration = 0.6f;              // Iz fade suresi
        public float torBlinkDelay = 0.2f;             // Blink arasi gecikme
        public float rotationSpeed = 180f;             // Teker donus hizi (derece/sn)

        private Vector3 initialPos;
        private bool isClicked = false;
        private Sequence motorSequence;

        void Start()
        {
            if (cartToMove != null)
                initialPos = cartToMove.localPosition;

            if (trailSprite != null)
            {
                Color clr = trailSprite.color;
                clr.a = 0f;
                trailSprite.color = clr;
            }

            foreach (var t in torTexts)
            {
                if (t != null)
                {
                    Color c = t.color;
                    c.a = 0f;
                    t.color = c;
                }
            }

            if (wheelToRotate != null)
                wheelToRotate.localRotation = Quaternion.identity;
        }

        void OnMouseDown()
        {
            if (!Input.GetMouseButtonDown(0) || isClicked || targetPos == null || cartToMove == null)
                return;

            isClicked = true;

            cartToMove.DOLocalMove(targetPos.localPosition, moveDuration)
                .SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    if (trailSprite != null)
                        trailSprite.DOFade(1f, fadeDuration).SetEase(Ease.InOutQuad);

                    BlinkTorTexts();
                    StartMotorEffect();
                    StartWheelRotation();
                });
        }

        void BlinkTorTexts()
        {
            if (torTexts == null || torTexts.Length == 0) return;

            Sequence seq = DOTween.Sequence();

            foreach (var t in torTexts)
            {
                if (t == null) continue;

                seq.Append(t.DOFade(1f, 0.2f))
                   .AppendInterval(0.1f)
                   .Append(t.DOFade(0f, 0.2f))
                   .AppendInterval(torBlinkDelay);
            }

            seq.SetLoops(-1, LoopType.Restart); // Sonsuz dongu
        }

        void StartMotorEffect()
        {
            if (cartToMove == null) return;

            float baseX = cartToMove.localPosition.x;

            motorSequence = DOTween.Sequence();
            motorSequence.Append(cartToMove.DOLocalMoveX(baseX + 0.05f, 0.3f).SetEase(Ease.InOutSine))
                         .Append(cartToMove.DOLocalMoveX(baseX - 0.05f, 0.3f).SetEase(Ease.InOutSine));
            motorSequence.SetLoops(-1, LoopType.Yoyo);
        }

        void StartWheelRotation()
        {
            if (wheelToRotate == null) return;

            wheelToRotate.DOLocalRotate(new Vector3(0f, 0f, -360f), 360f / rotationSpeed, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart);
        }

        void OnDisable()
        {
            if (cartToMove != null)
            {
                DOTween.Kill(cartToMove);
                cartToMove.localPosition = initialPos;
            }

            if (motorSequence != null)
                motorSequence.Kill();

            if (trailSprite != null)
            {
                Color clr = trailSprite.color;
                clr.a = 0f;
                trailSprite.color = clr;
            }

            foreach (var t in torTexts)
            {
                if (t != null)
                {
                    Color c = t.color;
                    c.a = 0f;
                    t.color = c;
                }
            }

            if (wheelToRotate != null)
            {
                DOTween.Kill(wheelToRotate);
                wheelToRotate.localRotation = Quaternion.identity;
            }

            isClicked = false;
        }
    }
}
