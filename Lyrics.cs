// modified ver of the default lyrics script
// edits by thunderbird2678
// you can contact me on twitter @thunderbird2678 or on discord (thunderbird#2678)
// feel free to use any or all parts of this storyboard code however you wish

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using OpenTK;
using OpenTK.Graphics;
using StorybrewCommon.Scripting;
using StorybrewCommon.Storyboarding;
using StorybrewCommon.Subtitles;

namespace StorybrewScripts
{
    public class Lyrics : StoryboardObjectGenerator
    {
        [Configurable]
        public string SubtitlesPath = "lyrics.srt";

        [Configurable]
        public string SpritesPath = "sb/lyrics";

        [Configurable]
        public int FontSize = 26;

        [Configurable]
        public float FontScale = 0.5f;

        [Configurable]
        public Color4 FontColor = Color4.White;

        [Configurable]
        public FontStyle FontStyle = FontStyle.Regular;

        [Configurable]
        public float SubtitleY = 400;

        [Configurable]
        public bool TrimTransparency = true;

        [Configurable]
        public OsbOrigin Origin = OsbOrigin.Centre;

        public override void Generate()
        {
            // load the font that I've provided in the project folder
            string FontName = "SNsanafonyu.ttf";

            var font = LoadFont(SpritesPath, new FontDescription()
            {
                FontPath = FontName,
                FontSize = FontSize,
                Color = FontColor,
                TrimTransparency = TrimTransparency,
            });

            // load the subtitles
            var subtitles = LoadSubtitles(SubtitlesPath);
            // call generateLyrics()
            generateLyrics(font, subtitles);
        }

        public void generateLyrics(FontGenerator font, SubtitleSet subtitles)
        {
            // whip up a random number generator            
            Random rand = new Random();

            // ** the way I set up this section is ridiculously janky but it works for my uses

            // an array to store the number of characters in each line
            int[] numChars = new int[0];

            // get the base layer
            var layer = GetLayer("");

            // set up a counter for the number of characters
            int chars = 0;
            // iterate through chorus lyrics line by line and calculate number of characters in each group
            foreach (var line in subtitles.Lines)
            {
                // get the text in each line
                var text = line.Text;
                // add the full length of that line to the character counter
                chars += text.Length;
                // once we hit the EOL symbol
                if (text.Contains("*"))
                {
                    // subtract one from the character counter
                    chars -= 1;
                    // push the character counter into the array
                    numChars = numChars.Concat(new int[] { chars }).ToArray();
                    // reset the character counter
                    chars = 0;
                }
            }

            // // log the amount of characters in each line
            // foreach (var bleh in numChars)
            // {
            //     Log(bleh);
            // }

            // counter variable
            var i = 0;
            // counter variable for the char array counter
            var numCharCounter = 0;

            // boolean flag to reset counter
            var reset = false;

            // iterate through chorus lyrics line by line
            foreach (var line in subtitles.Lines)
            {
                // grab the text
                var text = line.Text;
                // if the text contains my EOL symbol, cut the symbol and mark this as a reset point
                if (text.Contains("*"))
                {
                    text = text.Remove(text.Length - 1, 1);
                    reset = true;
                }

                // the start and end time for the entire group
                var StartTime = line.StartTime;
                var EndTime = line.EndTime;

                // go through each individual character
                foreach (var chara in text)
                {
                    // create the texture for it based off the font
                    var texture = font.GetTexture(chara.ToString());

                    // so long as the texture is valid
                    if (!texture.IsEmpty)
                    {
                        // experimentally determined buffer
                        var buffer = 30;
                        // using the number of characters in the whole line and the position of the current character
                        // we can write each individual character such that the whole line is still center aligned
                        // this could be rewritten in a much nicer way but sue me
                        var position = new Vector2((320 - (numChars[numCharCounter] * buffer) * FontScale * 0.5f) + (buffer * i), SubtitleY) + texture.OffsetFor(Origin) * FontScale;
                        var sprite = layer.CreateSprite(texture.Path, OsbOrigin.Centre, position);
                        // we do a random rotation
                        // it's confined to +/- .33 radians so it doesn't get too extreme
                        // please excuse my disgusting ternary statement im a dirty javasc*ipt dev*loper
                        float rotation = (float)(rand.NextDouble() / 3) * (rand.NextDouble() > 0.5 ? 1 : -1);
                        sprite.Rotate(StartTime, rotation);

                        // basic fade in / out effect
                        sprite.Fade(StartTime - 200, StartTime, 0, 1);
                        sprite.Fade(EndTime - 200, EndTime, 1, 0);

                        // increment character counter
                        i++;
                    }

                }

                // once the reset flag is triggered
                if (reset)
                {
                    // reset the horizontal offset
                    i = 0;
                    // add to numcharcounter
                    numCharCounter++;
                    // set the flag back to false
                    reset = false;
                }
            }
        }
    }
}
