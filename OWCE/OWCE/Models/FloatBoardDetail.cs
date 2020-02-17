using System;
namespace OWCE.Models
{
    public class FloatBoardDetail : BaseBoardDetail
    {
        private float _value;
        public float Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (_value.AlmostEqualTo(value) == false)
                {
                    _value = value;
                    OnPropertyChanged();
                }
            }
        }

        public FloatBoardDetail(string name) : base(name)
        {

        }
    }
}
