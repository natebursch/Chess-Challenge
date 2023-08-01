using ChessChallenge.API;
using System;
using System.Collections.Generic;
using System.Linq;

public class MyBot : IChessBot
{
    public Move Think(Board board, Timer timer)
    {
        //public Move Think(Board board, Timer timer) => board.GetLegalMoves()[0]; 
        //        results in 24 Bot Brain Capacity
        //public Move Think(Board board, Timer timer) { return board.GetLegalMoves()[0]; }
        //        results in 25 Bot Brain Capacity


        // gets if player is white DO THIS IF MORE THAN 3 TIMES USING BOARDISWHITE
        //bool isWhite = board.IsWhiteToMove;

        List<Move> moves = new List<Move>(board.GetLegalMoves());

        Dictionary<PieceType, int> pieceValue = new Dictionary<PieceType, int>
        {
            { PieceType.Pawn, 1 },
            { PieceType.Bishop, 3 },
            { PieceType.Knight, 3 },
            { PieceType.Rook, 5 },
            { PieceType.Queen, 8 },
            { PieceType.King, 0 }
        };



        // defense
        ulong myPieces = board.IsWhiteToMove ? board.WhitePiecesBitboard : board.BlackPiecesBitboard;
        Console.WriteLine(myPieces);
        PieceList[] pieces = board.GetAllPieceLists();
        Console.WriteLine(pieces[0][0]);

        // Create lists to hold black and white pieces
        PieceList[] whitePieces = pieces.Take(6).ToArray();
        PieceList[] blackPieces = pieces.Skip(6).Take(6).ToArray();
        PieceList[] myPieceList = board.IsWhiteToMove ? whitePieces: blackPieces;

        // Iterate through each 'whitePiece' in reverse order
        for (int i = myPieceList.Length - 1; i >= 0; i--)
        {
            PieceList pieceTypes = myPieceList[i];
            // Do whatever you want with 'whitePiece' here.

            foreach (Piece piece in pieceTypes)
            {
                if (board.SquareIsAttackedByOpponent(piece.Square))
                {
                    Console.WriteLine($"{piece} is being attacked!!!");
                    // try to attack
                    Console.WriteLine("On Square"+ piece.Square.Name);
                    foreach( Move m in moves)
                    {
                        if (m.StartSquare == piece.Square)
                        {
                            // we have found a move with the piece now do offense check
                            if (OffenseFunction(m)) { return m; } 
                            // do run away check
                            // need to defense firsttttt............
                            //if(OffenseFunction(m)) { return m 
                        }
                    }

                }
            }
        }


        // NEED TO ASSIGN POINTS TO THIS INSTEAD BECAUSE IT JUST TAKES THE FIRST VALID MOVE
        int OffenseFunction(Move move)
        {
            int moveTotal = 0;
            // give a score to each move with defense being the highest score.
            // check for capture moves
            if (move.IsCapture)
            {
                // make sure move cannot be attacked by opponent
                if (!board.SquareIsAttackedByOpponent(move.TargetSquare))
                {
                    // piece is hanging --- assign a value to this
                    Console.WriteLine("piece is hanging --- assign a value to this");
                    Console.WriteLine("win is worth =" + (pieceValue[move.CapturePieceType]));
                    Console.WriteLine(move);
                    moveTotal += pieceValue[move.CapturePieceType]zzzzzzz
                else if (pieceValue[move.CapturePieceType] == pieceValue[move.MovePieceType])
                {
                    // move is a trade --- assign a value to this
                    return true;
                }
                else if (pieceValue[move.CapturePieceType] > pieceValue[move.MovePieceType])
                {
                    // move is a win
                    return true;
                }

                // If it doesn't meet any of the conditions, it's a bad move
                return false;
            }
            else
            {
                // remove captures that would hang a piece
                // if the target square is attacked and the target square is not a capture, then don't move there.
                if (board.SquareIsAttackedByOpponent(move.TargetSquare))
                {
                    return false;
                }
            }

            // It's a good move if it's not a capture move and doesn't hang a piece
            return true;
        }

        // offense
        for (int i = moves.Count - 1; i >= 0; i--)
        {
            if (OffenseFunction(moves[i])) { return moves[i]; } 
            // if it is not anyof these, we should remove it from the list. but what if it is the only move. We need to check to see.
            if (moves.Count > 1)
            {
                Console.WriteLine("Removing a bad move" + moves[i]);
                moves.Remove(moves[i]);
            }
            else
            {
                Console.WriteLine("Can't remove move");
            }
        }


        // default move
        return moves[new Random().Next(moves.Count)];
    }
}