using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISceneController
{
    void LoadResources();
}

public interface IUserAction
{
    void StartGame();
    void Restart();
    void GameOver();

    void MoveInput();
}
