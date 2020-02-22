using OWCE.Models;
using Xamarin.Forms;

namespace OWCE.TemplateSelectors
{
    public class BoardDetailTemplateSelector : DataTemplateSelector
    {
        public DataTemplate AmpTemplate { get; set; }
        public DataTemplate AmpHoursTemplate { get; set; }
        public DataTemplate AngleTemplate { get; set; }
        public DataTemplate BatteryCellsTemplate { get; set; }
        public DataTemplate DistanceTemplate { get; set; }
        public DataTemplate FloatTemplate { get; set; }
        public DataTemplate IntTemplate { get; set; }
        public DataTemplate SpeedTemplate { get; set; }
        public DataTemplate RideModeTemplate { get; set; }
        public DataTemplate StringTemplate { get; set; }
        public DataTemplate TemperatureTemplate { get; set; }
        public DataTemplate VoltageTemplate { get; set; }
        
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (item is AmpBoardDetail)
            {
                return AmpTemplate;
            }
            else if (item is AmpHoursBoardDetail)
            {
                return AmpHoursTemplate;
            }
            else if (item is AngleBoardDetail)
            {
                return AngleTemplate;
            }
            else if (item is BatteryCellsBoardDetail)
            {
                return BatteryCellsTemplate;
            }
            else if (item is DistanceBoardDetail)
            {
                return DistanceTemplate;
            }
            else if (item is SpeedBoardDetail)
            {
                return SpeedTemplate;
            }
            else if (item is RideModeBoardDetail)
            {
                return RideModeTemplate;
            }
            else if (item is TemperatureBoardDetail)
            {
                return TemperatureTemplate;
            }
            else if (item is VoltageBoardDetail)
            {
                return VoltageTemplate;
            }

            // These are moved to the bottom so the subclass of them will still get hit.
            if (item is FloatBoardDetail)
            {
                return FloatTemplate;
            }
            else if (item is IntBoardDetail)
            {
                return IntTemplate;
            }
            else if (item is StringBoardDetail)
            {
                return StringTemplate;
            }

            return null;
        }
    }
}
