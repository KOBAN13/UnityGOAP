using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CharacterScripts
{
    public interface IRotate
    {
        void RotateCharacter(Vector2 moveDirection);
    }
}