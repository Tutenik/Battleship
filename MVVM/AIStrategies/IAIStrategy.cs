using Battleship.MVVM.Model;

namespace Battleship.MVVM.AIStrategies
{
    public interface IAIStrategy
    {
        bool MakeMove(GameBoard gameBoard);
    }
}