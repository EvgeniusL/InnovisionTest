using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Innovision.Data
{
    [Serializable]
    public class PlayerDTO
    {
        #region Comparisons
        public static int CompareByEasyScore(PlayerDTO data1, PlayerDTO data2)
        {
            return data2.EScore.CompareTo(data1.EScore);
        }

        public static int CompareByMediumScore(PlayerDTO data1, PlayerDTO data2)
        {
            return data2.MScore.CompareTo(data1.MScore);
        }

        public static int CompareByHardScore(PlayerDTO data1, PlayerDTO data2)
        {
            return data2.HScore.CompareTo(data1.HScore);
        }
        #endregion

        #region Data Enums 
        public enum Avatars
        {
            Bazookalicious,
            ElectroDeluxe,
            Mumbia,
            Psycolizzard,
            Pumpkin,
            PunkNStein,
            Skillet,
            Spitfire,
            XLightning
        }

        public enum Statuses
        {
            Rookie,
            Normal,
            Pro,
            Guru
        }

        public enum Names
        {
            Alex,
            Morgan,
            Elena,
            Aronny,
            Sam,
            Bruce,
            Anna,
            John,
            Sarah,
            Peter,
            Alice,
            Kronk
        }
        #endregion

        #region Properties
        [SerializeField]
        private string _name;
        [SerializeField]
        private string _avatar;
        [SerializeField]
        private string _status;
        [SerializeField]
        private int _eScore;
        [SerializeField]
        private int _mScore;
        [SerializeField]
        private int _hScore;
        #endregion

        #region Controls
        public PlayerDTO(string name, int eScore, int mScore, int hScore, string avatar, string status)
        {
            _name = name;
            _eScore = eScore;
            _mScore = mScore;
            _hScore = hScore;
            _avatar = avatar;
            _status = status;
        }
        #endregion

        #region Getters
        public string Name { get { return _name; } }
        public string Avatar { get { return _avatar; } }
        public string Status { get { return _status; } }

        public int EScore { get { return _eScore; } }
        public int MScore { get { return _mScore; } }
        public int HScore { get { return _hScore; } }
        #endregion
    }
}