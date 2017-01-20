using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using GameStateManagement.GameObjects;
using Microsoft.Xna.Framework.Input;

namespace GameStateManagement.Screens.GameScreens
{
    class OverWorld : GameScreen
    {
        private List<MovingGameObject> movingGameObjects;
        private List<NotMovingGameObject> notMovingGameObjects;
        private NotMovingGameObject touchableBackground;
        private SpriteBatch spriteBatch;
        private ContentManager contentManagerBackground;
        private ContentManager contentManagerHouses;
        private ContentManager contentManagerTrees;
        private ContentManager contentManagerPlayer;
        private ContentManager contentManagerNpc;
        private ContentManager contentManagerSpriteFonts;

        private ContentManager contentManagerEffect;
        private Player player;
        private Game game;
        private CollisionDetection.QuadTree<GameObject> quadTree;
        private int recieveCount;
        private SpriteFont spriteFontArial;
        private float pauseAlpha;
        List<GameObject> refGameObjects;
        public Vector2 vektormenu;
        public NPC inventar;
        public NotMovingGameObject effekt_left_test;
        float xstart;
        float ystart;
        int left = 0;
        int right = 0;
        int up = 0;
        int down = 0;
    


        public OverWorld()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            movingGameObjects = new List<MovingGameObject>();
            notMovingGameObjects = new List<NotMovingGameObject>();
        }

        public override void LoadContent()
        {
            this.spriteBatch = ScreenManager.SpriteBatch;
            game = ScreenManager.Game;

            contentManagerBackground = new ContentManager(game.Services, Config.CONTENT_BACKGROUND);
            contentManagerHouses = new ContentManager(game.Services, Config.CONTENT_HOUSES);
            contentManagerPlayer = new ContentManager(game.Services, Config.CONTENT_PLAYER);
            contentManagerNpc = new ContentManager(game.Services, Config.CONTENT_NPC);
            contentManagerSpriteFonts = new ContentManager(game.Services, Config.CONTENT_SPRITEFONTS);
            contentManagerEffect = new ContentManager(game.Services, Config.CONTENT_EFFEKT);

            quadTree = new CollisionDetection.QuadTree<GameObject>(new Point(25,25), 4, true);
            spriteFontArial = contentManagerSpriteFonts.Load<SpriteFont>("Arial");

            Vector2 playerPostion = new Vector2(game.GraphicsDevice.Adapter.CurrentDisplayMode.Width / 2,
                  game.GraphicsDevice.Adapter.CurrentDisplayMode.Height / 2);

            player = Player.Instance("Marvin", PlayerIndex.One, ref contentManagerPlayer, 
                new List<string>() { "monster_1" }, GameObjectID.Player, 
                GameObjectOption.Attackable & GameObjectOption.Interactive,
                playerPostion, game);
            xstart = player.Coordinates.X;
            ystart = player.Coordinates.Y;
            //
            vektormenu = new Vector2(player.Coordinates.X - 100, player.Coordinates.Y + 300);
            //
            refGameObjects = new List<GameObject>();

            //quadTree.Insert(player);

            touchableBackground = new NotMovingGameObject(ref contentManagerBackground,
                            new List<string>() { "Karte_oberwelt" }, GameObjectID.Gras,
                            GameObjectOption.None, new Vector2(0, 0), ScreenManager.Game);

            Utilities.Util.ActuellBackground = touchableBackground;

            //NPCs
            movingGameObjects.Add(
                new NPC(ref contentManagerNpc, new List<string>() { "hexe_stand_bewegung_rechts" },
                GameObjectID.Npc, GameObjectOption.Attackable, new Vector2(199, 500), game, 
                Config.ConfigNpcEvade, AI.AiOption.CHASE)
                );



            foreach (MovingGameObject element in movingGameObjects)
                quadTree.Insert(element);


            notMovingGameObjects.Add(
                new NotMovingGameObject(ref contentManagerHouses, new List<string>() { "house_fence" },
                GameObjectID.House, GameObjectOption.Interactive,
                new Vector2(150, 350), game)
                );
            //Houses
            /*notMovingGameObjects.Add(
                new NotMovingGameObject(ref contentManagerHouses, new List<string>() { "house_fence" }, 
                GameObjectID.House, GameObjectOption.Interactive, 
                new Vector2(199, 500), game)
                );

            notMovingGameObjects.Add(
                new NotMovingGameObject(ref contentManagerHouses, new List<string>() { "house_fence" },
                GameObjectID.House, GameObjectOption.Interactive,
                new Vector2(400, 700), game)
                );

            notMovingGameObjects.Add(
                new NotMovingGameObject(ref contentManagerHouses, new List<string>() { "house_fence" },
                GameObjectID.House, GameObjectOption.Interactive,
                new Vector2(10, 200), game)
                );

            notMovingGameObjects.Add(
                new NotMovingGameObject(ref contentManagerHouses, new List<string>() { "house_fence" },
                GameObjectID.House, GameObjectOption.Interactive,
                new Vector2(1600, 500), game)
                );

            notMovingGameObjects.Add(
                new NotMovingGameObject(ref contentManagerHouses, new List<string>() { "house_fence" },
                GameObjectID.House, GameObjectOption.Interactive,
                new Vector2(1400, 220), game)
                );

            notMovingGameObjects.Add(
                new NotMovingGameObject(ref contentManagerHouses, new List<string>() { "house_fence" },
                GameObjectID.House, GameObjectOption.Interactive,
                new Vector2(600, 50), game)
                );

            notMovingGameObjects.Add(
                new NotMovingGameObject(ref contentManagerHouses, new List<string>() { "house_fence" },
                GameObjectID.House, GameObjectOption.Interactive,
                new Vector2(550, 200), game)
                );*/
            

            foreach (NotMovingGameObject element in notMovingGameObjects)
                quadTree.Insert(element);

            base.LoadContent();

            /*contentManagerBackground.Dispose();
            contentManagerHouses.Dispose();
            contentManagerPlayer.Dispose();*/
            /*
           NPC inventar = new NPC(ref contentManagerNpc, new List<string>() { "Inventar" },
                GameObjectID.Npc, GameObjectOption.Attackable, new Vector2(player.Coordinates.X - 100, player.Coordinates.Y + 300), game,
                Config.ConfigNpcEvade, AI.AiOption.NONE);
            movingGameObjects.Add(inventar);
            */
            /*
            notMovingGameObjects.Add(
                new NotMovingGameObject(ref contentManagerHouses, new List<string>() { "Inventar" },
                GameObjectID., GameObjectOption.Interactive,
                new Vector2(1400, 220), game)
                );
                */
            
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
        }


        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            //richtung
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                up = 1;
                left = 0;
                down = 0;
                right = 0;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                up = 0;
                left = 1;
                down = 0;
                right = 0;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                up = 0;
                left = 0;
                down = 1;
                right = 0;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                up = 0;
                left = 0;
                down = 0;
                right = 1;
            }
                //player.Coordinates.X - 100, player.Coordinates.Y + 300

                //movingGameObjects.Remove(inventar);
                //vektormenu.X = player.Coordinates.X;

                movingGameObjects.Remove(inventar);
            notMovingGameObjects.Remove(effekt_left_test);
            /*
            vektormenu.X = player.Coordinates.X - 100;
            vektormenu.Y = player.Coordinates.Y + 300;
            */
            //inventar.Coordinates = new Vector2()
            vektormenu.X = player.Camera2D.objectX-680;
            vektormenu.Y = player.Camera2D.objectY-380;
            /*
            inventar = new NPC(ref contentManagerNpc, new List<string>() { "Inventar-test-r" },
                GameObjectID.Npc, GameObjectOption.None, vektormenu, game,
                Config.ConfigNpcEvade, AI.AiOption.NONE);
             */   
             
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                inventar = new NPC(ref contentManagerNpc, new List<string>() { "Inventar-test-grün-1000" },
                GameObjectID.Npc, GameObjectOption.None, vektormenu, game,
                Config.ConfigNpcEvade, AI.AiOption.NONE);

                if (left==1)
                {
                    notMovingGameObjects.Add(
                    effekt_left_test = new NotMovingGameObject(ref contentManagerEffect, new List<string>() { "Effekt-test-1" },
                    GameObjectID.Effekt, GameObjectOption.Interactive,
                    new Vector2(player.Coordinates.X - 20, player.Coordinates.Y + 20), game)
                    );
                }
                if (right==1)
                {
                    notMovingGameObjects.Add(
                    effekt_left_test = new NotMovingGameObject(ref contentManagerEffect, new List<string>() { "Effekt-test-1" },
                    GameObjectID.Effekt, GameObjectOption.Interactive,
                    new Vector2(player.Coordinates.X + 8, player.Coordinates.Y + 20), game)
                    );
                }
                // notMovingGameObjects.Add(effekt_left_test);



            }
            else
            {
                if (Keyboard.GetState().IsKeyDown(Keys.E))
                {
                    inventar = new NPC(ref contentManagerNpc, new List<string>() { "Inventar-test-grün-0100" },
                    GameObjectID.Npc, GameObjectOption.None, vektormenu, game,
                    Config.ConfigNpcEvade, AI.AiOption.NONE);
                }
                else
                {
                    inventar = new NPC(ref contentManagerNpc, new List<string>() { "Inventar-test-rot" },
                    GameObjectID.Npc, GameObjectOption.None, vektormenu, game,
                    Config.ConfigNpcEvade, AI.AiOption.NONE);
                }
            }
            
                movingGameObjects.Add(inventar);

          
            base.Update(gameTime, otherScreenHasFocus, false);

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);

            if(this.IsActive)
            {
                refGameObjects = quadTree.Query(player.Bounds);

                if (refGameObjects.Count > 0)
                {
                    foreach (GameObject element in refGameObjects)
                    {
                        if (CollisionDetection.CollisionDetection.checkPixelCollision(player, element))
                        {
                            if(element.isNpc())
                            {

                            }
                        }
                    }
                }
                else
                {
                    player.CollisionDirection = CollisionDetection.CollisionDirection.NONE;
                }

                foreach(NPC element in movingGameObjects)
                {
                    refGameObjects = quadTree.Query(element.Bounds);

                    if (refGameObjects.Count > 0)
                    {
                        element.CollisionDirection = CollisionDetection.CollisionDirection.NONE;

                        foreach (GameObject gameObject in refGameObjects)
                        {
                            CollisionDetection.CollisionDetection.checkSideCollision(element, gameObject);
                        }
                    }
                    else
                        element.CollisionDirection = CollisionDetection.CollisionDirection.NONE;

                    element.Update(gameTime);
                }

                player.Update(gameTime);
            }
        }

        public override void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;

            KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];

            if (input.IsPauseGame(ControllingPlayer))
            {
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.CornflowerBlue, 0, 0);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, player.Camera2D.TransformMatrix);
                

            spriteBatch.Draw(touchableBackground.ActuellTexture, touchableBackground.Coordinates, Color.White);

            foreach(NotMovingGameObject element in notMovingGameObjects)
                spriteBatch.Draw(element.ActuellTexture, element.Coordinates, Color.Red);

            foreach(MovingGameObject element in movingGameObjects)
            {
                spriteBatch.Draw(element.ActuellTexture, element.Coordinates, Color.White);
                spriteBatch.DrawString(spriteFontArial, ((NPC)element).Orientation.ToString(), new Vector2(10, 250), Color.Red);

            }


            //spriteBatch.DrawString(spriteFontArial, refGameObjects.Count.ToString(), new Vector2(10, 10), Color.Red);

            //spriteBatch.Draw(player.ActuellTexture, player.Coordinates, null, Color.White, 180f, player.Coordinates, SpriteEffects.FlipHorizontally,0.0f);
            spriteBatch.Draw(player.ActuellTexture, player.Coordinates, Color.White);

            spriteBatch.End();
            base.Draw(gameTime);
        }

    }
}
