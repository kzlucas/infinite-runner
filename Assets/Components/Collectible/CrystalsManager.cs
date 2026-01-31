

using Components.Audio.Scripts;
using Components.Stats;
using Components.UI.Scripts;
using Components.Tutorials;
using UnityEngine;
using WorldGenerator.Scripts;


namespace Components.Collectible
{
    public static class CrystalsManager
    {

        [Header("Dependencies")]
        private static UiRegistry UiRegistry => ServiceLocator.Scripts.ServiceLocator.Get<UiRegistry>();
        private static TutorialManager TutorialManager => ServiceLocator.Scripts.ServiceLocator.Get<TutorialManager>();


        [Header("Crystals Data")]
        public static int crystalsCollected = 0;
        public static int amountInBucket = 0;
        public static float bucketFillPct = 0f;



        public static void Reset()
        {
            crystalsCollected = 0;
            amountInBucket = 0;
            ClearBucket();
        }


        /// <summary>
        ///   Add crystals to the bucket
        /// </summary>
        /// <param name="amount">Amount to add (usually 1)</param>
        public static void AddCrystals(int amount)
        {
            amountInBucket += amount;
            bucketFillPct = amountInBucket / (float)BiomesDataManager.Instance.current.CrystalsNeeded;
            bucketFillPct = Mathf.Clamp01(bucketFillPct);

            // Check if bucket is full
            if (bucketFillPct >= 1f)
            {
                // Change to next biome
                TutorialManager.tutorialsCompleted = true;
                BiomesDataManager.Instance.CycleToNextBiome();
                AudioManager.Instance.PlaySound("biome-change");
                ClearBucket();
            }

            // Update HUD
            UiRegistry.Instance.Hud.UpdateCrystalsBucket(bucketFillPct);
            crystalsCollected += amount;
            StatsRecorder.SetMaxCoinsCollected(crystalsCollected);
        }


        /// <summary>
        ///  Clear crystals bucket
        /// </summary>
        private static void ClearBucket()
        {
            bucketFillPct = 0f;
            amountInBucket = 0;
            UiRegistry.Instance.Hud.UpdateCrystalsBucket(bucketFillPct);
        }



    }
}
