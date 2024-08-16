using Terraria.Audio;
using FrancisMod.Content.Projectiles.FriendlyProjectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using FrancisMod.Content.Items.Placeables;
using CalamityMod.Items.Materials;

namespace FrancisMod.Content.Items.Weapons
{
	// ExampleStaff is a typical staff. Staffs and other shooting weapons are very similar, this example serves mainly to show what makes staffs unique from other items.
	// Staff sprites, by convention, are angled to point up and to the right. "Item.staff[Type] = true;" is essential for correctly drawing staffs.
	// Staffs use mana and shoot a specific projectile instead of using ammo. Item.DefaultToStaff takes care of that.
	public class StaffOfTheEggBiscuit : ModItem
	{
        int count = 0;
		public override void SetStaticDefaults() {
			Item.staff[Type] = true; // This makes the useStyle animate as a staff instead of as a gun.
		}

		public override void SetDefaults() {
			Item.DefaultToStaff(ProjectileID.NebulaBlaze1, 15, 20, 1);
            Item.SetWeaponValues(560, 10);

			// Customize the UseSound. DefaultToStaff sets UseSound to SoundID.Item43, but we want SoundID.Item20
			Item.UseSound = SoundID.Item20;

            Item.DamageType = DamageClass.Magic;

			// Set rarity and value
			Item.SetShopValues(ItemRarityColor.Cyan9, 110000);
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (count >= 3)
            {
                SoundEngine.PlaySound(new SoundStyle($"{nameof(FrancisMod)}/Assets/Sounds/FrancisLaugh2"){
                Volume = 0.9f,
                PitchVariance = 0.3f,
                MaxInstances = 3,
		        }, Item.Center);
                
                Vector2 randVect = Main.rand.NextVector2Unit();
                for (int i = 0; i <= 270; i += 90)
                {
                    Vector2 worldPosition = Main.MouseWorld;
                    Vector2 projectilePosition = Main.MouseWorld + (randVect.RotatedBy(MathHelper.ToRadians(i)) * 400);
                    Vector2 direction = worldPosition - projectilePosition;
                    direction.Normalize();

                    Projectile.NewProjectile(source, projectilePosition, direction, ModContent.ProjectileType<EggBiscuit>(), damage, 0f, Main.myPlayer, 0);
                }
                count = 0;
            }
            else
                count++;

            return true;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone) {
			target.AddBuff(BuffID.Ichor, 60);
		}

		public override void AddRecipes() {
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<RefinedFrancisBar>(), 25);
			recipe.AddIngredient(ModContent.ItemType<BookOfFrancis>(), 1);
			recipe.AddIngredient(ModContent.ItemType<GalacticaSingularity>(), 5);
            recipe.AddIngredient(ModContent.ItemType<LifeAlloy>(), 5);

			recipe.AddTile(TileID.LunarCraftingStation);
			recipe.Register();
		}

	}
}