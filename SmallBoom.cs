// pretty much just slight modifications on the original HitobjectHighlighting script
// edits by thunderbird2678
// you can contact me on twitter @thunderbird2678 or on discord (thunderbird#2678)
// feel free to use any or all parts of this storyboard code however you wish

using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;
using OpenTK.Graphics;
using StorybrewCommon.Mapset;
using StorybrewCommon.Scripting;
using StorybrewCommon.Storyboarding;
using StorybrewCommon.Storyboarding.Util;
using StorybrewCommon.Subtitles;
using StorybrewCommon.Util;

namespace StorybrewScripts
{
    public class SmallBoom : StoryboardObjectGenerator
    {
        [Configurable]
        // An integer containing the duration (in ms) after which the sprite fades out
        public int FadeDuration = 200;

        [Configurable]
        // A string containing the path to the image to be used as the sprite
        public string SpritePath = "sb/glow.png";

        [Configurable]
        // A double containing the scale of the sprite to be generated
        public double SpriteScale = 1;

        public override void Generate()
        {
            var hitobjectLayer = GetLayer("");

            // an array of all the timestamps at which the effect should trigger
            // note we do this over multiple lines because this would be awful otherwise
            // these are largely grouped by musical phrasing / section
            int[] times = new int[] { 3855, 4822, 5145, 6113, 6435, 7403, 7725, 8693, 9016, 9500, 10306, 10790, 11596, 12080, 12887, 13371, 14338, 15306, 16919, 17887, 19500, 20467, 22080, 23048, 23209, 23371, 23532, 29016, 29177, 29338, 29500 };
            times = times.Concat(new int[] { 30790, 31274, 31435, 31596, 32080, 32564, 32725, 32887, 33532, 34016 }).ToArray();
            times = times.Concat(new int[] { 34338, 34500, 34661, 35951, 36435, 36596, 36758, 37242, 37725, 37887, 38048, 38693, 38854, 39016, 39177 }).ToArray();
            times = times.Concat(new int[] { 44015, 44176, 44338, 45789, 46434, 46757, 46918, 47079 }).ToArray();
            times = times.Concat(new int[] { 49015, 49257, 49499 }).ToArray();
            times = times.Concat(new int[] { 49660, 49983, 50305, 50789, 51273, 51596, 51918 }).ToArray();
            times = times.Concat(new int[] { 52241, 52563, 52886, 53370, 53854, 54176, 54499 }).ToArray();
            times = times.Concat(new int[] { 54821, 55144, 55467, 55950, 56434, 56757, 57079 }).ToArray();
            times = times.Concat(new int[] { 57402, 57725, 58047, 58370, 58531, 59015, 59338, 59660 }).ToArray();
            times = times.Concat(new int[] { 59983, 60305, 60628, 60950, 61112, 61596, 61918, 62241 }).ToArray();
            times = times.Concat(new int[] { 64499 }).ToArray();

            // a counter variable for the timestamp array
            var timeCounter = 0;

            // HAHAHAHAHA HORRIBLE HACK DON'T MIND ME LOL
            var offset = 3;
            times = times.Select(x => x + offset).ToArray();

            // iterate through all hitobjects
            foreach (var hitobject in Beatmap.HitObjects)
            {
                // initialize the sprite variable with the assumption of it being a hitcircle object
                var hSprite = hitobjectLayer.CreateSprite(SpritePath, OsbOrigin.Centre, hitobject.Position);
                var gen = false;

                // since some diffs don't have all the objects
                // if our hit object skips an object in the timestamp array, we increment the counter for the timestamp array
                if (hitobject.StartTime > times[timeCounter])
                {
                    timeCounter++;
                };

                // if the object's a slider
                if (hitobject is OsuSlider)
                {
                    // generate the effect if the slider's bounds surround the timestamp
                    if (hitobject.StartTime <= times[timeCounter] + 5 && hitobject.EndTime >= times[timeCounter] - 5)
                    {
                        // generate the sprite based on where the sliderball is at that point in time
                        hSprite = hitobjectLayer.CreateSprite(SpritePath, OsbOrigin.Centre, hitobject.PositionAtTime(times[timeCounter]));
                        // flip the flag
                        gen = true;
                    }

                }
                else
                {
                    // generate the effect if the hitcircle lands right on the timestamp
                    if (Math.Abs(hitobject.StartTime - times[timeCounter]) < 5)
                    {
                        // flip the flag
                        gen = true;
                    }
                }

                // if we are to generate the effect
                if (gen)
                {
                    // set up the sprite
                    hSprite.Scale(OsbEasing.In, times[timeCounter], times[timeCounter] + FadeDuration, SpriteScale, SpriteScale * 0.2);
                    hSprite.Fade(OsbEasing.In, times[timeCounter], times[timeCounter] + FadeDuration, 0.5, 0);
                    hSprite.Additive(times[timeCounter], times[timeCounter] + FadeDuration);
                    hSprite.Color(times[timeCounter], hitobject.Color);
                    // increment the timecounter
                    timeCounter++;
                    // we can exit the loop if we've incremented the timecounter to the end of the list
                    if (timeCounter == times.Length)
                    {
                        break;
                    }
                }
            }
        }
    }
}