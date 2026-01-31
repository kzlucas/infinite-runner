

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
        private static TutorialManager TutorialManager => ServiceLocator.Scripts.ServiceLocator.Get<TutorialManager>();


        [Header("Crystals Data")]
        public static int CrystalsCollected = 0;
        public static int AmountInBucket = 0;
        public static float BucketFillPct = 0f;



        public static void Reset()
        {
            CrystalsCollected = 0;
            AmountInBucket = 0;
            ClearBucket();
        }


        /// <summary>
        ///   Add crystals to the bucket
        /// </summary>
        /// <param name="amount">Amount to add (usually 1)</param>
        public static void AddCrystals(int amount)
        {
            AmountInBucket += amount;
            BucketFillPct = AmountInBucket / (float)BiomesDataManager.Instance.Current.CrystalsNeeded;
            BucketFillPct = Mathf.Clamp01(BucketFillPct);

            // Check if bucket is full
            if (BucketFillPct >= 1f)
            {
                // Change to next biome
                TutorialManager.AllTutorialsCompleted = true;
                BiomesDataManager.Instance.CycleToNextBiome();
                AudioManager.Instance.PlaySound("biome-change");
                ClearBucket();
            }

            // Update HUD
            UiRegistry.Instance.Hud.UpdateCrystalsBucket(BucketFillPct);
            CrystalsCollected += amount;
            StatsRecorder.SetMaxCoinsCollected(CrystalsCollected);
        }


        /// <summary>
        ///  Clear crystals bucket
        /// </summary>
        public static void ClearBucket()
        {
            BucketFillPct = 0f;
            AmountInBucket = 0;
            UiRegistry.Instance.Hud.UpdateCrystalsBucket(BucketFillPct);
        }



    }
}
