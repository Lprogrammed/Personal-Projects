using System;
using System.Collections.Generic;
using System.Text;

namespace QuartoAI_v2
{
    class QBoard
    {
        public byte[] board;
        public List<byte> unplayedPieces;
        public List<byte> legalMoves;

        /* ***CONSTRUCTORS*** */
        public QBoard()
        {
            board = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            unplayedPieces = new List<byte>() { 15, 30, 45, 60, 75, 90, 105, 120, 135, 150, 165, 180, 195, 210, 225, 240 };
            legalMoves = new List<byte>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
        }
        public QBoard(QBoard original)
        {
            this.board = new byte[16];
            original.board.CopyTo(this.board, 0);
            this.unplayedPieces = new List<byte>(original.unplayedPieces);
            this.legalMoves = new List<byte>(original.legalMoves);
        }
        public QBoard(byte[] boardState)
        {
            this.board = new byte[16];
            boardState.CopyTo(this.board, 0);
            this.unplayedPieces = new List<byte> { 15, 30, 45, 60, 75, 90, 105, 120, 135, 150, 165, 180, 195, 210, 225, 240 };
            this.legalMoves = new List<byte> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };

            //takes away moves
            for (byte index = 0; index < 16; index++)
            {
                if (this.board[index] != 0)
                {
                    this.legalMoves.Remove(index);
                    this.unplayedPieces.Remove(boardState[index]);
                }
            }
        }

        /* ***METHODS*** */
        //returns true if board is in win state
        public bool qCheckWin()
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
        //returns true if board is filled and isn't in a win state
        public bool qCheckTie()
        {
            return this.unplayedPieces.Count == 0 && !this.qCheckWin();
        }
        //places a piece on the board, returns true when finished
        public bool playPiece(byte piece, byte space)
        {
            board[space] = piece;
            unplayedPieces.Remove(piece);
            legalMoves.Remove(space);
            return true;
        }
    }
}
