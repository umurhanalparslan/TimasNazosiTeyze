using UnityEngine;
using DG.Tweening;

namespace NazosiTeyze
{
    // Kapinin acilmasini smooth yapar ve kapidan sonra karakteri aktif edip kafasini sallatir
    public class NazT_DoorCharacter : MonoBehaviour
    {
        [Header("Kapi Objeleri")]
        public SpriteRenderer doorClosed;
        public SpriteRenderer doorOpen;

        [Header("Karakter Objeleri")]
        public GameObject character;
        public Transform characterHead;

        [Header("Animasyon Ayarlari")]
        public float doorFadeDuration = 0.5f;
        public float headAngle = 20f;
        public float headDuration = 0.4f;
        public int headRepeat = 2;

        private Quaternion headStartRot;
        private Color closedStartColor;
        private Color openStartColor;
        private bool triggered = false;

        void Start()
        {
            if (characterHead != null)
                headStartRot = characterHead.localRotation;

            if (character != null)
                character.SetActive(false);

            if (doorClosed != null)
            {
                closedStartColor = doorClosed.color;
                doorClosed.color = new Color(closedStartColor.r, closedStartColor.g, closedStartColor.b, 1f);
            }

            if (doorOpen != null)
            {
                openStartColor = doorOpen.color;
                doorOpen.color = new Color(openStartColor.r, openStartColor.g, openStartColor.b, 0f);
            }
        }

        void OnMouseDown()
        {
            if (!Input.GetMouseButtonDown(0)) return;
            if (triggered) return;

            triggered = true;

            if (doorClosed != null)
                doorClosed.DOFade(0f, doorFadeDuration).SetEase(Ease.InOutSine);

            if (doorOpen != null)
            {
                doorOpen.DOFade(1f, doorFadeDuration).SetEase(Ease.InOutSine)
                    .OnComplete(() =>
                    {
                        if (character != null)
                            character.SetActive(true);

                        if (characterHead != null)
                        {
                            characterHead.DOLocalRotate(
                                new Vector3(0, 0, headAngle), headDuration)
                                .SetEase(Ease.InOutSine)
                                .SetLoops(headRepeat * 2, LoopType.Yoyo)
                                .OnComplete(() =>
                                {
                                    characterHead.DOLocalRotateQuaternion(headStartRot, 0.2f)
                                        .SetEase(Ease.OutQuad);
                                });
                        }
                    });
            }
        }

        void OnDisable()
        {
            DOTween.Kill(transform);

            if (characterHead != null)
                characterHead.localRotation = headStartRot;

            if (character != null)
                character.SetActive(false);

            if (doorClosed != null)
                doorClosed.color = new Color(closedStartColor.r, closedStartColor.g, closedStartColor.b, 1f);

            if (doorOpen != null)
                doorOpen.color = new Color(openStartColor.r, openStartColor.g, openStartColor.b, 0f);

            triggered = false;
        }
    }
}
