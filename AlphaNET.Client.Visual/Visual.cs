using AlphaNET.Framework.Client;
using AlphaNET.Framework.IO;
using CommandLine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text;
using System.Threading;

namespace AlphaNET.Client.Visual
{
    public class Visual : Game
    {
        const int WIDTH = 1280;
        const int HEIGHT = 720;
        const string FS_PATH = "debug.fs";
        const string DEFAULT_IP = "127.0.0.1";
        const int DEFAULT_PORT = 1337;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Thread _computerThread;
        private Computer _computer;
        private Tty _tty;

        public Visual(string[] args)
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = WIDTH;
            _graphics.PreferredBackBufferHeight = HEIGHT;
            _graphics.ApplyChanges();

            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            CliArgs.Parse(args).WithParsed(o =>
            {
                Filesystem fs;
                string ip = DEFAULT_IP;
                int port = DEFAULT_PORT;

                if (o.Host != null)
                    ip = o.Host;

                if (o.Port != 0)
                    port = o.Port;

                if (o.FilesystemPath != null)
                    fs = BinaryManager.CreateFilesystemFromBinary(BinaryManager.ReadBinaryFromFile(o.FilesystemPath));
                else
                    fs = null;

                _tty = new Tty(Content.Load<SpriteFont>(@"tty"));

                if (o.Offline)
                    _computer = new Computer(fs, true, null, 0, _tty);
                else
                    _computer = new Computer(fs, false, ip, port, _tty);

                _computerThread = new Thread(() => AlphaNET.ComputerThread(_computer));
                _computerThread.Start();
            });
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            _tty.Draw(gameTime, _spriteBatch);

            base.Draw(gameTime);
        }
    }
}
