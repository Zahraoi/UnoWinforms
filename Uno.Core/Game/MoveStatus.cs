namespace Uno.Core.Game;

public enum MoveStatus
{
    Success,
    InvalidCard,
    CardNotInHand,
    WrongColorOrFace,
    WildColorRequired,
    DrawFourNotAllowed,
    GameAlreadyFinished,
    NotPlayersTurn,
    CannotDraw
}
