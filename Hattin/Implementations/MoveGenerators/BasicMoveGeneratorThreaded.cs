using Hattin.Extensions.SideToMove;
using Hattin.Extensions.Squares;
using Hattin.Interfaces;
using Hattin.Types;

namespace Hattin.Implementations.MoveGenerators
{
    //TODO:
    //refactor attackedSquares-methods to automatically choose sliding or jumping
    public class BasicMoveGeneratorThreaded : IMoveGenerator
    {
        public BoardState Board { get; private set; } //make into interface later
        public List<GeneratedMove> Moves { get; private set; }
        private static readonly object moveLock = new object();
        public BasicMoveGeneratorThreaded(BoardState board)
        {
            Board = board;
            Moves = new List<GeneratedMove>();
        }

        private void LockMovesAddRange(List<GeneratedMove> moves)
        {
            lock (moveLock)
            {
                Moves.AddRange(moves);
            }
        }
        public List<GeneratedMove> GenerateSlidingMoves(NormalPiece piece, SideToMove opponentColor)
        {
            List<GeneratedMove> possibleMoves = [];
            BoardSquare positionAfterOffset;

            //foreach square where a given piece is placed
            foreach (BoardSquare piecePosition in Board.PieceProperties.PiecePositions[(int)piece])
            {
                //foreach offset of the given piece
                foreach (int offset in NormalPieceOffsets.GetOffsetFromNormalPiece(piece))
                {
                    positionAfterOffset = piecePosition + offset;

                    //while the square is on the board, check if the piece collides with something
                    while ((BoardSquare)positionAfterOffset.ToBase64Int() != BoardSquare.NoSquare)
                    {
                        SideToMove colorOfPieceOnSquare = Board.PieceProperties.GetColorOfPieceOnSquare(positionAfterOffset);
                        if (colorOfPieceOnSquare == opponentColor)
                        {
                            //set capture flag
                            List<AttackProjection> attackedSquares = GenerateSlidingAttackedSquares(piece, opponentColor, positionAfterOffset, piecePosition);
                            possibleMoves.Add(new GeneratedMove(piece, piecePosition, positionAfterOffset, attackedSquares, BoardSquare.NoSquare, false, true));
                            break;
                        }
                        else if (colorOfPieceOnSquare == SideToMove.None)
                        {
                            List<AttackProjection> attackedSquares = GenerateSlidingAttackedSquares(piece, opponentColor, positionAfterOffset, piecePosition);
                            possibleMoves.Add(new GeneratedMove(piece, piecePosition, positionAfterOffset, attackedSquares, BoardSquare.NoSquare, false, false));
                            positionAfterOffset += offset;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            return possibleMoves;
        }

        public List<AttackProjection> GenerateSlidingAttackedSquares(NormalPiece piece, SideToMove opponentColor, BoardSquare currentPosition, BoardSquare previousPosition)
        {
            List<AttackProjection> attackedSquares = [];
            BoardSquare positionAfterOffset;

            foreach (int offset in NormalPieceOffsets.GetOffsetFromNormalPiece(piece))
            {
                positionAfterOffset = currentPosition + offset;

                while ((BoardSquare)positionAfterOffset.ToBase64Int() != BoardSquare.NoSquare)
                {
                    SideToMove colorOfPieceOnSquare = Board.PieceProperties.GetColorOfPieceOnSquare(positionAfterOffset);
                    NormalPiece pieceOnSquare = Board.PieceProperties.GetPieceOnSquare(positionAfterOffset);
                    if (colorOfPieceOnSquare == opponentColor)
                    {
                        attackedSquares.Add(new AttackProjection(piece, positionAfterOffset, pieceOnSquare, SquareInteraction.Attacking, false));
                        break;
                    }
                    else if (colorOfPieceOnSquare == SideToMove.None || positionAfterOffset == previousPosition)
                    {
                        attackedSquares.Add(new AttackProjection(piece, positionAfterOffset, NormalPiece.Empty, SquareInteraction.ControllingEmpty, false));
                        positionAfterOffset += offset;
                    }
                    else if (colorOfPieceOnSquare == Board.SideToMove)
                    {
                        attackedSquares.Add(new AttackProjection(piece, positionAfterOffset, pieceOnSquare, SquareInteraction.Defending, false));
                        break;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return attackedSquares;
        }

        public List<GeneratedMove> GenerateJumpingMoves(NormalPiece piece, SideToMove opponentColor)
        {
            List<GeneratedMove> possibleMoves = [];
            BoardSquare positionAfterOffset;
            BoardSquare checkIfNoSquare;

            foreach (BoardSquare piecePosition in Board.PieceProperties.PiecePositions[(int)piece])
            {
                foreach (int offset in NormalPieceOffsets.GetOffsetFromNormalPiece(piece))
                {
                    positionAfterOffset = piecePosition + offset;

                    checkIfNoSquare = (BoardSquare)positionAfterOffset.ToBase64Int();
                    if (checkIfNoSquare == BoardSquare.NoSquare) { continue; }

                    SideToMove colorOfPieceOnSquare = Board.PieceProperties.GetColorOfPieceOnSquare(positionAfterOffset);
                    if (colorOfPieceOnSquare == opponentColor)
                    {
                        List<AttackProjection> attackedSquares = GenerateJumpingAttackedSquares(piece, opponentColor, positionAfterOffset, piecePosition);
                        possibleMoves.Add(new GeneratedMove(piece, piecePosition, positionAfterOffset, attackedSquares, BoardSquare.NoSquare, false, true));
                    }
                    else if (colorOfPieceOnSquare == SideToMove.None)
                    {
                        List<AttackProjection> attackedSquares = GenerateJumpingAttackedSquares(piece, opponentColor, positionAfterOffset, piecePosition);
                        possibleMoves.Add(new GeneratedMove(piece, piecePosition, positionAfterOffset, attackedSquares, BoardSquare.NoSquare, false, false));
                    }
                }
            }
            return possibleMoves;
        }
        public List<AttackProjection> GenerateJumpingAttackedSquares(NormalPiece piece, SideToMove opponentColor, BoardSquare currentPosition, BoardSquare previousPosition)
        {
            List<AttackProjection> attackedSquares = [];
            BoardSquare positionAfterOffset;

            foreach (int offset in NormalPieceOffsets.GetOffsetFromNormalPiece(piece))
            {
                positionAfterOffset = currentPosition + offset;

                if ((BoardSquare)positionAfterOffset.ToBase64Int() == BoardSquare.NoSquare) { continue; }

                SideToMove colorOfPieceOnSquare = Board.PieceProperties.GetColorOfPieceOnSquare(positionAfterOffset);
                NormalPiece pieceOnSquare = Board.PieceProperties.GetPieceOnSquare(positionAfterOffset);
                if (colorOfPieceOnSquare == opponentColor)
                {
                    attackedSquares.Add(new AttackProjection(piece, positionAfterOffset, pieceOnSquare, SquareInteraction.Attacking, false));
                }
                else if (colorOfPieceOnSquare == SideToMove.None || positionAfterOffset == previousPosition)
                {
                    attackedSquares.Add(new AttackProjection(piece, positionAfterOffset, NormalPiece.Empty, SquareInteraction.ControllingEmpty, false));
                }
                else if (colorOfPieceOnSquare == Board.SideToMove)
                {
                    attackedSquares.Add(new AttackProjection(piece, positionAfterOffset, pieceOnSquare, SquareInteraction.Defending, false));
                }

            }
            return attackedSquares;
        }

        //nearing spaghetti
        public void GeneratePawnMoves(object? state)
        {
            List<GeneratedMove> possibleMoves = [];
            NormalPiece pieceColor = Board.SideToMove == SideToMove.White ? NormalPiece.WhitePawn : NormalPiece.BlackPawn;
            SideToMove opponentColor = Board.SideToMove.ToOppositeColor();
            BoardSquare positionAfterOffset;
            BoardSquare checkIfNoSquare;
            BoardSquare[] startingSquares = NormalPieceStartingSquares.GetStartingSquareFromNormalPiece(pieceColor);
            BoardSquare[] promotionSquares = NormalPiecePromotionSquares.GetPromotionSquareFromNormalPiece(pieceColor);

            foreach (BoardSquare pawnPosition in Board.PieceProperties.PiecePositions[(int)pieceColor])
            {
                bool prevIsBlocked = false;
                bool isCapture = false;
                bool isPromotion = false;
                int offsetIndex = 1; //if pawn is not on starting square, skip the first index of offsets. Maybe find a clearer way to represent this
                if (startingSquares.Contains(pawnPosition))
                {
                    offsetIndex = 0;
                }
                //get correct color offsets for pawn 
                int[] offsets = NormalPieceOffsets.GetOffsetFromNormalPiece(pieceColor);

                //loop over each of the offsets for each pawn
                for (; offsetIndex < offsets.Length; offsetIndex++)
                {
                    List<AttackProjection> attackedSquares = [];
                    positionAfterOffset = pawnPosition + offsets[offsetIndex];

                    //if the move doesnt put the pawn off board
                    checkIfNoSquare = (BoardSquare)positionAfterOffset.ToBase64Int();
                    if (checkIfNoSquare == BoardSquare.NoSquare) { continue; }

                    if (promotionSquares.Contains(positionAfterOffset))
                    {
                        isPromotion = true;

                        //Knight
                        NormalPiece knightColor = Board.SideToMove == SideToMove.White ? NormalPiece.WhiteKnight : NormalPiece.BlackKnight;
                        attackedSquares.AddRange(GenerateJumpingAttackedSquares(knightColor, opponentColor, positionAfterOffset, pawnPosition));

                        //Bishop
                        NormalPiece bishopColor = Board.SideToMove == SideToMove.White ? NormalPiece.WhiteBishop : NormalPiece.BlackBishop;
                        attackedSquares.AddRange(GenerateSlidingAttackedSquares(bishopColor, opponentColor, positionAfterOffset, pawnPosition));

                        //Rook
                        NormalPiece rookColor = Board.SideToMove == SideToMove.White ? NormalPiece.WhiteRook : NormalPiece.BlackRook;
                        attackedSquares.AddRange(GenerateSlidingAttackedSquares(rookColor, opponentColor, positionAfterOffset, pawnPosition));

                        //Queen
                        //NormalPiece queenColor = Board.SideToMove == SideToMove.White ? NormalPiece.WhiteQueen : NormalPiece.BlackQueen;
                        //attackedSquares.AddRange(GenerateSlidingAttackedSquares(queenColor, opponentColor, positionAfterOffset, pawnPosition));
                    }
                    else
                    {
                        attackedSquares = GeneratePawnAttackedSquares(pieceColor, opponentColor, positionAfterOffset);
                    }
                    //if its an attacking move and (square is occupied by opponent or it is an enpassant square)
                    SideToMove colorOfPieceOnSquare = Board.PieceProperties.GetColorOfPieceOnSquare(positionAfterOffset);
                    if (offsetIndex > 1 && (colorOfPieceOnSquare == opponentColor || positionAfterOffset == Board.EnPassantSquare))
                    {
                        //set capture flag
                        possibleMoves.Add(new GeneratedMove(pieceColor, pawnPosition, positionAfterOffset, attackedSquares, BoardSquare.NoSquare, isPromotion, true));
                    }
                    //else if the square is not occupied and its a normal forward move
                    else if (colorOfPieceOnSquare == SideToMove.None && offsetIndex < 2)
                    {
                        //if its a double move, check if the pawn moved into another piece
                        if (offsetIndex == 0)
                        {
                            BoardSquare squareBehind = positionAfterOffset - offsets[1];
                            SideToMove pieceBehind = Board.PieceProperties.GetColorOfPieceOnSquare(squareBehind); //could check onesquare move first and set blocking flag instead
                            if (pieceBehind != SideToMove.None) { continue; }

                            //set enpassant flag
                            possibleMoves.Add(new GeneratedMove(pieceColor, pawnPosition, positionAfterOffset, attackedSquares, squareBehind, isPromotion, false));
                        }

                        else
                        {
                            possibleMoves.Add(new GeneratedMove(pieceColor, pawnPosition, positionAfterOffset, attackedSquares, BoardSquare.NoSquare, isPromotion, false));
                        }
                    }
                }
            }
            LockMovesAddRange(possibleMoves);
        }

        //TODO: Handle promotions
        public List<AttackProjection> GeneratePawnAttackedSquares(NormalPiece pawnColor, SideToMove opponentColor, BoardSquare currentPosition)
        {
            List<AttackProjection> attackedSquares = [];
            BoardSquare positionAfterOffset;
            bool isPromotion = false;
            BoardSquare[] promotionSquares = NormalPiecePromotionSquares.GetPromotionSquareFromNormalPiece(pawnColor);
            int[] offsets = NormalPieceOffsets.GetOffsetFromNormalPiece(pawnColor);

            //offsetindex = 2 because we only check attacking moves
            for (int offsetIndex = 2; offsetIndex < offsets.Length; offsetIndex++)
            {
                positionAfterOffset = currentPosition + offsets[offsetIndex];

                //if the move doesnt put the pawn off board
                if ((BoardSquare)positionAfterOffset.ToBase64Int() == BoardSquare.NoSquare) { continue; }

                if (promotionSquares.Contains(positionAfterOffset))
                {
                    isPromotion = true;
                }
                //if its an attacking move
                SideToMove colorOfPieceOnSquare = Board.PieceProperties.GetColorOfPieceOnSquare(positionAfterOffset);
                NormalPiece pieceOnSquare = Board.PieceProperties.GetPieceOnSquare(positionAfterOffset);
                if (colorOfPieceOnSquare == opponentColor)
                {
                    attackedSquares.Add(new AttackProjection(pawnColor, positionAfterOffset, pieceOnSquare, SquareInteraction.Attacking, isPromotion));
                }
                //technically not a legal moves in the current position
                else if (colorOfPieceOnSquare == Board.SideToMove)
                {
                    attackedSquares.Add(new AttackProjection(pawnColor, positionAfterOffset, pieceOnSquare, SquareInteraction.Defending, isPromotion));
                }
                else if (colorOfPieceOnSquare == SideToMove.None)
                {
                    attackedSquares.Add(new AttackProjection(pawnColor, positionAfterOffset, NormalPiece.Empty, SquareInteraction.ControllingEmpty, isPromotion));
                }
            }
            return attackedSquares;
        }

        public void GenerateKnightMoves(object? state)
        {
            NormalPiece pieceColor = Board.SideToMove == SideToMove.White ? NormalPiece.WhiteKnight : NormalPiece.BlackKnight;
            SideToMove opponentColor = Board.SideToMove.ToOppositeColor();
            LockMovesAddRange(GenerateJumpingMoves(pieceColor, opponentColor));
        }

        public void GenerateBishopMoves(object? state)
        {
            NormalPiece pieceColor = Board.SideToMove == SideToMove.White ? NormalPiece.WhiteBishop : NormalPiece.BlackBishop;
            SideToMove opponentColor = Board.SideToMove.ToOppositeColor();
            LockMovesAddRange(GenerateSlidingMoves(pieceColor, opponentColor));

        }

        public void GenerateRookMoves(object? state)
        {
            NormalPiece pieceColor = Board.SideToMove == SideToMove.White ? NormalPiece.WhiteRook : NormalPiece.BlackRook;
            SideToMove opponentColor = Board.SideToMove.ToOppositeColor();
            LockMovesAddRange(GenerateSlidingMoves(pieceColor, opponentColor));
        }

        public void GenerateQueenMoves(object? state)
        {
            NormalPiece pieceColor = Board.SideToMove == SideToMove.White ? NormalPiece.WhiteQueen : NormalPiece.BlackQueen;
            SideToMove opponentColor = Board.SideToMove.ToOppositeColor();
            LockMovesAddRange(GenerateSlidingMoves(pieceColor, opponentColor));
        }

        public void GenerateKingMoves(object? state)
        {
            NormalPiece pieceColor = Board.SideToMove == SideToMove.White ? NormalPiece.WhiteKing : NormalPiece.BlackKing;
            SideToMove opponentColor = Board.SideToMove.ToOppositeColor();

            //normal moves
            List<GeneratedMove> moves = GenerateJumpingMoves(pieceColor, opponentColor);

            //Castling
            //check that the king isnt in check
            //check if squares have 0 attack coverage of enemy pieces
            //check that king and rook have not moved
            //check that the squares between them are Empty

            LockMovesAddRange(moves);
        }

        public List<GeneratedMove> GeneratAllLegalMoves()
        {
            Moves.Clear();
            List<ManualResetEvent> events = new List<ManualResetEvent>();
            List<Action<object?>> jobs = new List<Action<object?>>([GeneratePawnMoves, GenerateBishopMoves, GenerateKnightMoves, GenerateRookMoves, GenerateQueenMoves, GenerateKingMoves]);

            foreach (Action<object?> job in jobs)
            {
                ManualResetEvent ensureComplete = new ManualResetEvent(false);
                ThreadPool.QueueUserWorkItem((object? parameters) =>
                {
                    job.Invoke(parameters);
                    ensureComplete.Set();
                });
                events.Add(ensureComplete);
            }
            WaitHandle.WaitAll(events.ToArray(), Timeout.Infinite);

            //ThreadPool.QueueUserWorkItem(new WaitCallback((task) => ensureComplete.Set()));
            //
            //ThreadPool.QueueUserWorkItem(new WaitCallback(GeneratePawnMoves));
            //ThreadPool.QueueUserWorkItem(new WaitCallback(GenerateBishopMoves));
            //ThreadPool.QueueUserWorkItem(new WaitCallback(GenerateKnightMoves));
            //ThreadPool.QueueUserWorkItem(new WaitCallback(GenerateRookMoves));
            //ThreadPool.QueueUserWorkItem(new WaitCallback(GenerateQueenMoves));
            //ThreadPool.QueueUserWorkItem(new WaitCallback(GenerateKingMoves));
            //
            //ensureComplete.WaitOne();


            return Moves;
        }

        public GeneratedMove GenerateNextValidMove()
        {
            throw new NotImplementedException();
        }
        public GeneratedMove GenerateNextCapture()
        {
            throw new NotImplementedException();
        }

        public GeneratedMove GenerateNextCheck()
        {
            throw new NotImplementedException();
        }
    }

}