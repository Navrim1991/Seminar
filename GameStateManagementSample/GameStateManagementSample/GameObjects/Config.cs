using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameStateManagement.AI;

namespace GameStateManagement.GameObjects
{
    static class Config
    {
        public const string CONTENT_BACKGROUND = "Content/game/graphics/background/";
        public const string CONTENT_WATER = "Content/game/graphics/background/water";
        public const string CONTENT_HOUSES = "Content/game/graphics/notMovingObjects/houses/";
        public const string CONTENT_TREES = "Content/game/graphics/notMovingObjects/trees/";
        public const string CONTENT_PLAYER = "Content/game/graphics/MovingGameObjects/Player/";
        public const string CONTENT_NPC = "Content/game/graphics/MovingGameObjects/Npc/";
        public const string CONTENT_SPRITEFONTS = "Content/game/spriteFonts/";
        public const string CONTENT_EFFEKT = "Content/game/graphics/notMovingObjects/Effekt/";
        public const string CONTENT_ROOMS = "Content/game/graphics/notMovingObjects/rooms_walls/";
        public const string CONTENT_UNDERWORLD = "Content/game/graphics/notMovingObjects/underwold_walls/";
        public const string CSV_OVERWORLD = "Content/Oberwelt_nicht_begehbar";
        public const string CSV_ROOMS = "Content/Raeume_nicht_begehbar";
        public const string CSV_UNDERWORLD = "Content/Unterwelt_nicht_begehbar";

        private static Dictionary<AI.AiTypes, float> configNpcEvade;
        private static Dictionary<AI.AiTypes, float> configNpcChase;

        public static Dictionary<AiTypes, float> ConfigNpcEvade
        {
            get
            {
                if (configNpcEvade == null)
                    initEvadeData();

                return configNpcEvade;
            }
        }

        public static Dictionary<AiTypes, float> ConfigNpcChase
        {
            get
            {
                if (configNpcChase == null)
                    initChaseData();

                return configNpcChase;
            }
        }

        public static void initConfig()
        {
            initChaseData();
            initEvadeData();
        }

        public static void initEvadeData()
        {
            configNpcEvade = new Dictionary<AI.AiTypes, float>();
            configNpcEvade.Add(AI.AiTypes.CAUGHT_DISTANCE, 100);
            configNpcEvade.Add(AI.AiTypes.DISTANCE, 200);
            configNpcEvade.Add(AI.AiTypes.HYSTERESIS, 100);
            configNpcEvade.Add(AI.AiTypes.SPEED, 60);
            configNpcEvade.Add(AI.AiTypes.TURN_SPEED, 0.1f);
        }

        public static void initChaseData()
        {
            configNpcChase = new Dictionary<AI.AiTypes, float>();
            configNpcChase.Add(AI.AiTypes.CAUGHT_DISTANCE, 0);
            configNpcChase.Add(AI.AiTypes.DISTANCE, 800);
            configNpcChase.Add(AI.AiTypes.HYSTERESIS, 100);
            configNpcChase.Add(AI.AiTypes.SPEED, 320);
            configNpcChase.Add(AI.AiTypes.TURN_SPEED, 0.25f);
        }
    }
}
