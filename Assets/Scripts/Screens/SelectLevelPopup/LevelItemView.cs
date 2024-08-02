using System;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utils;

namespace Screens.SelectLevelPopup
{
    public class LevelItemView : MonoBehaviour
    {
        [SerializeField] private Color pressedTextColor;
        [SerializeField] private Color normalTextColor;
        [SerializeField] private Color selectedTextColor;
        
        [SerializeField] private Material pressedTextMaterial;
        [SerializeField] private Material normalTextMaterial;
        [SerializeField] private Material selectedTextMaterial;
        
        [SerializeField] private Sprite normalBackground;
        [SerializeField] private Sprite lockedBackground;
        [SerializeField] private Sprite selectedBackground;

        [SerializeField] private Button button;

        public IObservable<Unit> OnClick;
        
        [SerializeField] private GameObject LockIcon;
        [SerializeField] private TMP_Text IndexText;
        [SerializeField] private Image BackgroundImage;

        private Material prevTextMaterial;
        private Color prevTextColor;

        private void Awake()
        {
            OnClick = button.OnClickWithThrottle();

            prevTextMaterial = normalTextMaterial;
            prevTextColor = normalTextColor;
            
            button.OnPointerDownAsObservable().Subscribe(_ =>
            {
                IndexText.fontMaterial = pressedTextMaterial;
                IndexText.color = pressedTextColor;
            }).AddTo(this);
            button.OnDeselectAsObservable().Subscribe(_ =>
            {
                IndexText.fontMaterial = prevTextMaterial;
                IndexText.color = prevTextColor;
            }).AddTo(this);
        }

        public void SetupView(int levelIndex, bool unlocked, bool selected = false)
        {
            if(LockIcon) LockIcon.SetActive(!unlocked);
            
            IndexText.enabled = unlocked;
            button.enabled = unlocked;
            BackgroundImage.sprite = unlocked 
                ? selected ? selectedBackground : normalBackground  
                : lockedBackground;
            
            if (unlocked)
            {
                IndexText.SetText($"{levelIndex}");
                IndexText.fontMaterial = normalTextMaterial;
                IndexText.color = normalTextColor;
                
                prevTextMaterial = normalTextMaterial;
                prevTextColor = normalTextColor;
            }

            if (selected)
            {
                IndexText.SetText($"{levelIndex}");
                IndexText.fontMaterial = selectedTextMaterial;
                IndexText.color = selectedTextColor;

                prevTextMaterial = selectedTextMaterial;
                prevTextColor = selectedTextColor;
            }
        }
    }
}