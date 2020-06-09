using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitType
    {
    Enemy,
    Player,
    Ally
    }

public enum PlayerStaminaCost
{
    Dash = 35,
    Jump = 20
}

/// <summary>
/// class for skill properties for every skill in the game
/// </summary>
public sealed class Skill
{
    public readonly int Amount; //amount of damage/healing dealt by the skill
    public readonly float Range; //range required to start using the skill
    public readonly float Cooldown; // cooldown of the skill. set at 0 for no cooldown
    public readonly bool TargetAlly; //whether this skill targets an ally
    public readonly int EnergyCost; //amount of energy needed to use skil (applies only to player)
    public readonly float ProjectileSpeed = 0; //speed of the projectile if this is a projectile skill
    public readonly bool Piercing = false; //if the projectile can pierce if the skill is a projectile
    public readonly int Bleed = 0; //bleed applied on hit with skill. bleed is dealt once per second for 4 seconds, and refreshes if a new bleed is applied
    public readonly int HealthOnHit = 0; //health gained on hit with skill
    public readonly int EnergyOnHit = 0; //energy gained on hit with skill




    private Skill(int amount, float range, float cooldown, bool targetAlly, int energyCost, int bleed, int healthOnHit, int energyOnHit) {
        Amount = amount;
        Range = range;
        Cooldown = cooldown;
        TargetAlly = targetAlly;
        EnergyCost = energyCost;
        Bleed = bleed;
        HealthOnHit = healthOnHit;
        EnergyOnHit = energyOnHit;

    }

    /// <summary>
    /// skill setup for projectiles
    /// </summary>
    private Skill(int amount, float range, float cooldown, bool targetAlly, int energyCost, int bleed, int healthOnHit, int energyOnHit, float projSpeed, bool piercing)
    {
        Amount = amount;
        Range = range;
        Cooldown = cooldown;
        TargetAlly = targetAlly;
        ProjectileSpeed = projSpeed;
        EnergyCost = energyCost;
        Piercing = piercing;
        Bleed = bleed;
        HealthOnHit = healthOnHit;
        EnergyOnHit = energyOnHit;
    }

    public static Skill Default = new Skill(0,0,0,false,0,0,0,5);
    //player skills (leave range at 0)
    public static Skill PlayerNormalAttack = new Skill(15, 0,0, false,0,0,0,5);
    public static Skill PlayerNormalRangedAttack = new Skill(10, 0, 0, false,0,0,0,0, 30, false);
    public static Skill PlayerRazorBlades = new Skill(8, 0, 5, false,10,0,0,0, 30, true); //fires 3 projectiles

    //enemy skills
    public static Skill WarriorSwing = new Skill(10, 10,10, false,0, 0, 0,0);
    public static Skill WarriorJab = new Skill(13, 7, 6, false,0, 0, 0,0);
    public static Skill WarriorNA = new Skill(10, 5, 0, false,0, 0, 0,0);

    

}

public sealed class PlayerSkill
{
    public readonly Skill UpgradableSkill; //skill being used
    public readonly List<SkillUpgrade> Upgrades;//updates available for the skill
    public readonly Icon Icon;

    private PlayerSkill(Skill skill, Icon icon, List<SkillUpgrade> upgrades)
    {
        Icon = icon;
        UpgradableSkill = skill;
        Upgrades = upgrades;
    }

    public static PlayerSkill Default = new PlayerSkill(Skill.Default,Icon.Default, new List<SkillUpgrade> { });
    
    public static PlayerSkill PlayerNormalAttack = new PlayerSkill(Skill.PlayerNormalAttack, Icon.Default,
        new List<SkillUpgrade> {
            SkillUpgrade.NormalAttackBleed,
            SkillUpgrade.NormalAttackDamage,
            SkillUpgrade.NormalAttackEnergyGain,
            SkillUpgrade.NormalAttackHeal
        });

    public static PlayerSkill PlayerNormalRangedAttack = new PlayerSkill(Skill.PlayerNormalRangedAttack, Icon.Default,
        new List<SkillUpgrade>
        {
            SkillUpgrade.NormalRangedAttackDamageSlow,
            SkillUpgrade.NormalRangedAttackAdditionalProjectiles,
            SkillUpgrade.NormalRangedAttackEnergy,
            SkillUpgrade.NormalRangedAttackPierce
        }
        );

    public static PlayerSkill PlayerRazorBlades = new PlayerSkill(Skill.PlayerRazorBlades, Icon.RazorBlades,
        new List<SkillUpgrade>
        {
            SkillUpgrade.RazorBladeAdditionalProjectiles,
            SkillUpgrade.RazorBladeBleed,
            SkillUpgrade.RazorBladeHeal,
            SkillUpgrade.RazorBladesCooldown,
        });

}

/// <summary>
/// modifiable skill class used temporarily every time a skill is used.
/// upgrades are applied to the values in this class before used
/// </summary>
public class TempPlayerSkill
{
    public int Amount; //amount of damage/healing dealt by the skill
    public float Range; //range required to start using the skill
    public float Cooldown; // cooldown of the skill. set at 0 for no cooldown
    public  bool TargetAlly; //whether this skill targets an ally
    public float EnergyCost; //amount of energy needed to use skil (applies only to player)
    public float ProjectileSpeed; //speed of the projectile if this is a projectile skill
    public  bool Piercing = false; //if the projectile can pierce if the skill is a projectile
    public int Bleed = 0; //bleed applied on hit with skill. bleed is dealt once per second for 4 seconds, and refreshes if a new bleed is applied
    public int HealthOnHit = 0; //health gained on hit with skill
    public int EnergyOnHit = 0;

    public PlayerSkill BaseSkill;
    public Dictionary<SkillUpgrade, int> Upgrades = new Dictionary<SkillUpgrade, int>();

    /// <summary>
    /// skill setup for projectiles
    /// </summary>
    public TempPlayerSkill(PlayerSkill pSkill)
    {
        Amount = pSkill.UpgradableSkill.Amount;
        Range = pSkill.UpgradableSkill.Range;
        Cooldown = pSkill.UpgradableSkill.Cooldown;
        TargetAlly = pSkill.UpgradableSkill.TargetAlly;
        ProjectileSpeed = pSkill.UpgradableSkill.ProjectileSpeed;
        EnergyCost = pSkill.UpgradableSkill.EnergyCost;
        Piercing = pSkill.UpgradableSkill.Piercing;
        Bleed = pSkill.UpgradableSkill.Bleed;
        HealthOnHit = pSkill.UpgradableSkill.HealthOnHit;
        EnergyOnHit = pSkill.UpgradableSkill.EnergyOnHit;

        BaseSkill = pSkill;
        for(int i = 0; i< pSkill.Upgrades.Count;i++)
        {
            Upgrades.Add(pSkill.Upgrades[i],0);
        }

    }

    public void ResetToBase()
    {
        Amount = BaseSkill.UpgradableSkill.Amount;
        Range = BaseSkill.UpgradableSkill.Range;
        Cooldown = BaseSkill.UpgradableSkill.Cooldown;
        TargetAlly = BaseSkill.UpgradableSkill.TargetAlly;
        ProjectileSpeed = BaseSkill.UpgradableSkill.ProjectileSpeed;
        EnergyCost = BaseSkill.UpgradableSkill.EnergyCost;
        Piercing = BaseSkill.UpgradableSkill.Piercing;
        Bleed = BaseSkill.UpgradableSkill.Bleed;
        HealthOnHit = BaseSkill.UpgradableSkill.HealthOnHit;
        EnergyOnHit = BaseSkill.UpgradableSkill.EnergyOnHit;
    }

}

/// <summary>
/// upgrade for skills of the Skill class. Has a number of generic upgrades, special upgrades are specified where the skill is used
/// </summary>
public sealed class SkillUpgrade
{
    public readonly string Description = ""; //description of upgrade
    public readonly int IncreasedAmount = 0; //increase to amount of the skill given by this upgrade
    public readonly float CooldownMultiplier = 1; //multiplier to cooldown given by this upgrade. 
    public readonly float CostMultiplier = 1; //multiplier to cost given by this upgrade
    public readonly float ProjectileSpeedMultiplier = 1; //multiplier to projectile speed given by this upgrade
    public readonly bool ProjectilePierce = false; //whether this projectile pierces
    public readonly int BleedAmount = 0; //bleed applied on hit with skill. bleed lasts for 4 seconds, dealing amount once per second
    public readonly int HealthOnHit = 0; //health gained on hit with skill
    public readonly int EnergyOnHit = 0; //energy gained on hit with skill


    public readonly int MaxUpgrades = 0; //maximum amount of times this upgrade can be picked
    private SkillUpgrade(string desc, int incAmount, float CDMulti, float CostMulti, int bleedAmount, int healthOnHit,int energyOnHit,int maxUps)
    {
        Description = desc;
        IncreasedAmount = incAmount;
        CooldownMultiplier = CDMulti;
        CostMultiplier = CostMulti;
        BleedAmount = bleedAmount;
        HealthOnHit = healthOnHit;
        EnergyOnHit = energyOnHit;
        MaxUpgrades = maxUps;
    }

    private SkillUpgrade(string desc, int incAmount, float CDMulti, float CostMulti, int bleedAmount, int healthOnHit, int energyOnHit, int maxUps, float ProjSpeedMulti)
    {
        Description = desc;
        IncreasedAmount = incAmount;
        CooldownMultiplier = CDMulti;
        CostMultiplier = CostMulti;
        ProjectileSpeedMultiplier = ProjSpeedMulti;
        BleedAmount = bleedAmount;
        HealthOnHit = healthOnHit;
        EnergyOnHit = energyOnHit;
        MaxUpgrades = maxUps;
    }

    private SkillUpgrade(string desc, int incAmount, float CDMulti, float CostMulti, int bleedAmount, int healthOnHit, int energyOnHit, int maxUps, float ProjSpeedMulti, bool pierce)
    {
        Description = desc;
        IncreasedAmount = incAmount;
        CooldownMultiplier = CDMulti;
        CostMultiplier = CostMulti;
        ProjectileSpeedMultiplier = ProjSpeedMulti;
        BleedAmount = bleedAmount;
        HealthOnHit = healthOnHit;
        EnergyOnHit = energyOnHit;
        MaxUpgrades = maxUps;
        ProjectilePierce = pierce;
    }

    #region Normal Attack
    public static SkillUpgrade NormalAttackHeal = new SkillUpgrade("Normal Attacks heal you for 3 Health", 0, 1, 1, 0, 3,0,4); 
    public static SkillUpgrade NormalAttackDamage = new SkillUpgrade("Normal Attacks deal +10 Damage", 10, 1, 1, 0, 0,0,5);
    public static SkillUpgrade NormalAttackBleed = new SkillUpgrade("Normal Attacks cause targets hit to Bleed for 3 damage for 4 seconds", 0, 1, 1, 3, 0,0,5);
    public static SkillUpgrade NormalAttackEnergyGain = new SkillUpgrade("Normal Attacks generate +1 Energy on hit", 0, 0, 0, 0, 0,0, 5);
    #endregion

    #region Normal Ranged Attack
    public static SkillUpgrade NormalRangedAttackAdditionalProjectiles = new SkillUpgrade("Normal Ranged Attacks throws an additional blade", 0, 1, 1, 0, 0,0,2);
    public static SkillUpgrade NormalRangedAttackEnergy = new SkillUpgrade("Normal Ranged Attacks generate +1 Energy on use", 0, 1, 1,  0, 0,0, 3); 
    public static SkillUpgrade NormalRangedAttackPierce = new SkillUpgrade("Normal Ranged Attacks now pierce",0,1,1,0,0,0,1,1,true); 
    public static SkillUpgrade NormalRangedAttackDamageSlow = new SkillUpgrade("Normal Ranged Attacks deal +5 Damage, but has 30% less Projectile Speed", 5, 1, 1, 0, 0,0, 5, 0.7f);
    #endregion


    #region Razor Blades
    public static SkillUpgrade RazorBladesCooldown = new SkillUpgrade("Razor Blades have 20% reduced cooldown", 0, 0.8f, 1,  0, 0, 0, 5,1,true);
    public static SkillUpgrade RazorBladeBleed = new SkillUpgrade("Razor Blades each cause targets hit to Bleed for 2 damamage for 4 seconds", 0, 1, 1, 2, 0, 0, 3, 1, true);
    public static SkillUpgrade RazorBladeAdditionalProjectiles = new SkillUpgrade("Razor Blades throws an additional projectile", 0, 1, 1, 0, 0, 0, 2, 1, true); //implement on use
    public static SkillUpgrade RazorBladeHeal = new SkillUpgrade("Razor Blades heal you for 1 Health", 0, 1, 1, 0, 1, 0, 3, 1, true); //implement on use
    #endregion
}

public sealed class Icon
{
    public readonly string IconName;
    private Icon( string iconName)
    {
        IconName = iconName;
    }

    public static Icon Default = new Icon("Default");
    public static Icon RazorBlades = new Icon("RazorBlades");

}