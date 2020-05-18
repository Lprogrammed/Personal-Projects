using System;
using System.Collections.Generic;

namespace QuartoAI_v2
{
    class MCTSNode_QAI
    {
        public bool[] flag;
        public byte nodeValue;
        public List<MCTSNode_QAI> children;
        public List<byte> possibleChildren;
        public QBoard boardLogic;
        public Tuple<long, long> score;


        /* ***CONSTRUCTORS*** */
        public MCTSNode_QAI(int Wval = 2, int Lval = 0, int Tval = 1)
        {
            flag = new bool[] { false, false, false, false, false, false };
            nodeValue = 0;
            children = new List<MCTSNode_QAI>();
            boardLogic = new QBoard();
            possibleChildren = new List<byte>();
            score = new Tuple<long, long>(0, 0);
        }
        public MCTSNode_QAI(MCTSNode_QAI parent, byte givenValue)
        {
            this.nodeValue = givenValue;
            this.flag = new bool[6];
            parent.flag.CopyTo(this.flag, 0);
            this.flag[qflags.TYPE] = !this.flag[qflags.TYPE];
            if (!this.flag[qflags.TYPE])
                this.flag[qflags.OWNER] = !this.flag[qflags.OWNER];
            if (this.flag[qflags.TYPE])
            {
                this.possibleChildren = new List<byte>(parent.boardLogic.legalMoves);
            }
            else
            {
                this.possibleChildren = new List<byte>(parent.boardLogic.unplayedPieces);
            }
            this.children = new List<MCTSNode_QAI>();
            boardLogic = new QBoard(parent.boardLogic);
            this.score = new Tuple<long, long>(0, 0);
        }
        public static long factorial(long n)
        {
            long result = n;
            while (n > 1)
            {
                result *= --n;
            }
            return result;
        }

        /* ***METHODS*** */
        //adds a new childto the list of the current node
        public MCTSNode_QAI addChild(byte givenValue)
        {
            MCTSNode_QAI newChild = new MCTSNode_QAI(this, givenValue);

            if (!newChild.flag[qflags.TYPE])
            {
                newChild.boardLogic.playPiece(this.nodeValue, givenValue);
                newChild.possibleChildren.Remove(nodeValue);
            }
            //check to see if child is losingBoard
            this.children.Add(newChild);
            return newChild;
        }
        //adds the result given from a new branch to the score calculations
        public Tuple<long, long> addResultToScore(Tuple<long, long> newResult)
        {
            if (this.flag[qflags.CHILDHASLOSINGBOARD])
            {
                if (this.score.Item2 > 0)
                {
                    bool stop = true;
                }
                Tuple<long, long> result = new Tuple<long, long>((this.score.Item1 * -1), 2 - (this.score.Item2));
                this.score = new Tuple<long, long>(0, 2);
                return result;
            }
            else if (this.flag[qflags.CHILDHASWINNINGBOARD])
            {
                if (this.score.Item2 > 0)
                {
                    bool stop = true;
                }
                Tuple<long, long> result = new Tuple<long, long>(((this.score.Item1 * -1) + 2), 2 - (this.score.Item2));
                this.score = new Tuple<long, long>(2, 2);
                return result;
            }
            else
            {
                this.score = new Tuple<long, long>(this.score.Item1 + newResult.Item1, this.score.Item2 + newResult.Item2);
                return newResult;
            }

        }
        //return node simulation score
        public double nodeSimScore(long parent)
        {
            if (this.score.Item2 == 0)
            {
                return 0;
            }
            double result = ((double)this.score.Item1 / (double)this.score.Item2) + Math.Sqrt(2) * Math.Sqrt(Math.Log(parent) / (double)this.score.Item2);
            return result;
        }
        //return node picking score
        public double nodePickScore()
        {
            if (this.score.Item2 == 0)
            {
                return 0;
            }
            if (this.flag[qflags.CHILDHASLOSINGBOARD])
            {
                return -1;
            }
            double result = (double)this.score.Item1 / (double)this.score.Item2;
            return result;
        }
        //returns true if board is a losing board
        public bool isLosingNode()
        {
            if (this.flag[qflags.ENDBOARD] && !this.flag[qflags.OWNER])
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //returns if board is in a win state
        public bool isWinState()
        {
            return this.boardLogic.qCheckWin();
        }
        //returns if board is in a end state that is a tie
        public bool isTie()
        {
            return this.boardLogic.qCheckTie();
        }
        //NOTE(LANE): needs to be renamed
        //calcutes if given node is an end state
        public bool isNodeScored()
        {
            if (this.isWinState())
            {
                if (this.flag[qflags.OWNER])
                {
                    this.score = new Tuple<long, long>(2, 2);
                }
                else
                {
                    this.score = new Tuple<long, long>(0, 2);
                }
                this.flag[qflags.ENDBOARD] = true;
                return true;
            }
            else if (this.isTie())
            {
                this.score = new Tuple<long, long>(1, 2);
                this.flag[qflags.ENDBOARD] = true;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
