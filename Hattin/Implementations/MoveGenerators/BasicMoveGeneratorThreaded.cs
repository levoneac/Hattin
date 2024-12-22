using Hattin.Extensions.NormalPiece;
using Hattin.Extensions.SideToMove;
using Hattin.Extensions.Squares;
using Hattin.Interfaces;
using Hattin.Types;
using Hattin.Utils;

namespace Hattin.Implementations.MoveGenerators
{
    //TODO:
    //refactor attackedSquares-methods to automatically choose sliding or jumping
    public class BasicMoveGeneratorThreaded : IMoveGenerator
    {
        public delegate bool SearchConstraint(GeneratedMove move);
        public BoardState Board { get; private set; } //make into interface later
        public int LastGeneratedPly { get; private set; }
        public BasicMoveGeneratorThreaded(BoardState board)
        {
            Board = board;
            LastGeneratedPly = -1;
        }
        public bool Test(SearchConstraint t)
        {
            t(new GeneratedMove());
            return true;
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

        //previousPosition is used as a way to ignore the piece on the given square in the board state
        //blockedSquare is used to signal that the given square is now blocked (useful for castling when the king now blocks the rook on the other side)
        public List<AttackProjection> GenerateSlidingAttackedSquares(NormalPiece piece, SideToMove opponentColor, BoardSquare currentPosition, BoardSquare previousPosition, BoardSquare blockedSquare = BoardSquare.NoSquare)
        {
            List<AttackProjection> attackedSquares = [];
            BoardSquare positionAfterOffset;
            int curSequence = 0;
            int xRayLevel = 0;

            foreach (int offset in NormalPieceOffsets.GetOffsetFromNormalPiece(piece))
            {
                positionAfterOffset = currentPosition + offset;
                if (positionAfterOffset == blockedSquare) { continue; }
                while ((BoardSquare)positionAfterOffset.ToBase64Int() != BoardSquare.NoSquare)
                {
                    SideToMove colorOfPieceOnSquare = Board.PieceProperties.GetColorOfPieceOnSquare(positionAfterOffset);
                    NormalPiece pieceOnSquare = Board.PieceProperties.GetPieceOnSquare(positionAfterOffset);
                    if (colorOfPieceOnSquare == opponentColor)
                    {
                        attackedSquares.Add(new AttackProjection(piece, positionAfterOffset, pieceOnSquare, SquareInteraction.Attacking, curSequence, false, xRayLevel));
                        positionAfterOffset += offset;
                        xRayLevel++;
                    }
                    else if (colorOfPieceOnSquare == SideToMove.None || positionAfterOffset == previousPosition)
                    {
                        attackedSquares.Add(new AttackProjection(piece, positionAfterOffset, NormalPiece.Empty, SquareInteraction.ControllingEmpty, curSequence, false, xRayLevel));
                        positionAfterOffset += offset;
                    }
                    else if (colorOfPieceOnSquare == Board.SideToMove)
                    {
                        attackedSquares.Add(new AttackProjection(piece, positionAfterOffset, pieceOnSquare, SquareInteraction.Defending, curSequence, false, xRayLevel));
                        positionAfterOffset += offset;
                        xRayLevel++;
                    }
                    else
                    {
                        break;
                    }
                }
                xRayLevel = 0;
                curSequence++;
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
        public List<AttackProjection> GenerateJumpingAttackedSquares(NormalPiece piece, SideToMove opponentColor, BoardSquare currentPosition, BoardSquare previousPosition, BoardSquare blockedSquare = BoardSquare.NoSquare)
        {
            List<AttackProjection> attackedSquares = [];
            BoardSquare positionAfterOffset;
            int curSequence = 0;
            foreach (int offset in NormalPieceOffsets.GetOffsetFromNormalPiece(piece))
            {
                positionAfterOffset = currentPosition + offset;
                if (positionAfterOffset == blockedSquare) { continue; }

                if ((BoardSquare)positionAfterOffset.ToBase64Int() == BoardSquare.NoSquare) { continue; }

                SideToMove colorOfPieceOnSquare = Board.PieceProperties.GetColorOfPieceOnSquare(positionAfterOffset);
                NormalPiece pieceOnSquare = Board.PieceProperties.GetPieceOnSquare(positionAfterOffset);
                if (colorOfPieceOnSquare == opponentColor)
                {
                    attackedSquares.Add(new AttackProjection(piece, positionAfterOffset, pieceOnSquare, SquareInteraction.Attacking, curSequence, false));
                }
                else if (colorOfPieceOnSquare == SideToMove.None || positionAfterOffset == previousPosition)
                {
                    attackedSquares.Add(new AttackProjection(piece, positionAfterOffset, NormalPiece.Empty, SquareInteraction.ControllingEmpty, curSequence, false));
                }
                else if (colorOfPieceOnSquare == Board.SideToMove)
                {
                    attackedSquares.Add(new AttackProjection(piece, positionAfterOffset, pieceOnSquare, SquareInteraction.Defending, curSequence, false));
                }
                curSequence++;
            }
            return attackedSquares;
        }

        //nearing spaghetti
        public List<GeneratedMove> GeneratePawnMoves()
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
                        attackedSquares = GeneratePawnAttackedSquares(pieceColor, opponentColor, positionAfterOffset, positionAfterOffset);
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
            return possibleMoves;
        }

        //TODO: Handle promotions
        public List<AttackProjection> GeneratePawnAttackedSquares(NormalPiece pawnColor, SideToMove opponentColor, BoardSquare currentPosition, BoardSquare placeholder)
        {
            List<AttackProjection> attackedSquares = [];
            BoardSquare positionAfterOffset;
            bool isPromotion = false;
            BoardSquare[] promotionSquares = NormalPiecePromotionSquares.GetPromotionSquareFromNormalPiece(pawnColor);
            int[] offsets = NormalPieceOffsets.GetOffsetFromNormalPiece(pawnColor);
            int curSequence = 0;

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
                    attackedSquares.Add(new AttackProjection(pawnColor, positionAfterOffset, pieceOnSquare, SquareInteraction.Attacking, curSequence, isPromotion));
                }
                //technically not a legal moves in the current position
                else if (colorOfPieceOnSquare == Board.SideToMove)
                {
                    attackedSquares.Add(new AttackProjection(pawnColor, positionAfterOffset, pieceOnSquare, SquareInteraction.Defending, curSequence, isPromotion));
                }
                else if (colorOfPieceOnSquare == SideToMove.None)
                {
                    attackedSquares.Add(new AttackProjection(pawnColor, positionAfterOffset, NormalPiece.Empty, SquareInteraction.ControllingEmpty, curSequence, isPromotion));
                }
                curSequence++;
            }
            return attackedSquares;
        }

        public List<GeneratedMove> GenerateKnightMoves()
        {
            NormalPiece pieceColor = Board.SideToMove == SideToMove.White ? NormalPiece.WhiteKnight : NormalPiece.BlackKnight;
            SideToMove opponentColor = Board.SideToMove.ToOppositeColor();
            return GenerateJumpingMoves(pieceColor, opponentColor);
        }

        public List<GeneratedMove> GenerateBishopMoves()
        {
            NormalPiece pieceColor = Board.SideToMove == SideToMove.White ? NormalPiece.WhiteBishop : NormalPiece.BlackBishop;
            SideToMove opponentColor = Board.SideToMove.ToOppositeColor();
            return GenerateSlidingMoves(pieceColor, opponentColor);

        }

        public List<GeneratedMove> GenerateRookMoves()
        {
            NormalPiece pieceColor = Board.SideToMove == SideToMove.White ? NormalPiece.WhiteRook : NormalPiece.BlackRook;
            SideToMove opponentColor = Board.SideToMove.ToOppositeColor();
            return GenerateSlidingMoves(pieceColor, opponentColor);
        }

        public List<GeneratedMove> GenerateQueenMoves()
        {
            NormalPiece pieceColor = Board.SideToMove == SideToMove.White ? NormalPiece.WhiteQueen : NormalPiece.BlackQueen;
            SideToMove opponentColor = Board.SideToMove.ToOppositeColor();
            return GenerateSlidingMoves(pieceColor, opponentColor);
        }

        //MINOR BUG: H1 and H8 still has a rook (PieceOnSquare property) in them after castling.
        public List<AttackProjection> GenerateCastleAttackSquares(NormalPiece king, NormalPiece rook, BoardSquare kingDest, BoardSquare rookDest, SideToMove opponentColor)
        {
            List<AttackProjection> attackProjections = GenerateSlidingAttackedSquares(rook, opponentColor, rookDest, Board.PieceProperties.GetPiecePositions(king)[0], kingDest); //rook moves
            attackProjections.AddRange(GenerateJumpingAttackedSquares(king, opponentColor, kingDest, BoardSquare.NoSquare, rookDest));//king moves
            return attackProjections;

        }

        //refactor
        //Assumes that Board.CastleRights is properly updated when king or rook has moved for example
        public List<GeneratedMove> GenerateCastlingMoves(NormalPiece pieceColor, SideToMove opponentColor)
        {
            List<GeneratedMove> moves = new List<GeneratedMove>();
            if (Board.IsCheck)
            {
                return moves;
            }

            if (pieceColor.ToColor() == SideToMove.White)
            {
                if (Board.CastleRights.HasFlag(CastleRights.WhiteKingsideCastle))
                {
                    bool canCastle = true;
                    foreach (BoardSquare square in NormalPieceCastleSquares.WhiteKingsideCastle)
                    {
                        if (Board.PieceProperties.GetAttackCountOnSquare(square).AttackTotals.Black > 0 || Board.PieceProperties.GetPieceOnSquare(square) != NormalPiece.Empty)
                        {
                            canCastle = false;
                            break;
                        }
                    }
                    if (canCastle)
                    {
                        List<AttackProjection> attackProjections = GenerateCastleAttackSquares(NormalPiece.WhiteKing, NormalPiece.WhiteRook, BoardSquare.G1, BoardSquare.F1, opponentColor);
                        moves.Add(new GeneratedMove(NormalPiece.WhiteKing, BoardSquare.E1, BoardSquare.G1, attackProjections, rookCastleSquare: BoardSquare.F1));
                    }

                }
                if (Board.CastleRights.HasFlag(CastleRights.WhiteQueensideCastle))
                {
                    bool canCastle = true;
                    foreach (BoardSquare square in NormalPieceCastleSquares.WhiteQueensideCastle)
                    {
                        if (Board.PieceProperties.GetAttackCountOnSquare(square).AttackTotals.Black > 0 || Board.PieceProperties.GetPieceOnSquare(square) != NormalPiece.Empty)
                        {
                            canCastle = false;
                            break;
                        }
                    }
                    if (canCastle)
                    {
                        List<AttackProjection> attackProjections = GenerateCastleAttackSquares(NormalPiece.WhiteKing, NormalPiece.WhiteRook, BoardSquare.C1, BoardSquare.D1, opponentColor);
                        moves.Add(new GeneratedMove(NormalPiece.WhiteKing, BoardSquare.C1, BoardSquare.G1, attackProjections, rookCastleSquare: BoardSquare.D1));
                    }
                }
            }
            else if (pieceColor.ToColor() == SideToMove.Black)
            {
                if (Board.CastleRights.HasFlag(CastleRights.BlackKingsideCastle))
                {
                    bool canCastle = true;
                    foreach (BoardSquare square in NormalPieceCastleSquares.BlackKingsideCastle)
                    {
                        if (Board.PieceProperties.GetAttackCountOnSquare(square).AttackTotals.Black > 0 || Board.PieceProperties.GetPieceOnSquare(square) != NormalPiece.Empty)
                        {
                            canCastle = false;
                            break;
                        }
                    }
                    if (canCastle)
                    {
                        List<AttackProjection> attackProjections = GenerateCastleAttackSquares(NormalPiece.BlackKing, NormalPiece.BlackRook, BoardSquare.G8, BoardSquare.F8, opponentColor);
                        moves.Add(new GeneratedMove(NormalPiece.BlackKing, BoardSquare.G8, BoardSquare.G1, attackProjections, rookCastleSquare: BoardSquare.F8));
                    }
                }
                if (Board.CastleRights.HasFlag(CastleRights.BlackQueensideCastle))
                {
                    bool canCastle = true;
                    foreach (BoardSquare square in NormalPieceCastleSquares.BlackQueensideCastle)
                    {
                        if (Board.PieceProperties.GetAttackCountOnSquare(square).AttackTotals.Black > 0 || Board.PieceProperties.GetPieceOnSquare(square) != NormalPiece.Empty)
                        {
                            canCastle = false;
                            break;
                        }
                    }
                    if (canCastle)
                    {
                        List<AttackProjection> attackProjections = GenerateCastleAttackSquares(NormalPiece.BlackKing, NormalPiece.BlackRook, BoardSquare.C8, BoardSquare.D8, opponentColor);
                        moves.Add(new GeneratedMove(NormalPiece.WhiteKing, BoardSquare.C8, BoardSquare.G1, attackProjections, rookCastleSquare: BoardSquare.D8));
                    }
                }
            }
            return moves;
        }

        public List<GeneratedMove> GenerateKingNormalMoves(NormalPiece piece, SideToMove opponentColor)
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

                    ColorCount attackCounts = Board.PieceProperties.GetAttackCountOnSquare(positionAfterOffset).AttackTotals;
                    if (opponentColor == SideToMove.White && attackCounts.White > 0) { continue; }
                    else if (opponentColor == SideToMove.Black && attackCounts.Black > 0) { continue; }

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

        public List<GeneratedMove> GenerateKingMoves()
        {
            NormalPiece pieceColor = Board.SideToMove == SideToMove.White ? NormalPiece.WhiteKing : NormalPiece.BlackKing;
            SideToMove opponentColor = Board.SideToMove.ToOppositeColor();

            List<GeneratedMove> moves = GenerateKingNormalMoves(pieceColor, opponentColor);
            moves.AddRange(GenerateCastlingMoves(pieceColor, opponentColor));
            return moves;
        }

        //Slower than non-threaded
        public List<AttackProjection> GenerateAllAttackedSquaresThreaded()
        {
            List<AttackProjection> attackProjections = new List<AttackProjection>();
            object attackProjectionsLock = new object();
            List<ManualResetEvent> events = new List<ManualResetEvent>();

            ManualResetEvent ensureCompleteSliding = new ManualResetEvent(false);
            ThreadPool.QueueUserWorkItem((object? parameters) =>
            {
                List<AttackProjection> aggregate = new List<AttackProjection>();
                foreach (var sliders in NormalPieceMovement.SlidingPieces)
                {
                    foreach (var square in Board.PieceProperties.PiecePositions[(int)sliders])
                    {
                        //attackProjections.AddRange(GenerateSlidingAttackedSquares(sliders, sliders.ToColor().ToOppositeColor(), square, BoardSquare.NoSquare));
                        aggregate.AddRange(GenerateSlidingAttackedSquares(sliders, sliders.ToColor().ToOppositeColor(), square, BoardSquare.NoSquare));
                    }
                }
                LockAttackProjectionsAddRange(aggregate);
                ensureCompleteSliding.Set();
            });
            events.Add(ensureCompleteSliding);

            ManualResetEvent ensureCompleteJumping = new ManualResetEvent(false);
            ThreadPool.QueueUserWorkItem((object? parameters) =>
            {
                List<AttackProjection> aggregate = new List<AttackProjection>();
                foreach (var jumpers in NormalPieceMovement.JumpingPieces)
                {
                    foreach (var square in Board.PieceProperties.PiecePositions[(int)jumpers])
                    {
                        //attackProjections.AddRange(GenerateSlidingAttackedSquares(sliders, sliders.ToColor().ToOppositeColor(), square, BoardSquare.NoSquare));
                        aggregate.AddRange(GenerateJumpingAttackedSquares(jumpers, jumpers.ToColor().ToOppositeColor(), square, BoardSquare.NoSquare));
                    }
                }
                LockAttackProjectionsAddRange(aggregate);
                ensureCompleteJumping.Set();
            });
            events.Add(ensureCompleteJumping);


            ManualResetEvent ensureCompletePawns = new ManualResetEvent(false);
            ThreadPool.QueueUserWorkItem((object? parameters) =>
            {
                List<AttackProjection> aggregate = new List<AttackProjection>();
                foreach (var pawns in NormalPieceMovement.PawnMoves)
                {
                    foreach (var square in Board.PieceProperties.PiecePositions[(int)pawns])
                    {
                        //attackProjections.AddRange(GenerateSlidingAttackedSquares(sliders, sliders.ToColor().ToOppositeColor(), square, BoardSquare.NoSquare));
                        aggregate.AddRange(GeneratePawnAttackedSquares(pawns, pawns.ToColor().ToOppositeColor(), square, BoardSquare.NoSquare));
                    }
                }
                LockAttackProjectionsAddRange(aggregate);
                ensureCompletePawns.Set();
            });
            events.Add(ensureCompletePawns);


            WaitHandle.WaitAll(events.ToArray(), Timeout.Infinite);

            return attackProjections;


            void LockAttackProjectionsAddRange(List<AttackProjection> attacks)
            {
                lock (attackProjectionsLock)
                {
                    attackProjections.AddRange(attacks);
                }
            }
        }

        public List<AttackProjection> GenerateAllAttackedSquares()
        {
            List<AttackProjection> attackProjections = new List<AttackProjection>();

            foreach (var sliders in NormalPieceMovement.SlidingPieces)
            {
                foreach (var square in Board.PieceProperties.PiecePositions[(int)sliders])
                {
                    attackProjections.AddRange(GenerateSlidingAttackedSquares(sliders, sliders.ToColor().ToOppositeColor(), square, BoardSquare.NoSquare));
                }
            }

            foreach (var jumpers in NormalPieceMovement.JumpingPieces)
            {
                foreach (var square in Board.PieceProperties.PiecePositions[(int)jumpers])
                {
                    attackProjections.AddRange(GenerateJumpingAttackedSquares(jumpers, jumpers.ToColor().ToOppositeColor(), square, BoardSquare.NoSquare));
                }
            }

            foreach (var pawns in NormalPieceMovement.PawnMoves)
            {
                foreach (var square in Board.PieceProperties.PiecePositions[(int)pawns])
                {
                    attackProjections.AddRange(GeneratePawnAttackedSquares(pawns, pawns.ToColor().ToOppositeColor(), square, BoardSquare.NoSquare));
                }
            }
            return attackProjections;
        }

        public List<GeneratedMove> GenerateAllLegalMoves(List<Func<GeneratedMove, bool>>? constraints)
        {
            List<GeneratedMove> Moves = new List<GeneratedMove>();
            object moveLock = new object();
            LastGeneratedPly = Board.PlyCounter;

            List<ManualResetEvent> events = new List<ManualResetEvent>();
            List<Func<List<GeneratedMove>>> jobs = new List<Func<List<GeneratedMove>>>([GeneratePawnMoves, GenerateBishopMoves, GenerateKnightMoves, GenerateRookMoves, GenerateQueenMoves, GenerateKingMoves]);

            foreach (Func<List<GeneratedMove>> job in jobs)
            {
                ManualResetEvent ensureComplete = new ManualResetEvent(false);
                ThreadPool.QueueUserWorkItem((object? parameters) =>
                        {
                            LockMovesAddRange(job.Invoke());
                            ensureComplete.Set();
                        });
                events.Add(ensureComplete);
            }
            WaitHandle.WaitAll(events.ToArray(), Timeout.Infinite);
            events.ForEach(e => e.Dispose());



            return Moves;


            void LockMovesAddRange(List<GeneratedMove> moves)
            {
                lock (moveLock)
                {
                    Moves.AddRange(moves);
                }
            }
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