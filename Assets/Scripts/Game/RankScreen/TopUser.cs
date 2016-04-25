using UnityEngine;
using UnityEngine.UI;
using Innovision.Data;
using Innovision.Helpers;
using System.Collections;
using System.Collections.Generic;

namespace Innovision.Game.Rank
{
    public class TopUser : MonoBehaviour, IPoolItem
    {
        #region Parameters
        [SerializeField] private Image _avatar;
        [SerializeField] private Image _avatarGlow;
        [SerializeField] private Text _index;
        [SerializeField] private Text _name;
        [SerializeField] private Text _score;
        #endregion

        #region Properties
        private PlayerDTO _data;
        #endregion

        #region Getters
        public int Index
        {
            get { return int.Parse(_index.text); }
        }

        public PlayerDTO Data
        {
            get { return _data; }
        }
        #endregion

        #region Controls
        public void SetData(PlayerDTO data, RankSceneController.Difficulty difficulty, bool writeScore = true)
        {
            if (data != null)
                _data = data;
            else
                return;

            _name.text = _data.Name;
            _avatar.sprite = Resources.Load<Sprite>("Avatars/" + _data.Avatar);
            _avatarGlow.sprite = Resources.Load<Sprite>("Avatars/" + _data.Avatar + "_glow");

            UpdateScore(difficulty, writeScore);
        }

        public void UpdateScore(RankSceneController.Difficulty difficulty, bool writeScore)
        {
            _score.text = writeScore ? "Score: " : "";
            if (difficulty == RankSceneController.Difficulty.Easy)
                _score.text += _data.EScore.ToString();
            else if (difficulty == RankSceneController.Difficulty.Medium)
                _score.text += _data.MScore.ToString();
            else if (difficulty == RankSceneController.Difficulty.Hard)
                _score.text += _data.HScore.ToString();
        }

        public void SetIndex(int index)
        {
            _index.text = index.ToString();
        }
        #endregion

        #region Implemented Methods
        public void Init() { }

        public void Activate(Vector3 position)
        {
            transform.position = position;
            gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }
        #endregion
    }
}