using System;
using System.Collections.Generic;
using System.Linq;
using SolastaCommunityExpansion.Features;
using SolastaModApi;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;
using UnityEngine.AddressableAssets;



namespace SolastaCommunityExpansion.Subclasses.Ranger
{
    class Arcanist : AbstractSubclass
    {
        private CharacterSubclassDefinition Subclass;
        internal override FeatureDefinitionSubclassChoice GetSubclassChoiceList()
        {
            return DatabaseHelper.FeatureDefinitionSubclassChoices.SubclassChoiceRangerArchetypes;
        }
        internal override CharacterSubclassDefinition GetSubclass()
        {
            if (Subclass == null)
            {
                Subclass = BuildAndAddSubclass();
            }
            return Subclass;
        }

        const string RangerArcanistRangerSubclassName = "RangerArcanistRangerSubclass";
        const string RangerArcanistRangerSubclassGuid = "5ABD870D-9ABD-4953-A2EC-E2109324FAB9";

        public static Guid RA_BASE_GUID = new Guid(RangerArcanistRangerSubclassGuid);

        static public FeatureDefinitionFeatureSet ranger_arcanist_magic = createRangerArcanistMagic();
        static public FeatureDefinitionAdditionalDamage arcanist_mark = createArcanistMark();
        static public FeatureDefinitionAdditionalDamage arcane_detonation = createArcaneDetonation();
        static public FeatureDefinition arcane_detonation_upgrade = createArcaneDetonationUpgrade();
        static public Dictionary<int, FeatureDefinitionPower> arcane_pulse_dict = createArcanePulseDict();

        public static CharacterSubclassDefinition BuildAndAddSubclass()
        {
            var subclassGuiPresentation = new GuiPresentationBuilder(
                    "Subclass/&RangerArcanistRangerSubclassDescription",
                    "Subclass/&RangerArcanistRangerSubclassTitle")
                    .SetSpriteReference(DatabaseHelper.CharacterSubclassDefinitions.RoguishShadowCaster.GuiPresentation.SpriteReference)
                    .Build();

            var definition = new CharacterSubclassDefinitionBuilder(RangerArcanistRangerSubclassName, RangerArcanistRangerSubclassGuid)
                    .SetGuiPresentation(subclassGuiPresentation)
                    .AddFeatureAtLevel(ranger_arcanist_magic, 3)
                    .AddFeatureAtLevel(arcanist_mark, 3)
                    .AddFeatureAtLevel(arcane_detonation, 3)
                    .AddFeatureAtLevel(arcane_pulse_dict[7], 7)
                    .AddFeatureAtLevel(arcane_detonation_upgrade, 11)
                    .AddFeatureAtLevel(arcane_pulse_dict[15], 15)
                    .AddToDB();

            return definition;
        }

        private static DiceByRank buildDiceByRank(int rank, int dice)
        {
            DiceByRank diceByRank = new DiceByRank();
            diceByRank.SetField("rank", rank);
            diceByRank.SetField("diceNumber", dice);
            return diceByRank;
        }

        static FeatureDefinitionFeatureSet createRangerArcanistMagic()
        {
            GuiPresentation blank = new GuiPresentationBuilder("Feature/&NoContentTitle", "Feature/&NoContentTitle").Build();

            FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup arcanistSpells1 = new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup()
            {
                ClassLevel = 2,
                SpellsList = new List<SpellDefinition>(){DatabaseHelper.SpellDefinitions.Shield,}
            };
            FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup arcanistSpells2 = new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup()
            {
                ClassLevel = 5,
                SpellsList = new List<SpellDefinition>() { DatabaseHelper.SpellDefinitions.MistyStep, }
            };
            FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup arcanistSpells3 = new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup()
            {
                ClassLevel = 9,
                SpellsList = new List<SpellDefinition>() { DatabaseHelper.SpellDefinitions.Haste, }
            };
            FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup arcanistSpells4 = new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup()
            {
                ClassLevel = 13,
                SpellsList = new List<SpellDefinition>() { DatabaseHelper.SpellDefinitions.DimensionDoor, }
            };
            FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup arcanistSpells5 = new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup()
            {
                ClassLevel = 17,
                SpellsList = new List<SpellDefinition>() { DatabaseHelper.SpellDefinitions.HoldMonster, }
            };

            FeatureDefinitionAutoPreparedSpells preparedSpells = new FeatureDefinitionAutoPreparedSpellsBuilder("ArcanistAutoPreparedSpells",
                GuidHelper.Create(RA_BASE_GUID, "ArcanistAutoPreparedSpells").ToString(),
               new List<FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup>() {
                    arcanistSpells1, arcanistSpells2, arcanistSpells3, arcanistSpells4, arcanistSpells5},
               DatabaseHelper.CharacterClassDefinitions.Ranger, blank).AddToDB();

            var arcanist_affinity = new FeatureDefinitionMagicAffinityBuilder(DatabaseHelper.FeatureDefinitionMagicAffinitys.MagicAffinityBattleMagic,
                "MagicAffinityRangerArcanist",
                GuidHelper.Create(RA_BASE_GUID, "MagicAffinityRangerArcanist").ToString(), blank).AddToDB();

            GuiPresentation arcanistMagicGui = new GuiPresentationBuilder("Feature/&RangerArcanistMagicDescription", "Feature/&RangerArcanistMagicTitle").Build();
            return new FeatureDefinitionFeatureSetBuilder("RangerArcanistMagic",
                GuidHelper.Create(RA_BASE_GUID, "RangerArcanistManaTouchedGuardian").ToString(), // Oops, will have to live with this name being off)
                new List<FeatureDefinition>() { preparedSpells, arcanist_affinity},
                FeatureDefinitionFeatureSet.FeatureSetMode.Union, arcanistMagicGui).AddToDB();
        }

        private class FeatureDefinitionAutoPreparedSpellsBuilder : BaseDefinitionBuilder<FeatureDefinitionAutoPreparedSpells>
        {
            public FeatureDefinitionAutoPreparedSpellsBuilder(string name, string guid, List<FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup> autospelllists,
            CharacterClassDefinition characterclass, GuiPresentation guiPresentation) : base(name, guid)
            {
                Definition.SetField("autoPreparedSpellsGroups", autospelllists);
                Definition.SetSpellcastingClass(characterclass);
                Definition.SetGuiPresentation(guiPresentation);
            }
        }

        private class FeatureDefinitionFeatureSetBuilder : BaseDefinitionBuilder<FeatureDefinitionFeatureSet>
        {
            public FeatureDefinitionFeatureSetBuilder(string name, string guid, List<FeatureDefinition> features,
                FeatureDefinitionFeatureSet.FeatureSetMode mode, GuiPresentation guiPresentation) : base(name, guid)
            {
                Definition.SetField("featureSet", features);
                Definition.SetMode(mode);
                Definition.SetGuiPresentation(guiPresentation);
                // enumerateInDescription and uniqueChoices default to false.
            }
        }

        static FeatureDefinitionAdditionalDamage createArcanistMark()
        {
            var marked_condition = ConditionMarkedByArcanistBuilder.GetOrAdd();

            var mark_apply = new FeatureDefinitionAdditionalDamageBuilder(
                DatabaseHelper.FeatureDefinitionAdditionalDamages.AdditionalDamageHuntersMark,
                "AdditionalDamageArcanistMark",
                GuidHelper.Create(RA_BASE_GUID, "AdditionalDamageArcanistMark").ToString(),
                new GuiPresentationBuilder("Feature/&ArcanistMarkDescription", "Feature/&ArcanistMarkTitle").Build());
            mark_apply.SetSpecificDamageType("DamageForce");
            mark_apply.SetDamageDice(RuleDefinitions.DieType.D6, 0);
            mark_apply.SetNotificationTag("ArcanistMark");
            mark_apply.SetTriggerCondition(RuleDefinitions.AdditionalDamageTriggerCondition.AlwaysActive);
            mark_apply.SetNoSave();
            mark_apply.NoAdvancement();
            mark_apply.SetConditionOperations(new List<ConditionOperationDescription>(){
                new ConditionOperationDescription()
                        {
                            ConditionDefinition = marked_condition,
                            Operation = ConditionOperationDescription.ConditionOperation.Add
                        }
            });

            return mark_apply.AddToDB();
        }

        static FeatureDefinitionAdditionalDamage createArcaneDetonation()
        {
            var marked_condition = ConditionMarkedByArcanistBuilder.GetOrAdd();

            var asset_reference = new AssetReference();
            asset_reference.SetField("m_AssetGUID", "9f1fe10e6ef8c9c43b6b2ef91b2ad38a");

            var mark_damage = new FeatureDefinitionAdditionalDamageBuilder(
                DatabaseHelper.FeatureDefinitionAdditionalDamages.AdditionalDamageHuntersMark,
                "AdditionalDamageArcaneDetonation",
                GuidHelper.Create(RA_BASE_GUID, "AdditionalDamageArcaneDetonation").ToString(),
                new GuiPresentationBuilder("Feature/&ArcaneDetonationDescription", "Feature/&ArcaneDetonationTitle").Build());
            mark_damage.SetSpecificDamageType("DamageForce");
            mark_damage.SetDamageDice(RuleDefinitions.DieType.D6, 1);
            mark_damage.SetNotificationTag("ArcanistMark");
            mark_damage.SetTargetCondition(marked_condition, RuleDefinitions.AdditionalDamageTriggerCondition.TargetHasConditionCreatedByMe);
            mark_damage.SetNoSave();
            mark_damage.SetConditionOperations(new List<ConditionOperationDescription>(){
               new ConditionOperationDescription()
                        {
                            ConditionDefinition = marked_condition,
                            Operation = ConditionOperationDescription.ConditionOperation.Remove
                        }
            });
            mark_damage.SetClassAdvancement(new List<DiceByRank>
                    {
                        buildDiceByRank(1, 1),
                        buildDiceByRank(2, 1),
                        buildDiceByRank(3, 1),
                        buildDiceByRank(4, 1),
                        buildDiceByRank(5, 1),
                        buildDiceByRank(6, 1),
                        buildDiceByRank(7, 1),
                        buildDiceByRank(8, 1),
                        buildDiceByRank(9, 1),
                        buildDiceByRank(10, 1),
                        buildDiceByRank(11, 2),
                        buildDiceByRank(12, 2),
                        buildDiceByRank(13, 2),
                        buildDiceByRank(14, 2),
                        buildDiceByRank(15, 2),
                        buildDiceByRank(16, 2),
                        buildDiceByRank(17, 2),
                        buildDiceByRank(18, 2),
                        buildDiceByRank(19, 2),
                        buildDiceByRank(20, 2)
                    });
            mark_damage.SetFrequencyLimit(RuleDefinitions.FeatureLimitedUsage.None);
            mark_damage.SetImpactParticleReference(asset_reference);    
            return mark_damage.AddToDB();
        }

        static FeatureDefinition createArcaneDetonationUpgrade()
        {
            // This is a blank feature. It does nothing except create a description for what happens at level 11.
            var blank_feature = new FeatureDefinitionBuilder("AdditionalDamageArcaneDetonationUpgrade",
                GuidHelper.Create(RA_BASE_GUID, "AdditionalDamageArcaneDetonationUpgrade").ToString(),
                new GuiPresentationBuilder("Feature/&ArcaneDetonationUpgradeDescription", "Feature/&ArcaneDetonationUpgradeTitle").Build()).AddToDB();
            return blank_feature;
        }

        private class FeatureDefinitionBuilder : BaseDefinitionBuilder<FeatureDefinition>
        {
            public FeatureDefinitionBuilder(string name, string guid, GuiPresentation guiPresentation) : base(name, guid)
            {
                Definition.SetGuiPresentation(guiPresentation);
            }
        }

        static Dictionary<int, FeatureDefinitionPower> createArcanePulseDict()
        {
            var marked_effect = new EffectForm();
            marked_effect.ConditionForm = new ConditionForm();
            marked_effect.FormType = EffectForm.EffectFormType.Condition;
            marked_effect.ConditionForm.Operation = ConditionForm.ConditionOperation.Add;
            marked_effect.ConditionForm.ConditionDefinition = ConditionMarkedByArcanistBuilder.GetOrAdd();

            var damage_effect = new EffectForm();
            damage_effect.DamageForm = new DamageForm();
            damage_effect.DamageForm.DamageType = "DamageForce";
            damage_effect.DamageForm.DieType = RuleDefinitions.DieType.D8;
            damage_effect.DamageForm.DiceNumber = 4;
            damage_effect.DamageForm.SetHealFromInflictedDamage(RuleDefinitions.HealFromInflictedDamage.Never);
            damage_effect.SavingThrowAffinity = RuleDefinitions.EffectSavingThrowType.None;

            var damage_upgrade_effect = new EffectForm();
            damage_upgrade_effect.DamageForm = new DamageForm();
            damage_upgrade_effect.DamageForm.DamageType = "DamageForce";
            damage_upgrade_effect.DamageForm.DieType = RuleDefinitions.DieType.D8;
            damage_upgrade_effect.DamageForm.DiceNumber = 8;
            damage_upgrade_effect.DamageForm.SetHealFromInflictedDamage(RuleDefinitions.HealFromInflictedDamage.Never);
            damage_upgrade_effect.SavingThrowAffinity = RuleDefinitions.EffectSavingThrowType.None;

            var arcane_pulse_action = createArcanePulse("ArcanePulse", "Feature/&ArcanePulseTitle", "Feature/&ArcanePulseDescription", marked_effect, damage_effect);

            var arcane_pulse_upgrade_action = createArcanePulse("ArcanePulseUpgrade", "Feature/&ArcanePulseTitle", "Feature/&ArcanePulseDescription", marked_effect, damage_upgrade_effect);
            arcane_pulse_upgrade_action.SetOverriddenPower(arcane_pulse_action);

            var arcane_pulse_dict = new Dictionary<int, FeatureDefinitionPower>();
            arcane_pulse_dict.Add(7, arcane_pulse_action);
            arcane_pulse_dict.Add(15, arcane_pulse_upgrade_action);

            return arcane_pulse_dict;
        }

        static FeatureDefinitionPower createArcanePulse(string name, string title, string description, EffectForm marked_effect, EffectForm damage_effect)
        {
            var pulse_description = new EffectDescription();
            pulse_description.Copy(DatabaseHelper.SpellDefinitions.MagicMissile.EffectDescription);
            pulse_description.SetCreatedByCharacter(true);
            pulse_description.SetTargetSide(RuleDefinitions.Side.Enemy);
            pulse_description.SetTargetType(RuleDefinitions.TargetType.Sphere);
            pulse_description.SetTargetParameter(3);
            pulse_description.SetRangeType(RuleDefinitions.RangeType.Distance);
            pulse_description.SetRangeParameter(30);

            pulse_description.EffectForms.Clear();
            pulse_description.EffectForms.AddRange(new List<EffectForm>
            {
                damage_effect,
                marked_effect
            });

            return new FeatureDefinitionPowerBuilder(name, GuidHelper.Create(RA_BASE_GUID, name).ToString(),
                new GuiPresentationBuilder(description, title)
                .SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerDomainElementalHeraldOfTheElementsThunder.GuiPresentation.SpriteReference).Build())
                .SetUsesAbility(0, AttributeDefinitions.Wisdom)
                .SetShowCasting(true)
                .SetRecharge(RuleDefinitions.RechargeRate.LongRest)
                .SetActivation(RuleDefinitions.ActivationTime.Action, 1)
                .SetEffect(pulse_description)
                .SetAbility(AttributeDefinitions.Wisdom)
                .SetShortTitle("Arcane Pulse")
                .AddToDB();
        }
    }


    // Creates a dedicated builder for the marked by arcanist condition. This helps with GUID wonkiness on the fact that separate features interact with it.
    internal class ConditionMarkedByArcanistBuilder : BaseDefinitionBuilder<ConditionDefinition>
    {
        protected ConditionMarkedByArcanistBuilder(string name, string guid) : base(DatabaseHelper.ConditionDefinitions.ConditionMarkedByBrandingSmite, name, guid)
        {
            Definition.GuiPresentation.Title = "Condition/&ConditionMarkedByArcanistTitle";
            Definition.GuiPresentation.Description = "Condition/&ConditionMarkedByArcanistDescription";

            Definition.SetAllowMultipleInstances(false);
            Definition.SetDurationParameter(1);
            Definition.SetDurationType(RuleDefinitions.DurationType.Permanent);
            Definition.SetTurnOccurence(RuleDefinitions.TurnOccurenceType.EndOfTurn);
            Definition.SetPossessive(true);
            Definition.SetSpecialDuration(true);
        }

        public static ConditionDefinition CreateAndAddToDB()
            => new ConditionMarkedByArcanistBuilder("ConditionMarkedByArcanist", GuidHelper.Create(Arcanist.RA_BASE_GUID, "ConditionMarkedByArcanist").ToString()).AddToDB();

        public static ConditionDefinition GetOrAdd()
        {
            var db = DatabaseRepository.GetDatabase<ConditionDefinition>();
            return db.TryGetElement("ConditionMarkedByArcanist", GuidHelper.Create(Arcanist.RA_BASE_GUID, "ConditionMarkedByArcanist").ToString()) ?? CreateAndAddToDB();
        }
    }
}