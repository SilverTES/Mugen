using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Mugen.Core
{
    public static class WindowManager
    {
        #region Attributes

        private static Game? _game;

        //public RenderTarget2D RenderTarget { get; private set; }
        private static GraphicsDeviceManager? _graphicsDeviceManager;
        public static GraphicsDeviceManager? GDManager => _graphicsDeviceManager;

        public static SpriteBatch? _batch;

        private static Vector2 Mouse = new();

        public static bool IsFullscreen { get; private set; } = false;

        static int _gameScreenW;
        static int _gameScreenH;
        private static Point _screenSize;

        private static float Scale = 1f;

        private static int _windowW;
        private static int _windowH;


        private static int _curWindowW;
        private static int _curWindowH;

        private static Rectangle _strechingRect = new();

        #endregion
        public static void Init(Game game, int gameScreenW, int gameScreenH)
        {
            _game = game;
            _graphicsDeviceManager = new GraphicsDeviceManager(game);
            _graphicsDeviceManager.HardwareModeSwitch = false; // Fullscreen Windowed Borderless 
            _graphicsDeviceManager.PreferredBackBufferWidth = gameScreenW;
            _graphicsDeviceManager.PreferredBackBufferHeight = gameScreenH;
            _graphicsDeviceManager.ApplyChanges();

            _gameScreenW = gameScreenW;
            _gameScreenH = gameScreenH;

            _screenSize = new Point(gameScreenW, gameScreenH);

            _windowW = gameScreenW;
            _windowH = gameScreenH;

            //RenderTarget = new RenderTarget2D(_game.GraphicsDevice, _gameScreenW, _gameScreenH);

            _batch = new SpriteBatch(game.GraphicsDevice);

            Node._nodeRoot.SetSize(_gameScreenW, _gameScreenH);

            GFX.GFX.Init(game.GraphicsDevice);

            //ScreenManager.Init(nbLayers, layerOrders);
        }

        //internal static void SetSpriteBatch(SpriteBatch spriteBatch)
        //{
        //    _batch = spriteBatch;
        //}
        public static float GetScale() { return Scale; }
        public static Point GetScreenSize()
        {
            return _screenSize;
        }
        public static Vector2 GetMousePosition()
        {
            return Mouse;
        }
        public static void ToggleFullscreen()
        {
            IsFullscreen = !IsFullscreen;

            if (IsFullscreen)
            {
                _windowW = _game!.Window.ClientBounds.Width;
                _windowH = _game.Window.ClientBounds.Height;
            }
            else
            {
                _graphicsDeviceManager!.PreferredBackBufferWidth = _windowW;
                _graphicsDeviceManager.PreferredBackBufferHeight = _windowH;
            }

            _graphicsDeviceManager!.ToggleFullScreen();

        }
        public static void PoolWindowSize()
        {
            _curWindowW = _game!.Window.ClientBounds.Width;
            _curWindowH = _game.Window.ClientBounds.Height;
        }
        public static Rectangle StrechtingRect()
        {
            _strechingRect.X = (int)((_curWindowW - (_gameScreenW * Scale)) * 0.5f);
            _strechingRect.Y = (int)((_curWindowH - (_gameScreenH * Scale)) * 0.5f);
            _strechingRect.Width = (int)(_gameScreenW * Scale);
            _strechingRect.Height = (int)(_gameScreenH * Scale);

            return _strechingRect;
        }
        public static void SetWindowSize(int windowW, int windowH)
        {
            _graphicsDeviceManager!.PreferredBackBufferWidth = windowW;
            _graphicsDeviceManager.PreferredBackBufferHeight = windowH;

            _graphicsDeviceManager.ApplyChanges();
        }
        public static void SetScale(float scale)
        {
            SetWindowSize((int)(_windowW * scale), (int)(_windowH * scale));
        }
        public static void Update(GameTime gameTime)
        {
            Vector2 mousePos = Microsoft.Xna.Framework.Input.Mouse.GetState().Position.ToVector2();
            PoolWindowSize();

            Scale = MathF.Min((float)_curWindowW / _gameScreenW, (float)_curWindowH / _gameScreenH);

            Mouse.X = MathF.Round((mousePos.X - (_curWindowW - (_gameScreenW * Scale)) * 0.5f) / Scale);
            Mouse.Y = MathF.Round((mousePos.Y - (_curWindowH - (_gameScreenH * Scale)) * 0.5f) / Scale);

            Mouse = Vector2.Clamp(Mouse, new Vector2(0, 0), new Vector2(_gameScreenW, _gameScreenH));


            //ScreenManager.Update(gameTime);
        }
        //public void Draw(SpriteBatch batch)
        //{
        //    batch.Draw(
        //        RenderTarget,
        //        //new Rectangle((int)((_curWindowW - (_gameScreenW * Scale)) * 0.5f), (int)((_curWindowH - (_gameScreenH * Scale)) * 0.5f), (int)(_gameScreenW * Scale), (int)(_gameScreenH * Scale)),
        //        StrechtingRect(),
        //        new Rectangle(0, 0, RenderTarget.Width, RenderTarget.Height),
        //        Color.White
        //        );
        //}
    }
}
