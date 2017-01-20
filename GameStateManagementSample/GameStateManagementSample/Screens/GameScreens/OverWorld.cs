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
using CsvHelper;
using System.IO;

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
        private ContentManager contentManagerWater;

        private bool foundEntry;
        private Vector2 foundEntryTextPosition;

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
        private static bool contentLoaded;

        public OverWorld()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            movingGameObjects = new List<MovingGameObject>();
            notMovingGameObjects = new List<NotMovingGameObject>();
        }

        public List<House> getAllEntryHouses(Rooms rooms)
        {
            List<House> houses = new List<House>();

            Rectangle rec = new Rectangle(0, 0, 0, 0);

            foreach(NotMovingGameObject element in notMovingGameObjects)
            {
                if (element is House)
                {
                    House house = (House)element;

                    if(house.EntryRec != rec)
                        houses.Add(((House)element));

                }
            }

            return houses;
        }

        public override void LoadContent()
        {
            foundEntry = false;
            foundEntryTextPosition = Vector2.Zero;

            if (contentLoaded)
            {

                Utilities.Util.ActuellBackground = touchableBackground;
                return;
            }
                

            this.spriteBatch = ScreenManager.SpriteBatch;
            game = ScreenManager.Game;

            contentManagerBackground = new ContentManager(game.Services, Config.CONTENT_BACKGROUND);
            contentManagerHouses = new ContentManager(game.Services, Config.CONTENT_HOUSES);
            contentManagerPlayer = new ContentManager(game.Services, Config.CONTENT_PLAYER);
            contentManagerNpc = new ContentManager(game.Services, Config.CONTENT_NPC);
            contentManagerSpriteFonts = new ContentManager(game.Services, Config.CONTENT_SPRITEFONTS);
            contentManagerEffect = new ContentManager(game.Services, Config.CONTENT_EFFEKT);
            contentManagerWater = new ContentManager(game.Services, Config.CONTENT_WATER);
            contentManagerTrees = new ContentManager(game.Services, Config.CONTENT_TREES);

            quadTree = new CollisionDetection.QuadTree<GameObject>(new Point(25,25), 5, true);
            spriteFontArial = contentManagerSpriteFonts.Load<SpriteFont>("Arial");

            entryPoint = new Vector2(game.GraphicsDevice.Adapter.CurrentDisplayMode.Width / 2,
                  game.GraphicsDevice.Adapter.CurrentDisplayMode.Height / 2);

            player = Player.Instance("Marvin", PlayerIndex.One, ref contentManagerPlayer, 
                new List<string>() { "monster_1" }, GameObjectID.Player, 
                GameObjectOption.Attackable & GameObjectOption.Interactive,
                entryPoint, game);
            xstart = player.Coordinates.X;
            ystart = player.Coordinates.Y;
            //
            vektormenu = new Vector2(player.Coordinates.X - 100, player.Coordinates.Y + 300);
            //
            refGameObjects = new List<GameObject>();

            //quadTree.Insert(player);

            touchableBackground = new NotMovingGameObject(ref contentManagerBackground,
                            new List<string>() { "HINTERGRUND" }, GameObjectID.Gras,
                            GameObjectOption.None, new Vector2(0, 0), ScreenManager.Game);

            Utilities.Util.ActuellBackground = touchableBackground;


            notMovingGameObjects.Add(
                    bogen = new NotMovingGameObject(ref contentManagerEffect, new List<string>() { "bogen-aufheben" },
                    GameObjectID.Effekt, GameObjectOption.Interactive,
                    new Vector2(340, 500), game)
                    );
            notMovingGameObjects.Add(
                    schwert = new NotMovingGameObject(ref contentManagerEffect, new List<string>() { "schwert-aufheben" },
                    GameObjectID.Effekt, GameObjectOption.Interactive,
                    new Vector2(340,560), game)
                    );

            var sr = new StreamReader(Config.CSV_OVERWORLD);

            var reader = new CsvReader(sr);
            reader.Configuration.Delimiter = ";";

            reader.Read();
            string[] headers = reader.FieldHeaders;

            while (reader.Read())
            {
                ContentManager tmpManager = null ;
                string contentName =  "";
                GameObjectID tmpObjectID = GameObjectID.None;
                GameObjectOption tmpGameObjectOption = GameObjectOption.None;
                int tmpX = -1;
                int tmpY = -1;

                for (int i = 0; i < headers.Count(); i++)
                {
                    switch (i)
                    {
                        case 0:
                            contentName = reader.GetField(headers[i]);

                            if (contentName.Contains("house") || contentName.Contains("schiff") || contentName.Contains("bruecke"))
                            {
                                tmpManager = contentManagerHouses;
                                tmpObjectID = GameObjectID.House;
                                tmpGameObjectOption = GameObjectOption.None;
                            }  
                            else if(contentName.Contains("wasser"))
                            {
                                tmpManager = contentManagerWater;
                                tmpObjectID = GameObjectID.Water;
                                tmpGameObjectOption = GameObjectOption.None;
                            }
                            else if (contentName.Contains("schloss"))
                            {
                                tmpManager = contentManagerHouses;
                                tmpObjectID = GameObjectID.House;
                                tmpGameObjectOption = GameObjectOption.Interactive;
                            }
                            else if (contentName.Contains("baum"))
                            {
                                tmpManager = contentManagerTrees;
                                tmpObjectID = GameObjectID.Tree;
                                tmpGameObjectOption = GameObjectOption.None;
                            }
                            break;
                        case 1:
                            tmpX = Convert.ToInt32(reader.GetField(headers[i]));
                            break;
                        case 2:
                            tmpY = Convert.ToInt32(reader.GetField(headers[i]));
                            break;
                    }
                }

                if(tmpManager != null && contentName != "" && tmpX != -1 && tmpY != -1)
                {
                    if(contentName.Contains("house") || contentName.Contains("schloss3_v2"))
                        notMovingGameObjects.Add(new House(ref tmpManager, new List<string>() { contentName }, tmpObjectID, tmpGameObjectOption, new Vector2(tmpX, tmpY), game));
                    else
                        notMovingGameObjects.Add(new NotMovingGameObject(ref tmpManager, new List<string>() { contentName }, tmpObjectID, tmpGameObjectOption, new Vector2(tmpX, tmpY), game));
                }

                tmpManager = null; ;
                contentName = "";
                tmpObjectID = GameObjectID.None;
                tmpGameObjectOption = GameObjectOption.None;
            }

            int roomCounter = 4;
            

            for(int i = 0; roomCounter > 0; i = i % notMovingGameObjects.Count)
            {
                if(notMovingGameObjects[i] is House)
                {
                    House house = (House)notMovingGameObjects[i];
                    
                    if (house.ActuellTexture.Name != "schloss3_v2" && house.GameObjectOption == GameObjectOption.None && house.setEnry())
                        roomCounter--;
                }

                i++;
            }

            foreach (NotMovingGameObject element in notMovingGameObjects)
                quadTree.Insert(element);

            Random random = new Random();
            NPC npc;            


            npc = new NPC(ref contentManagerNpc, new List<string>() { "katze_2" }, GameObjectID.Npc, GameObjectOption.Attackable, new Vector2(199, 500), game,
                Config.ConfigNpcEvade, AI.AiOption.CHASE);

            npc.Orientation = random.Next(-3, 3);
            movingGameObjects.Add(npc);

            npc = new NPC(ref contentManagerNpc, new List<string>() { "hexe_stand_bewegung_rechts" }, GameObjectID.Npc, GameObjectOption.Attackable, new Vector2(300, 800), game,
                Config.ConfigNpcEvade, AI.AiOption.CHASE);

            npc.Orientation = random.Next(-3, 3);
            movingGameObjects.Add(npc);
            npc = new NPC(ref contentManagerNpc, new List<string>() { "knochen_1" }, GameObjectID.Npc, GameObjectOption.Attackable, new Vector2(800, 1000), game,
                Config.ConfigNpcEvade, AI.AiOption.CHASE);

            npc.Orientation = random.Next(-3, 3);
            movingGameObjects.Add(npc);


            foreach (MovingGameObject element in movingGameObjects)
                quadTree.Insert(element);

            contentLoaded = true;

            base.LoadContent();
            
        }

        private void nextLevel()
        {
            ScreenManager.RemoveScreen(this);
            Manager.Instance().nextGameScreen();
            Manager.Instance().loadActuellScreen();
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
        }


        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);

            foundEntry = false;

            if (this.IsActive)
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

                //waffe 340, 500
                if (bogenbool == 0 && player.Coordinates.X <= 368 && player.Coordinates.X >= 316 && player.Coordinates.Y <= 510 && player.Coordinates.Y >= 464)
                {
                    quadTree.Remove(bogen);
                    notMovingGameObjects.Remove(bogen);                    
                    bogenbool = 1;
                }
                //bogen 340,550
                if (schwertbool == 0 && player.Coordinates.X <= 360 && player.Coordinates.X >= 310 && player.Coordinates.Y <= 585 && player.Coordinates.Y >= 525)
                {
                    quadTree.Remove(schwert);
                    notMovingGameObjects.Remove(schwert);                    
                    schwertbool = 1;
                }
                if ((Keyboard.GetState().IsKeyDown(Keys.W) || Keyboard.GetState().IsKeyDown(Keys.E)))
                {
                    if (Keyboard.GetState().IsKeyDown(Keys.W) && Keyboard.GetState().IsKeyUp(Keys.E) && schwertbool == 1)
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

                    if (Keyboard.GetState().IsKeyDown(Keys.E) && bogenbool == 1)
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
                            if(element is House)
                            {
                                House house = (House)element;

                                if(house.isInteractive() && house.EntryRec.Intersects(player.Bounds))
                                {                                 

                                    if(house.EntryDomainStruct.domainTyp == NotMovingGameObject.DomainTyp.ENTRY)
                                    {
                                        foundEntry = true;

                                        foundEntryTextPosition = new Vector2(player.Coordinates.X, player.Coordinates.Y + 50);

                                        if (Keyboard.GetState().IsKeyDown(Keys.Space))
                                        {
                                            player.CollisionDirection = CollisionDetection.CollisionDirection.NONE;

                                            ((House)element).EntryDomainStruct = new NotMovingGameObject.DomainStruct(true, house.EntryDomainStruct.domainTyp, house.EntryDomainStruct.position, house.EntryDomainStruct.link);

                                            foundEntry = false;

                                            nextLevel();
                                        }
                                    }       
                                }
                            }
                            else if (element is NPC)
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
                spriteBatch.Draw(element.ActuellTexture, element.Coordinates, Color.White);                
                

            foreach(MovingGameObject element in movingGameObjects)
                spriteBatch.Draw(element.ActuellTexture, element.Coordinates, Color.White);

            if(foundEntry)
                spriteBatch.DrawString(spriteFontArial, "Leertaste", foundEntryTextPosition, Color.Beige);

            spriteBatch.Draw(player.ActuellTexture, player.Coordinates, Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }

    }
}
