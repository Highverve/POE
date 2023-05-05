using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.Helper_Classes;
using Pilgrimage_Of_Embers.Entities.Actions;
using Pilgrimage_Of_Embers.TileEngine;

namespace Pilgrimage_Of_Embers.CombatEngine.Projectiles.Types.Physical
{
    public class BouncingBall : BaseProjectile
    {
        private Texture2D texture;
        private Jumping jumping;

        private float angle;

        public BouncingBall(int ID, float BaseSpeed)
            : base(ID, "Exploding Bottle", 10f, BaseSpeed, 0)
        {
            canHarmSelf = false;

            jumping = new Jumping(0f, 150f);
        }

        public override void Load(ContentManager cm)
        {
            texture = cm.Load<Texture2D>(directory + "Physical/bouncyBall");

            heightDistance = 70f;
            gravity = 0f;

            base.Load(cm);
        }

        protected override void Initialize()
        {
            //speed = ((float)random.NextDouble() - .5f) * 50f;

            jumping.Jump(1000f);
        }

        int prevJumpCount = 0;
        protected override void UpdateBehavior(GameTime gt)
        {
            if (isAssigned == true)
            {
                hitTimer += gt.ElapsedGameTime.Milliseconds;
                if (hitTimer >= 500)
                {
                    isAssigned = false;
                    hitTimer = 0;
                }
            }

            jumping.Update(gt, Vector2.Zero);

            Bounce(1, 750f, baseSpeed / 10);
            Bounce(2, 500f, baseSpeed / 10);
            Bounce(3, 300f, baseSpeed / 10);
            Bounce(4, 200f, baseSpeed / 10);
            Bounce(5, 150f, baseSpeed / 10);
            Bounce(6, 75f, baseSpeed / 10);

            if (jumping.TimesJumped >= 7)
            {
                baseSpeed -= 80f * (float)gt.ElapsedGameTime.TotalSeconds;
            }

            CurrentFloor = jumping.JumpHeightFloor + 1;

            prevJumpCount = jumping.TimesJumped;

            heightDistance = -jumping.Offset;
            angle += (baseSpeed / 50f) * (float)gt.ElapsedGameTime.TotalSeconds;

            projectileLine.locationA = new Vector2(position.X, position.Y + (heightDistance / 2));
            projectileLine.locationB = (new Vector2(position.X, position.Y + (heightDistance / 2)) + (direction * length));
        }
        private void Bounce(int jumpCount, float jumpForce, float speedReduction)
        {
            if (jumping.TimesJumped == jumpCount &&
                prevJumpCount == (jumpCount - 1))
            {
                jumping.Jump(jumpForce);
                baseSpeed -= (speedReduction);
            }
        }
        protected override void Terminate(GameTime gt)
        {
            state = ProjectileState.Deactivated;
        }

        private Color color = Color.Lerp(Color.Transparent, Color.Black, .3f);
        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(texture, new Vector2(position.X, position.Y - (heightDistance / 2)), Color.White, texture.Center(), angle, 1f, Depth);
            sb.Draw(texture, new Vector2(position.X, position.Y + (heightDistance / 2)), Color.White, texture.Center(), -angle, 1f, SpriteEffects.FlipVertically, Depth - .00001f);
            //projectileLine.DrawLine(sb, texture, Color.Purple, 20);
        }

        public override void OnEntityHit(GameTime gt)
        {
        }
        public override void OnShieldHit(GameTime gt)
        {
        }
        bool isAssigned = false; int hitTimer = 0;
        public override void OnColliderHit(GameTime gt)
        {
            if (isAssigned == false)
            {
                rotation = -rotation;
                isAssigned = true;
            }
        }
        public override void OnGroundHit(GameTime gt)
        {
            
        }

        public override BaseProjectile Copy(TileMap map)
        {
            BouncingBall copy = (BouncingBall)base.Copy(map);

            copy.jumping = new Jumping(0f, 180f);

            return copy;
        }
    }
}
