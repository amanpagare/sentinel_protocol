using UnityEngine;
using UnityEngine.SceneManagement;

namespace SentinelProtocol
{
    public enum GameOutcome { Playing, Won, Lost }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public GameOutcome Outcome { get; private set; } = GameOutcome.Playing;
        public Health PlayerHealth { get; private set; }
        public string StatusMessage { get; private set; } = "Reach the green extraction zone.";

        private void Awake()
        {
            if (Instance && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        public void RegisterPlayer(Health health) => PlayerHealth = health;

        public void Win()
        {
            if (Outcome != GameOutcome.Playing) return;
            Outcome = GameOutcome.Won;
            StatusMessage = "OBJECTIVE COMPLETE — Press R to restart.";
        }

        public void Lose()
        {
            if (Outcome != GameOutcome.Playing) return;
            Outcome = GameOutcome.Lost;
            StatusMessage = "MISSION FAILED — Press R to restart.";
        }

        private void Update()
        {
            if (PlayerHealth && PlayerHealth.IsDead) Lose();
            if (Outcome != GameOutcome.Playing &&
                UnityEngine.InputSystem.Keyboard.current != null &&
                UnityEngine.InputSystem.Keyboard.current.rKey.wasPressedThisFrame)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }
    }
}
