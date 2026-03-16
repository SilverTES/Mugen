using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Mugen.Core
{
    public static class ScreenManager
    {
        public static Rectangle PreviousScissorRect;
        public static RasterizerState ScissorRasterizerStateOn = new RasterizerState { ScissorTestEnable = true};
        public static RasterizerState ScissorRasterizerStateOff = new RasterizerState { ScissorTestEnable = false};

        public static void BeginScissor(SpriteBatch batch, Rectangle rect, int indexLayer, LayerParameter? layerParam = null)
        {
            // Sauvegarder l'état actuel
            PreviousScissorRect = batch.GraphicsDevice.ScissorRectangle;
            batch.End();

            if (layerParam == null)
                layerParam = GetLayerParameter(indexLayer);

            BeginDraw(indexLayer, layerParam!.sortMode, layerParam.blendState, layerParam.samplerState, layerParam.depthStencilState, ScissorRasterizerStateOn, layerParam.effect, layerParam.transformMatrix);
            batch.GraphicsDevice.ScissorRectangle = rect;

        }
        public static void EndScissor(SpriteBatch batch, int indexLayer, LayerParameter? layerParam = null)
        {
            batch.End();
            // Restaurer l'état précédent
            batch.GraphicsDevice.ScissorRectangle = PreviousScissorRect;

            if (layerParam == null)
                layerParam = GetLayerParameter(indexLayer);

            BeginDraw(indexLayer, layerParam!.sortMode, layerParam.blendState, layerParam.samplerState, layerParam.depthStencilState, ScissorRasterizerStateOff, layerParam.effect, layerParam.transformMatrix);
        }

        public class LayerParameter
        {
            public SpriteSortMode sortMode = SpriteSortMode.Deferred;
            public BlendState? blendState = null;
            public SamplerState? samplerState = null;
            public DepthStencilState? depthStencilState = null;
            public RasterizerState? rasterizerState = null;
            public Effect? effect = null;
            public Matrix? transformMatrix = null;
        }

        public static List<RenderTarget2D> Layers = [];
        public static List<LayerParameter> LayerParameters = [];
        public static int NbLayers => Layers.Count;
        //private static WindowManager? _windowManager;
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

        private static List<int> _layersOrder = [];

        public static RenderTarget2D? GetLayer(int indexLayer)
        {
            if (indexLayer < 0 || indexLayer > Layers.Count)
                return null;

            return Layers[indexLayer];

        }
        public static void SetLayerParameter(
            int indexLayer, 
            SpriteSortMode sortMode = SpriteSortMode.Deferred, 
            BlendState? blendState = null, 
            SamplerState? samplerState = null, 
            DepthStencilState? depthStencilState = null, 
            RasterizerState? rasterizerState = null, 
            Effect? effect = null, 
            Matrix? transformMatrix = null)
        {
            if (indexLayer < 0 || indexLayer > Layers.Count)
                return;

            LayerParameters[indexLayer].sortMode = sortMode;
            LayerParameters[indexLayer].blendState = blendState;
            LayerParameters[indexLayer].samplerState = samplerState;
            LayerParameters[indexLayer].depthStencilState = depthStencilState;
            LayerParameters[indexLayer].rasterizerState = rasterizerState;
            LayerParameters[indexLayer].effect = effect;
            LayerParameters[indexLayer].transformMatrix = transformMatrix;
        }
        public static void SetLayerParameter(int indexLayer, LayerParameter layerParameter)
        {
            SetLayerParameter(indexLayer, layerParameter.sortMode, layerParameter.blendState, layerParameter.samplerState, layerParameter.depthStencilState, layerParameter.rasterizerState, layerParameter.effect, layerParameter.transformMatrix);
        }
        public static LayerParameter? GetLayerParameter(int indexLayer)
        {
            if (indexLayer < 0 || indexLayer > Layers.Count)
                return null;

            return LayerParameters[indexLayer];
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
        public static void Init(List<int> layersOrder)
        {
            //_windowManager = windowManager;
            _spriteBatch = WindowManager._batch;

            //_layers = new RenderTarget2D[nbLayers];
            _layersOrder = layersOrder;

            for (int i = 0; i < layersOrder.Count; i++)
            {
                Layers.Add(new(WindowManager.GDManager!.GraphicsDevice, WindowManager.GetScreenSize().X, WindowManager.GetScreenSize().Y));
                LayerParameters.Add(new LayerParameter());
            }

        }
        public static void Init(Node initialScreen, List<int> layersOrder)
        {
            //_windowManager = windowManager;
            _spriteBatch = WindowManager._batch;

            _curScreen = initialScreen;
            _showScreen = initialScreen;
            _prevScreen = initialScreen;

            //_layers = new RenderTarget2D[nbLayers];
            _layersOrder = layersOrder;

            for (int i = 0; i < layersOrder.Count; i++)
            {
                Layers.Add(new(WindowManager.GDManager!.GraphicsDevice, WindowManager.GetScreenSize().X, WindowManager.GetScreenSize().Y));
                LayerParameters.Add(new LayerParameter());
            }

        }
        public static void Init(Node initialScreen)
        {
            _curScreen = initialScreen;
            _showScreen = initialScreen;
            _prevScreen = initialScreen;
        }
        public static void SetLayersOrder(List<int> layersOrder)
        {
            _layersOrder = layersOrder;
        }
        public static List<int> GetLayersOrder()
        {
            return _layersOrder!;
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

            if (_showScreen != null)
                _showScreen.Update(gameTime);

            if (null != _transition)
                _transition.Update(gameTime);


        }

        public static void Draw(GameTime gameTime, int indexLayer = -1, SpriteSortMode sortMode = SpriteSortMode.Deferred, BlendState? blendState = null, SamplerState? samplerState = null, DepthStencilState? depthStencilState = null, RasterizerState? rasterizerState = null, Effect? effect = null, Matrix? transformMatrix = null)
        {
            BeginDraw(indexLayer, sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect, transformMatrix);
            DrawLayer(indexLayer, gameTime);
            EndDraw();
        }
        public static void BeginDraw(int indexLayer = -1, SpriteSortMode sortMode = SpriteSortMode.Deferred, BlendState? blendState = null, SamplerState? samplerState = null, DepthStencilState? depthStencilState = null, RasterizerState? rasterizerState = null, Effect? effect = null, Matrix? transformMatrix = null)
        {
            if (indexLayer < 0 || indexLayer >= Layers.Count)
            {
                _spriteBatch!.GraphicsDevice.SetRenderTarget(null);
                _spriteBatch.Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect, transformMatrix);
            }
            else
            {
                _spriteBatch!.GraphicsDevice.SetRenderTarget(Layers![indexLayer]);
                _spriteBatch.Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect, transformMatrix);
            }
        }
        public static void BeginShow(SpriteSortMode sortMode = SpriteSortMode.Deferred, BlendState? blendState = null, SamplerState? samplerState = null, DepthStencilState? depthStencilState = null, RasterizerState? rasterizerState = null, Effect? effect = null, Matrix? transformMatrix = null)
        {
            _spriteBatch!.GraphicsDevice.SetRenderTarget(null);
            _spriteBatch.Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect, transformMatrix);
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
            if (_showScreen != null)
                _showScreen.Draw(_spriteBatch!, gameTime, indexLayer);

            if (null != _transition)
                _transition.Draw(_spriteBatch!, gameTime, indexLayer);
        }
        public static void ShowLayer(int indexLayer, Color color)
        {
            if (indexLayer < 0 || indexLayer >= Layers.Count)
                return;

            _spriteBatch!.Draw(Layers[indexLayer], WindowManager.StrechtingRect(), color);
        }
        public static void DrawScreen(GameTime gameTime)
        {
            for (int i = 0; i < _layersOrder.Count; i++)
            {
                int indexLayer = _layersOrder[i];
                BeginDraw(indexLayer, LayerParameters[indexLayer].sortMode, LayerParameters[indexLayer].blendState, LayerParameters[indexLayer].samplerState, LayerParameters[indexLayer].depthStencilState, LayerParameters[indexLayer].rasterizerState, LayerParameters[indexLayer].effect, LayerParameters[indexLayer].transformMatrix);
                DrawLayer(indexLayer, gameTime);
                EndDraw();
            }
        }
        public static void ShowScreen(GameTime gameTime, SpriteSortMode sortMode = SpriteSortMode.Deferred, BlendState? blendState = null, SamplerState? samplerState = null, DepthStencilState? depthStencilState = null, RasterizerState? rasterizerState = null, Effect? effect = null, Matrix? transformMatrix = null)
        {
            BeginShow(sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect, transformMatrix);
            for (int i = 0; i < _layersOrder.Count; i++)
            {
                ShowLayer(_layersOrder[i], Color.White);
            }
            EndShow();
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

            transition.StartScreenTransition();
        }
    }
}
