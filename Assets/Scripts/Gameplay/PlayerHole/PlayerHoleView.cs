using System;
using UnityEngine;

namespace Gameplay.PlayerHole
{
    public class PlayerHoleView : MonoBehaviour
    {
        [field: SerializeField] public Collider ItemCollectorTrigger { private set; get; }
        [field: SerializeField] public Transform HoleRoot { private set; get; }
        [field: SerializeField] public Transform ScoreFromToAnimationPoint { private set; get; }
        [field: SerializeField] public CollectableItemAbsorber CollectableItemAbsorber { private set; get; }

        [SerializeField] private PlayerHoleBoosterView boosterView;

        private void Start() => boosterView.Initialize();

        public void SetBoosterActive(bool active) => boosterView.SetBoosterActive(active);
    }
}