import chess


class PerftResult:
    def __init__(self, depth) -> None:
        self.Depth = depth
        self.NumMoves = 0
        self.NumEnPassant = 0
        self.NumCaptures = 0
        self.NumCastles = 0
        self.NumChecks = 0
        self.NumPromotions = 0


class Perft:
    def __init__(self, board: chess.Board) -> None:
        self.Board: chess.Board = board
        self.MaxDepth: int = 0
        self.TotalCounts: list[PerftResult] = []

    def MoveGeneration(self, depth):
        if depth == self.MaxDepth:
            return

        curPerftAggregate = self.TotalCounts[depth]
        for move in self.Board.generate_legal_moves():
            curPerftAggregate.NumMoves += 1

            self.Board.push(move)
            self.MoveGeneration(depth + 1)
            self.Board.pop()

    def PrintTotalMovesTillDepth(self, maxDepth: int):
        self.InitializeList(maxDepth)
        self.MoveGeneration(0)
        for depth in self.TotalCounts:
            print(f"depth: {depth.Depth} -> {depth.NumMoves}")

    def PrintTotalMovesPerBranchTillDepth(self, maxDepth: int):
        totalMoves = 0
        curBranchMoves = 0
        branchNum = 1
        for move in self.Board.generate_legal_moves():
            self.InitializeList(maxDepth)

            self.Board.push(move)
            self.MoveGeneration(1)
            self.Board.pop()
            curBranchMoves = Perft.SumPerft(self.TotalCounts).NumMoves
            totalMoves += curBranchMoves
            print(
                f"branch: {branchNum} - move: {move.uci()} -> {curBranchMoves}")
            branchNum += 1
        print(f"Total moves: {totalMoves}")

    def InitializeList(self, maxDepth: int):
        self.MaxDepth = maxDepth
        self.TotalCounts.clear()
        for i in range(self.MaxDepth):
            self.TotalCounts.append(PerftResult(i + 1))

    def SumPerft(arr: list[PerftResult]) -> PerftResult:
        result = PerftResult(0)
        for elem in arr:
            result.NumMoves += elem.NumMoves
            result.NumEnPassant += elem.NumEnPassant
            result.NumCaptures += elem.NumCaptures
            result.NumCastles += elem.NumCastles
            result.NumChecks += elem.NumChecks
            result.NumPromotions += elem.NumPromotions
        return result


board = chess.Board("rnbq1k1r/pp1PbBpp/2p5/8/8/8/PPP1N1PP/RNBQK2n w Q - 0 9"
                    )
perft = Perft(board)
perft.PrintTotalMovesPerBranchTillDepth(3)
