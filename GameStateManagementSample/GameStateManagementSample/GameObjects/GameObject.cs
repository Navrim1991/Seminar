using System;
using System.Windows;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using GameStateManagement.CollisionDetection;

namespace GameStateManagement.GameObjects
{
    #region Global GameObjects Enum

    enum GameObjectID
    {
        None = 0x00000000,
        //Not touchable GameObject Pattern 0x000000FF
        Tree = 0x00000001,
        Water = 0x00000002,
        Stone = 0x00000004,
        Wall = 0x0000008,
        House = 0x00000010,
        //Touchable GameObject Pattern 0x0000FF00
        Gras = 0x00000100,
        Road = 0x00000200,
        Door = 0x00000400,
        //Player GameObject  0x00010000
        Player = 0x00010000,
        //Skill GameObject 0x00100000
        Skill = 0x00100000,
        //Weapon GameObject 0x00200000
        Weapon = 0x00200000,
        //NPC GameObject  0x01000000
        Npc = 0x01000000,
        Effekt = 0x02000000,
    }

    enum GameObjectOption
    {
        None = 0x00000000,
        Useable = 0x00000001,
        Interactive = 0x00000002,
        Attackable = 0x00000004
    }

    #endregion

    #region Class GameObject

    class GameObject : DrawableGameComponent, IQuadObject
    {
        #region private static readonly check variables

        private static readonly int NOT_TOUCHABLE_GAMEOBJECT = 0x000000FF;
        private static readonly int TOUCHABLE_GAMEOBJECT = 0x0000FF00;
        private static readonly int PLAYER = 0x00010000;
        private static readonly int NPC = 0x01000000;
        private static readonly int SKILL = 0x00100000;
        private static readonly int WEAPON = 0x00200000;
        private static readonly int MENU = 0x30000000;
        private static readonly int HOUSE = 0x00000010;

        private static readonly int USEABLE = 0x00000001;
        private static readonly int INTERACTIVE = 0x00000002;
        private static readonly int ATTACKABLE = 0x00000004;


        #endregion

        #region protected variables & public attributes

        protected List<Texture2D> textures;
        protected Texture2D actuellTexture;
        protected ContentManager contentManager;
        protected List<String> spriteNames;
        protected GameObjectID gameObjectID;
        protected GameObjectOption gameObjectOption;
        protected Rectangle bounds;
        protected Vector2 coordinates;


        public event EventHandler BoundsChanged;

        public List<Texture2D> Textures
        {
            get
            {
                return textures;
            }

            set
            {
                textures = value;
            }
        }

        public Texture2D ActuellTexture
        {
            get
            {
                return actuellTexture;
            }

            set
            {
                actuellTexture = value;
            }
        }

        public GameObjectID GameObjectID
        {
            get
            {
                return gameObjectID;
            }

            set
            {
                gameObjectID = value;
            }
        }

        public GameObjectOption GameObjectOption
        {
            get
            {
                return gameObjectOption;
            }

            set
            {
                gameObjectOption = value;
            }
        }

        public Rectangle Bounds
        {
            get
            {
                return bounds;
            }

            set
            {
                bounds = value;
            }
        }

        public Vector2 Coordinates
        {
            get
            {
                return coordinates;
            }

            set
            {
                coordinates = value;
            }
        }

        #endregion

        #region Check Methods

        public bool isNotTouchable()
        {
            return Convert.ToBoolean(((int)gameObjectID & NOT_TOUCHABLE_GAMEOBJECT));
        }

        public bool isTouchable()
        {
            return Convert.ToBoolean(((int)gameObjectID & TOUCHABLE_GAMEOBJECT));
        }

        public bool isPlayer()
        {
            return Convert.ToBoolean(((int)gameObjectID & PLAYER));
        }

        public bool isNpc()
        {
            return Convert.ToBoolean(((int)gameObjectID & NPC));
        }

        public bool isSkill()
        {
            return Convert.ToBoolean(((int)gameObjectID & SKILL));
        }

        public bool isWeapon()
        {
            return Convert.ToBoolean(((int)gameObjectID & WEAPON));
        }

        public bool isMenu()
        {
            return Convert.ToBoolean(((int)gameObjectID & MENU));
        }

        public bool isUseable()
        {
            return Convert.ToBoolean(((int)gameObjectOption & USEABLE));
        }

        public bool isInteractive()
        {
            return Convert.ToBoolean(((int)gameObjectOption & INTERACTIVE));
        }

        public bool isAttackable()
        {
            return Convert.ToBoolean(((int)gameObjectOption & ATTACKABLE));
        }

        public bool isHouse()
        {
            return Convert.ToBoolean(((int)gameObjectOption & HOUSE));
        }

        #endregion

        #region Constructor

        public GameObject(ref ContentManager contentManager, List<string> spriteNames, GameObjectID gameObjectID,
            GameObjectOption gameObjectOption, Vector2 coordinates, Game game)
            : base (game)
        {
            textures = new List<Texture2D>();
            foreach (String element in spriteNames)
                textures.Add(contentManager.Load<Texture2D>(element));

            if (textures.Count > 0)
                actuellTexture = textures.First();
            else
                throw new ArgumentNullException("Keine Texturen vorhanden");

            if (coordinates == null)
                throw new ArgumentNullException("Keine Koordinaten zum Zeichen angegeben");

            this.contentManager = contentManager;
            this.spriteNames = spriteNames;
            this.gameObjectID = gameObjectID;
            this.gameObjectOption = gameObjectOption;
            this.coordinates = coordinates;

            bounds = new Rectangle((int)Math.Round(coordinates.X), (int)Math.Round(coordinates.Y), actuellTexture.Bounds.Width, actuellTexture.Bounds.Height);
        }

        #endregion

        public override void Update(GameTime gameTime)
        {
            if(this is MovingGameObject)
            {
                EventHandler handler = BoundsChanged;

                if (handler != null)
                    handler(this, new EventArgs());
            }

            base.Update(gameTime);
        }
    }
 

    #endregion

}
