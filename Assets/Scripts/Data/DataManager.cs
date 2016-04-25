using UnityEngine;
using System;
using System.IO;
using System.Collections;
using Innovision.Helpers;

namespace Innovision.Data
{
    public class DataManager : SimpleSingleton<DataManager>
    {
        #region Events
        private Action<string[]> _dataLoadedCallback;
        private Action _dataSavedCallback;
        #endregion

        #region Properties
        private string _playerDataPath;
        #endregion

        #region Getters
        public string PlayerDataPath { get { return _playerDataPath; } }
        #endregion

        #region Controls
        public DataManager()
        {
            _playerDataPath = Application.persistentDataPath + "/playersInfo.dat";
        }

        public void SaveUsers(string[] users, Action callback = null)
        {
            StreamWriter sw = File.CreateText(_playerDataPath);
            foreach (string data in users)
                sw.WriteLine(data);

            sw.Flush();
            sw.Close();

            if (callback != null)
                callback();
        }

        public void LoadUsers(Action<string[]> callback = null)
        {
            string[] users = File.ReadAllLines(DataManager.Instance.PlayerDataPath);
            if (users.Length > 0)
            {
                if (callback != null)
                    callback(users);
            }
        }
        #endregion
    }
}