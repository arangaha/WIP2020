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
    Jump = 15
}

/// <summary>
/// class for skill properties for every skill in the game
/// </summary>
public sealed class Skill
{
    public readonly string Name; // name of the skill
    public readonly Icon Icon; //icon of the skill (class)
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




    private Skill(string name,Icon icon, int amount, float range, float cooldown, bool targetAlly, int energyCost, int bleed, int healthOnHit, int energyOnHit) {
        Name = name;
        Icon = icon;
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
    private Skill(string name, Icon icon, int amount, float range, float cooldown, bool targetAlly, int energyCost, int bleed, int healthOnHit, int energyOnHit, float projSpeed, bool piercing)
    {
        Name = name;
        Icon = icon;
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

    public static Skill Default = new Skill("",Icon.Default,0,0,0,false,0,0,0,5);
    //player skills (leave range at 0)
    public static Skill PlayerNormalAttack = new Skill("Normal Attack",Icon.NormalAttack,15, 0,0, false,0,0,0,5);
    public static Skill PlayerNormalRangedAttack = new Skill("Normal Ranged Attack",Icon.RangedNormalAttack,10, 0, 0, false,0,0,0,0, 30, false);
    public static Skill PlayerRazorBlades = new Skill("Razor Blades",Icon.RazorBlades,10, 0, 5, false,10,0,0,0, 30, true); //fires 3 projectiles
    public static Skill PlayerPuncture = new Skill("Puncture",Icon.Puncture,20, 0, 8, false, 15, 10, 0, 0);
    public static Skill PlayerSlash = new Skill("Slash",Icon.Slash,15, 0, 12, false, 0, 0, 0, 0);
    public static Skill PlayerCull = new Skill("Cull",Icon.Cull,60, 0, 14, false, 25, 0, 0, 0); //deals 1% more damage per 1% health missing on target   
    public static Skill PlayerRainOfBlades = new Skill("Rain Of Blades", Icon.RainOfBlades, 4, 0, 16, false, 20, 0, 0, 0, 12, false); //spawns a portal that spawns projetiles
    public static Skill PlayerBladeStorm = new Skill("BladeStorm", Icon.BladeStorm, 6, 0, 0, false, 15, 0, 0, 0);
    //enemy skills
    public static Skill WarriorSwing = new Skill("Warrior Swing",Icon.Default,10, 10,10, false,0, 0, 0,0);
    public static Skill WarriorJab = new Skill("Warrior Jab", Icon.Default, 13, 7, 6, false,0, 0, 0,0);
    public static Skill WarriorNA = new Skill("Warrior Normal Attack", Icon.Default, 10, 5, 0, false,0, 0, 0,0);

    public static Skill HellHoundLeap = new Skill("Hellhound Leap", Icon.Default, 7, 12, 12, false, 0, 0, 0, 0);
    public static Skill HellHoundNA = new Skill("Hellhound Normal Attack", Icon.Default, 7, 3, 0, false, 0, 0, 0, 0);
    public static Skill HellHoundFireball = new Skill("Hellhound Fireball", Icon.Default, 10, 12, 8, false, 0, 0, 0, 0, 15, false);

    public static Skill HellbatNA = new Skill("Hellbat Normal Attack", Icon.Default, 4, 8, 0, false, 0, 0, 0, 0);
    public static Skill HellbatGouge = new Skill("Hellbat Gouge", Icon.Default, 3, 8, 10, false, 0, 1, 0, 0);
    public static Skill HellbatSonicWave = new Skill("Hellbat Sonic Wave", Icon.Default, 2, 12, 8, false, 0, 0, 0, 0, 8, false);

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

    public static PlayerSkill Default = new PlayerSkill(Skill.Default, Icon.Default, new List<SkillUpgrade> { });

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

    public static PlayerSkill PlayerPuncture = new PlayerSkill(Skill.PlayerPuncture, Icon.Puncture,
        new List<SkillUpgrade>
        {
            SkillUpgrade.PunctureBleed,
            SkillUpgrade.PunctureCooldown,
            SkillUpgrade.PunctureExtraDamage,
            SkillUpgrade.PunctureHeal,
            SkillUpgrade.PunctureExpose
        });

    public static PlayerSkill PlayerSlash = new PlayerSkill(Skill.PlayerSlash, Icon.Slash,
        new List<SkillUpgrade>
        {
            SkillUpgrade.SlashEnergyOnhit,
            SkillUpgrade.SlashCooldownOnHit,
            SkillUpgrade.SlashDistance,
            SkillUpgrade.SlashGainArmor,
            SkillUpgrade.SlashDamage
        });

    public static PlayerSkill PlayerCull = new PlayerSkill(Skill.PlayerCull, Icon.Cull,
        new List<SkillUpgrade>
    {
            SkillUpgrade.CullArmorOnKill,
            SkillUpgrade.CullHealthOnKill,
            SkillUpgrade.CullSize,
            SkillUpgrade.CullDamage,
    }
    );

    public static PlayerSkill PlayerRainOfBlades = new PlayerSkill(Skill.PlayerRainOfBlades, Icon.RainOfBlades
        , new List<SkillUpgrade>
        {
            SkillUpgrade.RoBBleed,
            SkillUpgrade.RoBDamage,
            SkillUpgrade.RoBDuration,
            SkillUpgrade.RoBFourth,
            SkillUpgrade.RoBInterval,
            SkillUpgrade.RoBSlow,
            SkillUpgrade.RoBMove,
           
        }
        );

    public static PlayerSkill PlayerBladeStorm = new PlayerSkill(Skill.PlayerBladeStorm, Icon.BladeStorm,
        new List<SkillUpgrade>
        {
            SkillUpgrade.BladeStormArmor,
            SkillUpgrade.BladeStormAutoCast,
            SkillUpgrade.BladeStormBlades,
            SkillUpgrade.BladeStormCost,
            SkillUpgrade.BladeStormSize
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
    public readonly Skill Skill = Skill.Default; //skill of the upgrade
    public readonly string Description = ""; //description of upgrade
    public readonly int IncreasedAmount = 0; //increase to amount of the skill given by this upgrade
    public readonly float CooldownMultiplier = 1; //multiplier to cooldown given by this upgrade. 
    public readonly float CostMultiplier = 1; //multiplier to cost given by this upgrade
    public readonly float ProjectileSpeedMultiplier = 1; //multiplier to projectile speed given by this upgrade
    public readonly bool ProjectilePierce = false; //whether this projectile pierces
    public readonly int BleedAmount = 0; //bleed applied on hit with skill. bleed lasts for 4 seconds, dealing amount once per second
    public readonly int HealthOnHit = 0; //health gained on hit with skill
    public readonly int EnergyOnHit = 0; //energy gained on hit with skill

    public readonly float SpecialAmount = 0;

    public readonly int MaxUpgrades = 0; //maximum amount of times this upgrade can be picked
    private SkillUpgrade(Skill skill, string desc, int incAmount, float CDMulti, float CostMulti, int bleedAmount, int healthOnHit,int energyOnHit,int maxUps)
    {
        Skill = skill;
        Description = desc;
        IncreasedAmount = incAmount;
        CooldownMultiplier = CDMulti;
        CostMultiplier = CostMulti;
        BleedAmount = bleedAmount;
        HealthOnHit = healthOnHit;
        EnergyOnHit = energyOnHit;
        MaxUpgrades = maxUps;
    }

    private SkillUpgrade(Skill skill, string desc, int incAmount, float CDMulti, float CostMulti, int bleedAmount, int healthOnHit, int energyOnHit, int maxUps, float ProjSpeedMulti)
    {
        Skill = skill;
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

    private SkillUpgrade(Skill skill, string desc, int incAmount, float CDMulti, float CostMulti, int bleedAmount, int healthOnHit, int energyOnHit, int maxUps, float ProjSpeedMulti, bool pierce)
    {
        Skill = skill;
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

    private SkillUpgrade(Skill skill, string desc, float SPAmount, int maxUps)
    {
        Skill = skill;
        Description = desc;
        SpecialAmount = SPAmount;
        MaxUpgrades = maxUps;
    }
    #region Normal Attack
    public static SkillUpgrade NormalAttackHeal = new SkillUpgrade(Skill.PlayerNormalAttack,"Normal Attacks heal you for +2 Health", 0, 1, 1, 0, 2,0,4); 
    public static SkillUpgrade NormalAttackDamage = new SkillUpgrade(Skill.PlayerNormalAttack, "Normal Attacks deal +10 Damage", 10, 1, 1, 0, 0,0,5);
    public static SkillUpgrade NormalAttackBleed = new SkillUpgrade(Skill.PlayerNormalAttack, "Normal Attacks cause targets hit to Bleed for +1 damage for 4 seconds", 0, 1, 1, 1, 0,0,5);
    public static SkillUpgrade NormalAttackEnergyGain = new SkillUpgrade(Skill.PlayerNormalAttack, "Normal Attacks generate +2 Energy on hit", 0, 0, 0, 0, 0,0, 5);
    #endregion

    #region Normal Ranged Attack
    public static SkillUpgrade NormalRangedAttackAdditionalProjectiles = new SkillUpgrade(Skill.PlayerNormalRangedAttack, "Normal Ranged Attacks throws an additional blade", 0, 1, 1, 0, 0,0,2);
    public static SkillUpgrade NormalRangedAttackEnergy = new SkillUpgrade(Skill.PlayerNormalRangedAttack, "Normal Ranged Attacks generate +1 Energy on use", 0, 1, 1,  0, 0,0, 3); 
    public static SkillUpgrade NormalRangedAttackPierce = new SkillUpgrade(Skill.PlayerNormalRangedAttack, "Normal Ranged Attacks now pierce",0,1,1,0,0,0,1,1,true); 
    public static SkillUpgrade NormalRangedAttackDamageSlow = new SkillUpgrade(Skill.PlayerNormalRangedAttack, "Normal Ranged Attacks deal +5 Damage, but has 40% less Projectile Speed", 5, 1, 1, 0, 0,0, 5, 0.6f);
    #endregion
    
    #region Razor Blades
    public static SkillUpgrade RazorBladesCooldown = new SkillUpgrade(Skill.PlayerRazorBlades, "Razor Blades have -50% cooldown", 0, 0.5f, 1,  0, 0, 0, 2,1,true);
    public static SkillUpgrade RazorBladeBleed = new SkillUpgrade(Skill.PlayerRazorBlades, "Razor Blades each cause targets hit to Bleed for +1 damage for 4 seconds", 0, 1, 1, 1, 0, 0, 1, 1, true);
    public static SkillUpgrade RazorBladeAdditionalProjectiles = new SkillUpgrade(Skill.PlayerRazorBlades, "Razor Blades throws an additional projectile", 0, 1, 1, 0, 0, 0, 2, 1, true); 
    public static SkillUpgrade RazorBladeHeal = new SkillUpgrade(Skill.PlayerRazorBlades, "Razor Blades heal you for +2 Health, up to once per blade", 0, 1, 1, 0, 2, 0, 3, 1, true);
    public static SkillUpgrade RazorBladeDamage = new SkillUpgrade(Skill.PlayerRazorBlades, "Razor Blades deal +5 damage", 5, 1, 1, 0, 0, 0, 4,1,true);
    #endregion

    #region Puncture
    public static SkillUpgrade PunctureBleed = new SkillUpgrade(Skill.PlayerPuncture, "Puncture applies an additional Bleed for +4 damage for 4 seconds", 0, 1, 1, 4, 0, 0, 4);
    public static SkillUpgrade PunctureExtraDamage = new SkillUpgrade(Skill.PlayerPuncture, "Puncuture deals an additional +30 damage to bleeding targets",30, 4); //implement use of value on skill hit
    public static SkillUpgrade PunctureCooldown = new SkillUpgrade(Skill.PlayerPuncture, "Puncture has -50% cooldown but costs +20% energy", 0, 0.5f, 1.2f, 0, 0, 0, 2);
    public static SkillUpgrade PunctureHeal = new SkillUpgrade(Skill.PlayerPuncture, "Puncture heals you for +8 Health if the target is bleeding", 8, 5); //implement use of value on skill hit
    public static SkillUpgrade PunctureExpose = new SkillUpgrade(Skill.PlayerPuncture, "Puncture Exposes targets for 4 hits, causing them to take 20% more damage from those hits", 4, 1);
    #endregion

    #region Slash
    public static SkillUpgrade SlashEnergyOnhit = new SkillUpgrade(Skill.PlayerSlash, "Slash gains +1 Energy per enemy hit",0,1,1,0,0,1,4);
    public static SkillUpgrade SlashCooldownOnHit = new SkillUpgrade(Skill.PlayerSlash, "Slash cooldown is reduced by 1 Second per enemy hit", 1, 3); //implement use of value on skill hit
    public static SkillUpgrade SlashDistance = new SkillUpgrade(Skill.PlayerSlash, "Slash travels 50% further", 0.5f, 1);//implement use of value on skill use
    public static SkillUpgrade SlashGainArmor = new SkillUpgrade(Skill.PlayerSlash, "Slash Grants +10 armor on use", 10, 3); //implement use of value on skill use
    public static SkillUpgrade SlashDamage = new SkillUpgrade(Skill.PlayerSlash, "Slash deal +25 Damage", 25, 1, 1, 0, 0, 0, 5); 

    #endregion

    #region Cull
    public static SkillUpgrade CullHealthOnKill = new SkillUpgrade(Skill.PlayerCull, "Cull grants +25 health on kill", 25, 3);
    public static SkillUpgrade CullArmorOnKill = new SkillUpgrade(Skill.PlayerCull, "Cull grants +20 armor on kill", 20, 4);
    public static SkillUpgrade CullSize = new SkillUpgrade(Skill.PlayerCull, "Cull gains +50% size", 0.50f, 2);
    public static SkillUpgrade CullDamage = new SkillUpgrade(Skill.PlayerCull, "Cull deals +40 Damage", 40, 1, 1, 0, 0, 0, 3);
    public static SkillUpgrade CullSlowDamage = new SkillUpgrade(Skill.PlayerCull, "Cull deals +30% more damage to slowed enemies", 0.3f, 4);
    #endregion


    #region Rain of Blades
    public static SkillUpgrade RoBBleed = new SkillUpgrade(Skill.PlayerRainOfBlades, "Rain of Blades has a +25% to cause target to bleed for 1 damage", 25, 4);
    public static SkillUpgrade RoBInterval = new SkillUpgrade(Skill.PlayerRainOfBlades, "Rain of Blades portal spawns 4 additional blades per second", 4, 3);
    public static SkillUpgrade RoBDuration = new SkillUpgrade(Skill.PlayerRainOfBlades, "Rain of Blades portal lasts 2 seconds longer", 2, 3);
    public static SkillUpgrade RoBFourth = new SkillUpgrade(Skill.PlayerRainOfBlades, "Rain of Blade's every 4th blade spawned is bigger, deals +200%, and pierces", 2, 2);
    public static SkillUpgrade RoBSlow = new SkillUpgrade(Skill.PlayerRainOfBlades, "Rain of Blades Slow movement speed of enemies hit by 40% for +1 second", 1, 3);
    public static SkillUpgrade RoBDamage = new SkillUpgrade(Skill.PlayerRainOfBlades, "Rain of Blades deals +3 damage per blade", 3, 1, 1, 0, 0, 0, 3);
    public static SkillUpgrade RoBMove = new SkillUpgrade(Skill.PlayerRainOfBlades, "Rain of Blade moves towards the closest enemy, more upgrades increases the movement speed",3, 3);

    #endregion


    #region BladeStorm
    public static SkillUpgrade BladeStormBlades = new SkillUpgrade(Skill.PlayerBladeStorm, "Bladestorm creates 1 additional blade", 1, 4);
    public static SkillUpgrade BladeStormCost = new SkillUpgrade(Skill.PlayerBladeStorm, "BladeStorm has -20% cost", 0, 1, 0.8f, 0, 0, 0, 3);
    public static SkillUpgrade BladeStormAutoCast = new SkillUpgrade(Skill.PlayerBladeStorm, "You have a +20% chance to create a blade from Bladestorm when you lose health", 20, 5);
    public static SkillUpgrade BladeStormArmor = new SkillUpgrade(Skill.PlayerBladeStorm, "Gain +1 Armor when a blade is created", 1, 4);
    public static SkillUpgrade BladeStormSize = new SkillUpgrade(Skill.PlayerBladeStorm, "Bladestorm gains +50 Size", 0.5f, 2);
    #endregion


}

public sealed class Icon
{
    public readonly string IconName;
    public readonly string Description;
    private Icon( string iconName, string desc)
    {
        IconName = iconName;
        Description = desc;
    }

    public static Icon Default = new Icon("Default","");
    public static Icon NormalAttack = new Icon("NormalAttack","Perform a melee attack in front of you");
    public static Icon RangedNormalAttack = new Icon("RangedNormalAttack", "Throw a blade forward");
    public static Icon RazorBlades = new Icon("RazorBlades", "Throw multiple blades forward");
    public static Icon Puncture = new Icon("Puncture", "Perform a melee attack in front of you that causes bleeding");
    public static Icon Slash = new Icon("Slash", "Dash forward dealing damage to enemies you pass through");
    public static Icon Cull = new Icon("Cull", "Cull enemies in front of you, dealing more damage the less health they have");
    public static Icon RainOfBlades = new Icon("RainOfBlades", "Summon a portal that lasts 3 seconds which rains blades on target location of your mouse. 6 blades are spawned per second.");
    public static Icon BladeStorm = new Icon("BladeStorm", "Create 3 spinning blades around you for 8 seconds which strikes enemies it passes through");
}