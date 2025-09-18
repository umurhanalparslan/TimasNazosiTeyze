using UnityEngine;

namespace NazosiTeyze
{
    // Sahnedeki bir objeye tiklayinca memory oyunu resetlenir
    public class NazT_MemoryResetTrigger : MonoBehaviour
    {
        void OnMouseDown()
        {
            if (!Input.GetMouseButtonDown(0)) return;

            if (NazT_MemoryGameManager.Instance != null)
            {
                NazT_MemoryGameManager.Instance.RestartGame();

                // SES: restart objesine  tiklandi
            }
        }
    }
}
