// by thunderbird2678
// you can contact me on twitter @thunderbird2678 or on discord (thunderbird#2678)
// feel free to use any or all parts of this storyboard code however you wish

using System.Linq;
using StorybrewCommon.Scripting;
using StorybrewCommon.Storyboarding;

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
            bg.Fade(62241, 62402, 0, Opacity);
            bg.Fade(65628, 71596, Opacity, 0);
        }
    }
}
