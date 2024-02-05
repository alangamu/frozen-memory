using System;

namespace Assets.Scripts
{
    [Serializable]
    public class PlayerInfo
    {
        public int AvatarIndex => _avatarIndex;
        public string PlayerName => _playerName;

        private int _avatarIndex;
        private string _playerName;

        public PlayerInfo(int avatarIndex, string playerName)
        {
            _avatarIndex = avatarIndex;
            _playerName = playerName;
        }

        public void SetAvatarIndex(int avatarIndex)
        {
            _avatarIndex = avatarIndex;
        }
    }
}