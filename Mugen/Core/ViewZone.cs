using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mugen.Core
{
    public class ViewZone
    {
        public Window _window;
        public RenderTarget2D _renderTarget2D;
        SpriteBatch _spriteBatch;
        public Rectangle _src;
        public Rectangle _dest;

        BlendState _blendState = BlendState.NonPremultiplied;
        SamplerState _samplerState = SamplerState.PointClamp;

        public ViewZone(Window window, Rectangle src, Rectangle dest)
        {
            _window = window;

            int width = window.ScreenW;
            int height = window.ScreenH;

            _renderTarget2D = new RenderTarget2D
            (
                _window._graphics!.GraphicsDevice,
                width, height,
                false,
                SurfaceFormat.Rgba64,
                DepthFormat.Depth24,
                0,
                RenderTargetUsage.PreserveContents
            );

            _spriteBatch = new SpriteBatch(_window._graphics.GraphicsDevice);
            _src = src;

            _dest = dest;
        }
        public SpriteBatch Batch() { return _spriteBatch; }

        public void SetSrc(Rectangle src) { _src = src; }
        public void SetDest(Rectangle dest) { _dest = dest; }
        public void SetBlendState(BlendState blendState)
        {
            _blendState = blendState;
        }
        public void SetSampleState(SamplerState samplerState)
        {
            _samplerState = samplerState;
        }


        public void BeginRender()
        {
            _window._graphics!.GraphicsDevice.SetRenderTarget(_renderTarget2D);
            _spriteBatch.Begin
            (
                SpriteSortMode.Deferred,
                _blendState,
                _samplerState,
                DepthStencilState.None,
                RasterizerState.CullCounterClockwise
            );
            _window._graphics.GraphicsDevice.Clear(Color.Transparent);

        }
        public void EndRender()
        {

            //_spriteBatch.DrawLine(4, 4, 24, 16, Color.Red, 4f); // Test

            _spriteBatch.End();


        }

        public void SetViewPosition(int x, int y)
        {
            _dest.X = x;
            _dest.Y = y;
        }

        public void RenderAt(int x, int y)
        {
            _dest.X = x;
            _dest.Y = y;

            _window._graphics!.GraphicsDevice.SetRenderTarget(_window.NativeRenderTarget);
            //window._graphics.GraphicsDevice.SetRenderTarget(null);
            _window.Batch!.Draw
            (
                _renderTarget2D,
                _dest,
                _src,
                Color.White
            );
        }

        public void Render()
        {
            _window._graphics!.GraphicsDevice.SetRenderTarget(_window.NativeRenderTarget);
            //window._graphics.GraphicsDevice.SetRenderTarget(null);
            _window.Batch!.Draw
            (
                _renderTarget2D,
                _dest,
                _src,
                Color.White
            );
        }


    }
}
