using System.Collections;
using UnityEngine;

namespace Assets.Scripts.ScriptableObjects
{
    [CreateAssetMenu]
    public class AvatarModel : ScriptableObject
    {
        public Sprite[] Avatars => _avatars;

        [SerializeField]
        private Sprite[] _avatars;
    }
}