using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles.Boss;
using Microsoft.Xna.Framework.Graphics;

namespace FrancisMod.Content.Projectiles.EnemyProjectiles
{
	public class ObamaBomb : ModProjectile
	{
		public override void SetDefaults() {
			// This method right here is the backbone of what we're doing here; by using this method, we copy all of
			// the Meowmere Projectile's SetDefault stats (such as projectile.friendly and projectile.penetrate) on to our projectile,
			// so we don't have to go into the source and copy the stats ourselves. It saves a lot of time and looks much cleaner;
			// if you're going to copy the stats of a projectile, use CloneDefaults().

			Projectile.CloneDefaults(ModContent.ProjectileType<AresGaussNukeProjectile>());
			Projectile.width = 120; // The width of projectile hitbox
			Projectile.height = 120; // The height of projectile hitbox
		}


    }
}
