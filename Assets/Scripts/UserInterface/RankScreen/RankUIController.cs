using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Innovision.UserInterface.Rank
{
    public class RankUIController : UIController
    {
        #region Events
        public Action<string> difficultyFilterChanged;
        public Action<bool> statusFilterChanged;
        #endregion

        #region Settings
        private const int tableCellHeight = 90;
        #endregion

        #region Parameter
        [SerializeField] private RectTransform _scrollContent;
        [SerializeField] private Scrollbar _scrollBar;
        [SerializeField] private Text _pointerText;
        [SerializeField] private Text _betterThanText;
        [SerializeField] private Text _statusText;
        [SerializeField] private Image _tableBackground;
        #endregion

        #region Properties
        private float _scrollTarget = 0f;
        private float _velocity = 0f;
        private bool _changeScroll;
        #endregion

        #region Getters
        public int TableCellHeight { get { return tableCellHeight; } }
        #endregion

        #region Event Handlers
        public void OnRankScrollBarChanged()
        {
            string result = Mathf.RoundToInt(_scrollBar.value * 100).ToString();
            _pointerText.text = result + '%';
            _betterThanText.text = "YOU ARE BETTER THAN " + _pointerText.text;
        }

        public void OnDifficultyToggleChanged(Toggle filter)
        {
            Text text = filter.GetComponentInChildren<Text>();
            if (filter.isOn)
            {
                text.color = Color.black;
                if (difficultyFilterChanged != null)
                    difficultyFilterChanged(text.text);
            }
            else
                text.color = Color.grey;
        }

        public void OnStatusToggleChanged(Toggle filter)
        {
            if (filter.isOn)
            {
                bool result = string.Equals(filter.gameObject.name, "AllStatus", StringComparison.InvariantCultureIgnoreCase);
                if (statusFilterChanged != null)
                    statusFilterChanged(result);
            }
        }

        public void OnBackButtonClicked()
        {
            Debug.Log("Back to the Challenge Screen!");
        }

        public void OnShareButtonClicked()
        {
            Debug.Log("Share a screenshot!");
        }
        #endregion

        #region Controls
        public void ChangeScrollValue(float target)
        {
            _scrollTarget = target;
            _changeScroll = true;
        }

        public void ChangeTableHeight(int playersCount)
        {
            _tableBackground.rectTransform.sizeDelta = new Vector2(_tableBackground.rectTransform.sizeDelta.x, playersCount * tableCellHeight);
            _scrollContent.sizeDelta = new Vector2(_scrollContent.sizeDelta.x, 850 + playersCount * tableCellHeight);
        }

        public void SetStatus(string text)
        {
            _statusText.text = "Only " + text;
        }
        #endregion

        #region Base Methods
        public override void Init()
        {
            if (!_initialized)
                _initialized = true;
        }

        public override void Cycle()
        {
            if (_changeScroll)  //Smooth movement of graph scroll
            {
                _scrollBar.value = Mathf.SmoothDamp(_scrollBar.value, _scrollTarget, ref _velocity, 0.5f);
                if (Mathf.RoundToInt(_scrollBar.value * 100) == Mathf.RoundToInt(_scrollTarget * 100))
                    _changeScroll = false;
            }
        }

        public override void Destroy() { }
        #endregion
    }
}