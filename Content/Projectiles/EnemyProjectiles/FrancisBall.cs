using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;


namespace FrancisMod.Content.Projectiles.EnemyProjectiles
{
	public class FrancisBall : ModProjectile
	{

        private bool firstSpawn = true;
		
		public override void SetDefaults() {
			Projectile.width = 30; //The width of projectile hitbox
			Projectile.height = 30; //The height of projectile hitbox;
            Projectile.aiStyle = 0;

			Projectile.hostile = true; //Can the projectile deal damage to the player?
			Projectile.DamageType = DamageClass.Ranged; //Is the projectile shoot by a ranged weapon?
			Projectile.ignoreWater = true; //Does the projectile's speed be influenced by water?
			Projectile.tileCollide = false; //Can the projectile collide with tiles?

            Projectile.maxPenetrate = 0;
			Projectile.timeLeft = 300; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)

		}

		public override void AI() {
            if (firstSpawn)
            {
				SoundEngine.PlaySound(new SoundStyle($"{nameof(FrancisMod)}/Assets/Sounds/FrancisLaugh2"){
                PitchVariance = 0.3f,
                MaxInstances = 3,
                Volume = 1.1f
			    }, Projectile.Center);
                firstSpawn = false;
            }
            
			if (Projectile.ai[0] % 2 == 0)
			{
                Lighting.AddLight(Projectile.Center, 0.9f, 0.1f, 0.3f);
				Dust dust2 = Dust.NewDustDirect(Projectile.Center, 40, 40, DustID.Smoke, 0f, 0f, 100, default, 3f);
				Dust dust1 = Dust.NewDustDirect(Projectile.Center, 40, 40, DustID.Torch, 0f, 0f, 100, default, 2f);
				dust1.noGravity = true;
				dust1.velocity *= 0.1f;
				dust2.noGravity = true;
				dust2.velocity *= 0.1f;
			}

			if (Projectile.ai[0] >= 60)
			{
                Player player = findClosestPlayer();
                float inertia = 50f;

                Vector2 direction = (player.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
                
                Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction * 23f) / inertia;
                Projectile.velocity *= 1.002f;
            }
            else{
                Projectile.velocity *= 0.99f;
            }


            Projectile.ai[0]++;
        }

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

			if (Main.netMode != NetmodeID.MultiplayerClient && Projectile.ai[1] == 69)
			{
				if (Main.rand.NextBool(4))
				{
					SoundEngine.PlaySound(SoundID.Item71, Projectile.Center);
					Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(0, -2), ModContent.ProjectileType<SkinnyPopBomb>(), Projectile.damage / 4, 0f, Main.myPlayer);
				}
			}
		}

	}
}