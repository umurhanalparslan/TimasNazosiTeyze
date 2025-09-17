using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

namespace NazosiTeyze
{
    public class NazT_CatInteraction : MonoBehaviour
    {
        [Header("Miyavlayan yazi sprite'lari")]
        public List<SpriteRenderer> meowTexts;

        [Header("Goz sprite'i (blink yapacak olan)")]
        public SpriteRenderer eyeSprite;

        [Header("Fade ve blink ayarlari")]
        public float meowFadeInDuration = 0.2f;
        public float meowFadeOutDuration = 0.2f;
        public float meowVisibleTime = 0.6f;
        public float meowDelayMin = 0.5f;
        public float meowDelayMax = 2f;

        public float eyeBlinkIntervalMin = 1.5f;
        public float eyeBlinkIntervalMax = 4f;
        public float eyeBlinkDuration = 0.1f;

        private Dictionary<SpriteRenderer, float> originalAlphas = new Dictionary<SpriteRenderer, float>();
        private List<Tweener> activeTweens = new List<Tweener>();

        private bool started = false;
        private float eyeOriginalAlpha = 1f;

        void Start()
        {
            // Baslangicta meow yazilarini sifirla
            foreach (var s in meowTexts)
            {
                if (s != null)
                {
                    originalAlphas[s] = s.color.a;
                    var c = s.color;
                    c.a = 0f;
                    s.color = c;
                }
            }

            // Goz sprite'ini sifirla
            if (eyeSprite != null)
            {
                eyeOriginalAlpha = eyeSprite.color.a;
                var c = eyeSprite.color;
                c.a = 0f;
                eyeSprite.color = c;
            }
        }

        void OnMouseDown()
        {
            if (!Input.GetMouseButtonDown(0) || started)
                return;

            started = true;

            // Miyav yazilari baslasin
            foreach (var text in meowTexts)
            {
                if (text != null)
                    StartCoroutine(MeowRoutine(text));
            }

            // Goz kirpmasi baslasin
            if (eyeSprite != null)
                StartCoroutine(BlinkRoutine(eyeSprite));
        }

        IEnumerator MeowRoutine(SpriteRenderer text)
        {
            while (true)
            {
                float delay = Random.Range(meowDelayMin, meowDelayMax);
                yield return new WaitForSeconds(delay);

                var fadeIn = text.DOFade(originalAlphas[text], meowFadeInDuration).SetEase(Ease.InOutSine);
                activeTweens.Add(fadeIn);
                yield return fadeIn.WaitForCompletion();

                yield return new WaitForSeconds(meowVisibleTime);

                var fadeOut = text.DOFade(0f, meowFadeOutDuration).SetEase(Ease.InOutSine);
                activeTweens.Add(fadeOut);
                yield return fadeOut.WaitForCompletion();
            }
        }

        IEnumerator BlinkRoutine(SpriteRenderer eye)
        {
            while (true)
            {
                float interval = Random.Range(eyeBlinkIntervalMin, eyeBlinkIntervalMax);
                yield return new WaitForSeconds(interval);

                // Goz acilir
                var open = eye.DOFade(eyeOriginalAlpha, eyeBlinkDuration).SetEase(Ease.Linear);
                activeTweens.Add(open);
                yield return open.WaitForCompletion();

                // Kisa sureli gorunur kalir
                yield return new WaitForSeconds(eyeBlinkDuration);

                // Goz tekrar kaybolur
                var close = eye.DOFade(0f, eyeBlinkDuration).SetEase(Ease.Linear);
                activeTweens.Add(close);
                yield return close.WaitForCompletion();
            }
        }

        void OnDisable()
        {
            // Meow yazilarini sifirla
            foreach (var s in meowTexts)
            {
                if (s != null && originalAlphas.ContainsKey(s))
                {
                    DOTween.Kill(s.transform);
                    var c = s.color;
                    c.a = 0f;
                    s.color = c;
                }
            }

            // Goz sifirla
            if (eyeSprite != null)
            {
                DOTween.Kill(eyeSprite.transform);
                var c = eyeSprite.color;
                c.a = 0f;
                eyeSprite.color = c;
            }

            // Tum aktif tweenleri durdur
            foreach (var t in activeTweens)
            {
                if (t != null && t.IsActive())
                    t.Kill();
            }

            activeTweens.Clear();
            StopAllCoroutines();
            started = false;
        }
    }
}
