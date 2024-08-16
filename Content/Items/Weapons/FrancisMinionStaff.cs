using FrancisMod.Content.Buffs;
using Microsoft.Xna.Framework;
using FrancisMod.Content.Projectiles.FriendlyProjectiles.FrancisMinion;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using FrancisMod.Content.Items.Placeables;
using FrancisMod.Content.Items.NonPlaceables;
using CalamityMod.Items.Materials;

namespace FrancisMod.Content.Items.Weapons
{
    public class FrancisMinionStaff : ModItem
	{
		public override void SetStaticDefaults() {
			ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true; // This lets the player target anywhere on the whole screen while using a controller
			ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;

			ItemID.Sets.StaffMinionSlotsRequired[Type] = 1f; // The default value is 1, but other values are supported. See the docs for more guidance. 
		}

		public override void SetDefaults() {
			Item.damage = 200;
			Item.knockBack = 3f;
			Item.mana = 10; // mana cost
			Item.width = 28;
			Item.height = 30;
			Item.useTime = 36;
			Item.useAnimation = 36;
			Item.useStyle = ItemUseStyleID.HoldUp; // how the player's arm moves when using the item
			Item.value = Item.sellPrice(platinum: 1, gold: 30);
			Item.rare = ItemRarityID.Cyan;
			Item.UseSound = SoundID.Item44; // What sound should play when using the item

			// These below are needed for a minion weapon
			Item.noMelee = true; // this item doesn't do any melee damage
			Item.DamageType = DamageClass.Summon; // Makes the damage register as summon. If your item does not have any damage type, it becomes true damage (which means that damage scalars will not affect it). Be sure to have a damage type
			Item.buffType = ModContent.BuffType<FrancisHeadBuff>();
			// No buffTime because otherwise the item tooltip would say something like "1 minute duration"
			Item.shoot = ModContent.ProjectileType<FrancisHeadMinion>(); // This item creates the minion projectile
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
			// Here you can change where the minion is spawned. Most vanilla minions spawn at the cursor position
			position = Main.MouseWorld;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
			// This is needed so the buff that keeps your minion alive and allows you to despawn it properly applies
			player.AddBuff(Item.buffType, 2);

			// Minions have to be spawned manually, then have originalDamage assigned to the damage of the summon item
			var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, Main.myPlayer);
			projectile.originalDamage = Item.damage;

			// Since we spawned the projectile manually already, we do not need the game to spawn it for ourselves anymore, so return false
			return false;
		}

		public override void AddRecipes() {
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<RefinedFrancisBar>(), 25);
			recipe.AddIngredient(ModContent.ItemType<BookOfFrancis>(), 1);
			recipe.AddIngredient(ModContent.ItemType<GalacticaSingularity>(), 5);
			recipe.AddIngredient(ModContent.ItemType<LifeAlloy>(), 5);
			
			recipe.AddTile(TileID.CrystalBall);
			recipe.Register();
		}

	}
}