using System;

namespace Assets.Scripts
{
    [Serializable]
    public class PlayerInfo
    {
        public int AvatarIndex => _avatarIndex;
        public string PlayerName => _playerName;
        public bool IsReady => _isReady;

        private int _avatarIndex;
        private string _playerName;
        private bool _isReady;

        public PlayerInfo(int avatarIndex, string playerName)
        {
            _avatarIndex = avatarIndex;
            _playerName = playerName;
            _isReady = false;
        }

        public void SetIsReady(bool isReady)
        {
            _isReady = isReady;
        }
    }
}