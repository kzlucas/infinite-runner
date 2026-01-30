using System.Collections;
using UnityEngine;


namespace WorldGenerator.Scripts.Utils
{

    public class PingPongPlatform : MonoBehaviour
    {
        public Vector3 minPosition;
        public Vector3 maxPosition;
        public float speed = 2f;


        private void Start()
        {
            StartCoroutine(PingPongRoutine());
        }

        /// <summary>
        ///  Each frame, move the platform between towards minPosition
        ///  then when reached, move towards maxPosition, and repeat
        /// </summary>
        private IEnumerator PingPongRoutine()
        {
            float journeyLength = Vector3.Distance(minPosition, maxPosition);
            float startTime = Time.time;


            // Randomize starting position
            startTime -= Random.Range(0f, journeyLength / speed);

            // Randomize initial direction
            bool movingToMax = Random.Range(0f, 1f) > 0.5f;



            while (true)
            {
                float distCovered = (Time.time - startTime) * speed;
                float fractionOfJourney = distCovered / journeyLength;

                if (movingToMax)
                {
                    transform.localPosition = Vector3.Lerp(minPosition, maxPosition, fractionOfJourney);
                }
                else
                {
                    transform.localPosition = Vector3.Lerp(maxPosition, minPosition, fractionOfJourney);
                }

                if (fractionOfJourney >= 1f)
                {
                    // Switch direction
                    movingToMax = !movingToMax;
                    startTime = Time.time;
                }

                yield return null;
            }
        }
    }
}
