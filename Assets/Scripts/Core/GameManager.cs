using System;
using UnityEngine;

namespace SeaOfLegends.Gameplay.Core
{
    /// <summary>
    /// Owns session lifecycle only: initialize, play, pause, resume, complete, and fail.
    /// Does not own content, input, or world systems.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [Header("Lifecycle")]
        [Tooltip("State applied when the manager first wakes.")]
        [SerializeField] private GameState initialState = GameState.Initializing;

        private GameState _currentState;

        /// <summary>
        /// Current session lifecycle state.
        /// </summary>
        public GameState CurrentState => _currentState;

        /// <summary>
        /// True while the session is actively playing and not paused.
        /// </summary>
        public bool IsPlaying => _currentState == GameState.Playing;

        /// <summary>
        /// True while the session is paused.
        /// </summary>
        public bool IsPaused => _currentState == GameState.Paused;

        /// <summary>
        /// Fired after a successful state transition. Arguments: previous state, new state.
        /// </summary>
        public event Action<GameState, GameState> OnStateChanged;

        private void Awake()
        {
            _currentState = initialState;
            ApplyTimeScale(_currentState);
        }

        /// <summary>
        /// Prepares a new session. Valid from any non-playing state except while already initializing.
        /// </summary>
        public bool InitializeGame()
        {
            return TrySetState(GameState.Initializing);
        }

        /// <summary>
        /// Starts or resumes active play from Initializing.
        /// </summary>
        public bool StartGame()
        {
            if (_currentState != GameState.Initializing)
            {
                Debug.LogWarning($"[GameManager] StartGame ignored from state '{_currentState}'.", this);
                return false;
            }

            return TrySetState(GameState.Playing);
        }

        /// <summary>
        /// Pauses an active session.
        /// </summary>
        public bool PauseGame()
        {
            if (_currentState != GameState.Playing)
            {
                Debug.LogWarning($"[GameManager] PauseGame ignored from state '{_currentState}'.", this);
                return false;
            }

            return TrySetState(GameState.Paused);
        }

        /// <summary>
        /// Resumes a paused session.
        /// </summary>
        public bool ResumeGame()
        {
            if (_currentState != GameState.Paused)
            {
                Debug.LogWarning($"[GameManager] ResumeGame ignored from state '{_currentState}'.", this);
                return false;
            }

            return TrySetState(GameState.Playing);
        }

        /// <summary>
        /// Marks the session as successfully completed.
        /// </summary>
        public bool CompleteGame()
        {
            if (_currentState != GameState.Playing && _currentState != GameState.Paused)
            {
                Debug.LogWarning($"[GameManager] CompleteGame ignored from state '{_currentState}'.", this);
                return false;
            }

            return TrySetState(GameState.Completed);
        }

        /// <summary>
        /// Marks the session as failed.
        /// </summary>
        public bool GameOver()
        {
            if (_currentState != GameState.Playing && _currentState != GameState.Paused)
            {
                Debug.LogWarning($"[GameManager] GameOver ignored from state '{_currentState}'.", this);
                return false;
            }

            return TrySetState(GameState.GameOver);
        }

        private bool TrySetState(GameState nextState)
        {
            if (_currentState == nextState)
            {
                return false;
            }

            GameState previousState = _currentState;
            _currentState = nextState;
            ApplyTimeScale(nextState);
            OnStateChanged?.Invoke(previousState, nextState);
            return true;
        }

        private static void ApplyTimeScale(GameState state)
        {
            Time.timeScale = state == GameState.Paused ? 0f : 1f;
        }

        private void OnDestroy()
        {
            if (Time.timeScale != 1f)
            {
                Time.timeScale = 1f;
            }
        }
    }
}
