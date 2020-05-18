using System;
using QuartoAI_v2;
using QuartoAI_v1;
using QuartoAI_v2_exp;
using QuartoAI_v3;
using System.Diagnostics;

public class TestConsole
{
    static bool gameWon(byte[,] board)
    {
        return ((board[0, 0] & board[0, 1] & board[0, 2] & board[0, 3]) > 0
                  || (board[1, 0] & board[1, 1] & board[1, 2] & board[1, 3]) > 0
                  || (board[2, 0] & board[2, 1] & board[2, 2] & board[2, 3]) > 0
                  || (board[3, 0] & board[3, 1] & board[3, 2] & board[3, 3]) > 0
                  || (board[0, 0] & board[1, 0] & board[2, 0] & board[3, 0]) > 0
                  || (board[0, 1] & board[1, 1] & board[2, 1] & board[3, 1]) > 0
                  || (board[0, 2] & board[1, 2] & board[2, 2] & board[3, 2]) > 0
                  || (board[0, 3] & board[1, 3] & board[2, 3] & board[3, 3]) > 0
                  || (board[0, 0] & board[1, 1] & board[2, 2] & board[3, 3]) > 0
                  || (board[0, 3] & board[1, 2] & board[2, 1] & board[3, 0]) > 0);
    }

    static bool gameTied(byte[,] board)
    {
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                if (board[x, y] == 0) return false;
            }
        }
        return true;
    }

    static string pieceToString(byte piece)
    {
        string result = "";

        if (piece / 128 > 0)
        {
            piece -= 128;
            result = result + "White, ";
        }
        else
        {
            result = result + "Black, ";
        }
        if (piece / 64 > 0)
        {
            piece -= 64;
            result = result + "Tall, ";
        }
        else
        {
            result = result + "Short, ";
        }
        if (piece / 32 > 0)
        {
            piece -= 32;
            result = result + "Square, ";
        }
        else
        {
            result = result + "Circle, ";
        }
        if (piece / 16 > 0)
        {
            piece -= 16;
            result = result + "and Hollow";
        }
        else
        {
            result = result + "and Solid";
        }

        return result;
    }

    static public byte convertStrToPiece(string pieceStr)
    {
        byte result = 0;
        foreach (char att in pieceStr)
        {
            switch (att)
            {
                case 'A':
                    result += 128;
                    break;
                case 'B':
                    result += 64;
                    break;
                case 'C':
                    result += 32;
                    break;
                case 'D':
                    result += 16;
                    break;
                case 'a':
                    result += 8;
                    break;
                case 'b':
                    result += 4;
                    break;
                case 'c':
                    result += 2;
                    break;
                case 'd':
                    result += 1;
                    break;
            }
        }
        return result;
    }

    static void Main(string[] args)
    {
        bool showMoves = true;
        bool playAlong = true;
        bool humanPlayer = false;
        int numOfGames = 0;
        int[] wins = new int[5] { 0, 0, 0, 0, 0 };
        Tuple<byte, byte> turnRecorder;
        Stopwatch timeToMove = new Stopwatch();


        bool isPlayer1First = true;
        bool isPlayer1Turn = true;
        AI player1;
        AI_exp player2;
        byte pieceGiven = 255;
        byte positionPlaced = 255;
        //ai player 2

        Console.Write("How many games do you want to simulate? ");
        numOfGames = Convert.ToInt32(Console.ReadLine());

        for (int gameNum = 0; gameNum < numOfGames; gameNum++)
        {
            //reset for new Game
            player1 = new AI(QuartoAI_v2.diff.hard);
            player2 = new AI_exp(QuartoAI_v2_exp.diff.hard);
            byte[,] board = new byte[4, 4] { { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 } };
            isPlayer1Turn = isPlayer1First;
            pieceGiven = 255;

            int turnNum = 0;
            Console.WriteLine("Simulating Game #" + (gameNum + 1));
            timeToMove.Start();
            while (!gameWon(board) && !gameTied(board))
            {
                Console.WriteLine("Turn " + turnNum++);
                
                if (isPlayer1Turn)
                {
                    timeToMove.Restart();
                    turnRecorder = player1.AIFullTurn(pieceGiven, positionPlaced);
                    timeToMove.Stop();
                    if (showMoves) Console.Write("Player 1 ");

                }
                else
                {
                    if (!humanPlayer)
                    {
                        timeToMove.Restart();
                        turnRecorder = player2.AIFullTurn(pieceGiven, positionPlaced);
                        timeToMove.Stop();
                    }
                    else
                    {
                        Console.Write("Where do you want to place: ");
                        int temp1 = Convert.ToInt32(Console.ReadLine());
                        byte pos = (byte)temp1;
                        Console.Write("What Piece do you want to give: ");
                        string temp2 = Console.ReadLine();
                        byte pie = convertStrToPiece(temp2);
                        turnRecorder = new Tuple<byte, byte>(pos, pie);
                    }
                    if (showMoves) Console.Write("Player 2 ");
                }

                positionPlaced = turnRecorder.Item1;
                if (playAlong && positionPlaced != 255) Console.Write("played the " + pieceToString(pieceGiven) + " piece at (" + (positionPlaced % 4 + 1) + ", " + (positionPlaced / 4 + 1) + ") and ");
                if (positionPlaced != 255) board[positionPlaced % 4, positionPlaced / 4] = pieceGiven;
                pieceGiven = turnRecorder.Item2;
                if (showMoves) Console.Write("gave " + pieceToString(pieceGiven) + " in " + Math.Round((double)((double)timeToMove.ElapsedMilliseconds / (double)1000),2) + " seconds\n");
                isPlayer1Turn = !isPlayer1Turn;
            }

            if (gameTied(board))
            {
                wins[4]++;
                if (showMoves) Console.WriteLine("GAME OVER! It is a tie");
            }
            else if (!isPlayer1Turn && isPlayer1First)
            {
                wins[0]++;
                if (showMoves) Console.WriteLine("GAME OVER! Player 1 wins");
            }
            else if (!isPlayer1Turn && !isPlayer1First)
            {
                wins[1]++;
                if (showMoves) Console.WriteLine("GAME OVER! Player 1 wins");
            }
            else if (isPlayer1Turn && !isPlayer1First)
            {
                wins[2]++;
                if (showMoves) Console.WriteLine("GAME OVER! Player 2 wins");
            }
            else if (isPlayer1Turn && isPlayer1First)
            {
                wins[3]++;
                if (showMoves) Console.WriteLine("GAME OVER! Player 2 wins");
            }

            isPlayer1First = !isPlayer1First;
        }

        Console.WriteLine("AI 1 wins when 1st Player = " + wins[0]);
        Console.WriteLine("AI 1 wins when 2nd Player = " + wins[1]);
        Console.WriteLine("AI 2 wins when 1st Player = " + wins[2]);
        Console.WriteLine("AI 2 wins when 2nd Player = " + wins[3]);
        Console.WriteLine("Ties = " + wins[4]);
        Console.ReadKey();
    }
}