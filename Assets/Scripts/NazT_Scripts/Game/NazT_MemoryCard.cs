using UnityEngine;
using DG.Tweening;

namespace NazosiTeyze
{
    public class NazT_MemoryCard : MonoBehaviour
    {
        public SpriteRenderer frontSprite;
        public SpriteRenderer backSprite;
        public int cardID;
        public float flipDuration = 0.3f;

        private bool isFlipped = false;
        private bool isMatched = false;
        private Vector3 originalScale;

        void Awake()
        {
            originalScale = transform.localScale;
        }

        public void FlipOpen()
        {
            isFlipped = true;

            Sequence seq = DOTween.Sequence();
            seq.Append(transform.DOScaleX(0f, flipDuration / 2f).SetEase(Ease.InOutSine))
               .AppendCallback(() =>
               {
                   backSprite.gameObject.SetActive(false);
                   frontSprite.gameObject.SetActive(true);
               })
               .Append(transform.DOScaleX(originalScale.x, flipDuration / 2f).SetEase(Ease.InOutSine));
        }

        public void FlipClose()
        {
            isFlipped = false;

            Sequence seq = DOTween.Sequence();
            seq.Append(transform.DOScaleX(0f, flipDuration / 2f).SetEase(Ease.InOutSine))
               .AppendCallback(() =>
               {
                   frontSprite.gameObject.SetActive(false);
                   backSprite.gameObject.SetActive(true);
               })
               .Append(transform.DOScaleX(originalScale.x, flipDuration / 2f).SetEase(Ease.InOutSine));
        }

        public void MarkAsMatched()
        {
            isMatched = true;
        }

        public void ResetCard()
        {
            DOTween.Kill(transform);

            isFlipped = false;
            isMatched = false;

            if (frontSprite != null) frontSprite.gameObject.SetActive(true);
            if (backSprite != null) backSprite.gameObject.SetActive(false);

            transform.localScale = originalScale;
        }

        void OnDisable()
        {
            ResetCard();
        }

        void OnMouseDown()
        {
            if (!Input.GetMouseButtonDown(0)) return;
            if (isFlipped || isMatched || NazT_MemoryGameManager.Instance == null) return;

            NazT_MemoryGameManager.Instance.CardClicked(this);
        }
    }
}
