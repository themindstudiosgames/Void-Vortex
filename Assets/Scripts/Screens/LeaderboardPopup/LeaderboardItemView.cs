using System;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Screens.LeaderboardPopup
{
    public class LeaderboardItemView : MonoBehaviour, IPoolable<LeaderboardItemViewInfo, IMemoryPool>, IDisposable
    {
        [SerializeField] private TMP_Text nicknameText;
        [SerializeField] private TMP_Text defaultPositionText;
        [SerializeField] private TMP_Text topPositionText;
        [SerializeField] private TMP_Text scoreText;
        
        [SerializeField] private Image cupImage;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image scoreTextBackground;

        [Space] [SerializeField] private TopPositionConfig[] topPositionConfigs;
        
        [SerializeField] private Color defaultBackgroundColor;
        [SerializeField] private Color defaultScoreBackgroundColor;
        
        [Header("Optional(only for local user panel)")]
        [SerializeField] private Image outlineImage;
        [SerializeField] private Color defaultOutlineColor;
        [SerializeField] private Color defaultPlayerBackgroundColor;
        [SerializeField] private Color defaultPlayerScoreBackgroundColor;
        
        private IMemoryPool _pool;
        
        public void Dispose()
        {
            _pool?.Despawn(this);
        }

        public void OnDespawned()
        {
            _pool = null;
        }
        
        public void OnSpawned(LeaderboardItemViewInfo viewInfo, IMemoryPool pool)
        {
            _pool = pool;

            transform.SetParent(viewInfo.Parent);

            Vector3 position = transform.position;
            position.z = 0f;
            transform.position = position;
            transform.localScale = Vector3.one;

            SetupViewEntryInfo(viewInfo.Entry);
        }
        
        public void SetupViewEntryInfo(PlayerLeaderboardEntry leaderboardEntry)
        {
            nicknameText.text = leaderboardEntry.DisplayName;
            scoreText.text = leaderboardEntry.StatValue.ToString();

            int positionIndex = leaderboardEntry.Position; 
            if (positionIndex < 0)
            {
                Debug.LogError($"Invalid Position {leaderboardEntry.Position} for user {leaderboardEntry.DisplayName} | {leaderboardEntry.StatValue}");
                return;
            }

            bool isTopPosition = positionIndex < topPositionConfigs.Length;
            
            cupImage.gameObject.SetActive(isTopPosition);
            topPositionText.gameObject.SetActive(isTopPosition);
            defaultPositionText.gameObject.SetActive(!isTopPosition);
            
            (isTopPosition ? topPositionText : defaultPositionText).text = $"#{leaderboardEntry.Position + 1}";
            
            if (isTopPosition)
            {
                cupImage.sprite = topPositionConfigs[positionIndex].cupSprite;
                backgroundImage.color = topPositionConfigs[positionIndex].backgroundColor;
                scoreTextBackground.color = topPositionConfigs[positionIndex].scoreBackgroundColor;
                if (outlineImage)
                {
                    outlineImage.color = topPositionConfigs[positionIndex].outlineColor;
                }
            }
            else
            {
                backgroundImage.color = defaultBackgroundColor;
                scoreTextBackground.color = defaultScoreBackgroundColor;
                if (outlineImage)
                {
                    backgroundImage.color = defaultPlayerBackgroundColor;
                    scoreTextBackground.color = defaultPlayerScoreBackgroundColor;
                    outlineImage.color = defaultOutlineColor;
                }
            }
        }
    }
}