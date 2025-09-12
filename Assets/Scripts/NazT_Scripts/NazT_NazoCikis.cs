using UnityEngine;
using DG.Tweening;

namespace NazosiTeyze
{
    public class NazT_NazoCikis : MonoBehaviour
    {
        public SpriteRenderer[] torTexts;
        public Transform headTransform;
        public Transform capeTransform;
        public Transform wheelTransform;
        public Transform cartTransform;               // 🔁 Araba (salinacak)

        public float headShakeAmount = 10f;
        public float headShakeDuration = 0.4f;

        public float capeSwingAmount = 15f;
        public float capeSwingDuration = 1f;

        public float cartSwingAmount = 0.05f;         // 🔁 Araba salinimi (X ekseni)
        public float cartSwingDuration = 0.3f;

        public float rotationSpeed = 180f;

        private Quaternion headStartRot;
        private Quaternion capeStartRot;
        private Vector3 cartStartPos;
        private bool isStarted = false;
        private Sequence cartMotorSequence;

        void Start()
        {
            if (headTransform != null)
                headStartRot = headTransform.localRotation;

            if (capeTransform != null)
                capeStartRot = capeTransform.localRotation;

            if (cartTransform != null)
                cartStartPos = cartTransform.localPosition;

            foreach (var t in torTexts)
            {
                if (t != null)
                {
                    Color c = t.color;
                    c.a = 0f;
                    t.color = c;
                }
            }

            if (wheelTransform != null)
                wheelTransform.localRotation = Quaternion.identity;
        }

        void OnMouseDown()
        {
            if (!Input.GetMouseButtonDown(0) || isStarted)
                return;

            isStarted = true;

            StartTorBlink();
            StartHeadShake();
            StartCapeSwing();
            StartWheelSpin();
            StartCartShake(); // 🔁 araba sallaniyor
        }

        void StartTorBlink()
        {
            if (torTexts == null || torTexts.Length == 0) return;

            Sequence seq = DOTween.Sequence();

            foreach (var t in torTexts)
            {
                if (t == null) continue;

                seq.Append(t.DOFade(1f, 0.1f))
                   .AppendInterval(0.05f)
                   .Append(t.DOFade(0f, 0.1f))
                   .AppendInterval(0.05f);
            }

            seq.SetLoops(-1, LoopType.Restart); // 🔁 hizli blink
        }

        void StartHeadShake()
        {
            if (headTransform == null) return;

            headTransform.DOLocalRotate(new Vector3(0f, 0f, headShakeAmount), headShakeDuration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }

        void StartCapeSwing()
        {
            if (capeTransform == null) return;

            capeTransform.DOLocalRotate(new Vector3(0f, 0f, -capeSwingAmount), capeSwingDuration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }

        void StartWheelSpin()
        {
            if (wheelTransform == null) return;

            wheelTransform.DOLocalRotate(new Vector3(0f, 0f, -360f), 360f / rotationSpeed, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart);
        }

        void StartCartShake()
        {
            if (cartTransform == null) return;

            float baseX = cartTransform.localPosition.x;

            cartMotorSequence = DOTween.Sequence();
            cartMotorSequence.Append(cartTransform.DOLocalMoveX(baseX + cartSwingAmount, cartSwingDuration).SetEase(Ease.InOutSine))
                             .Append(cartTransform.DOLocalMoveX(baseX - cartSwingAmount, cartSwingDuration).SetEase(Ease.InOutSine));
            cartMotorSequence.SetLoops(-1, LoopType.Yoyo);
        }

        void OnDisable()
        {
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

            if (headTransform != null)
            {
                DOTween.Kill(headTransform);
                headTransform.localRotation = headStartRot;
            }

            if (capeTransform != null)
            {
                DOTween.Kill(capeTransform);
                capeTransform.localRotation = capeStartRot;
            }

            if (wheelTransform != null)
            {
                DOTween.Kill(wheelTransform);
                wheelTransform.localRotation = Quaternion.identity;
            }

            if (cartTransform != null)
            {
                DOTween.Kill(cartTransform);
                cartTransform.localPosition = cartStartPos;
            }

            if (cartMotorSequence != null)
                cartMotorSequence.Kill();

            isStarted = false;
        }
    }
}
