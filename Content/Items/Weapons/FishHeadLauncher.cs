using CalamityMod.Items.Materials;
using FrancisMod.Content.Items.Placeables;
using FrancisMod.Content.Projectiles.FriendlyProjectiles;
using Humanizer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FrancisMod.Content.Items.Weapons
{
	public class FishHeadLauncher : ModItem
	{
		private int count = 0;
		public override void SetDefaults() {
			// Modders can use Item.DefaultToRangedWeapon to quickly set many common properties, such as: useTime, useAnimation, useStyle, autoReuse, DamageType, shoot, shootSpeed, useAmmo, and noMelee. These are all shown individually here for teaching purposes.

			// Common Properties
			Item.width = 120; // Hitbox width of the item.
			Item.height = 48; // Hitbox height of the item.
			Item.rare = ItemRarityID.Cyan;

			// Use Properties
			Item.useTime = 11; // The item's use time in ticks (60 ticks == 1 second.)
			Item.useAnimation = 11; // The length of the item's use animation in ticks (60 ticks == 1 second.)
			Item.useStyle = ItemUseStyleID.Shoot; // How you use the item (swinging, holding out, etc.)
			Item.autoReuse = true; // Whether or not you can hold click to automatically use it again.

			// The sound that this item plays when used.
			Item.UseSound = SoundID.Item40;

			// Weapon Properties
			Item.DamageType = DamageClass.Ranged; // Sets the damage type to ranged.
			Item.damage = 120; // Sets the item's damage. Note that projectiles shot by this weapon will use its and the used ammunition's damage added together.
			Item.knockBack = 5f; // Sets the item's knockback. Note that projectiles shot by this weapon will use its and the used ammunition's knockback added together.
			Item.noMelee = true; // So the item's animation doesn't do damage.
			Item.crit = 10;

			// Gun Properties
			Item.shoot = ProjectileID.PurificationPowder;
			Item.shootSpeed = 50f; // The speed of the projectile (measured in pixels per frame.)
			Item.useAmmo = AmmoID.Rocket; // The "ammo Id" of the ammo item that this weapon uses. Ammo IDs are magic numbers that usually correspond to the item id of one item that most commonly represent the ammo type.
            
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

            Vector2 direction = Main.MouseWorld - player.Center;
            direction.Normalize();
			if (count >= 3)
			{
				SoundEngine.PlaySound(SoundID.Item61, position);
				Projectile.NewProjectileDirect(source, position + (direction * 85), velocity * 2f, ModContent.ProjectileType<FishHead>(), damage * 5, knockback, player.whoAmI, 0, 0, 1);
				count = 0;
			}
			else
			{
				Projectile.NewProjectileDirect(source, position + (direction * 85), velocity.SafeNormalize(Vector2.UnitY).RotatedByRandom(MathHelper.ToRadians(10)) * Item.shootSpeed, ModContent.ProjectileType<FishHead>(), damage, knockback, player.whoAmI);
				count++;
			}

			return false;
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		public override void AddRecipes() {
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<RefinedFrancisBar>(), 25);
			recipe.AddIngredient(ModContent.ItemType<TheChair>(), 2);
			recipe.AddIngredient(ModContent.ItemType<GalacticaSingularity>(), 5);
			recipe.AddIngredient(ModContent.ItemType<LifeAlloy>(), 5);
			
			recipe.AddTile(TileID.LunarCraftingStation);
			recipe.Register();
		}


		// This method lets you adjust position of the gun in the player's hands. Play with these values until it looks good with your graphics.
		public override Vector2? HoldoutOffset() {
			return new Vector2(2f, -2f);
		}


	}
}
