using UnityEngine;
using UnityEngine.InputSystem;

namespace SentinelProtocol
{
    public class HUDController : MonoBehaviour
    {
        [SerializeField] private bool showDebug = false;
        private GUIStyle titleStyle, bodyStyle, debugStyle;

        private void Update()
        {
            if (Keyboard.current != null && Keyboard.current.tabKey.wasPressedThisFrame)
                showDebug = !showDebug;
        }

        private void OnGUI()
        {
            if (titleStyle == null)
            {
                titleStyle = new GUIStyle(GUI.skin.label) { fontSize = 22, fontStyle = FontStyle.Bold };
                bodyStyle = new GUIStyle(GUI.skin.label) { fontSize = 15 };
                debugStyle = new GUIStyle(GUI.skin.label) { fontSize = 13 };
                titleStyle.normal.textColor = Color.white;
                bodyStyle.normal.textColor = Color.white;
                debugStyle.normal.textColor = Color.cyan;
            }

            Health hp = GameManager.Instance ? GameManager.Instance.PlayerHealth : null;
            GUI.Label(new Rect(18, 14, 500, 35), "SENTINEL PROTOCOL", titleStyle);
            GUI.Label(new Rect(18, 48, 600, 25),
                $"Health: {(hp ? hp.CurrentHealth.ToString("0") : "--")}   |   WASD Move  •  Shift Sprint  •  Space Jump", bodyStyle);
            GUI.Label(new Rect(18, 76, 700, 25),
                GameManager.Instance ? GameManager.Instance.StatusMessage : "Find the green extraction zone.", bodyStyle);
            GUI.Label(new Rect(18, 104, 500, 22), "TAB: Toggle AI debug overlay", bodyStyle);

            if (!showDebug) return;
            float y = 140f;
            foreach (SentinelAI ai in FindObjectsByType<SentinelAI>(FindObjectsSortMode.None))
            {
                string line = $"{ai.name} | State: {ai.State} | Target visible: {(ai.Sensor && ai.Sensor.CanSeeTarget() ? "YES" : "NO")}";
                GUI.Label(new Rect(18, y, 700, 22), line, debugStyle);
                y += 22f;
            }
        }
    }
}
