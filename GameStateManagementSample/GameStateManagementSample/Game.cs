#region File Description

//-----------------------------------------------------------------------------
// Game.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

#endregion File Description

#region Using Statements

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using GameStateManagement.GameObjects;
using System;
using Microsoft.Xna.Framework.Input.Touch;
using ShootingSample;

#endregion Using Statements

namespace GameStateManagement
{
    /// <summary>
    /// Sample showing how to manage different game states, with transitions
    /// between menu screens, a loading screen, the game itself, and a pause
    /// menu. This main game class is extremely simple: all the interesting
    /// stuff happens in the ScreenManager component.
    /// </summary>
    public class GameStateManagementGame : Microsoft.Xna.Framework.Game
    {
        #region Fields
        private GraphicsDeviceManager graphics;
        private ScreenManager screenManager;
        SpriteBatch spriteBatch;
        Song song;
        List<SoundEffect> soundEffects;
        SoundEffect soundEngine;
        SoundEffectInstance soundEngineInstance;
        SoundEffectInstance soundsword1;
        SoundEffectInstance soundsword2;
        SoundEffectInstance soundsword3;
        SoundEffectInstance soundsword4;
        SoundEffectInstance bow;
        SoundEffectInstance hit;
        public List<Bullet> bullets = new List<Bullet>();
        Texture2D bulletTexture;
        Texture2D gunTexture;
        Vector2 gunPosition;
        // By preloading any assets used by UI rendering, we avoid framerate glitches
        // when they suddenly need to be loaded in the middle of a menu transition.
        private static readonly string[] preloadAssets =
        {
            "gradient",
        };

        #endregion Fields

        #region Initialization

        /// <summary>
        /// The main game constructor.
        /// </summary>
        public GameStateManagementGame()
        {
            Content.RootDirectory = "Content";

            GameObjects.Config.initConfig();

            graphics = new GraphicsDeviceManager(this);

            // Create the screen manager component.
            screenManager = new ScreenManager(this);
            Components.Add(screenManager);

            // Activate the first screens.
            screenManager.AddScreen(new BackgroundScreen(), null);
            screenManager.AddScreen(new MainMenuScreen(), null);
            soundEffects = new List<SoundEffect>();
           
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            //graphics.IsFullScreen = true;
            this.Window.Position = new Point(0, 0);
            //GraphicsDevice.Viewport = new Viewport(GraphicsDevice.Adapter.CurrentDisplayMode.Width, GraphicsDevice.Adapter.CurrentDisplayMode.Height, GraphicsDevice.Adapter.CurrentDisplayMode.Width, GraphicsDevice.Adapter.CurrentDisplayMode.Height);
            GraphicsDevice.Viewport = new Viewport(0, 0, GraphicsDevice.Adapter.CurrentDisplayMode.Width * 2, GraphicsDevice.Adapter.CurrentDisplayMode.Height * 2);

            graphics.PreferredBackBufferWidth = GraphicsDevice.Adapter.CurrentDisplayMode.Width;
            graphics.PreferredBackBufferHeight = GraphicsDevice.Adapter.CurrentDisplayMode.Height;
            graphics.IsFullScreen = true;
            graphics.ApplyChanges();

            base.Initialize();
        }

        /// <summary>
        /// Loads graphics content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            
            this.song = Content.Load<Song>("Soundeffekte/musik");
            MediaPlayer.Play(song);
            gunTexture = Content.Load<Texture2D>("Effekt-test-1");
            bulletTexture = Content.Load<Texture2D>("Effekt-test-1");
            gunPosition = new Vector2((480 / 2) - (gunTexture.Width / 2), 700);

            //  Uncomment the following line will also loop the song
            //  MediaPlayer.IsRepeating = true;
            //MediaPlayer.MediaStateChanged += MediaPlayer_MediaStateChanged;
            //soundEffects.Add(Content.Load<SoundEffect>("Soundeffekte/schritt"));
            soundEngine = Content.Load<SoundEffect>("Soundeffekte/schritt");
            soundEngineInstance = soundEngine.CreateInstance();
            soundEngine = Content.Load<SoundEffect>("Soundeffekte/Schwert1");
            soundsword1 = soundEngine.CreateInstance();
            soundEngine = Content.Load<SoundEffect>("Soundeffekte/Schwert2");
            soundsword2 = soundEngine.CreateInstance();
            soundEngine = Content.Load<SoundEffect>("Soundeffekte/Schwert3");
            soundsword3 = soundEngine.CreateInstance();
            soundEngine = Content.Load<SoundEffect>("Soundeffekte/Schwert4");
            soundsword4 = soundEngine.CreateInstance();
            soundEngine = Content.Load<SoundEffect>("Soundeffekte/bogen");
            bow = soundEngine.CreateInstance();
            soundEngine = Content.Load<SoundEffect>("Soundeffekte/Body_Hit_01");
            hit = soundEngine.CreateInstance();
            /*
            if (screenManager.GetScreens()==Screens.GameScreens.OverWorld)
            {

            }
            */
            for (int i = 0; i < 20; i++)
            {
                bullets.Add(new Bullet());
            }

        }


        public void FireBullet(Vector2 point, Vector2 center)
        {
            for (int i = 0; i < bullets.Count; i++)
            {
                if (!bullets[i].isActive)
                {
                    bullets[i].ActivateBullet(point, center, bulletTexture);
                    return;
                }
            }
        }
        /*
        void MediaPlayer_MediaStateChanged(object sender, System.
                                           EventArgs e)
        {
            // 0.0f is silent, 1.0f is full volume
            MediaPlayer.Volume -= 0.1f;
            
            MediaPlayer.Play(song);
        }
       */



        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            for (int i = 0; i < bullets.Count; ++i)
                if (bullets[i].isActive)
                    bullets[i].Update(gameTime);
            HandleInput();
            base.Update(gameTime);



            if (GamePad.GetState(PlayerIndex.One).Buttons.Back ==
                ButtonState.Pressed || Keyboard.GetState().IsKeyDown(
                Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                if (soundEngineInstance.State == SoundState.Stopped)
                {
                    soundEngineInstance.Volume = 0.75f;
                    soundEngineInstance.IsLooped = false;
                    soundEngineInstance.Play();
                }
                else
                    soundEngineInstance.Resume();
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                if (soundEngineInstance.State == SoundState.Stopped)
                {
                    soundEngineInstance.Volume = 0.75f;
                    soundEngineInstance.IsLooped = false;
                    soundEngineInstance.Play();
                }
                else
                    soundEngineInstance.Resume();
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                if (soundEngineInstance.State == SoundState.Stopped)
                {
                    soundEngineInstance.Volume = 0.75f;
                    soundEngineInstance.IsLooped = false;
                    soundEngineInstance.Play();
                }
                else
                    soundEngineInstance.Resume();
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                if (soundEngineInstance.State == SoundState.Stopped)
                {
                    soundEngineInstance.Volume = 0.75f;
                    soundEngineInstance.IsLooped = false;
                    soundEngineInstance.Play();
                }
                else
                    soundEngineInstance.Resume();
            }

            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                Random Rnd = new System.Random();
                int RndNr = Rnd.Next(1, 60);
                if (RndNr == 1)
                {
                    if (soundsword1.State == SoundState.Stopped)
                    {
                        soundsword1.Volume = 0.3f;
                        soundsword1.IsLooped = false;
                        soundsword1.Play();
                    }
                    else
                        soundsword1.Resume();
                }
                if (RndNr == 2)
                {
                    if (soundsword2.State == SoundState.Stopped)
                    {
                        soundsword2.Volume = 0.3f;
                        soundsword2.IsLooped = false;
                        soundsword2.Play();
                    }
                    else
                        soundsword2.Resume();
                }
                if (RndNr == 3)
                {
                    if (soundsword3.State == SoundState.Stopped)
                    {
                        soundsword3.Volume = 0.3f;
                        soundsword3.IsLooped = false;
                        soundsword3.Play();
                    }
                    else
                        soundsword3.Resume();
                }
                if (RndNr == 4)
                {
                    if (soundsword4.State == SoundState.Stopped)
                    {
                        soundsword4.Volume = 0.3f;
                        soundsword4.IsLooped = false;
                        soundsword4.Play();
                    }
                    else
                        soundsword4.Resume();
                }
            }


            if (Keyboard.GetState().IsKeyDown(Keys.E))
            {
                FireBullet(new Vector2(150, 200), new Vector2(1000, 200));

                if (bow.State == SoundState.Stopped)
                {
                    bow.Volume = 0.8f;
                    bow.IsLooped = false;
                    bow.Play();
                }
                else
                    bow.Resume();
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                if (SoundEffect.MasterVolume == 0.0f)
                    SoundEffect.MasterVolume = 1.0f;
                else
                    SoundEffect.MasterVolume = 0.0f;
            }
            base.Update(gameTime);
        }

        #endregion Initialization

        #region Draw
        
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            foreach (Bullet bullet in bullets)
                if (bullet.isActive)
                    bullet.Draw(gameTime, spriteBatch);
            spriteBatch.Draw(gunTexture, gunPosition, Color.White);
            spriteBatch.End();
            base.Draw(gameTime);
            // The real drawing happens inside the screen manager component.
            base.Draw(gameTime);
        }
        public void HandleInput()
        {
            int touches = 0;
            foreach (TouchLocation location in TouchPanel.GetState())
            {
                if (touches == 0)
                {
                    touches++;
                    if (location.Position.Y <= 600)
                    {
                        if (touches == 1)
                        {
                            Vector2 point = new Vector2(location.Position.X, location.Position.Y);
                            FireBullet(point, new Vector2(gunPosition.X + gunTexture.Width / 2, gunPosition.Y + gunTexture.Height / 2));
                        }
                    }
                }
            }
        }
        #endregion Draw
    }

    #region Entry Point

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    internal static class Program
    {
        private static void Main()
        {
            using (GameStateManagementGame game = new GameStateManagementGame())
            {
                game.Run();
            }
        }
    }

    #endregion Entry Point
}