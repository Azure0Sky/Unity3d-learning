using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tic_tac_toe : MonoBehaviour {

    private int turn = 1;   // 1 means O's turn, 2 means X's turn
    private int[,] board = new int[3, 3];
    private int empty = 9;

    void Reset()
    {
        turn = 0;
        empty = 9;
        for ( int i = 0; i < 3; ++i ) {
            for ( int j = 0; j < 3; ++j ) {
                board[i, j] = 0;
            }
        }
    }

    void Start()
    {
        Reset();
    }

    void OnGUI()
    {
        GUI.skin.button.fontSize = 15;
        if ( GUI.Button( new Rect( 100, 390, 100, 50 ), "RESTART" ) ) {
            Reset();
        }

        int res = Check();
        GUI.skin.label.fontSize = 25;
        if ( res == 1 ) {
            GUI.Label( new Rect( 110, 320, 120, 50 ), "O wins!" );
        } else if ( res == 2 ) {
            GUI.Label( new Rect( 110, 320, 120, 50 ), "X wins!" );
        } else if ( res == 3 ) {
            GUI.Label( new Rect( 120, 320, 120, 40 ), "Draw!" );
        }

        GUI.skin.button.fontSize = 25;
        for ( int i = 0; i < 3; ++i ) {
            for ( int j = 0; j < 3; ++j ) {
                if ( board[i, j] == 1 ) {
                    GUI.Button( new Rect( 40 + 75 * i, 40 + 75 * j, 70, 70 ), "O" );
                } else if ( board[i, j] == 2 ) {
                    GUI.Button( new Rect( 40 + 75 * i, 40 + 75 * j, 70, 70 ), "X" );
                } else {
                    if ( GUI.Button( new Rect( 40 + 75 * i, 40 + 75 * j, 70, 70 ), "" ) ) {
                        if ( res == 0 ) {
                            if ( turn == 1 ) {
                                board[i, j] = 1;
                                --empty;
                                turn = 2;
                            } else {
                                board[i, j] = 2;
                                --empty;
                                turn = 1;
                            }
                        }
                            
                    }
                }
            }
        }

    }

    //return value 0 means the game should continue, 1 means O wins, 2 means X wins, 3 means draw
    int Check()
    {   
        //row
        for ( int i = 0; i < 3; ++i ) {
            if ( board[i, 0] == board[i, 1] && board[i, 1] == board[i, 2] ) {
                return board[i, 0];
            }
        }

        //col
        for ( int i = 0; i < 3; ++i ) {
            if ( board[0, i] == board[1, i] && board[1, i] == board[2, i] ) {
                return board[0, i];
            }
        }

        //dia
        if ( board[0, 0] == board[1, 1] && board[1, 1] == board[2, 2] ) {
            return board[0, 0];
        } else if ( board[0, 2] == board[1, 1] && board[1, 1] == board[2, 0] ) {
            return board[0, 2];
        }

        if ( empty > 0 )
            return 0;
        else
            return 3;

    }

}
