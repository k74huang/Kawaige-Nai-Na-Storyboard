// pretty much just slight modifications on the original HitobjectHighlighting script
// edits by thunderbird2678
// you can contact me on twitter @thunderbird2678 or on discord (thunderbird#2678)
// feel free to use any or all parts of this storyboard code however you wish

using OpenTK;
using OpenTK.Graphics;
using StorybrewCommon.Mapset;
using StorybrewCommon.Scripting;
using StorybrewCommon.Storyboarding;
using StorybrewCommon.Storyboarding.Util;
using StorybrewCommon.Subtitles;
using StorybrewCommon.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StorybrewScripts
{
    public class BigBoom : StoryboardObjectGenerator
    {
        [Configurable]
        // An integer containing the timestamp at which the effect should trigger
        public int Time = 0;

        [Configurable]
        // An integer containing the duration (in ms) after which the sprite fades out
        public int FadeDuration = 400;

        [Configurable]
        // A string containing the path to the image to be used as the sprite
        public string SpritePath = "sb/boom.png";

        [Configurable]
        // A double containing the scale of the sprite to be generated
        public double SpriteScale = 1;

        [Configurable]
        public OsbEasing Easing = OsbEasing.None;

        public override void Generate()
        {
            var hitobjectLayer = GetLayer("");

            int[] times = new int[]{2565, 14177, 16758, 19338, 21919, 24500, 25790, 27080, 29661, 29984, 30306, 34822, 35145, 35467, 44499, 44821, 45144, 47079, 48370, 64499};
            var timeCounter = 0;

            // iterate through all hitobjects
            foreach (var hitobject in Beatmap.HitObjects)
            {

                var hSprite = hitobjectLayer.CreateSprite(SpritePath, OsbOrigin.Centre, hitobject.Position); 
                var gen = false;

                // if the object's a slider
                if (hitobject is OsuSlider)
                {
                    // generate the effect if the slider's bounds surround the timestamp
                    if (hitobject.StartTime <= times[timeCounter] + 5 && hitobject.EndTime >= times[timeCounter] - 5)
                    {
                        // generate the sprite based on where the sliderball is at that point in time
                        hSprite = hitobjectLayer.CreateSprite(SpritePath, OsbOrigin.Centre, hitobject.PositionAtTime(Time));
                        gen = true;
                    }

                }
                else
                {
                    // generate the effect if the hitcircle lands right on the timestamp
                    if (Math.Abs(hitobject.StartTime - times[timeCounter]) < 5)
                    {
                        gen = true;
                    }
                }

                if(gen)
                {
                    Log(times[timeCounter]);
                    hSprite.Scale(Easing, times[timeCounter], times[timeCounter] + 250, SpriteScale, SpriteScale * 4.5);
                    hSprite.Fade(times[timeCounter], times[timeCounter] + FadeDuration, 0.75, 0);
                    hSprite.Additive(times[timeCounter], times[timeCounter] + FadeDuration);
                    hSprite.Color(times[timeCounter], new Color4(255, 232, 150, 255));
                    timeCounter++;
                    if(timeCounter == times.Length)
                    {
                        break;
                    }
                }
            }
        }
    }
}