using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Windows.Forms;

namespace Mugen.Core
{
    public static class ScreenManager
    {
        public static RenderTarget2D[]? _layers;

        private static WindowManager? _windowManager;
        private static SpriteBatch? _spriteBatch;

        private static Node _curScreen = new Node();
        private static Node _prevScreen = new Node();
        private static Node _showScreen = new Node(); // the Screen showed until the end of transition
        private static Node? _transition = new Node(); // the Node played when transition !

        private static bool _onTransition = false;
        private static bool _isTransition = false;
        private static bool _offTransition = false;

        private static bool _onSwap = false;
        private static bool _isSwap = false; // false : Before Swap state / true : After Swap state !

        private static Stack<Node> _stackScreen = new Stack<Node>();

        public static RenderTarget2D? GetLayer(int indexLayer)
        {
            if (_layers == null)
                return null;

            if (indexLayer < 0 || indexLayer > _layers.Length)
                return null;

            return _layers[indexLayer];

        }
        public static Node ToScreen(Node screen)
        {
            _stackScreen.Push(screen);
            return screen;
        }
        public static Node BackScreen(Node homeScreen)
        {
            if (_stackScreen.Count <= 1)
            {
                if (_stackScreen.Count > 0)
                    _stackScreen.Pop();

                return homeScreen;
            }

            _stackScreen.Pop();

            return _stackScreen.Peek();
        }
        public static void Init(WindowManager windowManager, SpriteBatch spriteBatch, Node initialScreen, int nbLayers)
        {
            _windowManager = windowManager;
            _spriteBatch = spriteBatch;

            _curScreen = initialScreen;
            _showScreen = initialScreen;
            _prevScreen = initialScreen;

            _layers = new RenderTarget2D[nbLayers];

            for (int i = 0; i < _layers.Length; i++)
            {
                _layers[i] = new(_windowManager.GDManager.GraphicsDevice, _windowManager.GetScreenSize().X, _windowManager.GetScreenSize().Y);
            }

        }

        public static Node CurScreen()
        {
            return _curScreen;
        }
        public static Node PrevScreen()
        {
            return _prevScreen;
        }

        public static void Update(GameTime gameTime)
        {
            if (_onTransition)
            {
                _onTransition = false;
                _isTransition = true;
            }

            if (_isTransition)
            {
                if (_onSwap)
                {
                    _onSwap = false;
                    _showScreen = _curScreen; // Swap screen when OnSwap is called !
                }

            }

            if (_offTransition)
            {
                _offTransition = false;
                _isTransition = false;

                _transition = null;

                // Navi System
                if (null != _curScreen._naviGate)
                    _curScreen._naviGate.SetNaviGate(true);
            }

            _showScreen.Update(gameTime);

            if (null != _transition)
                _transition.Update(gameTime);


        }

        public static void Draw(GameTime gameTime, int indexLayer = -1, SpriteSortMode sortMode = SpriteSortMode.Deferred, BlendState blendState = null, SamplerState samplerState = null, DepthStencilState depthStencilState = null, RasterizerState rasterizerState = null, Effect effect = null, Matrix? transformMatrix = null)
        {
            BeginDraw(indexLayer, sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect, transformMatrix);
            DrawLayer(indexLayer, gameTime);
            EndDraw();
        }
        public static void BeginDraw(int indexLayer = -1, SpriteSortMode sortMode = SpriteSortMode.Deferred, BlendState blendState = null, SamplerState samplerState = null, DepthStencilState depthStencilState = null, RasterizerState rasterizerState = null, Effect effect = null, Matrix? transformMatrix = null)
        {
            if (indexLayer < 0 || indexLayer >= _layers!.Count())
            {
                _windowManager!.GDManager.GraphicsDevice.SetRenderTarget(null);
                _spriteBatch!.Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect, transformMatrix);
            }
            else
            {
                _windowManager!.GDManager.GraphicsDevice.SetRenderTarget(_layers![indexLayer]);
                _spriteBatch!.Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect, transformMatrix);
            }
        }
        public static void BeginShow(SpriteSortMode sortMode = SpriteSortMode.Deferred, BlendState blendState = null, SamplerState samplerState = null, DepthStencilState depthStencilState = null, RasterizerState rasterizerState = null, Effect effect = null, Matrix? transformMatrix = null)
        {
            _windowManager!.GDManager.GraphicsDevice.SetRenderTarget(null);
            _spriteBatch!.Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect, transformMatrix);
        }
        public static void EndDraw()
        {
            _spriteBatch!.End();
        }
        public static void EndShow()
        {
            _spriteBatch!.End();
        }
        public static void DrawLayer(int indexLayer, GameTime gameTime)
        {
            _showScreen.Draw(_spriteBatch!, gameTime, indexLayer);

            if (null != _transition)
                _transition.Draw(_spriteBatch!, gameTime, indexLayer);
        }
        public static void ShowLayer(int indexLayer, Color color)
        {
            if (indexLayer < 0 || indexLayer >= _layers!.Count())
                return;

            _spriteBatch!.Draw(_layers![indexLayer], _windowManager!.StrechtingRect(), color);
        }
        public static void Swap()
        {
            _onSwap = true;
            _isSwap = true;
        }
        public static bool IsSwap()
        {
            return _isSwap;
        }
        public static bool IsTransition()
        {
            return _isTransition;
        }

        public static void StartTransition()
        {
            _onTransition = true;
            _offTransition = false;
        }
        public static void StopTransition()
        {

            _onTransition = false;
            _offTransition = true;
        }
        public static void GoTo(Node nextScreen)
        {
            _prevScreen = _curScreen;
            _curScreen = nextScreen;
            _showScreen = _curScreen;
        }
        public static void GoTo(Node nextScreen, Node transition)
        {
            _prevScreen = _curScreen;
            _showScreen = _prevScreen;
            _curScreen = nextScreen;

            _transition = transition;

            //OnTransition();

            _isSwap = false;
            _onSwap = false;

            transition.Start();
        }
    }
}
