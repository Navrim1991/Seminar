using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections;

namespace GameStateManagement.GameObjects
{
    class NotMovingGameObject : GameObject
    {
        public enum DomainTyp
        {
            ENTRY,
            EXIT,
            NONE
        }

        public struct DomainStruct
        {
            public Rectangle position;
            public DomainTyp domainTyp;
            public bool hasContract;
            public NotMovingGameObject link;

            public DomainStruct(bool hasContract, DomainTyp domainTyp, Rectangle position, NotMovingGameObject link)
            {
                this.position = position;
                this.domainTyp = domainTyp;
                this.hasContract = hasContract;
                this.link = link;
            }

        }

        #region Constructor
        public NotMovingGameObject(ref ContentManager contentManager, List<string> spriteNames,
            GameObjectID gameObjectID, GameObjectOption gameObjectOption,
            Vector2 startingCoordinates, Game game)
            : base(ref contentManager, spriteNames, gameObjectID, gameObjectOption, startingCoordinates, game)
        {
            
        }

        #endregion

        #region override Drawing Methods

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        #endregion
    }
}
