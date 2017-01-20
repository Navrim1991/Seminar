using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameStateManagement.CollisionDetection
{
    enum CollisionDirection
    {
        NONE = 0x00,
        NORTH = 0x01,
        SOUTH = 0x02,
        WEST = 0x04,
        EAST = 0x08,
        DETECTED = 0x10

    }
    static class CollisionDetection
    {
        private static int firstLeft;
        private static int firstRight;
        private static int firstTop;
        private static int firstBottom;
        private static int firstWidth;
        private static int firstHeight;

        private static int secondLeft;
        private static int secondRight;
        private static int secondTop;
        private static int secondBottom;
        private static int secondWidth;
        private static int secondHeight;
        public static bool checkPixelCollision(GameObjects.GameObject first, GameObjects.GameObject second)
        {
            firstLeft = first.Bounds.Left;
            firstRight = first.Bounds.Right;
            firstTop = first.Bounds.Top;
            firstBottom = first.Bounds.Bottom;
            firstWidth = first.Bounds.Width;
            firstHeight = first.Bounds.Height;

            secondLeft = second.Bounds.Left;
            secondRight = second.Bounds.Right;
            secondTop = second.Bounds.Top;
            secondBottom = second.Bounds.Bottom;
            secondWidth = second.Bounds.Width;
            secondHeight = second.Bounds.Height;

            Color[] colorsFirst = new Color[firstWidth * firstHeight];
            Color[] colorsSecond = new Color[secondWidth * secondHeight];

            first.ActuellTexture.GetData(colorsFirst);
            second.ActuellTexture.GetData(colorsSecond);



            int top = Math.Max(firstTop, secondTop);
            int bottom = Math.Min(firstBottom, secondBottom);
            int left = Math.Max(firstLeft, secondLeft);
            int right = Math.Min(firstRight, secondRight);

            for (int y = top; y < bottom; y++)
            {
                for (int x = left; x < right; x++)
                {
                    Color colorFirst = colorsFirst[(x - firstLeft) + (y - firstTop) * firstWidth];
                    Color colorSecond = colorsSecond[(x - secondLeft) + (y - secondTop) * secondWidth];

                    if (colorFirst.A != 0 && colorSecond.A != 0)
                    {
                        GameObjects.MovingGameObject collisionObject;

                        if (first is GameObjects.MovingGameObject)
                        {                      

                            collisionObject = (GameObjects.MovingGameObject)first;
                            if(collisionObject.Direction != GameObjects.Direction.NONE)
                            {
                                collisionObject.CollisionDirection = (CollisionDirection)collisionObject.Direction;
                            }
                            
                        }

                        if (second is GameObjects.MovingGameObject)
                        {
                            collisionObject = (GameObjects.MovingGameObject)second;
                            if (collisionObject.Direction != GameObjects.Direction.NONE)
                            {
                                collisionObject.CollisionDirection = (CollisionDirection)collisionObject.Direction;
                            }
                        }

                        return true;
                    }
                    else
                    {
                        if(first is GameObjects.MovingGameObject)
                        {
                            ((GameObjects.MovingGameObject)first).CollisionDirection = CollisionDirection.NONE;  

                        }

                        if (second is GameObjects.MovingGameObject)
                        {
                            ((GameObjects.MovingGameObject)second).CollisionDirection = CollisionDirection.NONE;
                        }
                    }
                    
                        
                }
            }

            return false;
        }

        public static bool checkSideCollision(GameObjects.MovingGameObject movingGameObject, GameObjects.GameObject gameObject)
        {
            Rectangle MovingGameObjectRec = movingGameObject.Bounds;
            Rectangle gameObjectRec = gameObject.Bounds;

            if ((MovingGameObjectRec.Bottom - gameObjectRec.Top) == 1)
                movingGameObject.CollisionDirection = CollisionDirection.SOUTH;

            if ((MovingGameObjectRec.Top - gameObjectRec.Bottom) == -1)
                movingGameObject.CollisionDirection = CollisionDirection.NORTH;

            if ((MovingGameObjectRec.Left - gameObjectRec.Right) == -1)
                movingGameObject.CollisionDirection = CollisionDirection.WEST;

            if ((MovingGameObjectRec.Right - gameObjectRec.Left) == 1)
                movingGameObject.CollisionDirection = CollisionDirection.EAST;

            if (movingGameObject.CollisionDirection != CollisionDirection.NONE)
                return true;

            return false;
        }
    }
}
