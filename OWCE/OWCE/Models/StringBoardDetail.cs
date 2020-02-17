using System;
namespace OWCE.Models
{
    public class StringBoardDetail : BaseBoardDetail
    {
        private string _value;
        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (_value != value)
                {
                    _value = value;
                    OnPropertyChanged();
                }
            }
        }

        public StringBoardDetail(string name) : base(name)
        {

        }
    }
}
