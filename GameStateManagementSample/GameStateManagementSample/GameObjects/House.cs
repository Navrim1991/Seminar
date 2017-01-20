using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameStateManagement.GameObjects
{
    class House : NotMovingGameObject
    {
        private Rectangle entryRec;
        private DomainStruct entryDomainStruct;
        public House(ref ContentManager contentManager, List<string> spriteNames,
            GameObjectID gameObjectID, GameObjectOption gameObjectOption,
            Vector2 startingCoordinates, Game game)
            : base(ref contentManager, spriteNames, gameObjectID, gameObjectOption, startingCoordinates, game)
        {
            if (spriteNames.First() == "schloss3_v2")
            {
                entryRec = new Rectangle((int)(coordinates.X + 7 * bounds.Width / 18), (int)(coordinates.Y + bounds.Height), 4 * bounds.Width / 19, 50);
                this.gameObjectOption = GameObjectOption.Interactive;
                entryDomainStruct = new DomainStruct(false, DomainTyp.ENTRY, entryRec, null);
            }
                
        }



        public Rectangle EntryRec
        {
            get
            {
                return entryRec;
            }
        }

        public DomainStruct EntryDomainStruct
        {
            get
            {
                return entryDomainStruct;
            }

            set
            {
                entryDomainStruct = value;
            }
        }

        public bool setEnry()
        {
            string spriteName = spriteNames.First();

            if (spriteName == "house2_neu")
            {
                this.gameObjectOption = GameObjectOption.Interactive; 

                entryRec = new Rectangle((int)(coordinates.X + 5f * bounds.Width / 14f), (int)(coordinates.Y + bounds.Height), 2 * bounds.Width / 7, 50);

                entryDomainStruct = new DomainStruct(false, DomainTyp.ENTRY, entryRec, null);

                return true;
            }
            else if (spriteName == "house3_neu")
            {
                this.gameObjectOption = GameObjectOption.Interactive;

                entryRec = new Rectangle((int)(coordinates.X + 5f * bounds.Width / 14f), (int)(coordinates.Y + bounds.Height * 1f/7f), 3 * bounds.Width / 14, 50);

                entryDomainStruct = new DomainStruct(false, DomainTyp.ENTRY, entryRec, null);

                return true;
            }
            else if (spriteName == "house5_neu")
            {
                this.gameObjectOption = GameObjectOption.Interactive;

                entryRec = new Rectangle((int)(coordinates.X + 2f * bounds.Width / 10f), (int)(coordinates.Y + bounds.Height), 2 * bounds.Width / 5, 50);

                entryDomainStruct = new DomainStruct(false, DomainTyp.ENTRY, entryRec, null);

                return true;
            }
            else if (spriteName == "house6_neu")
            {
                this.gameObjectOption = GameObjectOption.Interactive;

                entryRec = new Rectangle((int)(coordinates.X + 5f * bounds.Width / 12f), (int)(coordinates.Y + bounds.Height), 2 * bounds.Width / 6, 50);

                entryDomainStruct = new DomainStruct(false, DomainTyp.ENTRY, entryRec, null);

                return true;
            }
            else if (spriteName == "house7_neu")
            {
                this.gameObjectOption = GameObjectOption.Interactive;

                entryRec = new Rectangle((int)(coordinates.X + 5f * bounds.Width / 22f), (int)(coordinates.Y + bounds.Height), 6 * bounds.Width / 11, 50);

                entryDomainStruct = new DomainStruct(false, DomainTyp.ENTRY, entryRec, null);

                return true;
            }
            else if (spriteName == "house8_neu")
            {
                this.gameObjectOption = GameObjectOption.Interactive;

                entryRec = new Rectangle((int)(coordinates.X + 1f * bounds.Width / 10f), (int)(coordinates.Y + bounds.Height), 6 * bounds.Width / 10, 50);

                entryDomainStruct = new DomainStruct(false, DomainTyp.ENTRY, entryRec, null);

                return true;
            }
            else if (spriteName == "house9_neu")
            {
                this.gameObjectOption = GameObjectOption.Interactive;

                entryRec = new Rectangle((int)(coordinates.X + 5f * bounds.Width / 22f), (int)(coordinates.Y + bounds.Height), 5 * bounds.Width / 11, 50);

                entryDomainStruct = new DomainStruct(false, DomainTyp.ENTRY, entryRec, null);

                return true;
            }
            else if (spriteName == "minehouse1_neu")
            {
                this.gameObjectOption = GameObjectOption.Interactive;

                entryRec = new Rectangle((int)(coordinates.X + 9f * bounds.Width / 14f), (int)(coordinates.Y + bounds.Height), 2 * bounds.Width / 7, 50);

                entryDomainStruct = new DomainStruct(false, DomainTyp.ENTRY, entryRec, null);

                return true;
            }
            else if (spriteName == "minehouse2_neu")
            {
                this.gameObjectOption = GameObjectOption.Interactive;

                entryRec = new Rectangle((int)(coordinates.X + 3f * bounds.Width / 14f), (int)(coordinates.Y + bounds.Height), 5 * bounds.Width / 7, 50);

                entryDomainStruct = new DomainStruct(false, DomainTyp.ENTRY, entryRec, null);

                return true;
            }

            return false;
        }

    }
}
