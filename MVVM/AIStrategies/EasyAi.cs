using Battleship.MVVM.Model;

namespace Battleship.MVVM.AIStrategies
{
    public class EasyAi : IAIStrategy
    {
        public bool MakeMove(GameBoard gameBoard)
        {
            return GameBoard.ShootCell(gameBoard.GetRandomCell());
        }
    }
}
