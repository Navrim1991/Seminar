using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace GameStateManagement.AI
{
    enum AiOption
    {
        NONE,
        CHASE,
        EVADE,
    }


    enum AiState
    {
        CHASING,
        CAUGHT,
        EVADE,
        WANDER
    }

    enum AiTypes
    {
        SPEED,
        DISTANCE,
        CAUGHT_DISTANCE,
        HYSTERESIS,
        TURN_SPEED

    }
    class ChaseAndEvade : GameComponent
    {
        protected GameObjects.NPC npc;
        public event EventHandler aiEvent;
        private AiOption aiOption;
        private float maxSpeed;
        private float currentSpeed;
        private float aiDistance;
        private float caughtDistance = -1;
        private float hysteresis;
        private float distance;
        private float turnSpeed;
        private GameObjects.Player player;
        private Random random;
        private Vector2 wanderVector;
        private Vector2 rangeRecMidPoint;
        private float maxDistanceFromRangeRec;
        private float distanceFromRangeRecMid;
        private float maxLerpX;
        private float minLerpX;
        private float maxLerpY;
        private float minLerpY;

        public ChaseAndEvade(GameObjects.NPC npc, Game game)
            :base(game)
        {
            this.npc = npc;
            this.npc.AiState = AiState.WANDER;

            aiOption = npc.AiOption;

            player = GameObjects.Player.Instance();

            Dictionary<AiTypes, float> speeds = npc.Speeds;

            maxSpeed = (from p in speeds where p.Key == AiTypes.SPEED select p.Value).First();
            aiDistance = (from p in speeds where p.Key == AiTypes.DISTANCE select p.Value).First();
            hysteresis = (from p in speeds where p.Key == AiTypes.HYSTERESIS select p.Value).First();
            caughtDistance = (from p in speeds where p.Key == AiTypes.CAUGHT_DISTANCE select p.Value).First();
            turnSpeed = (from p in speeds where p.Key == AiTypes.TURN_SPEED select p.Value).First();

            if (npc.AiOption == AiOption.CHASE)
                aiEvent += new EventHandler(chase);
            else
                aiEvent += new EventHandler(evade);
            random = new Random();
            wanderVector = Vector2.Zero;

            rangeRecMidPoint = Vector2.Zero;
            rangeRecMidPoint.X = npc.RangeRectangle.Center.X;
            rangeRecMidPoint.Y = npc.RangeRectangle.Center.Y;

            maxDistanceFromRangeRec = Math.Min(npc.RangeRectangle.Width / 2, npc.RangeRectangle.Height / 2);
            maxLerpX = 0.25f;
            maxLerpY = 0.25f;
            minLerpX = -0.25f;
            minLerpY = -0.25f;

        }

        private void chase(object sender, EventArgs e)
        {
            float chaseThreshold = aiDistance;
            float caughtThreshold = caughtDistance;


            if (npc.AiState == AiState.WANDER)
            {
                chaseThreshold -= hysteresis / 2;
            }
            else if (npc.AiState == AiState.CHASING)
            {
                chaseThreshold += hysteresis / 2;
                caughtThreshold -= hysteresis / 2;
            }
            else if (npc.AiState == AiState.CAUGHT)
            {
                caughtThreshold += hysteresis / 2;
            }
            

            if (distance > chaseThreshold)
                npc.AiState = AiState.WANDER;
            else if(distance > caughtThreshold)
                npc.AiState = AiState.CHASING;
            else
                npc.AiState = AiState.CAUGHT;

            if (npc.AiState == AiState.WANDER)
            {
                wander();
            }
            else if (npc.AiState == AiState.CHASING)
            {
                if(Vector2.Distance(npc.Coordinates, rangeRecMidPoint) > 700)
                {
                    wander();                        
                }
                else
                {
                    npc.Orientation = TurnToFace(npc.Coordinates, player.Coordinates, npc.Orientation, turnSpeed);

                    currentSpeed = 1.15f * GameObjects.Player.SPEED;
                }                    
            }
            else
                currentSpeed = 0;

        }

        private void wander()
        {
            wanderVector = npc.WanderDirection;
            float orientation = npc.Orientation;

            if (npc.CollisionDirection != CollisionDetection.CollisionDirection.NONE)
            {
                if (npc.CollisionDirection == CollisionDetection.CollisionDirection.SOUTH ||
                    npc.CollisionDirection == CollisionDetection.CollisionDirection.NORTH)
                    orientation *= -1;
                else
                {
                    if ((orientation >= -2.5 && orientation <= -0.5) ||
                                            (orientation < -2.5) ||
                                            (orientation > -0.5 && orientation < 0))
                        orientation = -3 - orientation;
                    else if ((orientation <= 2.5 && orientation >= 0.5) ||
                        (orientation > 2.5) ||
                        (orientation < 0.5 && orientation >= 0))
                        orientation = 3 - orientation;
                }
                    

            }
            else
            {
                wander(npc.Coordinates, ref wanderVector, ref orientation, turnSpeed);
            }

            orientation = (float)Math.Round(orientation, 1);
            npc.WanderDirection = wanderVector;
            npc.Orientation = orientation;

            currentSpeed = maxSpeed;
        }

        private void wander(Vector2 position, ref Vector2 wanderDirection, ref float orientation, float turnSpeed)
        {
            wanderDirection.X +=
                MathHelper.Lerp(minLerpX, maxLerpX, (float)random.NextDouble());
            wanderDirection.Y +=
                MathHelper.Lerp(minLerpY, maxLerpY, (float)random.NextDouble());

            if (wanderDirection != Vector2.Zero)
            {
                wanderDirection.Normalize();
            }

            orientation = TurnToFace(position, position + wanderDirection, orientation,
                        .1f * turnSpeed);

            distanceFromRangeRecMid = Vector2.Distance(rangeRecMidPoint, position);
            

            float normalizedDistance =
        distanceFromRangeRecMid / maxDistanceFromRangeRec;

            float turnToCenterSpeed = .3f * normalizedDistance * normalizedDistance *
        turnSpeed;

            orientation = TurnToFace(position, rangeRecMidPoint, orientation, turnToCenterSpeed);
        }

        private static float TurnToFace(Vector2 position, Vector2 faceThis,
       float currentAngle, float turnSpeed)
        {
            // consider this diagram:
            //         B 
            //        /|
            //      /  |
            //    /    | y
            //  / o    |
            // A--------
            //     x
            // 
            // where A is the position of the object, B is the position of the target,
            // and "o" is the angle that the object should be facing in order to 
            // point at the target. we need to know what o is. using trig, we know that
            //      tan(theta)       = opposite / adjacent
            //      tan(o)           = y / x
            // if we take the arctan of both sides of this equation...
            //      arctan( tan(o) ) = arctan( y / x )
            //      o                = arctan( y / x )
            // so, we can use x and y to find o, our "desiredAngle."
            // x and y are just the differences in position between the two objects.
            float x = faceThis.X - position.X;
            float y = faceThis.Y - position.Y;

            // we'll use the Atan2 function. Atan will calculates the arc tangent of 
            // y / x for us, and has the added benefit that it will use the signs of x
            // and y to determine what cartesian quadrant to put the result in.
            // http://msdn2.microsoft.com/en-us/library/system.math.atan2.aspx
            float desiredAngle = (float)Math.Atan2(y, x);

            // so now we know where we WANT to be facing, and where we ARE facing...
            // if we weren't constrained by turnSpeed, this would be easy: we'd just 
            // return desiredAngle.
            // instead, we have to calculate how much we WANT to turn, and then make
            // sure that's not more than turnSpeed.

            // first, figure out how much we want to turn, using WrapAngle to get our
            // result from -Pi to Pi ( -180 degrees to 180 degrees )
            float difference = WrapAngle(desiredAngle - currentAngle);

            // clamp that between -turnSpeed and turnSpeed.
            difference = MathHelper.Clamp(difference, -turnSpeed, turnSpeed);

            // so, the closest we can get to our target is currentAngle + difference.
            // return that, using WrapAngle again.
            return WrapAngle(currentAngle + difference);
        }

        /// <summary>
        /// Returns the angle expressed in radians between -Pi and Pi.
        /// <param name="radians">the angle to wrap, in radians.</param>
        /// <returns>the input value expressed in radians from -Pi to Pi.</returns>
        /// </summary>
        private static float WrapAngle(float radians)
        {
            while (radians < -MathHelper.Pi)
            {
                radians += MathHelper.TwoPi;
            }
            while (radians > MathHelper.Pi)
            {
                radians -= MathHelper.TwoPi;
            }
            return radians;
        }

        private void evade(object sender, EventArgs e)
        {
            
        }


        public override void Update(GameTime gameTime)
        {
            distance = Vector2.Distance(npc.Coordinates, player.Coordinates);

            if (aiEvent != null)
            {
                aiEvent(npc, new EventArgs());

                Vector2 heading = new Vector2(
                    (float)Math.Cos(npc.Orientation), (float)Math.Sin(npc.Orientation));

                npc.Coordinates += heading * (float)gameTime.ElapsedGameTime.TotalSeconds * currentSpeed;

                Utilities.Util.ClampToBackground(npc);

                base.Update(gameTime);
            }
 
        }
    }
}
