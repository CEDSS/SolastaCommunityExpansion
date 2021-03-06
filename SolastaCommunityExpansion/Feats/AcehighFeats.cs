using SolastaModApi;
using SolastaModApi.Extensions;
using System.Collections.Generic;
using static FeatureDefinitionSavingThrowAffinity;

namespace SolastaCommunityExpansion.Feats
{
    class AcehighFeats
    {
        private static bool initialized = false;
        public static void CreateFeats(List<FeatDefinition> feats)
        {
            feats.Add(PowerAttackFeatBuilder.AddToFeatList());
            feats.Add(RecklessFuryFeatBuilder.AddToFeatList());

            initialized = true;
        }

        private static FeatureDefinitionAttackModifier PowerAttackModifier;
        private static FeatureDefinitionAttackModifier PowerAttackModifierTwoHanded;

        public static void UpdatePowerAttackModifier()
        {
            if (!initialized)
            {
                return;
            }
            PowerAttackModifier.SetAttackRollModifier(-Main.Settings.FeatPowerAttackModifier);
            PowerAttackModifier.SetDamageRollModifier(Main.Settings.FeatPowerAttackModifier);
            PowerAttackModifierTwoHanded.SetAttackRollModifier(-Main.Settings.FeatPowerAttackModifier);
            PowerAttackModifierTwoHanded.SetDamageRollModifier(2 * Main.Settings.FeatPowerAttackModifier);
        }

        internal class PowerAttackPowerBuilder : BaseDefinitionBuilder<FeatureDefinitionPower>
        {
            const string PowerAttackPowerName = "PowerAttack";
            const string PowerAttackPowerNameGuid = "0a3e6a7d-4628-4189-b91d-d7146d774bb6";

            protected PowerAttackPowerBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionPowers.PowerDomainLifePreserveLife, name, guid)
            {
                Definition.GuiPresentation.Title = "Feature/&PowerAttackPowerTitle";
                Definition.GuiPresentation.Description = "Feature/&PowerAttackPowerDescription";

                Definition.SetRechargeRate(RuleDefinitions.RechargeRate.AtWill);
                Definition.SetActivationTime(RuleDefinitions.ActivationTime.NoCost);
                Definition.SetShortTitleOverride("Feature/&PowerAttackPowerTitle");

                //Create the power attack effect
                EffectForm powerAttackEffect = new EffectForm();
                powerAttackEffect.ConditionForm = new ConditionForm();
                powerAttackEffect.FormType = EffectForm.EffectFormType.Condition;
                powerAttackEffect.ConditionForm.Operation = ConditionForm.ConditionOperation.Add;
                powerAttackEffect.ConditionForm.ConditionDefinition = PowerAttackConditionBuilder.PowerAttackCondition;

                //Add to our new effect
                EffectDescription newEffectDescription = new EffectDescription();
                newEffectDescription.Copy(Definition.EffectDescription);
                newEffectDescription.EffectForms.Clear();
                newEffectDescription.EffectForms.Add(powerAttackEffect);
                newEffectDescription.HasSavingThrow = false;
                newEffectDescription.DurationType = RuleDefinitions.DurationType.Turn;
                newEffectDescription.SetTargetSide(RuleDefinitions.Side.Ally);
                newEffectDescription.SetTargetType(RuleDefinitions.TargetType.Self);
                newEffectDescription.SetCanBePlacedOnCharacter(true);

                Definition.SetEffectDescription(newEffectDescription);
            }

            public static FeatureDefinitionPower CreateAndAddToDB(string name, string guid)
                => new PowerAttackPowerBuilder(name, guid).AddToDB();

            public static FeatureDefinitionPower PowerAttackPower = CreateAndAddToDB(PowerAttackPowerName, PowerAttackPowerNameGuid);
        }

        internal class PowerAttackTwoHandedPowerBuilder : BaseDefinitionBuilder<FeatureDefinitionPower>
        {
            const string PowerAttackTwoHandedPowerName = "PowerAttackTwoHanded";
            const string PowerAttackTwoHandedPowerNameGuid = "b45b8467-7caa-428e-b4b5-ba3c4a153f07";

            protected PowerAttackTwoHandedPowerBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionPowers.PowerDomainElementalLightningBlade, name, guid)
            {
                Definition.GuiPresentation.Title = "Feature/&PowerAttackTwoHandedPowerTitle";
                Definition.GuiPresentation.Description = "Feature/&PowerAttackTwoHandedPowerDescription";

                Definition.SetRechargeRate(RuleDefinitions.RechargeRate.AtWill);
                Definition.SetActivationTime(RuleDefinitions.ActivationTime.NoCost);
                Definition.SetShortTitleOverride("Feature/&PowerAttackTwoHandedPowerTitle");

                //Create the power attack effect
                EffectForm powerAttackEffect = new EffectForm();
                powerAttackEffect.ConditionForm = new ConditionForm();
                powerAttackEffect.FormType = EffectForm.EffectFormType.Condition;
                powerAttackEffect.ConditionForm.Operation = ConditionForm.ConditionOperation.Add;
                powerAttackEffect.ConditionForm.ConditionDefinition = PowerAttackTwoHandedConditionBuilder.PowerAttackTwoHandedCondition;

                //Add to our new effect
                EffectDescription newEffectDescription = new EffectDescription();
                newEffectDescription.Copy(Definition.EffectDescription);
                newEffectDescription.EffectForms.Clear();
                newEffectDescription.EffectForms.Add(powerAttackEffect);
                newEffectDescription.HasSavingThrow = false;
                newEffectDescription.DurationType = RuleDefinitions.DurationType.Turn;
                newEffectDescription.SetTargetSide(RuleDefinitions.Side.Ally);
                newEffectDescription.SetTargetType(RuleDefinitions.TargetType.Self);
                newEffectDescription.SetCanBePlacedOnCharacter(true);

                Definition.SetEffectDescription(newEffectDescription);
            }

            public static FeatureDefinitionPower CreateAndAddToDB(string name, string guid)
                => new PowerAttackTwoHandedPowerBuilder(name, guid).AddToDB();

            public static FeatureDefinitionPower PowerAttackTwoHandedPower = CreateAndAddToDB(PowerAttackTwoHandedPowerName, PowerAttackTwoHandedPowerNameGuid);
        }

        internal class PowerAttackOnHandedAttackModifierBuilder : BaseDefinitionBuilder<FeatureDefinitionAttackModifier>
        {
            const string PowerAttackAttackModifierName = "PowerAttackAttackModifier";
            const string PowerAttackAttackModifierNameGuid = "87286627-3e62-459d-8781-ceac1c3462e6";

            protected PowerAttackOnHandedAttackModifierBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionAttackModifiers.AttackModifierFightingStyleArchery, name, guid)
            {
                Definition.GuiPresentation.Title = "Feature/&PowerAttackAttackModifierTitle";
                Definition.GuiPresentation.Description = "Feature/&PowerAttackAttackModifierDescription";

                //Ideally this would be proficiency but there isn't a nice way to subtract proficiency.
                //To do this properly you could likely make multiple versions of this that get replaced at proficiency level ups but it's a bit of a pain, so going with -3 for now.
                //Originally I made an implemenation that used FeatureDefinitionAdditionalDamage and was going to restrict to melee weapons etc. but really power attack should be avaiable for any build as you choose.
                //The FeatureDefinitionAdditionalDamage was limited in the sense that you couldn't check for things like the 'TwoHanded' or 'Heavy' properties of a weapon so it wasn't worth using really.
                Definition.SetAttackRollModifier(-Main.Settings.FeatPowerAttackModifier);
                Definition.SetDamageRollModifier(Main.Settings.FeatPowerAttackModifier);
                PowerAttackModifier = Definition;
            }

            public static FeatureDefinitionAttackModifier CreateAndAddToDB(string name, string guid)
                => new PowerAttackOnHandedAttackModifierBuilder(name, guid).AddToDB();

            public static FeatureDefinitionAttackModifier PowerAttackAttackModifier
                => CreateAndAddToDB(PowerAttackAttackModifierName, PowerAttackAttackModifierNameGuid);
        }

        internal class PowerAttackTwoHandedAttackModifierBuilder : BaseDefinitionBuilder<FeatureDefinitionAttackModifier>
        {
            const string PowerAttackTwoHandedAttackModifierName = "PowerAttackTwoHandedAttackModifier";
            const string PowerAttackTwoHandedAttackModifierNameGuid = "b1b05940-7558-4f03-98d1-01f616b5ae25";

            protected PowerAttackTwoHandedAttackModifierBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionAttackModifiers.AttackModifierFightingStyleArchery, name, guid)
            {
                Definition.GuiPresentation.Title = "Feature/&PowerAttackTwoHandedAttackModifierTitle";
                Definition.GuiPresentation.Description = "Feature/&PowerAttackTwoHandedAttackModifierDescription";

                //Ideally this would be proficiency but there isn't a nice way to subtract proficiency.
                //To do this properly you could likely make multiple versions of this that get replaced at proficiency level ups but it's a bit of a pain, so going with -3 for now.
                //Originally I made an implemenation that used FeatureDefinitionAdditionalDamage and was going to restrict to melee weapons etc. but really power attack should be avaiable for any build as you choose.
                //The FeatureDefinitionAdditionalDamage was limited in the sense that you couldn't check for things like the 'TwoHanded' or 'Heavy' properties of a weapon so it wasn't worth using really.
                Definition.SetAttackRollModifier(-Main.Settings.FeatPowerAttackModifier);
                Definition.SetDamageRollModifier(2 * Main.Settings.FeatPowerAttackModifier);

                PowerAttackModifierTwoHanded = Definition;
            }

            public static FeatureDefinitionAttackModifier CreateAndAddToDB(string name, string guid)
                => new PowerAttackTwoHandedAttackModifierBuilder(name, guid).AddToDB();

            public static FeatureDefinitionAttackModifier PowerAttackTwoHandedAttackModifier = CreateAndAddToDB(PowerAttackTwoHandedAttackModifierName, PowerAttackTwoHandedAttackModifierNameGuid);
        }

        internal class PowerAttackConditionBuilder : BaseDefinitionBuilder<ConditionDefinition>
        {
            const string PowerAttackConditionName = "PowerAttackCondition";
            const string PowerAttackConditionNameGuid = "c125b7b9-e668-4c6f-a742-63c065ad2292";

            protected PowerAttackConditionBuilder(string name, string guid) : base(DatabaseHelper.ConditionDefinitions.ConditionHeraldOfBattle, name, guid)
            {
                Definition.GuiPresentation.Title = "Feature/&PowerAttackConditionTitle";
                Definition.GuiPresentation.Description = "Feature/&PowerAttackConditionDescription";

                Definition.SetAllowMultipleInstances(false);
                Definition.Features.Clear();
                Definition.Features.Add(PowerAttackOnHandedAttackModifierBuilder.PowerAttackAttackModifier);

                Definition.SetDurationType(RuleDefinitions.DurationType.Turn);
            }

            public static ConditionDefinition CreateAndAddToDB(string name, string guid)
                => new PowerAttackConditionBuilder(name, guid).AddToDB();

            public static ConditionDefinition PowerAttackCondition = CreateAndAddToDB(PowerAttackConditionName, PowerAttackConditionNameGuid);
        }

        internal class PowerAttackTwoHandedConditionBuilder : BaseDefinitionBuilder<ConditionDefinition>
        {
            const string PowerAttackTwoHandedConditionName = "PowerAttackTwoHandedCondition";
            const string PowerAttackTwoHandedConditionNameGuid = "7d0eecbd-9ad8-4915-a3f7-cfa131001fe6";

            protected PowerAttackTwoHandedConditionBuilder(string name, string guid) : base(DatabaseHelper.ConditionDefinitions.ConditionHeraldOfBattle, name, guid)
            {
                Definition.GuiPresentation.Title = "Feature/&PowerAttackTwoHandedConditionTitle";
                Definition.GuiPresentation.Description = "Feature/&PowerAttackTwoHandedConditionDescription";

                Definition.SetAllowMultipleInstances(false);
                Definition.Features.Clear();
                Definition.Features.Add(PowerAttackTwoHandedAttackModifierBuilder.PowerAttackTwoHandedAttackModifier);

                Definition.SetDurationType(RuleDefinitions.DurationType.Turn);
            }

            public static ConditionDefinition CreateAndAddToDB(string name, string guid)
                => new PowerAttackTwoHandedConditionBuilder(name, guid).AddToDB();

            public static ConditionDefinition PowerAttackTwoHandedCondition = CreateAndAddToDB(PowerAttackTwoHandedConditionName, PowerAttackTwoHandedConditionNameGuid);
        }

        internal class PowerAttackFeatBuilder : BaseDefinitionBuilder<FeatDefinition>
        {
            const string PowerAttackFeatName = "PowerAttackFeat";
            const string PowerAttackFeatNameGuid = "88f1fb27-66af-49c6-b038-a38142b1083e";

            protected PowerAttackFeatBuilder(string name, string guid) : base(DatabaseHelper.FeatDefinitions.FollowUpStrike, name, guid)
            {
                Definition.GuiPresentation.Title = "Feat/&PowerAttackFeatTitle";
                Definition.GuiPresentation.Description = "Feat/&PowerAttackFeatDescription";

                Definition.Features.Clear();
                Definition.Features.Add(PowerAttackPowerBuilder.PowerAttackPower);
                Definition.Features.Add(PowerAttackTwoHandedPowerBuilder.PowerAttackTwoHandedPower);
                Definition.SetMinimalAbilityScorePrerequisite(false);
            }

            public static FeatDefinition CreateAndAddToDB(string name, string guid)
                => new PowerAttackFeatBuilder(name, guid).AddToDB();

            public static FeatDefinition PowerAttackFeat = CreateAndAddToDB(PowerAttackFeatName, PowerAttackFeatNameGuid);

            public static FeatDefinition AddToFeatList()
            {
                var powerAttackFeat = PowerAttackFeat;//Instantiating it adds to the DB
                return powerAttackFeat;
            }
        }
        internal class RecklessFuryFeatBuilder : BaseDefinitionBuilder<FeatDefinition>
        {
            const string RecklessFuryFeatName = "RecklessFuryFeat";
            const string RecklessFuryFeatNameGuid = "78c5fd76-e25b-499d-896f-3eaf84c711d8";

            protected RecklessFuryFeatBuilder(string name, string guid) : base(DatabaseHelper.FeatDefinitions.FollowUpStrike, name, guid)
            {
                Definition.GuiPresentation.Title = "Feat/&RecklessFuryFeatTitle";
                Definition.GuiPresentation.Description = "Feat/&RecklessFuryFeatDescription";

                Definition.Features.Clear();
                Definition.Features.Add(DatabaseHelper.FeatureDefinitionPowers.PowerReckless);
                Definition.Features.Add(RagePowerBuilder.RagePower);
                Definition.SetMinimalAbilityScorePrerequisite(false);
            }

            public static FeatDefinition CreateAndAddToDB(string name, string guid)
                => new RecklessFuryFeatBuilder(name, guid).AddToDB();

            public static FeatDefinition RecklessFuryFeat = CreateAndAddToDB(RecklessFuryFeatName, RecklessFuryFeatNameGuid);

            public static FeatDefinition AddToFeatList()
            {
                var RecklessFuryFeat = RecklessFuryFeatBuilder.RecklessFuryFeat;//Instantiating it adds to the DB
                return RecklessFuryFeat;
            }
        }

        internal class RagePowerBuilder : BaseDefinitionBuilder<FeatureDefinitionPower>
        {
            const string RagePowerName = "AHRagePower";
            const string RagePowerNameGuid = "a46c1722-7825-4a81-bca1-392b51cd7d97";

            protected RagePowerBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionPowers.PowerDomainElementalFireBurst, name, guid)
            {
                Definition.GuiPresentation.Title = "Feature/&RagePowerTitle";
                Definition.GuiPresentation.Description = "Feature/&RagePowerDescription";

                Definition.SetRechargeRate(RuleDefinitions.RechargeRate.LongRest);
                Definition.SetActivationTime(RuleDefinitions.ActivationTime.BonusAction);
                Definition.SetCostPerUse(1);
                Definition.SetFixedUsesPerRecharge(1);
                Definition.SetShortTitleOverride("Feature/&RagePowerTitle");

                //Create the power attack effect
                EffectForm rageEffect = new EffectForm();
                rageEffect.ConditionForm = new ConditionForm();
                rageEffect.FormType = EffectForm.EffectFormType.Condition;
                rageEffect.ConditionForm.Operation = ConditionForm.ConditionOperation.Add;
                rageEffect.ConditionForm.ConditionDefinition = RageFeatConditionBuilder.RageFeatCondition;

                //Add to our new effect
                EffectDescription newEffectDescription = new EffectDescription();
                newEffectDescription.Copy(Definition.EffectDescription);
                newEffectDescription.EffectForms.Clear();
                newEffectDescription.EffectForms.Add(rageEffect);
                newEffectDescription.HasSavingThrow = false;
                newEffectDescription.DurationType = RuleDefinitions.DurationType.Minute;
                newEffectDescription.DurationParameter = 1;
                newEffectDescription.SetTargetSide(RuleDefinitions.Side.Ally);
                newEffectDescription.SetTargetType(RuleDefinitions.TargetType.Self);
                newEffectDescription.SetCanBePlacedOnCharacter(true);

                Definition.SetEffectDescription(newEffectDescription);
            }

            public static FeatureDefinitionPower CreateAndAddToDB(string name, string guid)
                => new RagePowerBuilder(name, guid).AddToDB();

            public static FeatureDefinitionPower RagePower = CreateAndAddToDB(RagePowerName, RagePowerNameGuid);
        }

        internal class RageFeatConditionBuilder : BaseDefinitionBuilder<ConditionDefinition>
        {
            const string RageFeatConditionName = "AHRageFeatCondition";
            const string RageFeatConditionNameGuid = "2f34fb85-6a5d-4a4e-871b-026872bc24b8";

            protected RageFeatConditionBuilder(string name, string guid) : base(DatabaseHelper.ConditionDefinitions.ConditionHeraldOfBattle, name, guid)
            {
                Definition.GuiPresentation.Title = "Feature/&RageFeatConditionTitle";
                Definition.GuiPresentation.Description = "Feature/&RageFeatConditionDescription";

                Definition.SetAllowMultipleInstances(false);
                Definition.Features.Clear();
                Definition.Features.Add(DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityBludgeoningResistance);
                Definition.Features.Add(DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinitySlashingResistance);
                Definition.Features.Add(DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityPiercingResistance);
                Definition.Features.Add(DatabaseHelper.FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityConditionBullsStrength);
                Definition.Features.Add(RageStrengthSavingThrowAffinityBuilder.RageStrengthSavingThrowAffinity);
                Definition.Features.Add(RageDamageBonusAttackModifierBuilder.RageDamageBonusAttackModifier);
                Definition.SetDurationType(RuleDefinitions.DurationType.Minute);
                Definition.SetDurationParameter(1);


                Definition.SetDurationType(RuleDefinitions.DurationType.Turn);
            }

            public static ConditionDefinition CreateAndAddToDB(string name, string guid)
                => new RageFeatConditionBuilder(name, guid).AddToDB();

            public static ConditionDefinition RageFeatCondition = CreateAndAddToDB(RageFeatConditionName, RageFeatConditionNameGuid);
        }

        internal class RageStrengthSavingThrowAffinityBuilder : BaseDefinitionBuilder<FeatureDefinitionSavingThrowAffinity>
        {
            const string RageStrengthSavingThrowAffinityName = "AHRageStrengthSavingThrowAffinity";
            const string RageStrengthSavingThrowAffinityNameGuid = "17d26173-7353-4087-a295-96e1ec2e6cd4";

            protected RageStrengthSavingThrowAffinityBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinityCreedOfArun, name, guid)
            {
                Definition.GuiPresentation.Title = "Feature/&RageStrengthSavingThrowAffinityTitle";
                Definition.GuiPresentation.Description = "Feature/&RageStrengthSavingThrowAffinityDescription";

                Definition.AffinityGroups.Clear();
                var strengthSaveAffinityGroup = new SavingThrowAffinityGroup();
                strengthSaveAffinityGroup.affinity = RuleDefinitions.CharacterSavingThrowAffinity.Advantage;
                strengthSaveAffinityGroup.abilityScoreName = "Strength";
            }

            public static FeatureDefinitionSavingThrowAffinity CreateAndAddToDB(string name, string guid)
                => new RageStrengthSavingThrowAffinityBuilder(name, guid).AddToDB();

            public static FeatureDefinitionSavingThrowAffinity RageStrengthSavingThrowAffinity = CreateAndAddToDB(RageStrengthSavingThrowAffinityName, RageStrengthSavingThrowAffinityNameGuid);
        }

        internal class RageDamageBonusAttackModifierBuilder : BaseDefinitionBuilder<FeatureDefinitionAttackModifier>
        {
            const string RageDamageBonusAttackModifierName = "AHRageDamageBonusAttackModifier";
            const string RageDamageBonusAttackModifierNameGuid = "7bc1a47e-9519-4a37-a89a-10bcfa83e48a";

            protected RageDamageBonusAttackModifierBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionAttackModifiers.AttackModifierFightingStyleArchery, name, guid)
            {
                Definition.GuiPresentation.Title = "Feature/&RageDamageBonusAttackModifierTitle";
                Definition.GuiPresentation.Description = "Feature/&RageDamageBonusAttackModifierDescription";

                //Currently works with ranged weapons, in the end it's fine.
                Definition.SetAttackRollModifier(0);
                Definition.SetDamageRollModifier(2);//Could find a way to up this at level 9 to match barb but that seems like a lot of work right now :)
            }

            public static FeatureDefinitionAttackModifier CreateAndAddToDB(string name, string guid)
                => new RageDamageBonusAttackModifierBuilder(name, guid).AddToDB();

            public static FeatureDefinitionAttackModifier RageDamageBonusAttackModifier = CreateAndAddToDB(RageDamageBonusAttackModifierName, RageDamageBonusAttackModifierNameGuid);
        }
    }
}
