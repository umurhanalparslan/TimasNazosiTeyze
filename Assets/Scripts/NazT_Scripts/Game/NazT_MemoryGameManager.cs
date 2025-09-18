using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

namespace NazosiTeyze
{
    public class NazT_MemoryGameManager : MonoBehaviour
    {
        public static NazT_MemoryGameManager Instance;

        public List<NazT_MemoryCard> cards;
        public float previewTime = 2f;

        private NazT_MemoryCard firstCard;
        private NazT_MemoryCard secondCard;
        private bool inputLocked = true;

        private List<Vector3> cardPositions = new List<Vector3>();

        void OnEnable()
        {
            Instance = this;

            // Pozisyonlari kaydet (bir kez)
            cardPositions.Clear();
            foreach (var card in cards)
            {
                cardPositions.Add(card.transform.localPosition);
            }

            foreach (var card in cards)
            {
                card.ResetCard();
                card.transform.localScale = Vector3.one;
            }

            RestartGame();
        }

        public void RestartGame()
        {
            DOTween.KillAll();
            ShuffleCards();
            ShowAllCardsTemporarily();
        }

        void ShuffleCards()
        {
            // Kartlari siralamada karistir
            for (int i = 0; i < cards.Count; i++)
            {
                var temp = cards[i];
                int rand = Random.Range(i, cards.Count);
                cards[i] = cards[rand];
                cards[rand] = temp;
            }

            // Pozisyonlari karistir ve yerlestir
            List<Vector3> shuffledPositions = new List<Vector3>(cardPositions);

            for (int i = 0; i < cards.Count; i++)
            {
                int rand = Random.Range(0, shuffledPositions.Count);
                cards[i].transform.localPosition = shuffledPositions[rand];
                shuffledPositions.RemoveAt(rand);
            }
        }

        void ShowAllCardsTemporarily()
        {
            inputLocked = true;

            foreach (var card in cards)
            {
                card.ResetCard(); // On yuzler acik
            }

            // 2 saniye sonra flip ile kapanacak
            DOVirtual.DelayedCall(previewTime, () =>
            {
                foreach (var card in cards)
                {
                    card.FlipClose();
                }

                DOVirtual.DelayedCall(0.4f, () => inputLocked = false);
            });
        }

        public void CardClicked(NazT_MemoryCard card)
        {
            if (inputLocked) return;

            card.FlipOpen();
            AudioManager.Instance.Play("KartSecme");

            if (firstCard == null)
            {
                firstCard = card;
                return;
            }

            if (secondCard == null)
            {
                secondCard = card;
                inputLocked = true;

                DOVirtual.DelayedCall(0.8f, CheckMatch);
            }
        }

        void CheckMatch()
        {
            if (firstCard.cardID == secondCard.cardID)
            {
                firstCard.MarkAsMatched();
                secondCard.MarkAsMatched();
                AudioManager.Instance.Play("Eslesti");
            }

            else
            {
                firstCard.FlipClose();
                secondCard.FlipClose();
                AudioManager.Instance.Play("Eslesmedi");

            }

            firstCard = null;
            secondCard = null;
            inputLocked = false;
        }

        void OnDisable()
        {
            DOTween.Kill(transform);
            Instance = null;
        }
    }
}
