using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace QuartoAI_v2
{
    enum diff { easy, medium, hard }

    class AI
    {
        private Random random;
        public MCTSNode_QAI root;
        private int counter = 0;
        private diff difficulty;
        public Stopwatch time;

        /* ***CONSTRUCTORS*** */
        public AI(diff d)
        {
            random = new Random();
            difficulty = d;
            time = new Stopwatch();
        }

        /* ***METHODS*** */
        private Tuple<long, long> simulateBranch(MCTSNode_QAI currNode)
        {
            //checks to see if current node is in an simulation end state
            if (currNode.flag[qflags.ENDBOARD] || currNode.flag[qflags.ALLCHILDRENSIMULATED])
            {
                //currNode.flag[qflags.ENDBOARD] = true;
                return new Tuple<long, long>(0, 0);
            }

            if (currNode.possibleChildren.Count != 0)
            {
                int newChildDataIndex = random.Next(0, currNode.possibleChildren.Count);
                MCTSNode_QAI newChild = currNode.addChild(currNode.possibleChildren[newChildDataIndex]); //possibly simplified
                currNode.possibleChildren.RemoveAt(newChildDataIndex);

                if (newChild.isNodeScored())
                {
                    if (newChild.isLosingNode())
                    {
                        currNode.flag[qflags.CHILDHASLOSINGBOARD] = true;
                        currNode.flag[qflags.ALLCHILDRENSIMULATED] = true;

                    }
                    else if (newChild.isWinState())
                    {
                        currNode.flag[qflags.CHILDHASWINNINGBOARD] = true;
                        currNode.flag[qflags.ALLCHILDRENSIMULATED] = true;
                    }

                    return currNode.addResultToScore(newChild.score); ;
                }
                else
                {
                    Tuple<long, long> tempScore = simulateBranch(newChild);

                    return currNode.addResultToScore(tempScore); ;
                }
            }
            else
            {
                MCTSNode_QAI nextChild = selectMostPromisingNodeForSimulation(currNode);
                if (nextChild == null)
                {
                    //pos A.0
                    return new Tuple<long, long>(0, 0);
                }
                else
                {
                    Tuple<long, long> tempScore = simulateBranch(nextChild);
                    //currNode.addResultToScore(tempScore);
                    return currNode.addResultToScore(tempScore);
                }
            }
        }
        //Finds most promising node of currNodes children and returns a reference to it.
        private MCTSNode_QAI selectMostPromisingNodeForSimulation(MCTSNode_QAI currNode)
        {
            MCTSNode_QAI result = null;
            for (int index = 0; index < currNode.children.Count; index++)
            {
                if (!currNode.children[index].flag[qflags.CHILDHASLOSINGBOARD] && !currNode.children[index].flag[qflags.ALLCHILDRENSIMULATED] && !currNode.children[index].flag[qflags.ENDBOARD])
                {
                    if (result == null)
                    {
                        result = currNode.children[index];
                    }
                    else if (currNode.children[index].nodeSimScore(root.score.Item2) > result.nodeSimScore(root.score.Item2))
                    {
                        result = currNode.children[index];
                    }
                }
            }

            //possibly move this statement to simulatebranch at pos A.0
            if (result == null)
            {
                currNode.flag[qflags.ALLCHILDRENSIMULATED] = true;
            }

            return result;
        }
        //decides if move is valid based on difficulty
        private bool compareMove()
        {
            int prob = random.Next(0, 10);
            if (difficulty == diff.easy && prob > 6)
                return false;
            else if (difficulty == diff.medium && prob > 8)
                return false;
            return true;
        }
        //returns true if AI will win
        private bool simulatePerfectGame(MCTSNode_QAI currNode)
        {
            MCTSNode_QAI bestMove = null;

            if (currNode.flag[qflags.CHILDHASLOSINGBOARD])
            {
                return false;
            }
            else if (currNode.flag[qflags.CHILDHASWINNINGBOARD])
            {
                return true;
            }
            else if (currNode.flag[qflags.ENDBOARD])
            {
                if (currNode.flag[qflags.OWNER])
                    return currNode.isWinState();
                else
                    return currNode.isTie();
            }

            for (int index = 0; index < currNode.children.Count; index++)
            {

                if (currNode.children[index].flag[qflags.OWNER]
                    && (bestMove == null
                        || bestMove.nodePickScore() < currNode.children[index].nodePickScore()))
                {
                    bestMove = currNode.children[index];
                }
                else if (!currNode.children[index].flag[qflags.OWNER]
                        && (bestMove == null
                            || bestMove.nodePickScore() > currNode.children[index].nodePickScore()))
                {
                    bestMove = currNode.children[index];
                }
            }

            return simulatePerfectGame(bestMove);
        }

        //Selects best move and returns reference to its noded
        private MCTSNode_QAI selectBestMove(MCTSNode_QAI currNode)
        {
            MCTSNode_QAI selectedMove = null;
            bool gameWin = false;

            if (this.difficulty == diff.hard)
            {
                bool willWinPerfectGame = false;
                for (int index = 0; index < currNode.children.Count && !gameWin; index++)
                {
                    if (!willWinPerfectGame)
                    {
                        if (selectedMove == null)
                        {
                            selectedMove = currNode.children[index];
                            willWinPerfectGame = simulatePerfectGame(currNode.children[index]);
                        }
                        else
                        {
                            willWinPerfectGame = simulatePerfectGame(currNode.children[index]);
                            if (willWinPerfectGame || selectedMove.nodePickScore() < currNode.children[index].nodePickScore())
                            {
                                selectedMove = currNode.children[index];
                            }
                        }
                    }
                    else
                    {
                        if (currNode.children[index].flag[qflags.ENDBOARD])
                        {
                            selectedMove = currNode.children[index];
                            gameWin = true;
                        }
                        else if (selectedMove.nodePickScore() < currNode.children[index].nodePickScore())
                        {
                            if (simulatePerfectGame(currNode.children[index]))
                            {
                                selectedMove = currNode.children[index];
                            }
                        }
                    }
                }
            }
            else
            {
                for (int index = 0; index < currNode.children.Count && !gameWin; index++)
                {
                    if (compareMove())
                    {
                        if (selectedMove == null)
                        {
                            selectedMove = currNode.children[index];
                        }
                        if (currNode.children[index].flag[qflags.ENDBOARD])
                        {
                            selectedMove = currNode.children[index];
                            gameWin = true;
                        }
                        else if (selectedMove.nodePickScore() < currNode.children[index].nodePickScore())
                        {
                            selectedMove = currNode.children[index];
                        }
                    }
                }
            }

            if (selectedMove == null && currNode.children.Count > 0)
            {
                selectedMove = currNode.children[random.Next(0, currNode.children.Count)];
            }

            return selectedMove;
        }
        //returns child of root containing search value, null if child not found
        private MCTSNode_QAI findChildWithVal(byte searchValue)
        {
            bool found = false;
            MCTSNode_QAI newRoot = null;
            for (int index = 0; index < root.children.Count && !found; index++)
            {
                if (root.children[index].nodeValue == searchValue)
                {
                    newRoot = root.children[index];
                    found = true;
                }
            }
            return newRoot;
        }
        //returns null unless it was first turn
        public Tuple<byte, byte> AITurnUpkeep(byte givenPiece, byte boardPosition, int firstTurnSimulationTime = 4500)
        {
            //navigate tree to new state
            if (this.root == null && givenPiece == 255)  //first turn of game
            {
                root = new MCTSNode_QAI();
                root.flag[qflags.OWNER] = true; root.flag[qflags.TYPE] = true;
                root.nodeValue = (byte)(random.Next(1, 16) * 15);
                root.possibleChildren = new List<byte>(root.boardLogic.legalMoves);

                //simulate tree from seed root
                time.Restart();
                while (time.ElapsedMilliseconds < firstTurnSimulationTime)
                {
                    simulateBranch(root);
                }
                time.Stop();

                return new Tuple<byte, byte>(255, root.nodeValue);
            }
            else if (this.root == null && givenPiece != 255) //second turn of game
            {
                root = new MCTSNode_QAI();
                root.flag[qflags.OWNER] = false; root.flag[qflags.TYPE] = true;
                root.nodeValue = givenPiece;
                root.possibleChildren = new List<byte>(root.boardLogic.legalMoves);

                //below is a bandage for an issue where the AI would try to give back the first piece it was given
                //root.boardLogic.unplayedPieces.Remove(givenPiece);
            }
            else
            {
                MCTSNode_QAI tempRoot = findChildWithVal(boardPosition);
                if (tempRoot == null)
                    tempRoot = root.addChild(boardPosition);
                root = tempRoot;

                tempRoot = findChildWithVal(givenPiece);
                if (tempRoot == null)
                    tempRoot = root.addChild(givenPiece);
                root = tempRoot;
            }
            return null;
        }
        //simulates tree & return pointer to node containing best next move
        public MCTSNode_QAI AITurnPart(int endSimulationTime = 2000)
        {
            time.Restart();
            while (time.ElapsedMilliseconds < endSimulationTime)
            {
                simulateBranch(this.root);
            }

            root = selectBestMove(this.root);


            return root;
        }

        public Tuple<byte, byte> AIFullTurn_debug(byte givenPiece, byte boardPosition)
        {
            MCTSNode_QAI temp = null;
            byte pieceToGive;
            byte positionToPlace = 255;
            if (this.root != null && this.root.flag[qflags.ENDBOARD]) return new Tuple<byte, byte>(255, 255);

            if (AITurnUpkeep(givenPiece, boardPosition) == null)
            {
                temp = AITurnPart(2000);

                /*DEBUG*/
                Console.WriteLine("\n\nSummary:");
                /*DEBUG*/
                Console.WriteLine("Counter: ");
                /*DEBUG*/
                Console.WriteLine("Root: " + root.nodeValue + " " + root.score.Item1 + " " + root.score.Item2 + " " + counter);
                /*DEBUG*/
                for (int i = 0; i < root.children.Count; i++)
                /*DEBUG*/
                {
                    /*DEBUG*/
                    Console.WriteLine("Child" + i + ": " + root.children[i].nodeValue + " " + root.children[i].score.Item1 + " " + root.children[i].score.Item2 + " " + Math.Round((float)root.children[i].nodePickScore(), 4) + " " + Math.Round((float)root.children[i].nodeSimScore(root.score.Item2), 4));
                    /*DEBUG*/
                }

                this.root = temp;
                positionToPlace = this.root.nodeValue;
                temp = AITurnPart(2000);

                /*DEBUG*/
                Console.WriteLine("\n\nSummary:");
                /*DEBUG*/
                Console.WriteLine("Counter: ");
                /*DEBUG*/
                Console.WriteLine("Root: " + root.nodeValue + " " + root.score.Item1 + " " + root.score.Item2 + " " + counter);
                /*DEBUG*/
                for (int i = 0; i < root.children.Count; i++)
                /*DEBUG*/
                {
                    /*DEBUG*/
                    Console.WriteLine("Child" + i + ": " + root.children[i].nodeValue + " " + root.children[i].score.Item1 + " " + root.children[i].score.Item2 + " " + Math.Round((float)root.children[i].nodePickScore(), 4) + " " + Math.Round((float)root.children[i].nodeSimScore(root.score.Item2), 4));
                    /*DEBUG*/
                }

                if (temp != null)
                {
                    this.root = temp;
                    pieceToGive = this.root.nodeValue;
                    /*DEBUG*/
                    Console.WriteLine("Selected: " + root.nodeValue + " with score " + root.nodePickScore());
                }
                else
                {
                    pieceToGive = 0;
                }

                time.Stop();
                return new Tuple<byte, byte>(positionToPlace, pieceToGive);
            }
            else
            {
                time.Stop();
                return new Tuple<byte, byte>(positionToPlace, this.root.nodeValue);
            }
        }

        public byte GetRootValue()
        {
            return root.nodeValue;
        }

        public Tuple<byte, byte> AIFullTurn(byte givenPiece, byte boardPosition)
        {
            MCTSNode_QAI temp = null;
            byte pieceToGive;
            byte positionToPlace = 255;
            if (this.root != null && this.root.flag[qflags.ENDBOARD]) return new Tuple<byte, byte>(255, 255);

            if (AITurnUpkeep(givenPiece, boardPosition) == null)
            {
                temp = AITurnPart(2000);

                positionToPlace = this.root.nodeValue;
                temp = AITurnPart(2000);

                pieceToGive = this.root.nodeValue;

                time.Stop();
                return new Tuple<byte, byte>(positionToPlace, pieceToGive);
            }
            else
            {
                time.Stop();
                return new Tuple<byte, byte>(positionToPlace, this.root.nodeValue);
            }
        }
    }
}