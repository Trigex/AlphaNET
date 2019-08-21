using AlphaNET.Framework.Client;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace AlphaNET.Client.Visual
{
    public class Tty : IConsole
    {
        private readonly SpriteFont _font;
        private string _out = "";

        public Tty(SpriteFont font)
        {
            _font = font;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(_font, _out, new Vector2(0, 0), Color.White);
            spriteBatch.End();
        }

        public void Clear()
        {
            _out = "";
        }

        public int Read()
        {
            return 0;
        }

        public string ReadLine()
        {
            var output = "";
            var enter = false;
            while (!enter)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                    enter = true;
                else
                {
                    var keys = Keyboard.GetState().GetPressedKeys();
                    if (keys.Length > 0)
                    {
                        var key = Keyboard.GetState().GetPressedKeys()[0].ToString().ToLower();
                        output += key;
                        _out += key;
                    }
                    // wait after added key
                    System.Threading.Thread.Sleep(100);
                }
            }

            return output;
        }

        public void Write(string text)
        {
            _out += text;
        }

        public void WriteLine(string text)
        {
            _out += "\n" + text;
        }
    }
}
