using UnityEngine;
using DG.Tweening;

namespace NazosiTeyze
{
    public class NazT_Hourglass : MonoBehaviour
    {
        public Transform hourglassObj;         // Kum saati sprite
        public float rotateDuration = 1f;      // Donme suresi
        public float waitDuration = 0.5f;      // Donduktan sonra bekleme suresi
        public int rotateCount = 1;            // Kac kez donecek (1 tam tur)

        private Quaternion startRotation;
        private bool isStarted = false;
        private Sequence rotateSequence;

        void Start()
        {
            if (hourglassObj != null)
                startRotation = hourglassObj.localRotation;
        }

        void OnMouseDown()
        {
            if (!Input.GetMouseButtonDown(0) || isStarted || hourglassObj == null)
                return;

            isStarted = true;

            rotateSequence = DOTween.Sequence();

            rotateSequence.Append(hourglassObj.DOLocalRotate(
                new Vector3(0f, 0f, -360f * rotateCount),
                rotateDuration,
                RotateMode.FastBeyond360
            ).SetEase(Ease.InOutSine));

            rotateSequence.AppendInterval(waitDuration); // Bekleme

            rotateSequence.SetLoops(-1, LoopType.Restart); // Sonsuz tekrar
        }

        void OnDisable()
        {
            if (rotateSequence != null)
                rotateSequence.Kill();

            if (hourglassObj != null)
            {
                DOTween.Kill(hourglassObj);
                hourglassObj.localRotation = startRotation;
            }

            isStarted = false;
        }
    }
}
