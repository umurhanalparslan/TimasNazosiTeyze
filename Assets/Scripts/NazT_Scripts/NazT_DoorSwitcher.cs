using UnityEngine;
using DG.Tweening;

// Tiklaninca acik kapi fade out olur, kapali kapi fade in olur
namespace NazosiTeyze
{
    public class NazT_DoorSwitcher : MonoBehaviour
    {
        [Header("Acik kapi")]
        public SpriteRenderer acikKapi;

        [Header("Kapali kapi")]
        public SpriteRenderer kapaliKapi;

        [Header("Fade suresi")]
        public float fadeDuration = 0.4f;

        private float acikKapiStartAlpha;
        private float kapaliKapiStartAlpha;
        private bool isTriggered = false;

        void Start()
        {
            // Null kontrol
            if (acikKapi != null)
                acikKapiStartAlpha = acikKapi.color.a;

            if (kapaliKapi != null)
            {
                kapaliKapiStartAlpha = kapaliKapi.color.a;

                // Sahne giriste kapali kapi görünmez baslasin
                Color c = kapaliKapi.color;
                c.a = 0f;
                kapaliKapi.color = c;
            }
        }

        void OnMouseDown()
        {
            if (!Input.GetMouseButtonDown(0) || isTriggered || acikKapi == null || kapaliKapi == null)
                return;

            isTriggered = true;

            // Acik kapi fade out
            acikKapi.DOFade(0f, fadeDuration).SetEase(Ease.InOutQuad);

            // Kapali kapi fade in
            kapaliKapi.DOFade(kapaliKapiStartAlpha, fadeDuration).SetEase(Ease.InOutQuad);
        }

        void OnDisable()
        {
            // Tween'leri iptal et ve alpha'lari sifirla

            if (acikKapi != null)
            {
                DOTween.Kill(acikKapi.transform);
                Color c = acikKapi.color;
                c.a = acikKapiStartAlpha;
                acikKapi.color = c;
            }

            if (kapaliKapi != null)
            {
                DOTween.Kill(kapaliKapi.transform);
                Color c = kapaliKapi.color;
                c.a = 0f; // sahneye giriste fade out olacak
                kapaliKapi.color = c;
            }

            isTriggered = false;
        }
    }
}
