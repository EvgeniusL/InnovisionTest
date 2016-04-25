using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Innovision;
using Innovision.Data;
using Innovision.Helpers;
using Innovision.UserInterface.Rank;

namespace Innovision.Game.Rank
{
    public class RankSceneController : SceneController
    {
        #region Parameters
        [SerializeField] private TopUser _mainUser;
        [SerializeField] private RankUIController _ui;
        [SerializeField] private Transform _usersContainer;
        #endregion

        #region Settings
        public enum Difficulty
        {
            Easy,
            Medium,
            Hard
        }

        private const int hoursToUpdateData = 8;
        private const int topCount = 10;
        private const int playersCount = 100;
        #endregion

        #region Properties
        private PoolManager<TopUser> _usersPool;
        private List<PlayerDTO> _players;
        private List<PlayerDTO> _sameStatusPlayers;
        private List<PlayerDTO> _currentList;
        private Difficulty _currentDifficulty = Difficulty.Easy;
        #endregion

        #region Controls
        private void GetMainUserFromServer()
        {
            System.Array array = System.Enum.GetValues(typeof(PlayerDTO.Avatars));
            PlayerDTO.Avatars avatar = (PlayerDTO.Avatars)array.GetValue(Random.Range(0, array.Length));
            array = System.Enum.GetValues(typeof(PlayerDTO.Statuses));
            PlayerDTO.Statuses status = (PlayerDTO.Statuses)array.GetValue(Random.Range(0, array.Length));
            _ui.SetStatus(status.ToString());
            _mainUser.SetData(new PlayerDTO("Evgenius", Random.Range(0, 30000), Random.Range(0, 25000), Random.Range(0, 20000), avatar.ToString(), status.ToString()), _currentDifficulty, false);
        }

        private void GetDataFromServer(int count)
        {
            //Simulation of getting top players data
            for (int i = 0; i < count; i++)
            {
                System.Array array = System.Enum.GetValues(typeof(PlayerDTO.Avatars));
                PlayerDTO.Avatars avatar = (PlayerDTO.Avatars)array.GetValue(Random.Range(0, array.Length));
                array = System.Enum.GetValues(typeof(PlayerDTO.Statuses));
                PlayerDTO.Statuses status = (PlayerDTO.Statuses)array.GetValue(Random.Range(0, array.Length));
                array = System.Enum.GetValues(typeof(PlayerDTO.Names));
                PlayerDTO.Names nickname = (PlayerDTO.Names)array.GetValue(Random.Range(0, array.Length));

                _players.Add(new PlayerDTO(nickname.ToString(), Random.Range(0, 20000), Random.Range(0, 15000), Random.Range(0, 10000), avatar.ToString(), status.ToString()));
                if (status.ToString() == _mainUser.Data.Status)
                    _sameStatusPlayers.Add(_players[i]);
            }
            _players.Add(_mainUser.Data);

            _currentList = _players;
            SortPlayers();
            SetDataToUsers();
            SaveUsers();
            Initialize();
        }

        private void SetDataToUsers()
        {
            TopUser user = null;
            RectTransform rect = null;
            _usersPool.ReturnAllItemsToPool();
            for (int i = 0; i < topCount; i++)
            {
                user = _usersPool.GetItemFromPool();
                if (user == null)
                    return;
                user.SetData(_currentList[i], _currentDifficulty, true);
                user.SetIndex(i + 1);
                rect = user.GetComponent<RectTransform>();
                rect.localPosition = new Vector3(0, rect.localPosition.y - (_ui.TableCellHeight * (user.Index - 1)), 0);
                user.Activate(user.transform.position);
            }
            _ui.ChangeTableHeight(_currentList.Count > topCount ? topCount : _currentList.Count);
            _ui.ChangeScrollValue(CalculateScrollValue());
        }

        private void SaveUsers()
        {
            _players.Remove(_mainUser.Data);
            _players.Insert(0, _mainUser.Data);

            string[] users = new string[_players.Count];
            for (int i = 0; i < _players.Count; i++)
                users[i] = JsonUtility.ToJson(_players[i]);

            DataManager.Instance.SaveUsers(users, OnUsersSaved);
        }

        private void OnUsersSaved()
        {
            Debug.Log("Saved!");
        }

        private void LoadUsers()
        {
            DataManager.Instance.LoadUsers(OnUsersLoaded);
        }

        private void OnUsersLoaded(string[] users)
        {
            _mainUser.SetData(JsonUtility.FromJson<PlayerDTO>(users[0]), _currentDifficulty, false);
            _sameStatusPlayers.Clear();
            _players.Clear();

            _players.Add(_mainUser.Data);
            _sameStatusPlayers.Add(_mainUser.Data);
            for (int i = 1; i < users.Length; i++)
            {
                _players.Add(JsonUtility.FromJson<PlayerDTO>(users[i]));

                if (string.Equals(_players[i].Status, _mainUser.Data.Status))
                    _sameStatusPlayers.Add(_players[i]);
            }

            _currentList = _players;
            SortPlayers();
            SetDataToUsers();
            Initialize();
        }

        private float CalculateScrollValue()
        {
            _mainUser.SetIndex(_currentList.IndexOf(_mainUser.Data) + 1);
            if (_mainUser.Index == 1)
                return 1f;
            else if (_mainUser.Index == _currentList.Count)
                return 0f;
            else
            {
                float result = 100 - (float)((float)_mainUser.Index / (float)((float)_currentList.Count / 100));
                return result / 100;
            }
        }

        private void SortPlayers()
        {
            if (_currentDifficulty == Difficulty.Easy)
                _currentList.Sort(PlayerDTO.CompareByEasyScore);
            else if (_currentDifficulty == Difficulty.Medium)
                _currentList.Sort(PlayerDTO.CompareByMediumScore);
            else if (_currentDifficulty == Difficulty.Hard)
                _currentList.Sort(PlayerDTO.CompareByHardScore);
        }
        #endregion

        #region Event Handlers
        private void CreateEventListeners()
        {
            _ui.difficultyFilterChanged += OnDifficultyFilterChanged;
            _ui.statusFilterChanged += OnStatusFilterChanged;
        }

        private void RemoveEventListeners()
        {
            _ui.difficultyFilterChanged -= OnDifficultyFilterChanged;
            _ui.statusFilterChanged -= OnStatusFilterChanged;
        }

        private void OnDifficultyFilterChanged(string text)
        {
            Difficulty curDiff = (Difficulty)System.Enum.Parse(typeof(Difficulty), text);
            if (curDiff == _currentDifficulty)
                return;
            _currentDifficulty = curDiff;

            SortPlayers();

            for (int i = 0; i < topCount && i < _currentList.Count; i++)
            {
                _usersPool.Active[i].SetData(_currentList[i], _currentDifficulty);
                _usersPool.Active[i].SetIndex(i + 1);
            }
            _mainUser.UpdateScore(_currentDifficulty, false);
            _ui.ChangeScrollValue(CalculateScrollValue());
        }

        private void OnStatusFilterChanged(bool all)
        {
            _currentList = all ? _players : _sameStatusPlayers;
            SortPlayers();

            for (int i = 0; i < topCount; i++)
            {
                if (i < _currentList.Count)
                {
                    if (i >= _usersPool.Active.Count)
                    {
                        TopUser newUser = _usersPool.GetItemFromPool();
                        if (newUser == null)
                            return;
                        newUser.Activate(newUser.transform.position);
                    }
                    _usersPool.Active[i].SetData(_currentList[i], _currentDifficulty);
                    _usersPool.Active[i].SetIndex(i + 1);
                }
                else
                {
                    if (_usersPool.Active.Count <= i)
                        break;
                    _usersPool.ReturnItemToPool(_usersPool.Active[i]);
                    i--;
                }
            }
            _ui.ChangeTableHeight(_currentList.Count > topCount ? topCount : _currentList.Count);
            _ui.ChangeScrollValue(CalculateScrollValue());
        }
        #endregion

        #region Base Methods
        private void Initialize()
        {
            if (!_initialized)
            {
                _ui.Init();
                _initialized = true;
            }
        }

        protected override void Init()
        {
            CreateEventListeners();
            _usersPool = new PoolManager<TopUser>(_usersContainer);
            _players = new List<PlayerDTO>();
            _sameStatusPlayers = new List<PlayerDTO>();

            GetMainUserFromServer();
            if (File.Exists(DataManager.Instance.PlayerDataPath))
            {
                System.DateTime cacheTime = File.GetLastWriteTime(DataManager.Instance.PlayerDataPath);
                System.DateTime now = System.DateTime.Now;  //Simulates server time
                System.TimeSpan ts = now - cacheTime;
                if (ts.TotalHours >= hoursToUpdateData)
                    GetDataFromServer(playersCount);
                else
                    LoadUsers();
            }
            else
                GetDataFromServer(playersCount);
        }

        protected override void Cycle() { }

        protected override void Destroy()
        {
            RemoveEventListeners();
        }
        #endregion
    }
}