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
        public BasicNode(int layers, Color color)
        {
            _curLayer = layers;
            _color = color;
        }
        public void SetLayer(int layers)
        {
            _curLayer = layers;
        }
        public void SetColor(Color color)
        {
            _color = color;
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
            }

            return base.Draw(batch, gameTime, indexLayer);
        }
    }
}
