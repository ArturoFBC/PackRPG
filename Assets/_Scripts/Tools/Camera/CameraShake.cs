using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private Transform shakeTarget;

    [SerializeField] private float shakeAmount = .3f;
    [SerializeField] private float duration =1;
    [SerializeField] private AnimationCurve amountDuringTime;

    private void Awake()
    {
        if (shakeTarget == null)
            shakeTarget = transform;
    }

    public void Shake()
    {
        StartCoroutine(ShakeCoroutine());
    }

    private void OnEnable()
    {
        Shake();
    }

    private IEnumerator ShakeCoroutine()
    {
        float timeCounter = 0;

        while (timeCounter < duration)
        {
            yield return new WaitForEndOfFrame();
            timeCounter += Time.deltaTime;

            float currentShake = amountDuringTime.Evaluate(timeCounter / duration) * shakeAmount;

            float deltaX = (Random.value * currentShake) - (currentShake / 2);
            float deltaY = (Random.value * currentShake) - (currentShake / 2);

            Vector3 deltaPosition = new Vector3 (deltaX, deltaY, 0);

            shakeTarget.localPosition = deltaPosition;
        }

        shakeTarget.localPosition = Vector3.zero;
    }
}
