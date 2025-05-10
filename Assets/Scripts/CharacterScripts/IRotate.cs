using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CharacterScripts
{
    public interface IRotate
    {
        UniTask RotateCharacter(Vector3 mousePosition);
    }
}