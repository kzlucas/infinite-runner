


/// <summary>
/// Interface for UI management
/// </summary>
public interface IUiService
{
    void ShowEndGame();
    void ShowPauseMenu();
    void UpdateCrystalsBucket(float fillPercentage);
}



// /// <summary>
// /// Interface for collision merger services
// /// </summary>
// public interface IColliderMerger
// {
//     void GenerateSquareColliders();
// }

// /// <summary>
// /// Interface for crystal/collectible management
// /// </summary>
// public interface ICrystalsService
// {
//     int CrystalsCollected { get; }
//     int AmountInBucket { get; }
//     float BucketFillPercentage { get; }
    
//     void AddCrystals(int amount);
//     void Reset();
    
//     event System.Action<int> OnCrystalsChanged;
//     event System.Action OnBucketFull;
// }

// /// <summary>
// /// Interface for audio management
// /// </summary>
// public interface IAudioService
// {
//     bool MusicOn { get; set; }
//     bool SfxOn { get; set; }
    
//     void PlaySound(string soundName);
//     void PlayMusic(string musicName);
//     void StopMusic();
// }

// /// <summary>
// /// Interface for scene management
// /// </summary>
// public interface ISceneService
// {
//     string CurrentSceneName { get; }
//     bool IsGameScene();
//     void Load(string sceneName);
//     void ReloadCurrentScene();
    
//     event System.Action OnSceneLoaded;
//     event System.Action OnSceneExit;
// }


// /// <summary>
// /// Interface for player data persistence
// /// </summary>
// public interface IPlayerDataService
// {
//     void Save(string key, string value);
//     string Load(string key);
//     bool HasKey(string key);
// }