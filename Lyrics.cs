using OpenTK;
using OpenTK.Graphics;
using StorybrewCommon.Scripting;
using StorybrewCommon.Storyboarding;
using StorybrewCommon.Subtitles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.IO;

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
        public int GlowRadius = 0;

        [Configurable]
        public Color4 GlowColor = new Color4(255, 255, 255, 100);

        [Configurable]
        public bool AdditiveGlow = true;

        [Configurable]
        public int OutlineThickness = 3;

        [Configurable]
        public Color4 OutlineColor = new Color4(50, 50, 50, 200);

        [Configurable]
        public int ShadowThickness = 0;

        [Configurable]
        public Color4 ShadowColor = new Color4(0, 0, 0, 100);

        [Configurable]
        public Vector2 Padding = Vector2.Zero;

        [Configurable]
        public float SubtitleY = 400;

        [Configurable]
        public bool PerCharacter = true;

        [Configurable]
        public bool TrimTransparency = true;

        [Configurable]
        public bool EffectsOnly = false;

        [Configurable]
        public bool Debug = false;

        [Configurable]
        public OsbOrigin Origin = OsbOrigin.Centre;

        public override void Generate()
        {
            string FontName = "SNsanafonyu.ttf";

            var font = LoadFont(SpritesPath, new FontDescription()
            {
                FontPath = FontName,
                FontSize = FontSize,
                Color = FontColor,
                Padding = Padding,
                FontStyle = FontStyle,
                TrimTransparency = TrimTransparency,
                EffectsOnly = EffectsOnly,
                Debug = Debug,
            },
            new FontGlow()
            {
                Radius = AdditiveGlow ? 0 : GlowRadius,
                Color = GlowColor,
            },
            new FontOutline()
            {
                Thickness = OutlineThickness,
                Color = OutlineColor,
            },
            new FontShadow()
            {
                Thickness = ShadowThickness,
                Color = ShadowColor,
            });

            var subtitles = LoadSubtitles(SubtitlesPath);

            if (GlowRadius > 0 && AdditiveGlow)
            {
                var glowFont = LoadFont(Path.Combine(SpritesPath, "glow"), new FontDescription()
                {
                    FontPath = FontName,
                    FontSize = FontSize,
                    Color = FontColor,
                    Padding = Padding,
                    FontStyle = FontStyle,
                    TrimTransparency = TrimTransparency,
                    EffectsOnly = true,
                    Debug = Debug,
                },
                new FontGlow()
                {
                    Radius = GlowRadius,
                    Color = GlowColor,
                });
                generateLyrics(glowFont, subtitles, "glow", true);
            }
            generateLyrics(font, subtitles, "", false);
        }

        public void generateLyrics(FontGenerator font, SubtitleSet subtitles, string layerName, bool additive)
        {
            generateMyLyrics(font, subtitles, layerName);
            // var layer = GetLayer(layerName);
            // if (PerCharacter) generatePerCharacter(font, subtitles, layer, additive);
            // else generatePerLine(font, subtitles, layer, additive);
        }

        public void generateMyLyrics(FontGenerator font, SubtitleSet subtitles, string layerName)
        {
            // ** the way I set up this section is ridiculously janky but it works for my uses
            // ** go through the readme for documentation

            int[] numChars = new int[0];
            
            var layer = GetLayer(layerName);

            // LOOP 1 -----------------------------------------------------------------

            int chars = 0;
            // boolean flag to reset counter
            var reset = false;
            // iterate through lyrics based on delimiter and calculate number of characters in each group
            foreach (var line in subtitles.Lines)
            {
                var text=line.Text;
                chars += text.Length;
                if (text.Contains("*"))
                {
                    text = text.Remove(text.Length - 1, 1);
                    chars -= 1;
                    numChars = numChars.Concat(new int[]{chars}).ToArray();
                    reset = true;
                    chars = 0;
                }
            }

            foreach (var bleh in numChars)
            {
                Log(bleh);
            }

            // Log(numChars.Length);

            // LOOP 1 -----------------------------------------------------------------



            // LOOP 2 -----------------------------------------------------------------

            // counter variable
            var i = 0;
            var numCharCounter = 0;

            // boolean flag to reset counter
            reset = false;

            Random rand = new Random();

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

                foreach (var chara in text)
                {
                    var texture = font.GetTexture(chara.ToString());
                    
                    if (!texture.IsEmpty)
                    {
                        var buffer = 30;
                        var position = new Vector2((320 - (numChars[numCharCounter] * buffer)* FontScale * 0.5f)  + (buffer * i), SubtitleY) + texture.OffsetFor(Origin) * FontScale;
                        var sprite = layer.CreateSprite(texture.Path, OsbOrigin.Centre, position);
                        float rotation = (float)(rand.NextDouble() / 3);
                        sprite.Rotate(StartTime, rotation);

                        // basic fade in / out effect
                        sprite.Fade(StartTime - 200, StartTime, 0, 1);
                        sprite.Fade(EndTime - 200, EndTime, 1, 0);

                        i++;
                    }

                }

                // once the reset flag is triggered
                if (reset)
                {
                    // reset the horizontal offset
                    i = 0;
                    // add to numcharcounter
                    numCharCounter ++;
                    // set the flag back to false
                    reset = false;
                }

                // // create texture using the selected font
                // var texture = font.GetTexture(text);

                // // verify that the texture path is not null as this loop will encounter null chars
                // if (texture.Path != null)
                // {
                    
                //     var position = new Vector2(320 - texture.BaseWidth * FontScale * 0.5f, SubtitleY)
                //         + texture.OffsetFor(Origin) * FontScale;

                //     // draw the selected sprite
                //     var sprite = layer.CreateSprite(texture.Path, OsbOrigin.Centre, position);
                //     // scaling effect
                //     sprite.Scale(OsbEasing.Out, line.StartTime - 200, line.StartTime, Random(15, 20), 1);

                //     // horizontal offset so that "lines" will be drawn next to each other until EOL
                //     i += 75;
                // }
            }
        }

        public void generatePerLine(FontGenerator font, SubtitleSet subtitles, StoryboardLayer layer, bool additive)
        {
            foreach (var line in subtitles.Lines)
            {
                var texture = font.GetTexture(line.Text);
                var position = new Vector2(320 - texture.BaseWidth * FontScale * 0.5f, SubtitleY)
                    + texture.OffsetFor(Origin) * FontScale;

                var sprite = layer.CreateSprite(texture.Path, Origin, position);
                sprite.Scale(line.StartTime, FontScale);
                sprite.Fade(line.StartTime - 200, line.StartTime, 0, 1);
                sprite.Fade(line.EndTime - 200, line.EndTime, 1, 0);
                if (additive) sprite.Additive(line.StartTime - 200, line.EndTime);
            }
        }

        public void generatePerCharacter(FontGenerator font, SubtitleSet subtitles, StoryboardLayer layer, bool additive)
        {
            foreach (var subtitleLine in subtitles.Lines)
            {
                var letterY = SubtitleY;
                foreach (var line in subtitleLine.Text.Split('\n'))
                {
                    var lineWidth = 0f;
                    var lineHeight = 0f;
                    foreach (var letter in line)
                    {
                        var texture = font.GetTexture(letter.ToString());
                        lineWidth += texture.BaseWidth * FontScale;
                        lineHeight = Math.Max(lineHeight, texture.BaseHeight * FontScale);
                    }

                    var letterX = 320 - lineWidth * 0.5f;
                    foreach (var letter in line)
                    {
                        var texture = font.GetTexture(letter.ToString());
                        if (!texture.IsEmpty)
                        {
                            var position = new Vector2(letterX, letterY)
                                + texture.OffsetFor(Origin) * FontScale;

                            var sprite = layer.CreateSprite(texture.Path, Origin, position);
                            sprite.Scale(subtitleLine.StartTime, FontScale);
                            sprite.Fade(subtitleLine.StartTime - 200, subtitleLine.StartTime, 0, 1);
                            sprite.Fade(subtitleLine.EndTime - 200, subtitleLine.EndTime, 1, 0);
                            if (additive) sprite.Additive(subtitleLine.StartTime - 200, subtitleLine.EndTime);
                        }
                        letterX += texture.BaseWidth * FontScale;
                    }
                    letterY += lineHeight;
                }
            }
        }
    }
}
