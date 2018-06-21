using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameBase;

public interface ISceneController
{
    void LoadResources();
}

public interface IUserAction
{
    void Restart();
    void MoveCharacter( MyCharacterController ctrClicked );
    void MoveBoat();

    void Help();
}