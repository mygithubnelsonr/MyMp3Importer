using System.Windows.Media;

namespace MyMp3Importer.Common
{
    public class Colors
    {
        public static Color Playing { get; set; }
        public static Color NotFound { get; set; }
        public static Color Nutreal { get; set; }
        public static Color Played { get; set; }
        public static Color Standard { get; set; }

        public Colors()
        {
            Playing = Color.FromRgb(116, 237, 255);
            NotFound = Color.FromRgb(255, 199, 206);
            Nutreal = Color.FromRgb(255, 235, 156);
            Played = Color.FromRgb(198, 239, 206);
            Standard = Color.FromRgb(0, 0, 0);
        }

        public static void Initialize()
        {
            Playing = Color.FromRgb(116, 237, 255);
            NotFound = Color.FromRgb(255, 199, 206);
            Nutreal = Color.FromRgb(255, 235, 156);
            Played = Color.FromRgb(198, 239, 206);
            Standard = Color.FromRgb(0, 0, 0);
        }
    }
}
