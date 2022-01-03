using StorybrewCommon.Scripting;
using StorybrewCommon.Storyboarding;
using System.Linq;

namespace StorybrewScripts
{
    public class Background : StoryboardObjectGenerator
    {
        [Configurable]
        public string BackgroundPath = "";

        public override void Generate()
        {
            if (BackgroundPath == "") BackgroundPath = Beatmap.BackgroundPath ?? string.Empty;

            double Opacity = 0.75; 

            var bitmap = GetMapsetBitmap(BackgroundPath);
            var bg = GetLayer("").CreateSprite(BackgroundPath, OsbOrigin.Centre);
            bg.Scale(-337, 480.0f / bitmap.Height);
            bg.Fade(1274, 2565, 0, Opacity);
            bg.Fade(3210, 3855, Opacity, 0);
            // bg.Fade(26758, 26919, 0, Opacity);
            // bg.Fade(28854, 29177, Opacity, 0);
            bg.Fade(62241, 62402, 0, Opacity);
            bg.Fade(65628, 71596, Opacity, 0);
        }
    }
}
