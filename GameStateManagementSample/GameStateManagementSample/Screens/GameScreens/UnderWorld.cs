using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameStateManagement.GameObjects;
using CsvHelper;
using System.IO;
using GameStateManagement.Screens.GameScreens;

namespace GameStateManagement.Screens.GameScreens
{
    class UnderWorld : GameScreen
    {
        private ContentManager contentManagerBackground;
        private ContentManager contentManagerWalls;
        private ContentManager contentManagerSpriteFonts;
        private ContentManager contentManagerNpc;
        private ContentManager contentManagerEffect;
        private List<MovingGameObject> movingGameObjects;
        private List<NotMovingGameObject> notMovingGameObjects;
        private NotMovingGameObject touchableBackground;
        private SpriteBatch spriteBatch;
        private Player player;
        private Game game;
        private CollisionDetection.QuadTree<GameObject> quadTree;
        private float pauseAlpha;
        private static bool contentLoaded;
        List<GameObject> refGameObjects;
        private bool foundEntry;
        private Vector2 foundEntryTextPosition;
        private List<House> houses;
        private KeyboardState lastKeyboardState;
        private KeyboardState currentKeyboardState;
        private Wall throne;
        private SpriteFont spriteFontArial;
        public NPC inventar;
        public NotMovingGameObject effekt_left_test;
        public NotMovingGameObject schatz;
        public NotMovingGameObject win;
        
        float xstart;
        float ystart;
        Vector2 entryPoint;
        int up = 0;
        int left = 0;
        int down = 0;
        int right = 0;
        int bogenbool = 0;
        int schwertbool = 0;
        public Vector2 vektormenu;
        int winbool = 0;
        #region Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public UnderWorld()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            movingGameObjects = new List<MovingGameObject>();
            notMovingGameObjects = new List<NotMovingGameObject>();

            contentLoaded = false;
        }


        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            if (contentLoaded)
            {
                Utilities.Util.ActuellBackground = touchableBackground;
                return;
            }

            this.spriteBatch = ScreenManager.SpriteBatch;
            game = ScreenManager.Game;

            contentManagerBackground = new ContentManager(game.Services, Config.CONTENT_BACKGROUND);
            contentManagerWalls = new ContentManager(game.Services, Config.CONTENT_UNDERWORLD);
            contentManagerSpriteFonts = new ContentManager(game.Services, Config.CONTENT_SPRITEFONTS);
            contentManagerNpc = new ContentManager(game.Services, Config.CONTENT_NPC);
            contentManagerEffect = new ContentManager(game.Services, Config.CONTENT_EFFEKT);
            touchableBackground = new NotMovingGameObject(ref contentManagerBackground,
                            new List<string>() { "HINTERGRUND_UNDERWORLD" }, GameObjectID.Gras,
                            GameObjectOption.None, new Vector2(0, 0), ScreenManager.Game);

            Utilities.Util.ActuellBackground = touchableBackground;

            player = Player.Instance();

            quadTree = new CollisionDetection.QuadTree<GameObject>(new Point(25, 25), 6, true);

            spriteFontArial = contentManagerSpriteFonts.Load<SpriteFont>("Arial");

            var sr = new StreamReader(Config.CSV_UNDERWORLD);

            var reader = new CsvReader(sr);
            reader.Configuration.Delimiter = ";";

            reader.Read();
            string[] headers = reader.FieldHeaders;

            notMovingGameObjects.Add(
                    schatz = new NotMovingGameObject(ref contentManagerEffect, new List<string>() { "Zeug" },
                    GameObjectID.Effekt, GameObjectOption.Interactive,
                    new Vector2(3400, 500), game)
                    );

            do
            {
                ContentManager tmpManager = contentManagerWalls;
                string contentName = "";
                int tmpX = -1;
                int tmpY = -1;

                

                for (int i = 0; i < headers.Count(); i++)
                {
                    switch (i)
                    {
                        case 0:
                            contentName = reader.GetField(headers[i]);
                            break;
                        case 1:
                            tmpX = Convert.ToInt32(reader.GetField(headers[i]));
                            break;
                        case 2:
                            tmpY = Convert.ToInt32(reader.GetField(headers[i]));
                            break;
                    }

                    if (tmpManager != null && contentName != "" && tmpX != -1 && tmpY != -1)
                        notMovingGameObjects.Add(new Wall(ref contentManagerWalls, new List<string>() { contentName }, GameObjectID.Wall, GameObjectOption.None, new Vector2(tmpX, tmpY), game));

                }

            } while (reader.Read());

            throne = Manager.Instance().Rooms.getThroneGameObject(this);

            foreach (NotMovingGameObject element in notMovingGameObjects)
                quadTree.Insert(element);

            contentLoaded = true;
        }

        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
        {

        }

        #endregion Initialization

        #region Update and Draw

        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);
            notMovingGameObjects.Remove(win);
            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);

            if (IsActive)
            {
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
                //3400 500
                
                if (player.Coordinates.X <= 3450 && player.Coordinates.X >= 3365 && player.Coordinates.Y <= 540 && player.Coordinates.Y >= 450)
                {
                    notMovingGameObjects.Remove(schatz);
                    winbool = 1;
                }
                if(winbool==1)
                {
                    notMovingGameObjects.Add(
                    win = new NotMovingGameObject(ref contentManagerEffect, new List<string>() { "Winner-PNG-Image" },
                    GameObjectID.Effekt, GameObjectOption.Interactive,
                    new Vector2(player.Camera2D.objectX, player.Camera2D.objectY), game)
                    );
                }
                
                movingGameObjects.Remove(inventar);
                notMovingGameObjects.Remove(effekt_left_test);

                vektormenu.X = player.Camera2D.objectX - 680;
                vektormenu.Y = player.Camera2D.objectY - 380;


                if ((Keyboard.GetState().IsKeyDown(Keys.W) || Keyboard.GetState().IsKeyDown(Keys.E)))
                {
                    if (Keyboard.GetState().IsKeyDown(Keys.W) && Keyboard.GetState().IsKeyUp(Keys.E))
                    {
                        inventar = new NPC(ref contentManagerNpc, new List<string>() { "Inventar-test-grün-1000" },
                        GameObjectID.Npc, GameObjectOption.None, vektormenu, game,
                        Config.ConfigNpcEvade, AI.AiOption.NONE);

                        if (up == 1)
                        {
                            notMovingGameObjects.Add(
                            effekt_left_test = new NotMovingGameObject(ref contentManagerEffect, new List<string>() { "Effekt-test-2" },
                            GameObjectID.Effekt, GameObjectOption.Interactive,
                            new Vector2(player.Coordinates.X + 10, player.Coordinates.Y - 15), game)
                            );

                        }
                        if (left == 1)
                        {
                            notMovingGameObjects.Add(
                            effekt_left_test = new NotMovingGameObject(ref contentManagerEffect, new List<string>() { "Effekt-test-1" },
                            GameObjectID.Effekt, GameObjectOption.Interactive,
                            new Vector2(player.Coordinates.X - 20, player.Coordinates.Y + 20), game)
                            );
                        }
                        if (down == 1)
                        {
                            notMovingGameObjects.Add(
                            effekt_left_test = new NotMovingGameObject(ref contentManagerEffect, new List<string>() { "Effekt-test-2" },
                            GameObjectID.Effekt, GameObjectOption.Interactive,
                            new Vector2(player.Coordinates.X + 10, player.Coordinates.Y + 7), game)
                            );
                        }
                        if (right == 1)
                        {
                            notMovingGameObjects.Add(
                            effekt_left_test = new NotMovingGameObject(ref contentManagerEffect, new List<string>() { "Effekt-test-1" },
                            GameObjectID.Effekt, GameObjectOption.Interactive,
                            new Vector2(player.Coordinates.X + 7, player.Coordinates.Y + 20), game)
                            );
                        }
                    }

                    if (Keyboard.GetState().IsKeyDown(Keys.E))
                    {
                        inventar = new NPC(ref contentManagerNpc, new List<string>() { "Inventar-test-grün-0100" },
                        GameObjectID.Npc, GameObjectOption.None, vektormenu, game,
                        Config.ConfigNpcEvade, AI.AiOption.NONE);

                        if (up == 1)
                        {
                            notMovingGameObjects.Add(
                            effekt_left_test = new NotMovingGameObject(ref contentManagerEffect, new List<string>() { "Bogen-w" },
                            GameObjectID.Effekt, GameObjectOption.Interactive,
                            new Vector2(player.Coordinates.X, player.Coordinates.Y - 5), game)
                            );
                        }
                        if (left == 1)
                        {
                            notMovingGameObjects.Add(
                            effekt_left_test = new NotMovingGameObject(ref contentManagerEffect, new List<string>() { "Bogen-a" },
                            GameObjectID.Effekt, GameObjectOption.Interactive,
                            new Vector2(player.Coordinates.X - 7, player.Coordinates.Y), game)
                            );
                        }
                        if (down == 1)
                        {
                            notMovingGameObjects.Add(
                            effekt_left_test = new NotMovingGameObject(ref contentManagerEffect, new List<string>() { "Bogen-s" },
                            GameObjectID.Effekt, GameObjectOption.Interactive,
                            new Vector2(player.Coordinates.X, player.Coordinates.Y + 32), game)
                            );
                        }
                        if (right == 1)
                        {
                            notMovingGameObjects.Add(
                            effekt_left_test = new NotMovingGameObject(ref contentManagerEffect, new List<string>() { "Bogen-d" },
                            GameObjectID.Effekt, GameObjectOption.Interactive,
                            new Vector2(player.Coordinates.X + 28, player.Coordinates.Y), game)
                            );
                        }
                    }
                }
                else
                {
                    inventar = new NPC(ref contentManagerNpc, new List<string>() { "Inventar-test-rot" },
                    GameObjectID.Npc, GameObjectOption.None, vektormenu, game,
                    Config.ConfigNpcEvade, AI.AiOption.NONE);
                }
                //Bogen

                // notMovingGameObjects.Add(effekt_left_test);

                movingGameObjects.Add(inventar);

                refGameObjects = quadTree.Query(player.Bounds);

                if (refGameObjects.Count > 0)
                {
                    foreach (GameObject element in refGameObjects)
                    {

                        if (CollisionDetection.CollisionDetection.checkPixelCollision(player, element))
                        {
                            if (element.ActuellTexture.Name == "Zeug")
                            {
                                winbool = 1;
                            }
                        }
                    }
                }
                else
                {
                    player.CollisionDirection = CollisionDetection.CollisionDirection.NONE;
                }

                player.Update(gameTime);
            }
        }

        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;

            KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];
            GamePadState gamePadState = input.CurrentGamePadStates[playerIndex];

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !gamePadState.IsConnected &&
                                       input.GamePadWasConnected[playerIndex];


            if (input.IsPauseGame(ControllingPlayer) || gamePadDisconnected)
            {
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
            }
        }

        private void prevLevel()
        {
            ScreenManager.RemoveScreen(this);
            Manager.Instance().preGameScreen();
            Manager.Instance().loadActuellScreen();
        }

        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {

            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                              Color.Black, 0, 0);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, player.Camera2D.TransformMatrix);


            spriteBatch.Draw(touchableBackground.ActuellTexture, touchableBackground.Coordinates, Color.White);

            foreach (NotMovingGameObject element in notMovingGameObjects)
                spriteBatch.Draw(element.ActuellTexture, element.Coordinates, Color.White);

            foreach (MovingGameObject element in movingGameObjects)
                spriteBatch.Draw(element.ActuellTexture, element.Coordinates, Color.White);

            spriteBatch.Draw(player.ActuellTexture, player.Coordinates, Color.White);

            spriteBatch.DrawString(spriteFontArial, player.Coordinates.ToString(), new Vector2(player.Coordinates.X, player.Coordinates.Y - 100), Color.Red);

            spriteBatch.End();

        }

        #endregion Update and Draw
    }
}
