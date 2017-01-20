using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace GameStateManagement.GameObjects
{
    enum Direction
    {
        NONE = 0x00,
        NORTH = 0x01,
        SOUTH = 0x02,
        WEST = 0x04,
        EAST = 0x08
    }
    class MovingGameObject : NotMovingGameObject
    {
        protected CollisionDetection.CollisionDirection collisionDirection;
        protected Direction direction;

        public CollisionDetection.CollisionDirection CollisionDirection
        {
            get
            {
                return collisionDirection;
            }

            set
            {
                collisionDirection = value;
            }
        }

        public Direction Direction
        {
            get
            {
                return direction;
            }

            set
            {
                direction = value;
            }
        }

        #region Constructor
        public MovingGameObject(ref ContentManager contentManager, List<string> spriteNames,
            GameObjectID gameObjectID, GameObjectOption gameObjectOption,
            Vector2 startingCoordinates, Game game)
            : base(ref contentManager, spriteNames, gameObjectID, gameObjectOption, startingCoordinates, game)
        {

        }

        public override void Update(GameTime gameTime)
        {
            bounds = new Rectangle((int)Math.Round(coordinates.X), (int)Math.Round(coordinates.Y), actuellTexture.Bounds.Width, actuellTexture.Bounds.Height);

            base.Update(gameTime);
        }

        #endregion
    }
}
