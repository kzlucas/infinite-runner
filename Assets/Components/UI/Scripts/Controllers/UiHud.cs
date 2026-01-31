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
        [HideInInspector] public VisualElement bucketContainer;
        [HideInInspector] public VisualElement bucketFill;

        [HideInInspector] public VisualElement hpContainer;
        [HideInInspector] public VisualElement hpFill;
        public List<Sprite> hpFillSprites;


        [HideInInspector] public Label bucketCounter;
        private int bucketFullPxWidth = 239;


        public override void OnDocReady()
        {
            StartCoroutine(_OnDocReady());
        }

        private IEnumerator _OnDocReady()
        {
            bucketContainer = root.Q<VisualElement>("bucket-container");
            bucketFill = root.Q<VisualElement>("bucket");
            bucketCounter = root.Q<Label>("bucket-counter");

            hpContainer = root.Q<VisualElement>("hp-container");
            hpFill = root.Q<VisualElement>("hp");

            UpdateHp(1f);

            yield return null;
        }


        /// <summary>
        /// Updates the HP UI fill percentage.
        /// </summary>
        /// <param name="fillPct"></param>
        public void UpdateHp(float fillPct)
        {
            var index = Mathf.Clamp(Mathf.FloorToInt(fillPct * hpFillSprites.Count), 0, hpFillSprites.Count - 1);
            var hpFillSprite = hpFillSprites[index];

            hpFill.style.backgroundImage = new StyleBackground(hpFillSprite);
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
            yield return new WaitUntil(() => docReady && bucketFill != null);
            bucketFill.style.width = fillPct * (float)bucketFullPxWidth;
            bucketCounter.text = Mathf.RoundToInt(fillPct * 100f).ToString() + "%";

            CrystalsBucketColor(
                BiomesDataManager.Instance.current.crystalColor,
                BiomesDataManager.Instance.current.GaugeImage
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
            yield return new WaitUntil(() => docReady && bucketFill != null && bucketContainer != null);
            bucketFill.style.unityBackgroundImageTintColor = color;
            bucketContainer.style.backgroundImage = new StyleBackground(gaugeImage);

        }
    }
}
