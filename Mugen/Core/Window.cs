using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Mugen.GFX;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Mugen.Core
{
    public enum Mode
    {
        NORMAL,
        RETRO
    };

    [Obsolete]
    public class Window
    {
        #region Variables

        //public static Dictionary<int, ConsoleComponent> _console = new Dictionary<int, ConsoleComponent>();
        //public static Window _window = new Window();

        Game? _game;
        public Game? Game() { return _game; }

        // Get all monitors infos !
        //System.Windows.Forms.Screen[] _screens = System.Windows.Forms.Screen.AllScreens;

        public GraphicsDeviceManager? _graphics;
        public SpriteBatch? Batch { get; private set; }

        // the RenderTarget2D that we will draw to
        public RenderTarget2D? FinalRenderTarget { get; private set; }
        public RenderTarget2D? NativeRenderTarget { get; private set; }

        public string Name { get; private set; } = ""; // name of windows

        // Mode 
        Mode _mode = Mode.RETRO;
        BlendState _blendState = BlendState.NonPremultiplied;
        SamplerState _samplerState = SamplerState.LinearClamp;

        //RasterizerState _rasterizerState;

        // Screen of Game
        public Rectangle ScreenRect { get; private set; }
        public int ScreenW { get; private set; }
        public int ScreenH { get; private set; }

        public int FinalScreenW { get; private set; }
        public int FinalScreenH { get; private set; }

        public int CenterX { get; private set; }
        public int CenterY { get; private set; }

        public float Scale { get; private set; }
        public Rectangle ScaleRect { get; private set; }

        // Windowed
        public float ScaleWin { get; private set; }
        public int WindowX { get; private set; }
        public int WindowY { get; private set; }
        //public int WindowW { get; private set; }
        //public int WindowH { get; private set; }

        // FullScreen
        public float ScaleFull { get; private set; }
        public int ViewX { get; private set; }
        public int ViewY { get; private set; }
        public int ViewW { get; private set; }
        public int ViewH { get; private set; }

        // Monitor
        //public int CurrentMonitor { get; private set; }
        public int CurrentMonitorX { get; private set; }
        public int CurrentMonitorY { get; private set; }
        public int CurrentMonitorW { get; private set; }
        public int CurrentMonitorH { get; private set; }

        // View state
        public bool IsFullScreen { get; private set; }
        public bool IsMaxScale { get; private set; }
        public bool IsVsync { get; private set; }
        public bool IsSmooth { get; private set; }

        // Offset View
        public int OriginX { get; private set; }
        public int OriginY { get; private set; }

        // Input State
        public MouseState MouseState { get; private set; }

        // Events
        public bool OnWindowResize { get; private set; }
        public bool OnToggleFullScreen { get; private set; }
        public bool OnSwitchMonitor { get; private set; }


        #endregion

        //public static int AddConsole(Game game, int index)
        //{
        //    // Init Console
        //    _console[index] = new ConsoleComponent(game);
        //    game.Components.Add(_console[index]);
        //    _console[index].BackgroundColor = new Color(50, 120, 100, 150);
        //    return index;
        //}

        // Convert string to Mode Value
        public static Mode StringToMode(String str)
        {
            if (str.ToUpper() == "RETRO")
                return Mode.RETRO;
            else
                return Mode.NORMAL;
        }

        //public void SetNativeRenderTarget()
        //{
        //    _graphics!.GraphicsDevice.SetRenderTarget(NativeRenderTarget);
        //}
        //public void SetFinalRenderTarget()
        //{
        //    _graphics!.GraphicsDevice.SetRenderTarget(FinalRenderTarget);
        //}

        //public void SetMainRenderTarget()
        //{
        //    _graphics!.GraphicsDevice.SetRenderTarget(null);
        //}

        // Window setup
        public int Setup
        (
            Game game, Mode mode, string name, int screenW, int screenH, float scaleWin, float scaleFull, bool fullScreen = false, bool isVsync = true, bool isSmooth = false)
        {
            _game = game;
            SetMode(mode);
            Name = name;

            OnWindowResize = false;
            OnToggleFullScreen = false;
            OnSwitchMonitor = false;

            FinalScreenW = ScreenW = screenW;
            FinalScreenH = ScreenH = screenH;
            ScreenRect = new Rectangle(0, 0, ScreenW, ScreenH);
            CenterX = screenW / 2;
            CenterY = screenH / 2;

            ScaleWin = scaleWin;
            ScaleFull = scaleFull;

            IsFullScreen = fullScreen;
            IsVsync = isVsync;
            IsSmooth = isSmooth;

            _graphics = new GraphicsDeviceManager(_game);


            _game.Content.RootDirectory = "Content";

            // Final Render Size
            _graphics.PreferredBackBufferWidth = ScreenW;
            _graphics.PreferredBackBufferHeight = ScreenH;

            _graphics.SynchronizeWithVerticalRetrace = IsVsync;

            //CurrentMonitorX = _screens[CurrentMonitor].Bounds.X;
            //CurrentMonitorY = _screens[CurrentMonitor].Bounds.Y;
            //CurrentMonitorW = _screens[CurrentMonitor].Bounds.Width;
            //CurrentMonitorH = _screens[CurrentMonitor].Bounds.Height;
            
            CurrentMonitorX = 0;
            CurrentMonitorY = 0;
            CurrentMonitorW = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            CurrentMonitorH = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            //IsFixedTimeStep = false;
            _graphics.PreferMultiSampling = IsSmooth;
            //this.Window.AllowUserResizing = true;
            //_rasterizerState = new RasterizerState { MultiSampleAntiAlias = true };

            _graphics.ApplyChanges();

            return Misc.Log("Window Setup OK \n");
        }
        public int Init(SpriteFont defaultFont)
        {

            NativeRenderTarget = new RenderTarget2D
            (
                _graphics!.GraphicsDevice,
                ScreenW,
                ScreenH,
                false,
                _graphics.GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24
            );

            FinalRenderTarget = new RenderTarget2D
            (
                _graphics.GraphicsDevice,
                FinalScreenW,
                FinalScreenH,
                false,
                _graphics.GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24
            );

            // Create a new SpriteBatch, which can be used to draw textures.
            Batch = new SpriteBatch(_graphics.GraphicsDevice);

            _game!.Window.Title = Name;

            // Init Addons :
            GFX.GFX.Init(_graphics.GraphicsDevice); //, defaultFont);



            // Init Node root
            //Node._root
            //.SetPosition(0f, 0f)
            //.SetSize(ScreenW, ScreenH);

            return Misc.Log("Window Init OK \n");
        }
        //public SpriteBatch Batch() { return _spriteBatch; }

        public void SetFinalScreenSize(int finalScreenW, int finalScreenH, float scale = 0)
        {

            FinalScreenW = finalScreenW;
            FinalScreenH = finalScreenH;

            FinalRenderTarget = new RenderTarget2D
            (
                _graphics!.GraphicsDevice,
                FinalScreenW,
                FinalScreenH,
                false,
                _graphics.GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24
            );

            //SetScale(IsFullScreen?ScaleFull:ScaleWin);
            SetScale(scale);

            // Final Render Size

            //if (!IsFullScreen)
            //{
            //    _graphics.PreferredBackBufferWidth = FinalScreenW;
            //    _graphics.PreferredBackBufferHeight = FinalScreenH;
            //    _graphics.ApplyChanges();
            //}

        }

        // Render
        public void SetMode(Mode mode)
        {
            _mode = mode;

            if (_mode == Mode.RETRO)
            {
                _samplerState = SamplerState.PointClamp;
            }
            else
            {
                _samplerState = SamplerState.LinearClamp;
            }

        }
        public void SetBlendState(BlendState blendState)
        {
            _blendState = blendState;
        }

        public void SetRenderTarget(RenderTarget2D renderTarget)
        {
            _graphics!.GraphicsDevice.SetRenderTarget(renderTarget);
        }
        //[Obsolete("Not used any more", true)]
        //public void BeginRender()
        //{
        //    SetRenderTarget(MainRenderTarget);

        //    Batch.Begin
        //    (
        //        SpriteSortMode.Deferred,
        //        _blendState,
        //        _samplerState,
        //        DepthStencilState.None,
        //        RasterizerState.CullCounterClockwise
        //    );

        //}
        //[Obsolete("Not used any more", true)]
        //public void EndRender()
        //{
        //    Batch.End();

        //}

        public void RenderMainTarget(Color color)
        {
            ScaleRect = new Rectangle(0, 0, FinalScreenW, FinalScreenH);
            Batch!.Draw(NativeRenderTarget, ScaleRect, color);

            OnWindowResize = false;
            OnToggleFullScreen = false;
            OnSwitchMonitor = false;
        }
        public void RenderFinalTarget(Color color)
        {
            ScaleRect = new Rectangle(ViewX + OriginX, ViewY + OriginY, ViewW, ViewH);
            Batch!.Draw(FinalRenderTarget, ScaleRect, color);

            OnWindowResize = false;
            OnToggleFullScreen = false;
            OnSwitchMonitor = false;
        }

        public void Render(Color color)
        {
            ScaleRect = new Rectangle(ViewX + OriginX, ViewY + OriginY, ViewW, ViewH);
            Batch!.Draw(NativeRenderTarget, ScaleRect, color);

            OnWindowResize = false;
            OnToggleFullScreen = false;
            OnSwitchMonitor = false;
        }

        public void Render(Texture2D buffer, Color color)
        {
            //if (IsFullScreen)
            //    ScaleRect = new Rectangle(ViewX + OriginX, ViewY + OriginY, ViewW, ViewH);
            //else
            //    ScaleRect = new Rectangle(OriginX, OriginY, ViewW, ViewH);

            ScaleRect = new Rectangle(ViewX + OriginX, ViewY + OriginY, ViewW, ViewH);

            Batch!.Draw(buffer, ScaleRect, color);

            OnWindowResize = false;
            OnToggleFullScreen = false;
            OnSwitchMonitor = false;
        }

        // Manager
        public void ToggleFullScreen(int scale) // Toggle windowed to FullScreen : 0 = Max Zoom , -1 = Default Zoom
        {
            IsFullScreen = !IsFullScreen;
            SetScale(scale);

            OnToggleFullScreen = true;
        }
        public void SetFullScreen(bool isFullScreen, int scale)
        {
            IsFullScreen = isFullScreen;
            SetScale(scale);
        }
        //public void SwitchMonitor(int scale) // Switch monitor : 0 = Max Zoom, -1 Default Zoom
        //{
        //    CurrentMonitor++;
        //    if (CurrentMonitor > _screens.Length - 1) CurrentMonitor = 0;
        //    SetMonitor(CurrentMonitor, scale); // -1 scale default , 0 scale Max !

        //    OnSwitchMonitor = true;
        //}
        //public void SetMonitor(float scale) // Select the Monitor : O = Max Zoom, -1 Default Zoom
        ////public void SetMonitor(int monitor, float scale) // Select the Monitor : O = Max Zoom, -1 Default Zoom
        //{
        //    // if prev Monitor is on maxScale then new Monitor go maxScale too !
        //    if (!IsFullScreen)
        //        IsMaxScale = (ScaleWin == GetMaxScale() - 1);
        //    else
        //        IsMaxScale = (ScaleFull == GetMaxScale());

        //    //CurrentMonitor = monitor;

        //    //PollMonitor(CurrentMonitor); // peek Monitor infos !
        //    PollMonitor(); // peek Monitor infos !

        //    if (IsFullScreen)
        //    {
        //        scale = ScaleFull;
        //        // if prev Monitor Scale is different than Max scale Monitor possible !
        //        if (scale > GetMaxScale() || IsMaxScale || scale == 0)
        //            scale = GetMaxScale();
        //    }
        //    else
        //    {
        //        scale = ScaleWin;
        //        // if prev Monitor Scale is different than Max scale Monitor possible !
        //        if (scale > GetMaxScale() - 1 || IsMaxScale || scale == 0)
        //            scale = GetMaxScale() - 1;

        //    }

        //    //SetWindow(CurrentMonitor, IsFullScreen, scale);
        //    SetWindow(IsFullScreen, scale);
        //}
        public void SetScale(float scale) // set Scale : 0 = Max Zoom, -1 Default Zoom
        {


            //PollMonitor(CurrentMonitor);
            PollMonitor();

            if (IsFullScreen)
            {
                if (scale > GetMaxScale() || scale == 0)
                    scale = GetMaxScale();

                if (scale == -1)
                    if (ScaleFull > GetMaxScale())
                        scale = GetMaxScale();
                    else
                        scale = ScaleFull;
            }
            else
            {
                if (scale > GetMaxScale() - 1 || scale == 0)
                    scale = GetMaxScale() - 1;

                if (scale == -1)
                    if (ScaleWin > GetMaxScale() - 1)
                        scale = GetMaxScale() - 1;
                    else
                        scale = ScaleWin;
            }

            Scale = scale;

            //SetWindow(GetCurrentMonitorOfWindow(), IsFullScreen, scale);
            SetWindow(IsFullScreen, scale);

            OnWindowResize = true;
        }
        public void SetWindow(bool isFullScreen, float scale, bool center = true)
        //public void SetWindow(int adapter, bool isFullScreen, float scale, bool center = true)
        {
            IsFullScreen = isFullScreen;
            //CurrentMonitor = adapter;

            if (IsFullScreen)
            {

                if (!_graphics!.IsFullScreen)
                {
                    _graphics!.HardwareModeSwitch = false;
                    WindowX = _game!.Window.Position.X;
                    WindowY = _game!.Window.Position.Y;
                    _graphics.ToggleFullScreen();
                }

                //_game!.Window.IsBorderless = true;

                //PollMonitor(adapter); // peek Monitor infos !
                PollMonitor(); // peek Monitor infos !

                if (scale == 0)
                    ScaleFull = GetMaxScale();
                else
                    ScaleFull = scale;

                if (center) SetViewAtCenter(ScaleFull);

                //_game.Window.Position = new Point(CurrentMonitorX, CurrentMonitorY);

                //_graphics!.PreferredBackBufferWidth = CurrentMonitorW;
                //_graphics.PreferredBackBufferHeight = CurrentMonitorH;

                

                _graphics.ApplyChanges();
            }
            else
            {
                //_game!.Window.IsBorderless = false;

                //PollMonitor(adapter); // peek Monitor infos !
                PollMonitor(); // peek Monitor infos !

                if (scale == 0)
                    ScaleWin = GetMaxScale() - 1;
                else
                    ScaleWin = scale;

                if (ScaleWin < 1) ScaleWin = 1;

                if (center) SetViewAtCenter(ScaleWin);

                //_game!.Window.Position = new Point(CurrentMonitorX + ViewX + WindowX, CurrentMonitorY + ViewY + WindowY);

                ViewX = 0;
                ViewY = 0;

                _graphics!.PreferredBackBufferWidth = (int)(FinalScreenW * ScaleWin);
                _graphics.PreferredBackBufferHeight = (int)(FinalScreenH * ScaleWin);

                if (_graphics.IsFullScreen)
                {
                    _game!.Window.Position = new Point(WindowX, WindowY);

                    _graphics.ToggleFullScreen();
                }

                _graphics.ApplyChanges();
            }

            OnWindowResize = true;
        }
        public void SetOriginPosition(int originX, int originY)
        {
            OriginX = originX;
            OriginY = originY;
        }
        public void SetViewAtCenter(float scale) // Place the View in the center of Monitor by scale !
        {
            ViewW = (int)(FinalScreenW * scale);
            ViewH = (int)(FinalScreenH * scale);
            ViewX = (CurrentMonitorW - ViewW) / 2;
            ViewY = (CurrentMonitorH - ViewH) / 2;
        }
        public void SetViewAtPos(int viewX, int viewY)
        {
            ViewX = viewX;
            ViewY = viewY;
            //_viewX = (_currentMonitorW - _viewW) / 2;
            //_viewY = (_currentMonitorH - _viewH) / 2;
        }
        public void SetViewAtForced(int viewX, int viewY, int viewW, int viewH)
        {
            ViewX = viewX;
            ViewY = viewY;
            ViewW = viewW;
            ViewH = viewH;
        }
        public float GetMaxScale() // Calculate max scaling factor !
        {
            float ratioHorizontal = (float)CurrentMonitorW / (float)FinalScreenW;
            float ratioVertical = (float)CurrentMonitorH / (float)FinalScreenH;

            return Math.Min(ratioHorizontal, ratioVertical);
        }
        public void PollMonitor() // Get monitor/adapter bounds infos
        //public void PollMonitor(int adapter) // Get monitor/adapter bounds infos
        {
            //CurrentMonitorX = _screens[adapter].Bounds.X;
            //CurrentMonitorY = _screens[adapter].Bounds.Y;
            //CurrentMonitorW = _screens[adapter].Bounds.Width;
            //CurrentMonitorH = _screens[adapter].Bounds.Height;

            CurrentMonitorX = 0;
            CurrentMonitorY = 0;
            CurrentMonitorW = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            CurrentMonitorH = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        }

        // Find the current monitor where the window is
        //public int GetCurrentMonitorOfWindow()
        //{
        //    for (int i = 0; i < _screens.Count(); i++)
        //    {
        //        int winX = _game!.Window.Position.X;
        //        int winY = _game.Window.Position.Y;

        //        PollMonitor(CurrentMonitor);

        //        int areaX = _screens[i].WorkingArea.X;
        //        int areaY = _screens[i].WorkingArea.Y;
        //        int areaW = _screens[i].WorkingArea.Width;
        //        int areaH = _screens[i].WorkingArea.Height;


        //        if (winX >= areaX && winY >= areaY && winX < areaX + areaW && winY < areaY + areaH)
        //        {
        //            CurrentMonitor = i;
        //            return i;
        //        }

        //    }
        //    return 0; // Main/Default monitor

        //}
        public void GetMouse(ref int xMouse, ref int yMouse, ref MouseState mouseState)
        {
            MouseState = Mouse.GetState();
            if (IsFullScreen)
            {
                xMouse = (int)((float)(MouseState.X - ViewX) / ScaleFull * ((float)ScreenW / (float)FinalScreenW));
                yMouse = (int)((float)(MouseState.Y - ViewY) / ScaleFull * ((float)ScreenH / (float)FinalScreenH));
            }
            else
            {
                xMouse = (int)((float)MouseState.X / ScaleWin * ((float)ScreenW / (float)FinalScreenW));
                yMouse = (int)((float)MouseState.Y / ScaleWin * ((float)ScreenH / (float)FinalScreenH));

            }
            if (xMouse < 0) xMouse = 0;
            if (xMouse > ScreenW - 1) xMouse = ScreenW - 1;
            if (yMouse < 0) yMouse = 0;
            if (yMouse > ScreenH - 1) yMouse = ScreenH - 1;

            mouseState = MouseState;
        }

        // Standard Control
        public void UpdateStdWindowControl()
        {
            // switch mode
            if (Input.Button.OnePress("switchMode", Keyboard.GetState().IsKeyDown(Keys.F10)))
            {
                if (_mode == Mode.NORMAL)
                    _mode = Mode.RETRO;
                else
                    _mode = Mode.NORMAL;
            }

            if (Input.Button.OnePress("fullScreen", Keyboard.GetState().IsKeyDown(Keys.F11))) ToggleFullScreen(-1); // -1 keep previous scale
            //if (Input.Button.OnePress("switchMonitor", Keyboard.GetState().IsKeyDown(Keys.F12))) SwitchMonitor(-1); // -1 keep previous scale

            if (Input.Button.OnePress("scale0", Keyboard.GetState().IsKeyDown(Keys.NumPad0))) SetScale(0);
            if (Input.Button.OnePress("scale1", Keyboard.GetState().IsKeyDown(Keys.NumPad1))) SetScale(1);
            if (Input.Button.OnePress("scale2", Keyboard.GetState().IsKeyDown(Keys.NumPad2))) SetScale(2);
            if (Input.Button.OnePress("scale3", Keyboard.GetState().IsKeyDown(Keys.NumPad3))) SetScale(3);
            if (Input.Button.OnePress("scale4", Keyboard.GetState().IsKeyDown(Keys.NumPad4))) SetScale(4);

            if (Input.Button.OnePress("natif", Keyboard.GetState().IsKeyDown(Keys.D0))) SetFinalScreenSize(ScreenW, ScreenH);
            if (Input.Button.OnePress("160x90", Keyboard.GetState().IsKeyDown(Keys.D1))) SetFinalScreenSize(160, 90);
            if (Input.Button.OnePress("320x180", Keyboard.GetState().IsKeyDown(Keys.D2))) SetFinalScreenSize(320, 180);
            if (Input.Button.OnePress("640x360", Keyboard.GetState().IsKeyDown(Keys.D3))) SetFinalScreenSize(640, 360);
            if (Input.Button.OnePress("960x540", Keyboard.GetState().IsKeyDown(Keys.D4))) SetFinalScreenSize(960, 540);
            if (Input.Button.OnePress("1280x720", Keyboard.GetState().IsKeyDown(Keys.D5))) SetFinalScreenSize(1280, 720);
            if (Input.Button.OnePress("1366x768", Keyboard.GetState().IsKeyDown(Keys.D6))) SetFinalScreenSize(1366, 768);
            if (Input.Button.OnePress("1600x900", Keyboard.GetState().IsKeyDown(Keys.D7))) SetFinalScreenSize(1600, 900);
            if (Input.Button.OnePress("1920x1080", Keyboard.GetState().IsKeyDown(Keys.D8))) SetFinalScreenSize(1920, 1080);
        }
    }
}
