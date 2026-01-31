
using UnityEngine;

public class TimeScaleManager : Singleton.Model<TimeScaleManager>
{
    public float InitialTimeScale = 1.3f;

    /// <summary> Flag indicating whether the game is currently paused. </summary>
    private bool _isPaused = false;

    /// <summary> Current time scale of the game. </summary>
    public float _currentTimeScale; 

    /// <summary> Maximum time scale the game can reach. </summary>
    private float _maxTimeScale = 2f; 

    /// <summary> Minimum time scale the game can have. </summary>
    private float _minTimeScale; 

    /// <summary> Scaled time elapsed since the start of the game. </summary>
    private float _timeElapsedFactor = 0f;


    /// <summary>
    ///     On scene loaded, reset the time elapsed counter.
    /// </summary>
    private void Start()
    {
        SceneLoader.Instance.OnSceneLoaded += () =>
        {
            _timeElapsedFactor = 0f;
            _currentTimeScale = InitialTimeScale;
            _minTimeScale = InitialTimeScale;
        };
    }


    /// <summary>
    ///     On update, gradually increase the time scale if the game is not paused.
    /// </summary>
    private void Update()
    {
        if (_isPaused) return;
        
        // gradually increase time scale over time. 400 unity seconds to reach 2x speed
        _timeElapsedFactor += Time.deltaTime;
        _currentTimeScale = _minTimeScale + (_timeElapsedFactor / 400f); 
        _currentTimeScale = Mathf.Clamp(_currentTimeScale, 0f, _maxTimeScale);
        Time.timeScale = _currentTimeScale;


        // gradually restore MinTimeScale
        if(_minTimeScale < InitialTimeScale)
        {
            _minTimeScale = _minTimeScale + (Time.deltaTime / 50f); 
        }
    }


    /// <summary>
    ///     Slows down time by setting a lower minimum time scale.
    /// </summary>
    public void SlowDownTime()
    {
        Debug.Log("[TimeScaleManager] Slowing down time");
        _minTimeScale = 1f;

        // reduce the elapsed time to make the slowdown more pronounced
        // and reward player if he collects multiple hourglasses
        _timeElapsedFactor = _timeElapsedFactor * .7f; 
    }


    /// <summary>
    ///     Pauses the game by setting time scale to zero.
    /// </summary>
    public void PauseGame()
    {
        Debug.Log("[TimeScaleManager] Pausing game");
        _isPaused = true;
        Time.timeScale = 0f;
    }

    /// <summary>
    ///     Resumes the game by restoring time scale to one.
    /// </summary>
    public void ResumeGame()
    {
        Debug.Log("[TimeScaleManager] Resuming game");
        _isPaused = false;
        Time.timeScale = _currentTimeScale;
    }

}