using System.Collections;
using UnityEngine;

namespace Gameplay.PlayerHole
{
    public class PlayerHoleBoosterView : MonoBehaviour
    {
        [SerializeField] private ParticleSystem root;
        [SerializeField] private ParticleSystem spiral1;
        [SerializeField] private ParticleSystem spiral2;
        [SerializeField] private ParticleSystem contour;

        [Header("Booster Inactive Colors")]
        [SerializeField] private Color rootColorInactive;
        [SerializeField] private Color spiral1ColorInactive;
        [SerializeField] private Color spiral2ColorInactive;
        [SerializeField] private Color contourColorInactive;
        
        [Header("Booster Active Colors")]
        [SerializeField] private Color rootColorActive;
        [SerializeField] private Color spiral1ColorActive;
        [SerializeField] private Color spiral2ColorActive;
        [SerializeField] private Color contourColorActive;

        public void Initialize()
        {
            SetStartColor(root, rootColorInactive);
            SetStartColor(spiral1, spiral1ColorInactive);
            SetStartColor(spiral2, spiral2ColorInactive);
            SetStartColor(contour, contourColorInactive);
        }

        public void SetBoosterActive(bool active) => StartCoroutine(SetBoosterActiveCoroutine(active, 0.5f));

        private IEnumerator SetBoosterActiveCoroutine(bool active, float duration)
        {
            float progress = 0;
            float progressScale = 1 / duration;
            Color startRootColor = active ? rootColorInactive : rootColorActive;
            Color startSpiral1Color = active ? spiral1ColorInactive : spiral1ColorActive;
            Color startSpiral2Color = active ? spiral2ColorInactive : spiral2ColorActive;
            Color startContourColor = active ? contourColorInactive : contourColorActive;
            Color rootColor = active ? rootColorActive : rootColorInactive;
            Color spiral1Color = active ? spiral1ColorActive : spiral1ColorInactive;
            Color spiral2Color = active ? spiral2ColorActive : spiral2ColorInactive;
            Color contourColor = active ? contourColorActive : contourColorInactive;

            do
            {
                progress = Mathf.Clamp01(progress);
                SetStartColor(root, Color.Lerp(startRootColor, rootColor, progress));
                SetStartColor(spiral1, Color.Lerp(startSpiral1Color, spiral1Color, progress));
                SetStartColor(spiral2, Color.Lerp(startSpiral2Color, spiral2Color, progress));
                SetStartColor(contour, Color.Lerp(startContourColor, contourColor, progress));
                progress += Time.deltaTime * progressScale;
                Debug.Log($"Progress: {progress}");
                yield return null;
            } while (progress <= 1);
        }

        private void SetStartColor(ParticleSystem particle, Color color)
        {
            ParticleSystem.ColorOverLifetimeModule module = particle.colorOverLifetime;
            module.color = color;
        }
    }
}