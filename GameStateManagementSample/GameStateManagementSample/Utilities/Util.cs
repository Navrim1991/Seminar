using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameStateManagement.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameStateManagement.Utilities
{
    static class Util
    {
        private static NotMovingGameObject actuellBackground;

        public static NotMovingGameObject ActuellBackground
        {
            get
            {
                return actuellBackground;
            }

            set
            {
                actuellBackground = value;
            }
        }

        public static Vector2 ClampToBackground(MovingGameObject movingObject)
        {
            /*float x = MathHelper.Clamp(movingObject.Coordinates.X, actuellBackground.ActuellTexture.Bounds.X, 
                actuellBackground.ActuellTexture.Width - movingObject.ActuellTexture.Width);
            float y = MathHelper.Clamp(movingObject.Coordinates.Y, actuellBackground.ActuellTexture.Bounds.Y, 
                actuellBackground.ActuellTexture.Height - movingObject.ActuellTexture.Height * 2);*/

            float x = MathHelper.Clamp(movingObject.Coordinates.X, actuellBackground.ActuellTexture.Bounds.X,
                actuellBackground.ActuellTexture.Width - movingObject.ActuellTexture.Width);
            float y = MathHelper.Clamp(movingObject.Coordinates.Y, actuellBackground.ActuellTexture.Bounds.Y,
                actuellBackground.ActuellTexture.Height - movingObject.ActuellTexture.Height);

            return new Vector2(x, y);
        }

        public static Vector2 ClampToRectangle(MovingGameObject movingObject, Rectangle rec)
        {
            float x = MathHelper.Clamp(movingObject.Coordinates.X, rec.X,
                rec.X + rec.Width - movingObject.ActuellTexture.Width);
            float y = MathHelper.Clamp(movingObject.Coordinates.Y, rec.Y,
                rec.Y + rec.Height - movingObject.ActuellTexture.Height * 2);
            return new Vector2(x, y);
        }
    }
}
