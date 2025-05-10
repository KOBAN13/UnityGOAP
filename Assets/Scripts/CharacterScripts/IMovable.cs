using UnityEngine;

namespace CharacterScripts
{
    public interface IMovable
    {
        void Move(Vector2 input, float speed);
    }
}