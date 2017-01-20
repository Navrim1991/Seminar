using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameStateManagement.Camera
{
    class Camera2D : GameComponent
    {
        protected Vector2 origin;
        protected Matrix transformMatrix;
        public float objectX { get; set; }
        public float objectY { get; set; }

        public Camera2D(Game game, Vector2 origin)
            : base(game)
        {
            this.origin = origin;
            objectX = origin.X;
            objectY = origin.Y;

            transform();
        } 

        public Matrix TransformMatrix
        {
            get
            {
                return transformMatrix;
            }
        }

        private void transform()
        {
            float scale = 1f;

            transformMatrix = Matrix.CreateScale(new Vector3(scale, scale, 0)) *
                Matrix.CreateTranslation(-objectX, -objectY, 0) *
                Matrix.CreateTranslation(Origin.X, Origin.Y, 0) *
                Matrix.CreateScale(new Vector3(scale, scale, 0));
        }

        public Vector2 Origin
        {
            get
            {
                return origin;
            }

            set
            {
                origin = value;
            }
        }

        public void Update(GameTime gameTime, GameObjects.Player player)
        {
            Texture2D background = Utilities.Util.ActuellBackground.ActuellTexture;
            KeyboardState keyboard = Keyboard.GetState();

            if (player.Coordinates.X > base.Game.GraphicsDevice.Adapter.CurrentDisplayMode.Width / 2 &&
                (player.Coordinates.X < (background.Width -
                (background.Width / 4))))
                objectX = player.Coordinates.X;
            else
            {
                if (player.Coordinates.X < base.Game.GraphicsDevice.Adapter.CurrentDisplayMode.Width / 2)
                    objectX = base.Game.GraphicsDevice.Adapter.CurrentDisplayMode.Width / 2;
                else if (player.Coordinates.X > (background.Width - (background.Width / 4)))
                    objectX = background.Width - (background.Width / 4);
            }

            if (player.Coordinates.Y > base.Game.GraphicsDevice.Adapter.CurrentDisplayMode.Height / 2 &&
                (player.Coordinates.Y < (background.Height - (background.Height / 4) + player.ActuellTexture.Height)))
                objectY = player.Coordinates.Y;
            else
            {
                if (player.Coordinates.Y < base.Game.GraphicsDevice.Adapter.CurrentDisplayMode.Height / 2)
                    objectY = base.Game.GraphicsDevice.Adapter.CurrentDisplayMode.Height / 2;
                else if (player.Coordinates.Y > (background.Height - (background.Height / 4) + player.ActuellTexture.Height))
                    objectY = background.Height - (background.Height / 4) + player.ActuellTexture.Height;

            }

            transform();

            base.Update(gameTime);
        }
    }
}
