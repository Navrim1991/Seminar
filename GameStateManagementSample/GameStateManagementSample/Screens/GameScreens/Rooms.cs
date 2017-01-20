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

namespace GameStateManagement.Screens.GameScreens
{
    class Rooms : GameScreen
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

        private SpriteFont spriteFontArial;
        public NPC inventar;
        public NotMovingGameObject effekt_left_test;
        public NotMovingGameObject bogen;
        public NotMovingGameObject schwert;
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
        #region Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public Rooms()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            movingGameObjects = new List<MovingGameObject>();
            notMovingGameObjects = new List<NotMovingGameObject>();

            contentLoaded = false;
        }

        public Wall getThroneGameObject(UnderWorld underworld)
        {
            Wall throne = null;

            Rectangle rec = new Rectangle(0, 0, 0, 0);

            foreach (NotMovingGameObject element in notMovingGameObjects)
            {
                if(element.ActuellTexture.Name == "trohn")
                {
                    if(element is Wall)
                        throne = (Wall)element;
                }
            }

            return throne;
        }

        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            if (contentLoaded)
            {
                setPlayerCoordinatesFromHouse();

                Utilities.Util.ActuellBackground = touchableBackground;
                return;
            }

            this.spriteBatch = ScreenManager.SpriteBatch;
            game = ScreenManager.Game;

            contentManagerBackground = new ContentManager(game.Services, Config.CONTENT_BACKGROUND);
            contentManagerWalls = new ContentManager(game.Services, Config.CONTENT_ROOMS);
            contentManagerSpriteFonts = new ContentManager(game.Services, Config.CONTENT_SPRITEFONTS);
            contentManagerNpc = new ContentManager(game.Services, Config.CONTENT_NPC);
            contentManagerEffect = new ContentManager(game.Services, Config.CONTENT_EFFEKT);
            spriteFontArial = contentManagerSpriteFonts.Load<SpriteFont>("Arial");

            quadTree = new CollisionDetection.QuadTree<GameObject>(new Point(25, 25), 6, true);

            touchableBackground = new NotMovingGameObject(ref contentManagerBackground,
                            new List<string>() { "HINTERGRUND_ROOMS" }, GameObjectID.Gras,
                            GameObjectOption.None, new Vector2(0, 0), ScreenManager.Game);

            Utilities.Util.ActuellBackground = touchableBackground;

            player = Player.Instance();

            var sr = new StreamReader(Config.CSV_ROOMS);

            var reader = new CsvReader(sr);
            reader.Configuration.Delimiter = ";";

            reader.Read();
            string[] headers = reader.FieldHeaders;

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

            }while(reader.Read());

            Wall hotel = null;
            Wall cellarEntry = null;
            Wall reception = null;
            Wall treasury = null;
            Wall castleEntry = null;
            Wall cellarStairs = null;
            Wall cellar = null;
            Wall throneRoom = null;

            foreach (Wall element in notMovingGameObjects)
            {
                if(element.ActuellTexture.Name == "hotel")
                    hotel = element;
                else if (element.ActuellTexture.Name == "keller_eingang")
                    cellarEntry = element;
                else if (element.ActuellTexture.Name == "rezeption")
                    reception = element;
                else if (element.ActuellTexture.Name == "schatzkammer")
                    treasury = element;
                else if (element.ActuellTexture.Name == "schloss_eingang")
                    castleEntry = element;
                else if (element.ActuellTexture.Name == "keller_treppe")
                    cellarStairs = element;
                else if (element.ActuellTexture.Name == "keller")
                    cellar = element;
                else if (element.ActuellTexture.Name == "trohn_raum")
                    throneRoom = element;
            }

            houses = Manager.Instance().OverWorld.getAllEntryHouses(this);

            for(int i = 0; i<houses.Count;i++)
            {
                NotMovingGameObject.DomainStruct refObjHouse = houses[i].EntryDomainStruct;
                NotMovingGameObject.DomainStruct refObjWall_1;
                NotMovingGameObject.DomainStruct refObjWall_2;
                switch (i)
                {
                    case 0:
                        if (castleEntry == null)
                            continue;

                        refObjWall_1 = castleEntry.Domains.First();
                        castleEntry.Domains.Remove(refObjWall_1);
                        setLink(ref refObjHouse, ref refObjWall_1, houses[0], castleEntry);
                        castleEntry.Domains.Add(refObjWall_1);

                        refObjWall_1 = castleEntry.Domains.First();
                        refObjWall_2 = throneRoom.Domains.First();

                        castleEntry.Domains.Remove(refObjWall_1);
                        throneRoom.Domains.Remove(refObjWall_2);

                        setLink(ref refObjWall_1, ref refObjWall_2, castleEntry, throneRoom);
                        castleEntry.Domains.Add(refObjWall_1);
                        throneRoom.Domains.Add(refObjWall_2);

                        break;
                    case 1:
                        if (hotel == null)
                            continue;

                        refObjWall_1 = hotel.Domains.First();
                        hotel.Domains.Remove(refObjWall_1);
                        setLink(ref refObjHouse, ref refObjWall_1, houses[1], hotel);
                        hotel.Domains.Add(refObjWall_1);
                        break;
                    case 2:
                        if (cellarEntry == null)
                            continue;

                        refObjWall_1 = cellarEntry.Domains.First();
                        cellarEntry.Domains.Remove(refObjWall_1);
                        setLink(ref refObjHouse, ref refObjWall_1, houses[2], cellarEntry);
                        cellarEntry.Domains.Add(refObjWall_1);

                        refObjWall_1 = cellarEntry.Domains.First();
                        refObjWall_2 = cellarStairs.Domains.First();

                        cellarEntry.Domains.Remove(refObjWall_1);
                        cellarStairs.Domains.Remove(refObjWall_2);

                        setLink(ref refObjWall_1, ref refObjWall_2, cellarEntry, cellarStairs);
                        cellarEntry.Domains.Add(refObjWall_1);
                        cellarStairs.Domains.Add(refObjWall_2);

                        refObjWall_1 = cellar.Domains.First();
                        refObjWall_2 = cellarStairs.Domains.First();  
                        
                        cellar.Domains.Remove(refObjWall_1);
                        cellarStairs.Domains.Remove(refObjWall_2);

                        setLink(ref refObjWall_1, ref refObjWall_2, cellar, cellarStairs);
                        cellar.Domains.Add(refObjWall_1);
                        cellarStairs.Domains.Add(refObjWall_2);

                        break;
                    case 3:
                        if (reception == null)
                            continue;

                        refObjWall_1 = reception.Domains.First();
                        reception.Domains.Remove(refObjWall_1);
                        setLink(ref refObjHouse, ref refObjWall_1, houses[3], reception);
                        reception.Domains.Add(refObjWall_1);
                        break;
                    case 4:
                        if (treasury == null)
                            continue;

                        refObjWall_1 = treasury.Domains.First();
                        treasury.Domains.Remove(refObjWall_1);
                        setLink(ref refObjHouse, ref refObjWall_1, houses[4], treasury);
                        treasury.Domains.Add(refObjWall_1);


                        break;
                }

                houses[i].EntryDomainStruct = refObjHouse;
            }

            setPlayerCoordinatesFromHouse();

            foreach (NotMovingGameObject element in notMovingGameObjects)
                quadTree.Insert(element);

            contentLoaded = true;

            base.LoadContent();
        }

        private void setLink(ref NotMovingGameObject.DomainStruct houseStruct, ref NotMovingGameObject.DomainStruct wallStruct, House house, Wall wall)
        {
            houseStruct = new NotMovingGameObject.DomainStruct(houseStruct.hasContract, houseStruct.domainTyp, houseStruct.position, wall);
            wallStruct = new NotMovingGameObject.DomainStruct(wallStruct.hasContract, wallStruct.domainTyp, wallStruct.position, house);
        }

        private void setLink(ref NotMovingGameObject.DomainStruct wallStruct_1, ref NotMovingGameObject.DomainStruct wallStruct_2, Wall wall_1, Wall wall_2)
        {
            wallStruct_1 = new NotMovingGameObject.DomainStruct(wallStruct_1.hasContract, wallStruct_1.domainTyp, wallStruct_1.position, wall_2);
            wallStruct_2 = new NotMovingGameObject.DomainStruct(wallStruct_2.hasContract, wallStruct_2.domainTyp, wallStruct_2.position, wall_1);
        }

        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
        {
        }

        #endregion Initialization

        private void setPlayerCoordinatesFromHouse()
        {
            for (int i = 0; i < houses.Count; i++)
            {
                NotMovingGameObject.DomainStruct tmpHouse = houses[i].EntryDomainStruct;
                if (tmpHouse.hasContract)
                {
                    if (tmpHouse.link is Wall)
                    {
                        Wall tmpWall = (Wall)tmpHouse.link;

                        foreach (NotMovingGameObject.DomainStruct element in tmpWall.Domains)
                        {
                            if (element.link is House)
                            {
                                player.Coordinates = new Vector2(element.position.X, element.position.Y);
                                break;
                            }
                        }
                    }

                    break;
                }
            }
        }

        #region Update and Draw

        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            currentKeyboardState = Keyboard.GetState();

            base.Update(gameTime, otherScreenHasFocus, false);

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);

            if (IsActive)
            {
                foundEntry = false;

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


                movingGameObjects.Remove(inventar);
                notMovingGameObjects.Remove(effekt_left_test);

                vektormenu.X = player.Camera2D.objectX - 680;
                vektormenu.Y = player.Camera2D.objectY - 380;

               
                if ((Keyboard.GetState().IsKeyDown(Keys.W) || Keyboard.GetState().IsKeyDown(Keys.E)))
                {
                    if (Keyboard.GetState().IsKeyDown(Keys.W) && Keyboard.GetState().IsKeyUp(Keys.E) )
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

                        }

                        if(element is Wall)
                        {
                            Wall wall = (Wall)element;

                            if(wall.ActuellTexture.Name == "trohn")
                            {
                                foundEntry = true;

                                foundEntryTextPosition = new Vector2(player.Coordinates.X, player.Coordinates.Y + 50);

                                if (Keyboard.GetState().IsKeyDown(Keys.Space))
                                {
                                    player.CollisionDirection = CollisionDetection.CollisionDirection.NONE;

                                    foundEntry = false;

                                    player.Coordinates = new Vector2(3503, 752);

                                    nextLevel();
                                }
                            }

                            int i = 0;

                            foreach(NotMovingGameObject.DomainStruct structs in wall.Domains)
                            {

                                if (wall.isInteractive() && structs.position.Intersects(player.Bounds))
                                {

                                    if (structs.domainTyp == NotMovingGameObject.DomainTyp.ENTRY)
                                    {
                                        foundEntry = true;

                                        foundEntryTextPosition = new Vector2(player.Coordinates.X, player.Coordinates.Y + 50);

                                        if (currentKeyboardState != lastKeyboardState && currentKeyboardState.IsKeyDown(Keys.Space))
                                        {
                                            player.CollisionDirection = CollisionDetection.CollisionDirection.NONE;

                                            foundEntry = false;

                                            if(structs.link is House)
                                            {
                                                House house = (House)structs.link;
                                                ((House)structs.link).EntryDomainStruct = new NotMovingGameObject.DomainStruct(false, house.EntryDomainStruct.domainTyp, house.EntryDomainStruct.position, house.EntryDomainStruct.link);

                                                player.Coordinates = new Vector2(house.EntryDomainStruct.position.X, house.EntryDomainStruct.position.Y);

                                                prevLevel();
                                            }
                                            else if (structs.link is Wall)
                                            {                                                   
                                                foreach (NotMovingGameObject.DomainStruct structsWall in ((Wall)structs.link).Domains)
                                                {
                                                    if (wall == structsWall.link)
                                                        player.Coordinates = new Vector2(structsWall.position.X, structsWall.position.Y);
                                                }                                                                                                               
                                            }
                                        }

                                        i++;
                                    }
                                }                                        
                            }
                        }
                    }
                }
                else
                {
                    player.CollisionDirection = CollisionDetection.CollisionDirection.NONE;
                }

                player.Update(gameTime);

                lastKeyboardState = currentKeyboardState;
            }
        }

        private void nextLevel()
        {
            ScreenManager.RemoveScreen(this);
            Manager.Instance().nextGameScreen();
            Manager.Instance().loadActuellScreen();
        }

        private void prevLevel()
        {
            ScreenManager.RemoveScreen(this);
            Manager.Instance().preGameScreen();
            Manager.Instance().loadActuellScreen();
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

        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // This game has a blue background. Why? Because!
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.Black, 0, 0);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, player.Camera2D.TransformMatrix);


            spriteBatch.Draw(touchableBackground.ActuellTexture, touchableBackground.Coordinates, Color.White);

            foreach (NotMovingGameObject element in notMovingGameObjects)
                spriteBatch.Draw(element.ActuellTexture, element.Coordinates, Color.White);

            foreach (MovingGameObject element in movingGameObjects)
                spriteBatch.Draw(element.ActuellTexture, element.Coordinates, Color.White);

            if (foundEntry)
                spriteBatch.DrawString(spriteFontArial, "Leertaste", foundEntryTextPosition, Color.Beige);

            spriteBatch.Draw(player.ActuellTexture, player.Coordinates, Color.White);

            spriteBatch.End();
        }
    }
}

#endregion