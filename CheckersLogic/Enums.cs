namespace Ex05.CheckersLogic
{
    public class Enums
    {
        public enum eCoinType
        {
            None,
            X,
            O
        }
        public enum ePlayersType
        {
            RealPlayer,
            Computer
        }

        public enum eStatus    
        {
            None,
            Win,
            Loose,
            Draw        
        }

        public enum eVerticalDirections
        {
            SamePlace,
            Forword,
            Backword
        }

        public enum eHorizontalDirections
        {
            SamePlace,
            Right,
            Left
        }

        public enum eMoveType
        {
            None,
            Step,
            Eat,
        }
    }
}
