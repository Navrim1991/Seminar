using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameStateManagement.AI;

namespace GameStateManagement.GameObjects
{
    class NPC : MovingGameObject
    {
        protected Dictionary<AiTypes, float> speeds;
        protected AiOption aiOption;
        protected AiState aiState;
        protected ChaseAndEvade ai;
        protected Vector2 wanderDirection;
        protected Rectangle rangeRectangle;
        protected float turnSpeed;
        protected float orientation;


        public NPC(ref ContentManager contentManager, List<string> spriteNames,
            GameObjectID gameObjectID, GameObjectOption gameObjectOption,
            Vector2 startingCoordinates, Game game, Dictionary<AiTypes, float> speeds, AiOption aiOption)
            : base(ref contentManager, spriteNames, gameObjectID, gameObjectOption, startingCoordinates, game)
        {
            this.speeds = speeds;
            this.aiOption = aiOption;

            int width = game.GraphicsDevice.Viewport.Width / 3;
            int height = game.GraphicsDevice.Viewport.Height / 3;

            rangeRectangle = new Rectangle((int)(startingCoordinates.X - (width / 2)), 
                (int)((startingCoordinates.Y - height / 2)), 
                width, height);

            ai = new ChaseAndEvade(this, game);

            turnSpeed = 0.10f;
        }

        public Dictionary<AiTypes, float> Speeds
        {
            get
            {
                return speeds;
            }

            set
            {
                speeds = value;
            }
        }

        public AiOption AiOption
        {
            get
            {
                return aiOption;
            }

            set
            {
                aiOption = value;
            }
        }

        public AiState AiState
        {
            get
            {
                return aiState;
            }

            set
            {
                aiState = value;
            }
        }

        public Vector2 WanderDirection
        {
            get
            {
                return wanderDirection;
            }

            set
            {
                wanderDirection = value;
            }
        }

        public Rectangle RangeRectangle
        {
            get
            {
                return rangeRectangle;
            }

            set
            {
                rangeRectangle = value;
            }
        }

        public float TurnSpeed
        {
            get
            {
                return turnSpeed;
            }

            set
            {
                turnSpeed = value;
            }
        }

        public float Orientation
        {
            get
            {
                return orientation;
            }

            set
            {
                orientation = value;
            }
        }

        public override void Update(GameTime gameTime)
        {

            ai.Update(gameTime);

            coordinates = Utilities.Util.ClampToBackground(this);

            base.Update(gameTime);
        }
    }
}
