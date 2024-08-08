using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Mugen.Core
{
    public class WindowManager
    {
        #region Attributes

        Game _game;

        //public RenderTarget2D RenderTarget { get; private set; }
        private GraphicsDeviceManager _graphicsDeviceManager;
        public GraphicsDeviceManager GDManager => _graphicsDeviceManager;

        private Vector2 Mouse = new();

        public bool IsFullScreen { get; private set; } = false;

        readonly int _gameScreenW;
        readonly int _gameScreenH;
        private Point _screenSize;

        private float Scale = 1f;

        private int _windowW;
        private int _windowH;


        private int _curWindowW;
        private int _curWindowH;

        private Rectangle _strechingRect = new();

        #endregion
        public WindowManager(Game game, int gameScreenW, int gameScreenH)
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

            Node._nodeRoot.SetSize(_gameScreenW, _gameScreenH);
        }

        public Point GetScreenSize()
        {
            return _screenSize;
        }
        public Vector2 GetMousePosition()
        {
            return Mouse;
        }
        public void ToggleFullscreen()
        {
            IsFullScreen = !IsFullScreen;

            if (IsFullScreen)
            {
                _windowW = _game.Window.ClientBounds.Width;
                _windowH = _game.Window.ClientBounds.Height;
            }
            else
            {
                _graphicsDeviceManager.PreferredBackBufferWidth = _windowW;
                _graphicsDeviceManager.PreferredBackBufferHeight = _windowH;
            }

            _graphicsDeviceManager.ToggleFullScreen();

        }
        public void PoolWindowSize()
        {
            _curWindowW = _game.Window.ClientBounds.Width;
            _curWindowH = _game.Window.ClientBounds.Height;
        }
        public Rectangle StrechtingRect()
        {
            _strechingRect.X = (int)((_curWindowW - (_gameScreenW * Scale)) * 0.5f);
            _strechingRect.Y = (int)((_curWindowH - (_gameScreenH * Scale)) * 0.5f);
            _strechingRect.Width = (int)(_gameScreenW * Scale);
            _strechingRect.Height = (int)(_gameScreenH * Scale);

            return _strechingRect;
        }
        public void SetWindowSize(int windowW, int windowH)
        {
            _graphicsDeviceManager.PreferredBackBufferWidth = windowW;
            _graphicsDeviceManager.PreferredBackBufferHeight = windowH;

            _graphicsDeviceManager.ApplyChanges();
        }
        public void SetScale(float scale)
        {
            SetWindowSize((int)(_windowW * scale), (int)(_windowH * scale));
        }
        public void Update(Vector2 mousePos)
        {
            PoolWindowSize();

            Scale = MathF.Min((float)_curWindowW / _gameScreenW, (float)_curWindowH / _gameScreenH);

            Mouse.X = MathF.Round((mousePos.X - (_curWindowW - (_gameScreenW * Scale)) * 0.5f) / Scale);
            Mouse.Y = MathF.Round((mousePos.Y - (_curWindowH - (_gameScreenH * Scale)) * 0.5f) / Scale);

            Mouse = Vector2.Clamp(Mouse, new Vector2(0, 0), new Vector2(_gameScreenW, _gameScreenH));
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
