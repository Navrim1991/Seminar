using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;



namespace GameStateManagement.GameObjects
{
    class Player : MovingGameObject
    {
        #region private variables

        private string name;
        private const float PLAYER_SPEED = 150;
        private PlayerIndex playerIndex;
        private Camera.Camera2D camera;
        private Texture2D healthRec;


        public Camera.Camera2D Camera2D
        {
            get
            {
                return camera;
            }
        }

        public static float SPEED
        {
            get
            {
                return PLAYER_SPEED;
            }
        }

        #endregion

        #region Singleton variable
        //Singleton
        private static Player thisPlayer;

        #endregion

        #region Constructor
        private Player(string name, PlayerIndex playerIndex, ref ContentManager contentManager, List<string> spriteNames,
            GameObjectID gameObjectID, GameObjectOption gameObjectOption,
            Vector2 startingCoordinates, Game game)
            : base(ref contentManager, spriteNames, gameObjectID, gameObjectOption, startingCoordinates, game)
        {
            this.name = name;
            this.playerIndex = playerIndex;
            camera = new Camera.Camera2D(game, startingCoordinates);

            healthRec = new Texture2D(GraphicsDevice, actuellTexture.Width * 2, (int)((1f / 5f) * actuellTexture.Height));


        }

        #endregion

        #region Singlton Instance

        public static Player Instance()
        {
            if (thisPlayer == null)
                throw new ArgumentNullException("Player is null");

            return thisPlayer;
        }
        public static Player Instance(string name, PlayerIndex playerIndex,
            ref ContentManager contentManager, List<string> spriteNames,
            GameObjectID gameObjectID, GameObjectOption gameObjectOption,
            Vector2 startingCoordinates, Game game)
        {
            if (thisPlayer == null)
                thisPlayer = new Player(name, playerIndex, ref contentManager, spriteNames, gameObjectID, gameObjectOption, startingCoordinates, game);

            return thisPlayer;
        }
        #endregion

        #region override update Method
        public override void Update(GameTime gameTime)
        {
            KeyboardState keyboard = Keyboard.GetState();
            //throw new NotImplementedException();
            int forceX = 0;
            int forceY = 0;

            if (keyboard.IsKeyDown(Keys.Left) ||
                keyboard.IsKeyDown(Keys.Right) ||
                keyboard.IsKeyDown(Keys.Up) ||
                keyboard.IsKeyDown(Keys.Down))
                direction = Direction.NONE;

            if (keyboard.IsKeyDown(Keys.Left))
            {
                //resetDirection(Direction.EAST);
                direction |= Direction.WEST;

                if (!Convert.ToBoolean(collisionDirection & CollisionDetection.CollisionDirection.WEST))
                {
                    forceX += -1;

                    forceX = Math.Min(Math.Max(-1, forceX), 1);

                    this.coordinates.X += forceX * (float)gameTime.ElapsedGameTime.TotalSeconds * PLAYER_SPEED;
                }
            }
            else if (keyboard.IsKeyDown(Keys.Right))
            {
                //resetDirection(Direction.WEST);
                direction |= Direction.EAST;

                if (!Convert.ToBoolean(collisionDirection & CollisionDetection.CollisionDirection.EAST))
                {
                    forceX += 1;

                    forceX = Math.Min(Math.Max(-1, forceX), 1);

                    this.coordinates.X += forceX * (float)gameTime.ElapsedGameTime.TotalSeconds * PLAYER_SPEED;
                }
            }

            if (keyboard.IsKeyDown(Keys.Up))
            {
                //resetDirection(Direction.SOUTH);
                direction |= Direction.NORTH;

                if (!Convert.ToBoolean(collisionDirection & CollisionDetection.CollisionDirection.NORTH))
                {
                    forceY += 1;

                    forceY = Math.Min(Math.Max(-1, forceY), 1);

                    this.coordinates.Y += -forceY * (float)gameTime.ElapsedGameTime.TotalSeconds * PLAYER_SPEED;
                }
            }

            if (keyboard.IsKeyDown(Keys.Down))
            {
                //resetDirection(Direction.NORTH);
                direction |= Direction.SOUTH;

                if (!Convert.ToBoolean(collisionDirection & CollisionDetection.CollisionDirection.SOUTH))
                {
                    forceY += -1;

                    forceY = Math.Min(Math.Max(-1, forceY), 1);

                    this.coordinates.Y += -forceY * (float)gameTime.ElapsedGameTime.TotalSeconds * PLAYER_SPEED;
                }


            }
            //Marvin zeigen
            if (keyboard.IsKeyDown(Keys.V))
            {
                this.coordinates.Y = 500;
                this.coordinates.X = 3400;
            }
            if (this.coordinates.Y == 1100 && this.coordinates.X == 1000)
            {
                this.coordinates.Y = 100;
                this.coordinates.X = 100;
            }
            //
            camera.Update(gameTime, this);

            coordinates = Utilities.Util.ClampToBackground(this);

            base.Update(gameTime);
        }


        #endregion

    }
}
