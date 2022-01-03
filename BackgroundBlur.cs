using StorybrewCommon.Scripting;
using StorybrewCommon.Storyboarding;
using System.Linq;

namespace StorybrewScripts
{
    public class BackgroundBlur : StoryboardObjectGenerator
    {
        [Configurable]
        public string BackgroundPath = "";

        public override void Generate()
        {
            if (BackgroundPath == "") BackgroundPath = Beatmap.BackgroundPath ?? string.Empty;

            double Opacity = 0.75;

            var bitmap = GetMapsetBitmap(BackgroundPath);
            var bg = GetLayer("").CreateSprite(BackgroundPath, OsbOrigin.Centre);
            bg.Scale(3210, 480.0f / bitmap.Height);
            bg.Fade(3210, 3694, 0, Opacity);
            bg.Fade(8693, 9016, Opacity, 0.5);
            bg.Fade(13854, 14177, 0.5, Opacity);
            bg.Fade(19016, 19177, Opacity, 0.5);
            bg.Fade(19177, 19338, 0.5, Opacity);
            bg.Fade(24500, 27000, Opacity, 0.25);
            bg.Fade(27000, 27080, 0.25, 0.3);
            bg.Fade(27322, 27403, 0.3, 0.35);
            bg.Fade(27645, 27725, 0.35, 0.4);
            bg.Fade(28129, 28209, 0.4, 0.45);
            bg.Fade(28451, 28532, 0.45, 0.55);
            bg.Fade(28774, 28854, 0.55, 0.65);
            bg.Fade(28935, 29016, 0.65, 0.75);
            bg.Fade(29177, 29661, 0.75, 0.9);
            bg.Fade(30629, 30790, 0.9, 0.75);
            bg.Fade(34338, 34822, 0.75, 0.9);
            bg.Fade(35629, 35790, 0.9, 0.75);
            bg.Fade(46757, 47079, 0.75, 0.25);
            bg.Fade(47725, 48370, 0.25, 0);
            bg.Fade(48934, 49015, 0, 0.2);
            bg.Fade(49176, 49257, 0.2, 0.4);
            bg.Fade(49418, 49499, 0.4, 0.6);
            bg.Fade(49579, 49660, 0.6, 0.75);
            bg.Fade(54499, 54660, Opacity, 0.5);
            bg.Fade(54660, 54821, 0.5, Opacity);
            bg.Fade(59660, 59983, 0.75, 0.85);
            bg.Fade(62563, 64499, 0.85, 0);
            // bg.Fade(26758, 27080, Opacity, 0);
            // bg.Fade(28854, 29016, 0, Opacity);
            // bg.Fade(62241, 62563, Opacity, 0);
        }
    }
}
