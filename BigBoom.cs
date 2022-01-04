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
    public class BigBoom : StoryboardObjectGenerator
    {
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

            // an array of all the timestamps at which the effect should trigger
            int[] times = new int[] { 2565, 14177, 16758, 19338, 21919, 24500, 25790, 27080, 29661, 29984, 30306, 34822, 35145, 35467, 44499, 44821, 45144, 47079, 48370, 64499 };
            // a counter variable for the timestamp array
            var timeCounter = 0;
            var offset = 3;
            times = times.Select(x => x + offset).ToArray();

            // iterate through all hitobjects
            foreach (var hitobject in Beatmap.HitObjects)
            {

                // initialize the sprite variable with the assumption of it being a hitcircle object
                var hSprite = hitobjectLayer.CreateSprite(SpritePath, OsbOrigin.Centre, hitobject.Position);

                // use a boolean flag to determine whether we generate the effect on this hitobject or not
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
                // otherwise
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
                    // // log the timestamp for debug purposes
                    // Log(times[timeCounter]);

                    // scale it outwards, duration here is hardcoded lmao but can be made configurable in the future if needed
                    hSprite.Scale(Easing, times[timeCounter], times[timeCounter] + 250, SpriteScale, SpriteScale * 4.5);
                    // we do a bit of fade
                    hSprite.Fade(times[timeCounter], times[timeCounter] + FadeDuration, 0.75, 0);
                    hSprite.Additive(times[timeCounter], times[timeCounter] + FadeDuration);
                    // use light orange as the color (this can also be made configurable in the future)
                    hSprite.Color(times[timeCounter], new Color4(255, 232, 150, 255));
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