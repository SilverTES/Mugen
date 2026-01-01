using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mugen.GFX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mugen.Core
{
    public class BasicNode : Node
    {
        private int _curLayer = 0;
        private Color _color = Color.CornflowerBlue;
        private Color _colorBorder = Color.Black;
        private float _thicknessBorder = 1f;
        public BasicNode(int layers, Color color, Color colorBorder, float thicknessBorder = 1f)
        {
            _curLayer = layers;
            _color = color;
            _colorBorder = colorBorder;
            _thicknessBorder = thicknessBorder;
        }
        public void SetLayer(int layers)
        {
            _curLayer = layers;
        }
        public void SetColor(Color color)
        {
            _color = color;
        }
        public void SetBorderColor(Color colorBorder)
        {
            _colorBorder = colorBorder;
        }
        public void SetBorderThickness(float thicknessBorder)
        {
            _thicknessBorder = thicknessBorder;
        }
        public override Node Update(GameTime gameTime)
        {
            UpdateRect();

            return base.Update(gameTime);
        }
        public override Node Draw(SpriteBatch batch, GameTime gameTime, int indexLayer)
        {
            if (indexLayer == _curLayer)
            {
                batch.FillRectangle(AbsRectF, _color);
                batch.Rectangle(AbsRectF, _colorBorder, _thicknessBorder);
            }

            return base.Draw(batch, gameTime, indexLayer);
        }
    }
}
