using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
namespace GameStateManagement.Screens.GameScreens
{
    class Manager
    {
        private static Manager singleton;
        private List<GameScreen> gameScreens;
        private GameScreen actuellScreen;
        private int currentScreenIndex;
        private ScreenManager screenManager;

        public GameScreen ActuellScreen
        {
            get
            {
                return actuellScreen;
            }
        }

        public Rooms Rooms
        {
            get
            {
                if (gameScreens[1] is Rooms)
                    return (Rooms)gameScreens[1];
                else
                    return null;
            }
        }

        public UnderWorld Underworld
        {
            get
            {
                if (gameScreens[2] is UnderWorld)
                    return (UnderWorld)gameScreens[2];
                else
                    return null;
            }
        }

        public OverWorld OverWorld
        {
            get
            {
                if (gameScreens[0] is OverWorld)
                    return (OverWorld)gameScreens[0];
                else
                    return null;
            }
        }

        private Manager(ScreenManager screenManager)
        {
            this.screenManager = screenManager;
            currentScreenIndex = 0;
            gameScreens = new List<GameScreen>();
            
            gameScreens.Add(new OverWorld());
            gameScreens.Add(new Rooms());
            gameScreens.Add(new UnderWorld());

            actuellScreen = gameScreens.First();
        }

        public static Manager Instance(ScreenManager screenManager)
        {
            if (singleton == null)
                singleton = new Manager(screenManager);

            return singleton;
        }

        public static Manager Instance()
        {
            if (singleton == null)
                throw new ArgumentNullException("Singleton Instance is null! First you have to call Instance(ScreenManager)");

            return singleton;
        }

        public void preGameScreen()
        {
            if (currentScreenIndex == 0)
                return;

            actuellScreen = gameScreens[--currentScreenIndex];
        }

        public void nextGameScreen()
        {
            if (currentScreenIndex == gameScreens.Count - 1)
                return;

            actuellScreen = gameScreens[++currentScreenIndex];
        }

        public void loadActuellScreen()
        {
            LoadingScreen.Load(screenManager, true, PlayerIndex.One, actuellScreen);
        }
    }
}
