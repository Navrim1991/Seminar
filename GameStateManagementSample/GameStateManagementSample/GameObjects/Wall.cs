using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameStateManagement.GameObjects
{
    class Wall : NotMovingGameObject
    {
        List<DomainStruct> domains;
        public Wall(ref ContentManager contentManager, List<string> spriteNames,
            GameObjectID gameObjectID, GameObjectOption gameObjectOption,
            Vector2 startingCoordinates, Game game)
            : base(ref contentManager, spriteNames, gameObjectID, gameObjectOption, startingCoordinates, game)
        {
            domains = new List<DomainStruct>();

            string spriteName = actuellTexture.Name;

            //List<House> houses = Screens.GameScreens.Manager.Instance().OverWorld.getAllEntryHouses(this); 

            if(spriteName == "hotel")
            {
                this.gameObjectOption = GameObjectOption.Interactive;
                setDomain((int)(this.bounds.X + (4f / 9f) * actuellTexture.Width), this.bounds.Y + actuellTexture.Height - 70);
            }
            else if(spriteName == "keller_eingang")
            {
                this.gameObjectOption = GameObjectOption.Interactive;
                setDomain((int)(this.bounds.X + (2f / 9f) * actuellTexture.Width), this.bounds.Y + actuellTexture.Height - 80);
                setDomain((int)(this.bounds.X + (5f / 9f) * actuellTexture.Width), (int)(this.bounds.Y + (7f/12f) * actuellTexture.Height));
            }
            else if(spriteName == "keller")
            {
                this.gameObjectOption = GameObjectOption.Interactive;
                setDomain((int)(this.bounds.X + (4.3f / 8f) * actuellTexture.Width), (int)(this.bounds.Y + (1f / 3f) * actuellTexture.Height));
            }
            else if(spriteName == "rezeption")
            {
                this.gameObjectOption = GameObjectOption.Interactive;
                setDomain((int)(this.bounds.X + (8f / 13f) * actuellTexture.Width), this.bounds.Y + actuellTexture.Height - 70);
            }
            else if(spriteName == "schatzkammer")
            {
                this.gameObjectOption = GameObjectOption.Interactive;
                setDomain((int)(this.bounds.X + (6f / 10f) * actuellTexture.Width), this.bounds.Y + actuellTexture.Height - 50);
            }
            else if (spriteName == "keller_treppe")
            {
                this.gameObjectOption = GameObjectOption.Interactive;
                setDomain((int)(this.bounds.X + 15), (int)(this.bounds.Y + (1f/5f)*actuellTexture.Height));
                setDomain((int)(this.bounds.X + (4f / 6f) * actuellTexture.Width), (int)(this.bounds.Y + (7f / 10f) * actuellTexture.Height));
            }
            else if(spriteName == "schloss_eingang")
            {
                this.gameObjectOption = GameObjectOption.Interactive;
                setDomain((int)(this.bounds.X + (13f / 30f) * actuellTexture.Width), this.bounds.Y + actuellTexture.Height - 90);
                setDomain((int)(this.bounds.X + (13f / 30f) * actuellTexture.Width), (int)(this.bounds.Y + (11f / 39f) * actuellTexture.Height));
            }
            else if (spriteName == "schloss_raum_links")
            {

            }
            else if (spriteName == "schloss_raum_links")
            {

            }
            else if (spriteName == "schloss_raum_links2")
            {

            }
            else if (spriteName == "schloss_raum_rechts")
            {

            }
            else if (spriteName == "schloss_raum_rechts2")
            {

            }
            else if (spriteName == "trohn")
            {
                this.gameObjectOption = GameObjectOption.Interactive;
                setDomain(this.bounds.X, this.bounds.Y + actuellTexture.Height);
            }
            else if (spriteName == "trohn_raum")
            {
                this.gameObjectOption = GameObjectOption.Interactive;
                setDomain((int)(this.bounds.X + (11f / 24f) * actuellTexture.Width), this.bounds.Y + actuellTexture.Height - 90);
            }
        }

        public List<DomainStruct> Domains
        {
            get
            {
                return domains;
            }

            set
            {
                domains = value;
            }
        }

        private void setDomain(int x, int y)
        {
            Rectangle rec = new Rectangle(x, y, 50, 50);
            domains.Add(new DomainStruct(false, DomainTyp.ENTRY, rec, null));
        }
    }
}
