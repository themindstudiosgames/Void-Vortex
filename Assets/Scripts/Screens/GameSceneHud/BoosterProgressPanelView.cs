using Sounds;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Screens.GameSceneHud
{
    public class BoosterProgressPanelView : MonoBehaviour
    {
        [SerializeField] private Slider progressBar;
        [SerializeField] private TextMeshProUGUI leftTimeText;

        public void SetProgress(float progress) => progressBar.value = progress;

        public void SetBoosterActiveStatus(bool active) => leftTimeText.gameObject.SetActive(active);

        public void SetLeftTimeText(int leftTime) => leftTimeText.text = $"{leftTime}s";
    }
}