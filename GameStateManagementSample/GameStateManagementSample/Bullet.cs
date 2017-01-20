using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShootingSample
{
    public class Bullet
    {
        private Texture2D bulletTexture;
        private Vector2 bulletTarget;
        public Vector2 bulletPosition;
        private Vector2 bulletVelocity;
        public bool isActive;
        private float moveSpeed;
        public Rectangle bulletRectangle;

        public Bullet()
        {
            isActive = false;
        }
        public void ActivateBullet(Vector2 point, Vector2 center, Texture2D texture)
        {
            bulletTarget = point;
            bulletPosition = center;
            bulletTexture = texture;
            moveSpeed = 200;
            isActive = true;
            SetVelocity();
        }
        private void SetVelocity()
        {
            bulletVelocity = -(bulletPosition - bulletTarget);
            bulletVelocity.Normalize();
        }
        public void Update(GameTime gameTime)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (bulletPosition.Y < -30)
                isActive = false;
            if (bulletPosition.X < -30 || bulletPosition.X > 600)
                isActive = false;
            bulletPosition += (bulletVelocity * moveSpeed * elapsedTime);
            bulletRectangle = new Rectangle((int)bulletPosition.X, (int)bulletPosition.Y, bulletTexture.Width, bulletTexture.Height);
        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(bulletTexture, bulletPosition, null, Color.White, 0.0f, new Vector2(bulletTexture.Width / 2, bulletTexture.Height / 2), 1.0f, SpriteEffects.None, 0f);
        }

        public void Kill()
        {
            isActive = false;
        }

    }
}