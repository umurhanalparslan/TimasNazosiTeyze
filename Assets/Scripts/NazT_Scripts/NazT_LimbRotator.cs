using UnityEngine;
using DG.Tweening;
using System.Collections;

namespace NazosiTeyze
{
    // Bu kod herhangi bir uzvun rotasyon ile hareket etmesini saglar
    public class NazT_LimbRotator : MonoBehaviour
    {
        [Header("Target Limb")]
        public Transform limbTarget;

        [Header("Rotation Settings")]
        public float rotationAngle = 30.0f;
        public float oneWayRotationDuration = 1.0f;
        public Vector3 rotationAxis = Vector3.forward;
        public Ease rotationEase = Ease.InOutSine;
        public float delay = 0f;

        [Header("Loop Duration (Optional)")]
        public bool loopForSpecificDuration = false;
        public float totalLoopDuration = 5.0f;

        [Header("Tutorial Trigger (Optional)")]
        public bool triggerAfterTutorial = false;
        public TutorialItem waitForTutorialItem;

        private Quaternion initialLocalRotation;
        private Sequence currentRotateSequence;
        private Coroutine controlCoroutine;

        void Awake()
        {
            if (limbTarget == null) return;

            initialLocalRotation = limbTarget.localRotation;

            if (rotationAxis.sqrMagnitude == 0)
                rotationAxis = Vector3.forward;
            else
                rotationAxis.Normalize();

            oneWayRotationDuration = Mathf.Max(0.01f, oneWayRotationDuration);
            delay = Mathf.Max(0, delay);
            totalLoopDuration = Mathf.Max(0.01f, totalLoopDuration);
        }

        void OnEnable()
        {
            if (limbTarget == null) return;

            limbTarget.localRotation = initialLocalRotation;
            KillAndClearSequence();
            StopControlCoroutine();

            if (triggerAfterTutorial && waitForTutorialItem != null)
                controlCoroutine = StartCoroutine(WaitForTutorialAndStartRotationLogic());
            else
                controlCoroutine = StartCoroutine(DelayedStartRotationLogic(delay));
        }

        void OnDisable()
        {
            StopControlCoroutine();
            KillAndClearSequence();

            if (limbTarget != null)
                limbTarget.localRotation = initialLocalRotation;
        }

        void KillAndClearSequence()
        {
            if (currentRotateSequence != null && currentRotateSequence.IsActive())
            {
                currentRotateSequence.Kill(false);
            }
            currentRotateSequence = null;
        }

        void StopControlCoroutine()
        {
            if (controlCoroutine != null)
            {
                StopCoroutine(controlCoroutine);
                controlCoroutine = null;
            }
        }

        private IEnumerator DelayedStartRotationLogic(float waitTime)
        {
            if (waitTime > 0)
                yield return new WaitForSeconds(waitTime);

            StartActualRotationSequence();

            if (loopForSpecificDuration && currentRotateSequence != null && currentRotateSequence.IsActive())
            {
                float initialSwingDuration = oneWayRotationDuration / 2.0f;
                yield return new WaitForSeconds(initialSwingDuration);

                if (currentRotateSequence != null && currentRotateSequence.IsActive())
                {
                    yield return new WaitForSeconds(totalLoopDuration);

                    if (currentRotateSequence != null && currentRotateSequence.IsActive())
                    {
                        currentRotateSequence.Kill(false);
                        currentRotateSequence = null;
                    }
                }
            }
            controlCoroutine = null;
        }

        private IEnumerator WaitForTutorialAndStartRotationLogic()
        {
            yield return null;

            if (waitForTutorialItem == null)
            {
                yield return StartCoroutine(DelayedStartRotationLogic(delay));
                yield break;
            }

            if (waitForTutorialItem.IsSkipped)
            {
                yield return StartCoroutine(DelayedStartRotationLogic(delay));
                yield break;
            }

            if (!waitForTutorialItem.IsActive)
                yield return new WaitUntil(() => waitForTutorialItem.IsActive || waitForTutorialItem.IsSkipped);

            if (waitForTutorialItem.IsSkipped)
            {
                yield return StartCoroutine(DelayedStartRotationLogic(delay));
                yield break;
            }

            yield return new WaitUntil(() => !waitForTutorialItem.IsActive || waitForTutorialItem.IsSkipped);
            yield return StartCoroutine(DelayedStartRotationLogic(delay));
        }

        private void StartActualRotationSequence()
        {
            if (limbTarget == null) return;

            KillAndClearSequence();
            limbTarget.localRotation = initialLocalRotation;

            currentRotateSequence = DOTween.Sequence();

            Quaternion targetPositiveRotation = initialLocalRotation * Quaternion.AngleAxis(rotationAngle, rotationAxis);
            Quaternion targetNegativeRotation = initialLocalRotation * Quaternion.AngleAxis(-rotationAngle, rotationAxis);

            currentRotateSequence.Append(limbTarget.DOLocalRotateQuaternion(targetPositiveRotation, oneWayRotationDuration / 2.0f)
                .SetEase(rotationEase));

            Tween yoyoTween = limbTarget.DOLocalRotateQuaternion(targetNegativeRotation, oneWayRotationDuration)
                .SetEase(rotationEase)
                .SetLoops(-1, LoopType.Yoyo);

            currentRotateSequence.Append(yoyoTween);
        }
    }
}
