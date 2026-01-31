using System.Collections;
using UnityEngine;



namespace Components.Camera
{
    public class Behaviors : MonoBehaviour
    {
        public void FreezeCamera()
        {
            UnityEngine.Camera.main.transform.SetParent(null, true);
            StopAllCoroutines();
            ShakeCamera(.2f, .5f);
        }


        /// <summary>
        ///  Shakes the camera for a specified duration in seconds.
        /// </summary>
        /// <param name="shakingAmplitude"> The amplitude of the shake effect.</param>
        /// <param name="shakeDuration"> The duration of the shake effect in seconds.</param
        public void ShakeCamera(
            float shakingAmplitude
            , float shakeDuration
        )
        {
            StartCoroutine(_ShakeCamera(shakingAmplitude, shakeDuration));
        }
        private IEnumerator _ShakeCamera(
            float shakingAmplitude
            , float shakeDuration
        )
        {
            Vector3 originalPos = UnityEngine.Camera.main.transform.localPosition;
            float elapsed = 0f;

            while (elapsed < shakeDuration)
            {
                elapsed += Time.unscaledDeltaTime; // game could be paused

                float xOffset = Random.Range(-shakingAmplitude, shakingAmplitude);
                float yOffset = Random.Range(-shakingAmplitude, shakingAmplitude);

                UnityEngine.Camera.main.transform.localPosition = new Vector3(
                    originalPos.x + xOffset
                    , originalPos.y + yOffset
                    , originalPos.z
                );

                yield return null;
            }

            UnityEngine.Camera.main.transform.localPosition = originalPos;
        }
    }
}