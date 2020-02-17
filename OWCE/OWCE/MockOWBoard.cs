using System;
namespace OWCE
{
    public class MockOWBoard : OWBoard
    {
        public MockOWBoard()
        {
            _name = "MockBoard";
        }

        public MockOWBoard(string name, OWBoardType boardType)
        {
            _name = name;
            BoardType = boardType;
        }
    }
}
