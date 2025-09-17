using UnityEngine;
using DG.Tweening;

namespace NazosiTeyze
{
    // Karaktere tiklandiginda hapsu yazisini sekilli gosterir ve kol + kafa hareketi yapar
    public class NazT_SneezeCharacter : MonoBehaviour
    {
        [Header("Karakter Parcalari")]
        public Transform arm;
        public Transform head;

        [Header("Hapsu Yazisi")]
        public GameObject sneezeTextObj;
        public float textFadeDuration = 0.3f;
        public float textScalePunch = 0.4f;
        public float textMoveUp = 0.5f;

        [Header("Animasyon Ayarlari")]
        public float armRotateAngle = -30f;
        public float headTiltAngle = -15f;
        public float moveDuration = 0.25f;

        private Quaternion armStartRot;
        private Quaternion headStartRot;
        private Vector3 textStartPos;
        private Vector3 textStartScale;
        private CanvasGroup textCanvasGroup;
        private bool isAnimating = false;

        void Start()
        {
            if (arm != null) armStartRot = arm.localRotation;
            if (head != null) headStartRot = head.localRotation;

            if (sneezeTextObj != null)
            {
                Transform t = sneezeTextObj.transform;
                textStartPos = t.localPosition;
                textStartScale = t.localScale;

                textCanvasGroup = sneezeTextObj.GetComponent<CanvasGroup>();
                if (textCanvasGroup == null)
                    textCanvasGroup = sneezeTextObj.AddComponent<CanvasGroup>();

                sneezeTextObj.SetActive(false); // basta gizli
            }
        }

        void OnMouseDown()
        {
            if (!Input.GetMouseButtonDown(0)) return;
            if (isAnimating) return;

            isAnimating = true;

            Sequence seq = DOTween.Sequence();

            // Kol rotasyon hareketi
            if (arm != null)
                seq.Join(arm.DOLocalRotate(new Vector3(0, 0, armRotateAngle), moveDuration)
                    .SetEase(Ease.InOutSine).SetLoops(2, LoopType.Yoyo));

            // Kafa hareketi
            if (head != null)
                seq.Join(head.DOLocalRotate(new Vector3(0, 0, headTiltAngle), moveDuration)
                    .SetEase(Ease.InOutSine).SetLoops(2, LoopType.Yoyo));

            // Hapsu yazisi sekilli cikis
            if (sneezeTextObj != null && textCanvasGroup != null)
            {
                Transform t = sneezeTextObj.transform;
                sneezeTextObj.SetActive(true);
                textCanvasGroup.alpha = 0f;
                t.localScale = textStartScale;
                t.localPosition = textStartPos;

                seq.Insert(0, textCanvasGroup.DOFade(1f, textFadeDuration));
                seq.Insert(0, t.DOPunchScale(Vector3.one * textScalePunch, 0.4f, 6, 0.8f));
                seq.Insert(0, t.DOLocalMoveY(textStartPos.y + textMoveUp, 0.6f).SetEase(Ease.OutQuad));
                seq.AppendInterval(0.6f);
                seq.Append(textCanvasGroup.DOFade(0f, textFadeDuration));
                seq.OnComplete(() =>
                {
                    sneezeTextObj.SetActive(false);
                    t.localScale = textStartScale;
                    t.localPosition = textStartPos;
                    isAnimating = false;
                });
            }
            else
            {
                seq.OnComplete(() => isAnimating = false);
            }
        }

        void OnDisable()
        {
            DOTween.Kill(transform);

            if (arm != null) arm.localRotation = armStartRot;
            if (head != null) head.localRotation = headStartRot;

            if (sneezeTextObj != null)
            {
                sneezeTextObj.SetActive(false);
                sneezeTextObj.transform.localPosition = textStartPos;
                sneezeTextObj.transform.localScale = textStartScale;
                if (textCanvasGroup != null) textCanvasGroup.alpha = 0f;
            }

            isAnimating = false;
        }
    }
}
