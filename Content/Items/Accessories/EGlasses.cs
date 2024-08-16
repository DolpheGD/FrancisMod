using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using CalamityMod;
using FrancisMod.Content.Items.Placeables;

namespace FrancisMod.Content.Items.Accessories
{
	public class EGlasses : ModItem
	{
		// By declaring these here, changing the values will alter the effect, and the tooltip
		public static readonly int AdditiveDamageBonus = 12;
		public static readonly int RogueDamageBonus = 6;


		// Insert the modifier values into the tooltip localization. More info on this approach can be found on the wiki: https://github.com/tModLoader/tModLoader/wiki/Localization#binding-values-to-localizations

		public override void SetDefaults() {
			Item.width = 40;
			Item.height = 40;
			Item.accessory = true;
            Item.defense = 22;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.buyPrice(1, 30, 0, 0);
		}

		public override void UpdateAccessory(Player player, bool hideVisual) {
			// GetDamage returns a reference to the specified damage class' damage StatModifier.
			// Since it doesn't return a value, but a reference to it, you can freely modify it with mathematics operators (+, -, *, /, etc.).
			// StatModifier is a structure that separately holds float additive and multiplicative modifiers, as well as base damage and flat damage.
			// When StatModifier is applied to a value, its additive modifiers are applied before multiplicative ones.
			// Base damage is added directly to the weapon's base damage and is affected by damage bonuses, while flat damage is applied after all other calculations.
			// In this case, we're doing a number of things:
			// - Adding 25% damage, additively. This is the typical "X% damage increase" that accessories use, use this one.
			// - Adding 12% damage, multiplicatively. This effect is almost never used in Terraria, typically you want to use the additive multiplier above. It is extremely hard to correctly balance the game with multiplicative bonuses.
			// - Adding 4 base damage.
			// - Adding 5 flat damage.
			// Since we're using DamageClass.Generic, these bonuses apply to ALL damage the player deals.
			player.GetDamage(DamageClass.Generic) += AdditiveDamageBonus / 100f;
            player.GetDamage(ModContent.GetInstance<RogueDamageClass>()) += RogueDamageBonus / 100f;

        }

		public override void AddRecipes() {
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<RefinedFrancisBar>(), 10);
			recipe.AddIngredient(ItemID.IronBar, 10);
			
			recipe.AddTile(TileID.LunarCraftingStation);
			recipe.Register();
		}


	}
}