using UnityEngine;
using UnityEngine.UI;

namespace Screens.SelectLevelPopup
{
    public class PageListView : MonoBehaviour
    {
        [SerializeField] private Sprite unselectedSprite;
        [SerializeField] private Sprite selectedSprite;

        [SerializeField] private Color unselectedColor;
        [SerializeField] private Color selectedColor;

        [field: SerializeField] public Button[] PageButtonsArray { get; private set; }

        private Button _activePage;

        public void InitializePageList(int pageCount)
        {
            if (pageCount >= PageButtonsArray.Length)
            {
                Debug.LogError($"requested page count is exceeding max pool capacity");
                return;
            }
            
            for (int i = 0; i < PageButtonsArray.Length; i++)
            {
                PageButtonsArray[i].gameObject.SetActive(i < pageCount);
            }
        }

        public void SetPageActiveByIndex(int page)
        {
            if (_activePage != null)
            {
                _activePage.image.sprite = unselectedSprite;
                _activePage.image.color = unselectedColor;
            }

            _activePage = PageButtonsArray[page];
            _activePage.image.sprite = selectedSprite;
            _activePage.image.color = selectedColor;
        }
    }
}