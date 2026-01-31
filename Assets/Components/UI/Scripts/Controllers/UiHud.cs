using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using WorldGenerator.Scripts;

namespace Components.UI.Scripts.Controllers
{

    [RequireComponent(typeof(UIDocument))]
    public class UiHud : BaseClasses.UiController
    {
        [HideInInspector] public VisualElement BucketContainer;
        [HideInInspector] public VisualElement BucketFill;

        [HideInInspector] public VisualElement HpContainer;
        [HideInInspector] public VisualElement HpFill;
        public List<Sprite> HpFillSprites;


        [HideInInspector] public Label BucketCounter;
        private int _bucketFullPxWidth = 239;


        public override void OnDocReady()
        {
            StartCoroutine(_OnDocReady());
        }

        private IEnumerator _OnDocReady()
        {
            BucketContainer = root.Q<VisualElement>("bucket-container");
            BucketFill = root.Q<VisualElement>("bucket");
            BucketCounter = root.Q<Label>("bucket-counter");

            HpContainer = root.Q<VisualElement>("hp-container");
            HpFill = root.Q<VisualElement>("hp");

            UpdateHp(1f);

            yield return null;
        }


        /// <summary>
        /// Updates the HP UI fill percentage.
        /// </summary>
        /// <param name="fillPct"></param>
        public void UpdateHp(float fillPct)
        {
            var index = Mathf.Clamp(Mathf.FloorToInt(fillPct * HpFillSprites.Count), 0, HpFillSprites.Count - 1);
            var hpFillSprite = HpFillSprites[index];

            HpFill.style.backgroundImage = new StyleBackground(hpFillSprite);
        }



        /// <summary>
        /// Updates the crystals bucket UI fill percentage. 
        /// </summary>
        /// <param name="fillPct"></param>
        public void UpdateCrystalsBucket(float fillPct)
        {
            if (this == null) return;
            StartCoroutine(_UpdateCrystalsBucket(fillPct));
        }

        private IEnumerator _UpdateCrystalsBucket(float fillPct)
        {
            yield return new WaitUntil(() => DocReady && BucketFill != null);
            BucketFill.style.width = fillPct * (float)_bucketFullPxWidth;
            BucketCounter.text = Mathf.RoundToInt(fillPct * 100f).ToString() + "%";

            CrystalsBucketColor(
                BiomesDataManager.Instance.Current.crystalColor,
                BiomesDataManager.Instance.Current.GaugeImage
            );
        }

        /// <summary>
        /// Sets the crystals bucket color and gauge image.
        /// </summary>
        /// <param name="color"></param>
        /// <param name="gaugeImage"></param>

        private void CrystalsBucketColor(Color color, Sprite gaugeImage)
        {
            if (this == null) return;
            StartCoroutine(_CrystalsBucketColor(color, gaugeImage));
        }
        private IEnumerator _CrystalsBucketColor(Color color, Sprite gaugeImage)
        {
            yield return new WaitUntil(() => DocReady && BucketFill != null && BucketContainer != null);
            BucketFill.style.unityBackgroundImageTintColor = color;
            BucketContainer.style.backgroundImage = new StyleBackground(gaugeImage);

        }
    }
}
