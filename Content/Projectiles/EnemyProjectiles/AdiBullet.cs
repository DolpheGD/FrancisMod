using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;


namespace FrancisMod.Content.Projectiles.EnemyProjectiles
{
	public class AdiBullet : ModProjectile
	{

        private bool firstSpawn = true;
		
		public override void SetDefaults() {
			Projectile.width = 104; //The width of projectile hitbox
			Projectile.height = 38; //The height of projectile hitbox;
            Projectile.aiStyle = 0;

			Projectile.light = 0.6f;

			Projectile.hostile = true; //Can the projectile deal damage to the player?
			Projectile.DamageType = DamageClass.Ranged; //Is the projectile shoot by a ranged weapon?
			Projectile.ignoreWater = true; //Does the projectile's speed be influenced by water?
			Projectile.tileCollide = false; //Can the projectile collide with tiles?

            Projectile.maxPenetrate = 0;
			Projectile.timeLeft = 180; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)

		}

		public override void AI() {
            if (firstSpawn)
            {
                Projectile.rotation = Projectile.velocity.ToRotation();
                firstSpawn = false;
            }

                Lighting.AddLight(Projectile.Center, 0.9f, 0.5f, 0.3f);
				Dust dust1 = Dust.NewDustPerfect(Projectile.Center, DustID.Torch, Vector2.Zero, 0, default, 4f);
				dust1.noGravity = true;
				dust1.velocity *= 0f;

            if (Projectile.velocity.Length() < 20)
            {
                Projectile.velocity *= 1.05f;
            }
        }


	}
}