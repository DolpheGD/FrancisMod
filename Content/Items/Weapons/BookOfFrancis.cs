using FrancisMod.Content.Items.NonPlaceables;
using FrancisMod.Content.Items.Placeables;
using Terraria;
using Terraria.Enums;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using FrancisMod.Content.Projectiles.FriendlyProjectiles;

namespace FrancisMod.Content.Items.Weapons
{
	public class BookOfFrancis : ModItem
	{
		public int chargeAmount = 0;
		public override void SetDefaults() {
			// DefaultToStaff handles setting various Item values that magic staff weapons use.
			// Hover over DefaultToStaff in Visual Studio to read the documentation!
			// Shoot a black bolt, also known as the projectile shot from the onyx blaster.
			chargeAmount = 0;
			
			Item.DefaultToStaff(ProjectileID.DiamondBolt, 8f, 40, 25);

			Item.width = 45;
			Item.height = 48;
			Item.UseSound = SoundID.Item8;


			// A special method that sets the damage, knockback, and bonus critical strike chance.
			// This weapon has a crit of 32% which is added to the players default crit chance of 4%
			Item.SetWeaponValues(50, 5, 10);

			Item.SetShopValues(ItemRarityColor.Pink5, 26773);
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
			const int NumProjectiles = 5; // The number of projectiles that this gun will shoot.

			if (player.altFunctionUse == 0)
			{
				chargeAmount++;
				for (int i = 0; i < NumProjectiles; i++) {
					// Rotate the velocity randomly by 30 degrees at max.
					Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(13));

					// Decrease velocity randomly for nicer visuals.
					newVelocity *= 1f - Main.rand.NextFloat(0.3f);

					// Create a projectile.
					Projectile.NewProjectileDirect(source, position, newVelocity, type, damage, knockback, player.whoAmI);
				}
			}
			else if (player.altFunctionUse == 2)
			{
				if (player.statMana >= 100 && chargeAmount >= 5)
				{
					player.statMana -= 100;
					Projectile.NewProjectileDirect(source, position, velocity, ModContent.ProjectileType<FrancisOrb>(), damage, knockback, player.whoAmI);
					chargeAmount -= 5;
				}
				
			}

			if (chargeAmount > 10)
			{
				chargeAmount = 10;
			}

			return false;
		}

		public override bool AltFunctionUse(Player player) {
			return true;
		}

		public override bool CanUseItem(Player player){
			
			if ((player.altFunctionUse == 2 && player.statMana >= 100 && chargeAmount >= 5) || player.altFunctionUse == 0)
			{
				return true;
			}
			return false;
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		public override void AddRecipes() {

			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<FrancisDust>(), 15);
			recipe.AddIngredient(ModContent.ItemType<FrancisBar>(), 15);
            recipe.AddIngredient(ItemID.Book, 1);

			recipe.AddTile(TileID.CrystalBall);
			recipe.Register();
		}


	}
}
