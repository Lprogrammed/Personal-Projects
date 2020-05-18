using System;
using System.Collections.Generic;
using System.Text;

namespace QuartoAI_v1
{
    class AIBoard
    {
        byte[] board;
        public List<byte> unplayedPieces;
        public List<byte> legalMoves;

        public AIBoard()
        {
            board = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            unplayedPieces = new List<byte>() { 15, 30, 45, 60, 75, 90, 105, 120, 135, 150, 165, 180, 195, 210, 225, 240 };
            legalMoves = new List<byte>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
        }
        public AIBoard(AIBoard original)
        {
            this.board = new byte[16];
            original.board.CopyTo(this.board, 0);
            this.unplayedPieces = new List<byte>(original.unplayedPieces);
            this.legalMoves = new List<byte>(original.legalMoves);
        }
        //returns winning row between 1-10, 0 if no winning rows on board
        public bool checkWin()
        {
            return ((this.board[0] & this.board[1] & this.board[2] & this.board[3]) > 0)
                  || ((this.board[4] & this.board[5] & this.board[6] & this.board[7]) > 0)
                  || ((this.board[8] & this.board[9] & this.board[10] & this.board[11]) > 0)
                  || ((this.board[12] & this.board[13] & this.board[14] & this.board[15]) > 0)
                  || ((this.board[3] & this.board[6] & this.board[9] & this.board[12]) > 0)
                  || ((this.board[0] & this.board[4] & this.board[8] & this.board[12]) > 0)
                  || ((this.board[1] & this.board[5] & this.board[9] & this.board[13]) > 0)
                  || ((this.board[2] & this.board[6] & this.board[10] & this.board[14]) > 0)
                  || ((this.board[3] & this.board[7] & this.board[11] & this.board[15]) > 0)
                  || ((this.board[0] & this.board[5] & this.board[10] & this.board[15]) > 0);
        }
        public bool checkTie()
        {
            return unplayedPieces.Count == 0;
        }
        public bool placePieceOnBoard(byte piece, byte space)
        {
            board[space] = piece;
            unplayedPieces.Remove(piece);
            legalMoves.Remove(space);
            return true;
        }

        static bool isValidPiece(byte prospectivePiece)
        {
            return prospectivePiece <= 240 && prospectivePiece > 0 && prospectivePiece % 15 == 0;
        }
        private string convertPieceToStr(byte piece)
        {
            switch (piece / 15)
            {
                case 0:
                    return "    ";
                case 1:
                    return "abcd";
                case 2:
                    return "abcD";
                case 3:
                    return "abCd";
                case 4:
                    return "abCD";
                case 5:
                    return "aBcd";
                case 6:
                    return "aBcD";
                case 7:
                    return "aBCd";
                case 8:
                    return "aBCD";
                case 9:
                    return "Abcd";
                case 10:
                    return "AbcD";
                case 11:
                    return "AbCd";
                case 12:
                    return "AbCD";
                case 13:
                    return "ABcd";
                case 14:
                    return "ABcD";
                case 15:
                    return "ABCd";
                case 16:
                    return "ABCD";

            }
            return "\nERROR: Illegal Piece Detected\n";
        }
        public void printBoard()
        {
            Console.WriteLine("  1    2    3    4    5");
            Console.WriteLine(convertPieceToStr(this.board[0]) + "|" + convertPieceToStr(this.board[1]) + "|" + convertPieceToStr(this.board[2]) + "|" + convertPieceToStr(this.board[3]) + "   6");
            Console.WriteLine("________________________");
            Console.WriteLine(convertPieceToStr(this.board[4]) + "|" + convertPieceToStr(this.board[5]) + "|" + convertPieceToStr(this.board[6]) + "|" + convertPieceToStr(this.board[7]) + "   7");
            Console.WriteLine("________________________");
            Console.WriteLine(convertPieceToStr(this.board[8]) + "|" + convertPieceToStr(this.board[9]) + "|" + convertPieceToStr(this.board[10]) + "|" + convertPieceToStr(this.board[11]) + "   8");
            Console.WriteLine("________________________");
            Console.WriteLine(convertPieceToStr(this.board[12]) + "|" + convertPieceToStr(this.board[13]) + "|" + convertPieceToStr(this.board[14]) + "|" + convertPieceToStr(this.board[15]) + "   9");
            Console.WriteLine("                     10");

        }
        public void displayUnplayedPieces()
        {
            Console.WriteLine("\nAvailable Pieces:\n");
            for (int i = 0; i < this.unplayedPieces.Count; i++)
            {
                Console.Write(" " + convertPieceToStr(this.unplayedPieces[i]) + ",");
            }
            Console.WriteLine();
        }
        public byte convertStrToPiece(string pieceStr)
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
        bool isLegalMove(byte prospectivePiece)
        {
            for (int i = 0; i < board.Length; i++)
            {
                if (board[i] == prospectivePiece)
                {
                    return false;
                }
            }
            return isValidPiece(prospectivePiece);
        }
        //returns piece chosen by player;
        public byte playerPickPiece()
        {
            displayUnplayedPieces();
            Console.WriteLine("\nPlease Pick a Piece: ");
            string pickedPiece = Console.ReadLine();
            while (!isLegalMove(convertStrToPiece(pickedPiece)))
            {
                Console.WriteLine("ERROR: Invalid Piece, please try again.");
                pickedPiece = Console.ReadLine();
            }
            return convertStrToPiece(pickedPiece);
        }

        public byte playerPlacesPiece(byte currPiece)
        {
            Console.Write("You have been given \'" + convertPieceToStr(currPiece) + "\', where would you like to place it: ");
            byte pickedPosition = Convert.ToByte(Console.ReadLine());
            while (!placePieceOnBoard(currPiece, pickedPosition))
            {
                Console.WriteLine("ERROR: Invalid Space, please try again.");
                pickedPosition = Convert.ToByte(Console.ReadLine());
            }
            //            Console.Clear();
            Console.WriteLine("Player Turn");
            printBoard();
            return pickedPosition;
        }
    }
}
