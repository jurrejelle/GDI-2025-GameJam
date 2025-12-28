using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class CreditsButton : MonoBehaviour
    {
        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(OnButtonClick);
        }

        private void OnDestroy()
        {
            button.onClick.RemoveListener(OnButtonClick);
        }

        private static void OnButtonClick()
        {
            SceneManager.LoadScene("Credits");
        }
    }
}