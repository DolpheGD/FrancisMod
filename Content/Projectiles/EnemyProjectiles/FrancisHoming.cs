using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FrancisMod.Content.Projectiles.EnemyProjectiles
{
	public class FrancisHoming : ModProjectile
	{
		Player closestPlayer = null;

        private const int DefaultWidthHeight = 14;
		private const int ExplosionWidthHeight = 150;


		public override void SetDefaults() {
			Projectile.width = DefaultWidthHeight; // The width of projectile hitbox
			Projectile.height = DefaultWidthHeight; // The height of projectile hitbox

			Projectile.aiStyle = 0; // The ai style of the projectile (0 means custom AI). For more please reference the source code of Terraria
			Projectile.DamageType = DamageClass.Ranged; // What type of damage does this projectile affect?

			Projectile.friendly = false; // Can the projectile deal damage to enemies?
			Projectile.hostile = true; // Can the projectile deal damage to the player?

			Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
			Projectile.light = 0.2f; // How much light emit around the projectile

			Projectile.tileCollide = true; // Can the projectile collide with tiles?
            Projectile.maxPenetrate = 0;
			Projectile.timeLeft = 600; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)

            DrawOriginOffsetX = -9;
            DrawOriginOffsetY = -2;

		}

		// Additional hooks/methods here.

		public override void AI() {

            Lighting.AddLight(Projectile.Center, 0.9f, 0.1f, 0.3f);
            Dust dust1 = Dust.NewDustDirect(Projectile.position, 15, 15, DustID.Torch, 0f, 0f, 100, default, 3f);
            dust1.noGravity = true;
            dust1.velocity *= 0.1f;

            Dust dust2 = Dust.NewDustDirect(Projectile.position, 15, 15, DustID.Smoke, 0f, 0f, 100, default, 2f);
            dust2.noGravity = true;
            dust2.velocity *= 0.1f;

            Projectile.ai[0] += 1f;
            
            if (Projectile.ai[0] < 60)
            {
                Projectile.velocity *= 0.96f;
                Projectile.rotation = MathHelper.PiOver2 * 3;
                
            }
            else if (Projectile.ai[0] == 60)
            {
                SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);
                closestPlayer = findClosestPlayer();

                if (closestPlayer == null)
                    return;

                Projectile.velocity = (closestPlayer.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
                Projectile.rotation = Projectile.velocity.ToRotation();
            }
            else if (Projectile.ai[0] > 60)
            {
				if (Projectile.velocity.Length() < 12f)
					Projectile.velocity *= 1.05f;
            }

			if (Projectile.ai[0] > 597)
			{
				Projectile.Resize(ExplosionWidthHeight, ExplosionWidthHeight);
			}

			if (Projectile.ai[0] > 600)
			{
				Projectile.Kill();
			}

		}

		// Finding the closest NPC to attack within maxDetectDistance range
		// If not found then returns null

		public Player findClosestPlayer() {
			Player closestPlayer = null;
			float closestDistance = 99999999999f;

			// Loop through all Players(max always 200)
			for (int i = 0; i < Main.maxPlayers; i++) {
				Player target = Main.player[i];
				if (target.active) {
					// The DistanceSquared function returns a squared distance between 2 points, skipping relatively expensive square root calculations
					float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);
					if (sqrDistanceToTarget < closestDistance)
					{
						closestPlayer = target;
						closestDistance = sqrDistanceToTarget;
					}
				}
			}
			return closestPlayer;
		}


        public override bool OnTileCollide(Vector2 oldVelocity) {
            Projectile.ai[0] = 597;
            return false;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            Projectile.Kill();
        }

        public override void OnKill(int timeLeft)
        {
			SoundEngine.PlaySound(SoundID.Item14, Projectile.position);

			// Smoke Dust spawn
			for (int i = 0; i < 50; i++) {
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 100, default, 2f);
				dust.velocity *= 1.4f;
			}

			for (int i = 0; i < 80; i++) {
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 3f);
				dust.noGravity = true;
				dust.velocity *= 5f;
				dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 2f);
				dust.velocity *= 3f;
			}

			for (int g = 0; g < 2; g++) {
				var goreSpawnPosition = new Vector2(Projectile.position.X + Projectile.width / 2 - 24f, Projectile.position.Y + Projectile.height / 2 - 24f);
				Gore gore = Gore.NewGoreDirect(Projectile.GetSource_FromThis(), goreSpawnPosition, default, Main.rand.Next(61, 64), 1f);
				gore.scale = 1.5f;
				gore.velocity.X += 1.5f;
				gore.velocity.Y += 1.5f;
				gore = Gore.NewGoreDirect(Projectile.GetSource_FromThis(), goreSpawnPosition, default, Main.rand.Next(61, 64), 1f);
				gore.scale = 1.5f;
				gore.velocity.X -= 1.5f;
				gore.velocity.Y += 1.5f;
				gore = Gore.NewGoreDirect(Projectile.GetSource_FromThis(), goreSpawnPosition, default, Main.rand.Next(61, 64), 1f);
				gore.scale = 1.5f;
				gore.velocity.X += 1.5f;
				gore.velocity.Y -= 1.5f;
				gore = Gore.NewGoreDirect(Projectile.GetSource_FromThis(), goreSpawnPosition, default, Main.rand.Next(61, 64), 1f);
				gore.scale = 1.5f;
				gore.velocity.X -= 1.5f;
				gore.velocity.Y -= 1.5f;
			}
        }
	}
}