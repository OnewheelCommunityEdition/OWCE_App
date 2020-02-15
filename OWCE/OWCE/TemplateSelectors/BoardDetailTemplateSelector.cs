using System;
using Xamarin.Forms;

namespace OWCE.TemplateSelectors
{
    public class BoardDetailTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Normal { get; set; }
        public DataTemplate Voltage { get; set; }
        public DataTemplate Temperature { get; set; }
        public DataTemplate Speed { get; set; }
        public DataTemplate BatteryCells { get; set; }
        public DataTemplate Angle { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (item is AngleBoardDetail)
            {
                return Angle;
            }
            if (item is BatteryCellsBoardDetail)
            {
                return BatteryCells;
            }
            else if (item is TemperatureBoardDetail)
            {
                return Temperature;
            }
            else if (item is VoltageBoardDetail)
            {
                return Voltage;
            }
            else if (item is SpeedBoardDetail)
            {
                return Speed;
            }
            else if (item is BoardDetail)
            {
                return Normal;
            }

            return null;
        }
    }
}
