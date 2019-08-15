using AlphaNET.Framework.Client;
using AlphaNET.Framework.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text;

namespace AlphaNET.Client.Visual
{
    public class Visual : Game
    {
        const string FS_PATH = "debug.fs";
        const int WIDTH = 1280;
        const int HEIGHT = 720;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private Computer computer;

        public Visual()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = WIDTH;
            graphics.PreferredBackBufferHeight = HEIGHT;
            graphics.ApplyChanges();

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

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

            base.Draw(gameTime);
        }

        // Bootstrapper copied from Client.Console
        static Filesystem BootstrapFilesystem()
        {
            System.Console.WriteLine("Bootstraping filesystem...");
            Filesystem fs = new Filesystem();
            var root = new Directory("root", IOUtils.GenerateID());
            root.Owner = root;
            var bin = new Directory("bin", IOUtils.GenerateID());
            var sub = new Directory("sub", IOUtils.GenerateID());
            var lib = new Directory("lib", IOUtils.GenerateID());
            var hello = new File("hello.txt", IOUtils.GenerateID(), true, Encoding.UTF8.GetBytes("Hello, World!"));
            var src = new Directory("src", IOUtils.GenerateID());

            fs.AddFilesystemObject(root);
            fs.AddFilesystemObject(bin, root);
            fs.AddFilesystemObject(sub, bin);
            fs.AddFilesystemObject(hello, sub);
            fs.AddFilesystemObject(lib, root);
            fs.AddFilesystemObject(src, root);

            BinaryManager.WriteBinaryToFile(FS_PATH, BinaryManager.CreateBinaryFromFilesystem(fs));
            return BinaryManager.CreateFilesystemFromBinary(BinaryManager.CreateBinaryFromFilesystem(fs));
        }
    }
}
