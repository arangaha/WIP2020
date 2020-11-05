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
    Dash = 25,
    Jump = 15,
    Sprint = 15, //per second
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
    public static Skill PlayerNormalAttack = new Skill("Normal Attack",Icon.NormalAttack,20, 0,0, false,0,0,0,5);
    public static Skill PlayerNormalRangedAttack = new Skill("Normal Ranged Attack",Icon.RangedNormalAttack,10, 0, 0, false,0,0,0,0, 30, false);
    public static Skill PlayerRazorBlades = new Skill("Razor Blades",Icon.RazorBlades,12, 0, 4, false,10,0,0,0, 30, true); //fires 3 projectiles
  //  public static Skill PlayerRazorBlades = new Skill("Razor Blades", Icon.RazorBlades, 12, 0, 0, false, 0, 0, 0, 0, 30, true); //fires 3 projectiles
    public static Skill PlayerPuncture = new Skill("Puncture",Icon.Puncture,20, 0, 6, false, 15, 5, 0, 0);
    public static Skill PlayerSlash = new Skill("Slash",Icon.Slash,15, 0, 12, false, 0, 0, 0, 5);
    public static Skill PlayerCull = new Skill("Cull",Icon.Cull,100, 0, 12, false, 25, 0, 0, 0); //deals 1% more damage per 1% health missing on target   
    public static Skill PlayerRainOfBlades = new Skill("Rain Of Blades", Icon.RainOfBlades, 4, 0, 12, false, 20, 0, 0, 0, 12, false); //spawns a portal that spawns projetiles
    public static Skill PlayerBladeStorm = new Skill("BladeStorm", Icon.BladeStorm, 4, 0, 0, false, 15, 0, 0, 0);
    public static Skill PlayerChakram = new Skill("Chakram", Icon.Chakram, 40, 0, 12, false, 20, 0, 0, 0, 20, true);
    public static Skill PlayerPerforate = new Skill("Perforate", Icon.Perforate, 80, 0, 12, false, 15, 0, 0, 0);
    public static Skill PlayerFortify = new Skill("Fortify", Icon.Fortify, 5, 8, 20, false, 0, 0, 0, 0);
    public static Skill PlayerSpikeTrap = new Skill("Spike Trap", Icon.SpikeTrap, 40, 0, 15, false, 4, 0, 0, 0);
    //public static Skill PlayerSpikeTrap = new Skill("Spike Trap", Icon.SpikeTrap, 25, 0, 0, false, 4, 0, 0, 0);
    //enemy skills
    public static Skill WarriorSwing = new Skill("Warrior Swing",Icon.Default,10, 10,10, false,0, 0, 0,0);
    public static Skill WarriorJab = new Skill("Warrior Jab", Icon.Default, 13, 7, 6, false,0, 0, 0,0);
    public static Skill WarriorNA = new Skill("Warrior Normal Attack", Icon.Default, 10, 3, 0, false,0, 0, 0,0);

    public static Skill EliteWarriorSwing = new Skill("Elite Warrior Swing", Icon.Default, 15, 15, 0, false, 0, 0, 0, 0);
    public static Skill EliteWarriorBeam = new Skill("Elite Warrior Beam", Icon.Default, 15, 0, 0, false, 0, 0, 0, 0); //do not add to skill pool
    public static Skill EliteWarriorJab = new Skill("Elite Warrior Jab", Icon.Default, 10, 15, 6, false, 0, 3, 0, 0);
    public static Skill EliteWarriorWave = new Skill("Elite Warrior Wave", Icon.Default, 25, 0, 0, false, 0, 0, 0, 0); // do not add to skill pool
    public static Skill EliteWarriorSpikes = new Skill("Elite Warrior Spikes", Icon.Default, 5 , 12, 6, false, 0, 0, 0, 0);
    public static Skill EliteWarriorLeap = new Skill("Elite Warrior Leap", Icon.Default, 25, 12, 6, false, 0, 0, 0, 0);
    public static Skill EliteWarriorStormCall = new Skill("Elite Warrior Storm Call", Icon.Default, 25, 18, 6, false, 0, 0, 0, 0);
    public static Skill EliteWarriorMegaBeam = new Skill("Elite Warrior Mega Beam", Icon.Default, 5, 18, 8, false, 0, 0, 0, 0);

    public static Skill HellHoundLeap = new Skill("Hellhound Leap", Icon.Default, 7, 12, 12, false, 0, 0, 0, 0);
    public static Skill HellHoundNA = new Skill("Hellhound Normal Attack", Icon.Default, 7, 3, 0, false, 0, 0, 0, 0);
    public static Skill HellHoundFireball = new Skill("Hellhound Fireball", Icon.Default, 10, 12, 8, false, 0, 0, 0, 0, 15, false);

    public static Skill HellHoundVariantNA = new Skill("Hellhound Elite Normal Attack", Icon.Default, 10, 4, 0, false, 0, 1, 0, 0);
    public static Skill HellHoundVariantFireball = new Skill("Hellhound Elite Fireball", Icon.Default, 20, 12, 12, false, 0, 0, 0, 0,10,true);
  //  public static Skill HellHoundVariantSpikeShot = new Skill("Hellhound Elite Spike Shot", Icon.Default, 5, 8,25, false, 0, 1, 0, 0); //spikes come of body to hit, fades away and reappears back in body after
  //  public static Skill HellHoundVariantMortar = new Skill("Hellhound Elite Mortar", Icon.Default, 30, 15, 30, false, 0, 0, 0, 0); //lobs mortars that plants themselves on the floor, explodes after 5 seconds
    public static Skill HellHoundVariantSlam = new Skill("Hellhound Elite Slam", Icon.Default, 20, 10, 15, false, 0, 0, 0, 0);
  //  public static Skill HellHoundVariantCharge = new Skill("Hellhound Elite Charge", Icon.Default, 15, 12, 12, false, 0, 0, 0, 0); // charge in the direction of enemy, hit enemies are slowed
    public static Skill HellHoundVariantLeap = new Skill("Hellhound Elite Leap", Icon.Default, 10, 12, 8, false, 0, 0, 0, 0);


    public static Skill HellbatNA = new Skill("Hellbat Normal Attack", Icon.Default, 4, 8, 0, false, 0, 0, 0, 0);
    public static Skill HellbatGouge = new Skill("Hellbat Gouge", Icon.Default, 3, 8, 10, false, 0, 1, 0, 0);
    public static Skill HellbatSonicWave = new Skill("Hellbat Sonic Wave", Icon.Default, 4, 12, 8, false, 0, 0, 0, 0, 8, false);

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
            SkillUpgrade.NormalAttackHeal,
            SkillUpgrade.NormalAttackArmor,
        });

    public static PlayerSkill PlayerNormalRangedAttack = new PlayerSkill(Skill.PlayerNormalRangedAttack, Icon.Default,
        new List<SkillUpgrade>
        {
            SkillUpgrade.NormalRangedAttackDamageSlow,
            SkillUpgrade.NormalRangedAttackAdditionalProjectiles,
            SkillUpgrade.NormalRangedAttackEnergy,
                        SkillUpgrade.NormalRangedAttackSlow,

            SkillUpgrade.NormalRangedAttackPierce,
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
            SkillUpgrade.SlashDamage,
            SkillUpgrade.SlashExpose,
        });

    public static PlayerSkill PlayerCull = new PlayerSkill(Skill.PlayerCull, Icon.Cull,
        new List<SkillUpgrade>
    {
            SkillUpgrade.CullArmorOnKill,
            //SkillUpgrade.CullHealthOnKill,
            SkillUpgrade.CullSize,
            SkillUpgrade.CullDamage,
            SkillUpgrade.CullSlowDamage,
            SkillUpgrade.CullBleedConsume,
            SkillUpgrade.CullCooldownCost,
            
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

    public static PlayerSkill PlayerChakram = new PlayerSkill(Skill.PlayerChakram, Icon.Chakram,
        new List<SkillUpgrade>
        {
            SkillUpgrade.ChakramReturnDamage,
            SkillUpgrade.ChakramArmor,
            SkillUpgrade.ChakramSpeedCost,
            SkillUpgrade.ChakramCooldownLessDamage,
            SkillUpgrade.ChakramStationarySpin,
            SkillUpgrade.ChakramDamage
        });

    public static PlayerSkill PlayerPerforate = new PlayerSkill(Skill.PlayerPerforate, Icon.Perforate,
        new List<SkillUpgrade>
        {
            SkillUpgrade.PerforateArmor,
            SkillUpgrade.PerforateBlades,
            SkillUpgrade.PerforateCooldown,
            SkillUpgrade.PerforateDamage,
            SkillUpgrade.PerforateHeight,
            SkillUpgrade.PerforateSlow,

        });

    public static PlayerSkill PlayerFortify = new PlayerSkill(Skill.PlayerFortify, Icon.Fortify,
        new List<SkillUpgrade>
        {
            SkillUpgrade.FortifyCooldownOnNA,
            SkillUpgrade.FortifyExtraArmor,
            SkillUpgrade.FortifyProtection,
            SkillUpgrade.FortifyEnergyGain,
            SkillUpgrade.FortifyRemoveBleed,

        });
    public static PlayerSkill PlayerSpikeTrap = new PlayerSkill(Skill.PlayerSpikeTrap, Icon.SpikeTrap,
        new List<SkillUpgrade>
        {
            SkillUpgrade.SpikeTrapSpikes,
            SkillUpgrade.SpikeTrapCooldown,
            SkillUpgrade.SpikeTrapMultipleTraps,
            SkillUpgrade.SpikeTrapIncBleedDmg,
            SkillUpgrade.SpikeTrapSlowDamage,
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
    public static SkillUpgrade NormalAttackHeal = new SkillUpgrade(Skill.PlayerNormalAttack,"Normal Attacks heal you for +1 Health", 0, 1, 1, 0, 1,0,4); 
    public static SkillUpgrade NormalAttackDamage = new SkillUpgrade(Skill.PlayerNormalAttack, "Normal Attacks deal +10 Damage", 10, 1, 1, 0, 0,0,5);
    public static SkillUpgrade NormalAttackBleed = new SkillUpgrade(Skill.PlayerNormalAttack, "Normal Attacks cause targets hit to Bleed for +1 damage for 4 seconds", 0, 1, 1, 1, 0,0,5);
    public static SkillUpgrade NormalAttackEnergyGain = new SkillUpgrade(Skill.PlayerNormalAttack, "Normal Attacks generate +2 Energy on hit", 0, 0, 0, 0, 0,0, 5);
    public static SkillUpgrade NormalAttackArmor = new SkillUpgrade(Skill.PlayerNormalAttack, "Normal Attacks grant +4 armor if you have no armor", 4, 4);
    #endregion

    #region Normal Ranged Attack
    public static SkillUpgrade NormalRangedAttackAdditionalProjectiles = new SkillUpgrade(Skill.PlayerNormalRangedAttack, "Normal Ranged Attacks throws an additional blade", 0, 1, 1, 0, 0,0,2);
    public static SkillUpgrade NormalRangedAttackEnergy = new SkillUpgrade(Skill.PlayerNormalRangedAttack, "Normal Ranged Attacks generate +1 Energy on use", 0, 1, 1,  0, 0,0, 3); 
    public static SkillUpgrade NormalRangedAttackDamageSlow = new SkillUpgrade(Skill.PlayerNormalRangedAttack, "Normal Ranged Attacks deal +5 Damage, but has 40% less Projectile Speed", 5, 1, 1, 0, 0,0, 5, 0.6f);
    public static SkillUpgrade NormalRangedAttackSlow = new SkillUpgrade(Skill.PlayerNormalRangedAttack, "Normal Ranged Attacks Slow for +1 second", 1, 4);
    public static SkillUpgrade NormalRangedAttackPierce = new SkillUpgrade(Skill.PlayerNormalRangedAttack, "Normal Ranged Attacks now pierce", 0, 1, 1, 0, 0, 0, 1, 1, true);

    #endregion

    #region Razor Blades
    public static SkillUpgrade RazorBladesCooldown = new SkillUpgrade(Skill.PlayerRazorBlades, "Razor Blades have -50% cooldown", 0, 0.5f, 1,  0, 0, 0, 2,1,true);
    public static SkillUpgrade RazorBladeBleed = new SkillUpgrade(Skill.PlayerRazorBlades, "Razor Blades each cause targets hit to Bleed for +1 damage for 4 seconds", 0, 1, 1, 1, 0, 0, 2, 1, true);
    public static SkillUpgrade RazorBladeAdditionalProjectiles = new SkillUpgrade(Skill.PlayerRazorBlades, "Razor Blades throws an additional projectile", 0, 1, 1, 0, 0, 0, 4, 1, true); 
    public static SkillUpgrade RazorBladeHeal = new SkillUpgrade(Skill.PlayerRazorBlades, "Razor Blades heal you for +2 Health, up to once per blade", 0, 1, 1, 0, 2, 0, 3, 1, true);
    public static SkillUpgrade RazorBladeDamage = new SkillUpgrade(Skill.PlayerRazorBlades, "Razor Blades deal +12 damage", 12, 1, 1, 0, 0, 0, 4,1,true);
    #endregion

    #region Puncture
    public static SkillUpgrade PunctureBleed = new SkillUpgrade(Skill.PlayerPuncture, "Puncture applies an additional Bleed for +3 damage for 4 seconds", 0, 1, 1, 3, 0, 0, 4);
    public static SkillUpgrade PunctureExtraDamage = new SkillUpgrade(Skill.PlayerPuncture, "Puncuture deals an additional +30 damage to bleeding targets",30, 4); //implement use of value on skill hit
    public static SkillUpgrade PunctureCooldown = new SkillUpgrade(Skill.PlayerPuncture, "Puncture has -50% cooldown but costs +20% energy", 0, 0.5f, 1.2f, 0, 0, 0, 2);
    public static SkillUpgrade PunctureHeal = new SkillUpgrade(Skill.PlayerPuncture, "Puncture heals you for +5 Health if the target is bleeding", 5, 3); //implement use of value on skill hit
    public static SkillUpgrade PunctureExpose = new SkillUpgrade(Skill.PlayerPuncture, "Puncture Exposes targets for 4 hits, causing them to take 20% more damage from those hits", 4, 1);
    #endregion

    #region Slash
    public static SkillUpgrade SlashEnergyOnhit = new SkillUpgrade(Skill.PlayerSlash, "Slash gains +4 Energy per enemy hit",0,1,1,0,0,4,4);
    public static SkillUpgrade SlashCooldownOnHit = new SkillUpgrade(Skill.PlayerSlash, "Slash cooldown is reduced by 2 Second per enemy hit", 2, 3); //implement use of value on skill hit
    public static SkillUpgrade SlashDistance = new SkillUpgrade(Skill.PlayerSlash, "Slash travels 50% further", 0.5f, 1);//implement use of value on skill use
    public static SkillUpgrade SlashGainArmor = new SkillUpgrade(Skill.PlayerSlash, "Slash Grants +2 armor on use", 2, 3); //implement use of value on skill use
    public static SkillUpgrade SlashDamage = new SkillUpgrade(Skill.PlayerSlash, "Slash deal +50 Damage", 50, 1, 1, 0, 0, 0, 5);
    public static SkillUpgrade SlashExpose = new SkillUpgrade(Skill.PlayerSlash, "Slash Exposes targets for 2 hits, causing them to take 20% more damage from those hits", 2, 1);
    #endregion

    #region Cull
  //  public static SkillUpgrade CullHealthOnKill = new SkillUpgrade(Skill.PlayerCull, "Cull grants +25 health on kill", 25, 2);
    public static SkillUpgrade CullArmorOnKill = new SkillUpgrade(Skill.PlayerCull, "Cull grants +20 armor on kill", 20, 4);
    public static SkillUpgrade CullSize = new SkillUpgrade(Skill.PlayerCull, "Cull gains +50% size", 0.50f, 2);
    public static SkillUpgrade CullDamage = new SkillUpgrade(Skill.PlayerCull, "Cull deals +60 Damage", 60, 1, 1, 0, 0, 0, 3);
    public static SkillUpgrade CullSlowDamage = new SkillUpgrade(Skill.PlayerCull, "Cull deals +30% more damage to slowed enemies", 0.3f, 4);
    public static SkillUpgrade CullBleedConsume = new SkillUpgrade(Skill.PlayerCull, "Cull consumes all bleed on unit, dealing +2 times the total amount", 2, 3);
    public static SkillUpgrade CullCooldownCost = new SkillUpgrade(Skill.PlayerCull, "Cull has -20% Cooldown and -20% Cost", 0, 0.8f, 0.8f, 0, 0, 0, 3);
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
    public static SkillUpgrade BladeStormCost = new SkillUpgrade(Skill.PlayerBladeStorm, "BladeStorm has -20% cost", 0, 1, 0.8f, 0, 0, 0, 2);
    public static SkillUpgrade BladeStormAutoCast = new SkillUpgrade(Skill.PlayerBladeStorm, "You have a +20% chance to create a blade from Bladestorm when you lose health", 20, 5);
    public static SkillUpgrade BladeStormArmor = new SkillUpgrade(Skill.PlayerBladeStorm, "Gain +1 Armor when a blade is created", 1, 4);
    public static SkillUpgrade BladeStormSize = new SkillUpgrade(Skill.PlayerBladeStorm, "Bladestorm gains +50 Size", 0.5f, 2);
    #endregion

    #region chakram
    public static SkillUpgrade ChakramReturnDamage = new SkillUpgrade(Skill.PlayerChakram, "Enemies hit by Initial and Returning hit of Chakram take +30 damage and are Exposed for +1 hits. (Exposed enemies take 20% more damage for X hits)", 30, 4); //returning damage + expose
    public static SkillUpgrade ChakramArmor = new SkillUpgrade(Skill.PlayerChakram, "Gain +1 armor per enemy hit by Initial and Returning hit when the Chakram returns to you", 1, 2);//armor per enemy hit
    public static SkillUpgrade ChakramSpeedCost = new SkillUpgrade(Skill.PlayerChakram, "Gain +20 projectile speed, -20% cost", 0, 1, 0.8f, 0, 0, 0, 3, 1.2f, true);//projectile speed , - cost
    public static SkillUpgrade ChakramCooldownLessDamage = new SkillUpgrade(Skill.PlayerChakram, "-25% cooldown, -5% damage to all damage dealt by this skill", 0, 0.75f, 1, 0, 0, 0, 4, 1, true);//reduced cooldown, reduced damage, implement reduced damage manually
    public static SkillUpgrade ChakramStationarySpin = new SkillUpgrade(Skill.PlayerChakram, "Chakram stays stationary for 0.5 second when it reaches its maximum range, dealing +30 damage to touching enemies every 0.1 second", 30, 4);//stationaryspin
    public static SkillUpgrade ChakramDamage = new SkillUpgrade(Skill.PlayerChakram, "Chakram deals +30 Damage", 30, 1, 1, 0, 0, 0, 3);

    #endregion

    #region Perforate
    public static SkillUpgrade PerforateHeight = new SkillUpgrade(Skill.PlayerPerforate, "Blade has 2x Height, enemies hit by blade have their bleed refreshed", 0, 1);
    public static SkillUpgrade PerforateBlades = new SkillUpgrade(Skill.PlayerPerforate, "for the next +1 seconds after using perforate, every 1 seconds lesser blades emerge under nearby enemies, dealing 30% of original damage", 1, 4);
    public static SkillUpgrade PerforateArmor = new SkillUpgrade(Skill.PlayerPerforate, "Perforate deals 50% more to armored units, gain 1 armor per enemy hit", 1, 1);
    public static SkillUpgrade PerforateCooldown = new SkillUpgrade(Skill.PlayerPerforate, "-50% cooldown when used above 45 energy", 0.5f, 2);
    public static SkillUpgrade PerforateSlow = new SkillUpgrade(Skill.PlayerPerforate, "Perforate Slows for +2 Seconds", 2, 4);
    public static SkillUpgrade PerforateDamage = new SkillUpgrade(Skill.PlayerPerforate, "Perforate Deals +60 Damage", 60, 1, 1, 0, 0, 0, 5);
    #endregion

    #region Fortify
    public static SkillUpgrade FortifyProtection = new SkillUpgrade(Skill.PlayerFortify, "If you gained armor from at least one enemy, you take -20% damage the next hit you take.",0.2f,5);
    public static SkillUpgrade FortifyExtraArmor = new SkillUpgrade(Skill.PlayerFortify, "If you gained armor from exactly one enemy, gain +5 armor",5, 3);
    public static SkillUpgrade FortifyCooldownOnNA = new SkillUpgrade(Skill.PlayerFortify, "Normal Melee Attacks reduce the cooldown of this skill by 1 second if it hits an enemy", 1, 3);
    public static SkillUpgrade FortifyEnergyGain = new SkillUpgrade(Skill.PlayerFortify, "Gain 1 Energy per nearby enemy when used", 1, 3);
    public static SkillUpgrade FortifyRemoveBleed = new SkillUpgrade(Skill.PlayerFortify, "Remove bleeding effects on you when used. If you are below 50% health, you will not be slowed", 0, 1);
    #endregion

    #region SpikeTrap
    public static SkillUpgrade SpikeTrapIncBleedDmg = new SkillUpgrade(Skill.PlayerSpikeTrap, "Enemies hit by trap takes +10% more bleed damage for 8 seconds", 0.1f, 5);//enemies hit take more bleed damage for 6 seconds
    public static SkillUpgrade SpikeTrapCooldown = new SkillUpgrade(Skill.PlayerSpikeTrap, "-50% cooldown, +4 Energy Cost", 0, 0.5f, 2, 0, 0, 0, 2);//+energy cost, - cd
    public static SkillUpgrade SpikeTrapMultipleTraps = new SkillUpgrade(Skill.PlayerSpikeTrap, "Throws 1 additional trap, exposes -2 hits", 1, 2);//throw multiple traps, + energy cost
    public static SkillUpgrade SpikeTrapSlowDamage = new SkillUpgrade(Skill.PlayerSpikeTrap, "Trap deals +30% more damage to slowed enemies", 0.3f, 5); //if enemy hit is already slowed, they take more damage from the trap
    public static SkillUpgrade SpikeTrapSpikes = new SkillUpgrade(Skill.PlayerSpikeTrap, "When triggered, +3 piercing spike projectiles are released in random directions, each dealing 5 damage", 3, 3);
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
    public static Icon NormalAttack = new Icon("NormalAttack","Perform a melee attack in front of you, dealing 20 Damage");
    public static Icon RangedNormalAttack = new Icon("RangedNormalAttack", "Throw a blade forward 10 Damage");
    public static Icon RazorBlades = new Icon("RazorBlades", "Throw 3 blades forward, each dealing 12 Damage");
    public static Icon Puncture = new Icon("Puncture", "Perform a melee attack in front of you, dealing 20 Damage, and causing the target to bleed for 5 Damage for 4 seconds");
    public static Icon Slash = new Icon("Slash", "Dash forward dealing 15 Damage to enemies you pass through, gaining 5 energy per enemy hit");
    public static Icon Cull = new Icon("Cull", "Cull enemies in front of you, dealing 100 Damage. This skill deals 1% more damage per % enemy missing health");
    public static Icon RainOfBlades = new Icon("RainOfBlades", "Summon a portal that lasts 3 seconds which rains blades on target location of your mouse, dealing 4 Damage per blade. 6 blades are spawned per second");
    public static Icon BladeStorm = new Icon("BladeStorm", "Create 3 spinning blades around you for 6 seconds which strikes enemies it passes through for 4 Damage. Each blade can hit once per spin");
    public static Icon Chakram = new Icon("Chakram", "Throw a Chakram that returns to you, dealing 40 Damage initially and on the way back");
    public static Icon Perforate = new Icon("Perforate", "Stab into the ground, causing blades to emerge under nearby enemies, dealing 80 Damage");
    public static Icon Fortify = new Icon("Fortify","Gain 5 armor per nearby enemy. You are slowed for 2 seconds after use");
    public static Icon SpikeTrap = new Icon("SpikeTrap", "Deploy a spike trap towards your cursor. When triggered by an enemy, they take 40 Damage. They are then Exposed for 4 hits, and are Slowed for 6 seconds. (Exposed enemies take 20% more damage for X hits)");
}