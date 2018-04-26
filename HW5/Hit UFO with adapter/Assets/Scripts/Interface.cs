using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISceneController
{
    void LoadResources();
}

public interface IUserAction
{
    void StartGame( Fly.Mode mode );

    void Restart( Fly.Mode mode );
    void GameOver();
    void Shoot( Vector3 pos );           // 点击画面即射击
}

public interface IActionManager
{
    void PlayDisk( DiskData disk );
}
