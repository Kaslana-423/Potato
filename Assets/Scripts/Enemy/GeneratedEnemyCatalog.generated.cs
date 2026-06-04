using System.Collections.Generic;

public static class GeneratedEnemyCatalog
{
    public static IEnumerable<EnemyDefinition> CreateAll()
    {
        yield return Regular("enemy.tree", "Tree", "Neutral, drops fruit or crate on death.", 10f, 5f, 0f, 0f, 0f, 0f, 1f, 3, 1f, 0.2f, 1);
        yield return Regular("enemy.baby_alien", "Baby Alien", "Chases the character and deals damage on touch.", 3f, 2f, 200f, 300f, 1f, 0.6f, 0f, 1, 0.01f, 0.01f, 1);
        yield return Regular("enemy.chaser", "Chaser", "Chases the character, deals damage on touch, spawns in groups.", 1f, 1f, 380f, 380f, 1f, 0.6f, 0f, 1, 0.02f, 0.03f, 2);
        yield return Regular("enemy.spitter", "Spitter", "Runs away if too close and fires projectiles.", 8f, 1f, 200f, 200f, 1f, 0.6f, 0.95f, 1, 0.03f, 0.1f, 4);
        yield return Regular("enemy.charger", "Charger", "Chases the character and can charge.", 4f, 2.5f, 400f, 400f, 1f, 0.85f, 0.8f, 1, 0.01f, 0.01f, 3);
        yield return Regular("enemy.pursuer", "Pursuer", "Chases the character and gets faster over time.", 10f, 24f, 150f, 600f, 1f, 1.2f, 0f, 3, 0.03f, 0.03f, 11);
        yield return Regular("enemy.bruiser", "Bruiser", "Chases the character and can charge.", 20f, 11f, 300f, 300f, 2f, 0.85f, 0.9f, 3, 0.03f, 0.03f, 8);
        yield return Regular("enemy.buffer", "Buffer", "Runs away and buffs nearby enemies.", 20f, 3f, 150f, 150f, 1f, 0.6f, 0.95f, 2, 0.01f, 0.01f, 16);
        yield return Regular("enemy.fly", "Fly", "Moves around the player and fires projectiles when hit.", 15f, 4f, 325f, 375f, 1f, 0.85f, 0f, 1, 0.01f, 0.01f, 4);
        yield return Regular("enemy.healer", "Healer", "Moves around enemies and heals nearby enemies.", 10f, 8f, 400f, 400f, 1f, 0.85f, 0f, 2, 0.03f, 0.03f, 7);
        yield return Regular("enemy.looter", "Looter", "Runs away and drops a loot crate and materials on death.", 5f, 30f, 300f, 400f, 1f, 0.85f, 0f, 8, 1f, 1f, 3);
        yield return Regular("enemy.helmet_alien", "Helmet Alien", "Chases the character and deals damage on touch.", 8f, 4f, 225f, 275f, 1f, 1f, 0.5f, 1, 0.01f, 0.01f, 10);
        yield return Regular("enemy.fin_alien", "Fin Alien", "Chases the character and deals damage on touch.", 12f, 2f, 400f, 400f, 1f, 1f, 0f, 1, 0.02f, 0.03f, 9);
        yield return Regular("enemy.spawner", "Spawner", "Spawns Junkie Aliens on death.", 10f, 1f, 120f, 120f, 1f, 0.85f, 0.7f, 1, 0.01f, 0.01f, 14);
        yield return Regular("enemy.junkie", "Junkie", "Moves around the player and fires projectiles near the player.", 5f, 5f, 350f, 350f, 1f, 1f, 0.5f, 1, 0.01f, 0.01f, 14);
        yield return Regular("enemy.horned_bruiser", "Horned Bruiser", "Chases the character and can charge.", 30f, 22f, 300f, 300f, 1f, 1.15f, 0.9f, 3, 0.03f, 0.03f, 8);
        yield return Regular("enemy.horned_charger", "Horned Charger", "Chases the character and can charge.", 12f, 5f, 425f, 425f, 1f, 1.1f, 0.8f, 1, 0.01f, 0.01f, 18);
        yield return Regular("enemy.slasher_egg", "Slasher Egg", "Stays still, then spawns a Slasher unless killed.", 5f, 3f, 0f, 0f, 1f, 0.6f, 1f, 1, 0f, 0f, 7);
        yield return Regular("enemy.slasher", "Slasher", "Follows the character and attacks from medium range.", 50f, 25f, 250f, 300f, 1f, 1.15f, 0.9f, 3, 0.01f, 0.01f, 4);
        yield return Regular("enemy.tentacle", "Tentacle", "Follows the character and attacks from medium range.", 100f, 20f, 175f, 175f, 1f, 1f, 0.7f, 3, 0.02f, 0.02f, 13);
        yield return Regular("enemy.lamprey", "Lamprey", "Follows the player and charges.", 30f, 15f, 350f, 350f, 1f, 0.75f, 0.95f, 1, 0.01f, 0.01f, 1);
        yield return Regular("enemy.gobbler", "Gobbler", "Targets materials, eats them, and grows.", 5f, 30f, 300f, 400f, 1f, 0.85f, 0f, 8, 1f, 1f, 1);

        yield return Elite("enemy.rhino", "Rhino", "Elite that charges the player.", 1f, 750f, 250f, 250f, 1f, 1.5f, 10, 11);
        yield return Elite("enemy.butcher", "Butcher", "Elite that creates repeated slashes.", 1f, 750f, 200f, 200f, 1f, 1.5f, 10, 11);
        yield return Elite("enemy.monk", "Monk", "Elite that spawns eggs and fires projectiles.", 1f, 700f, 350f, 350f, 1f, 1.5f, 10, 11);
        yield return Elite("enemy.croc", "Croc", "Elite that charges and creates slashes or projectiles.", 1f, 750f, 350f, 350f, 1f, 1.5f, 10, 11);
        yield return Elite("enemy.colossus", "Colossus", "Elite that chases and creates many projectiles.", 1f, 750f, 300f, 300f, 1f, 1.5f, 10, 11);
        yield return Elite("enemy.mantis", "Mantis", "Elite that chases, slashes, and charges.", 1f, 750f, 250f, 250f, 1f, 1.5f, 10, 11);
        yield return Elite("enemy.mother", "Mother", "Elite that chases, slashes, and spawns enemies.", 1f, 750f, 250f, 250f, 1f, 1.5f, 10, 11);
        yield return Elite("enemy.gargoyle", "Gargoyle", "Elite that alternates random movement and projectile patterns.", 1f, 750f, 350f, 350f, 1f, 1.5f, 10, 11);

        yield return Boss("enemy.predator", "Predator", "Boss that chases, dashes, and uses projectile patterns.", 29250f, 0f, 300f, 30f);
        yield return Boss("enemy.invoker", "Invoker", "Boss that creates projectile areas around the player.", 29250f, 0f, 200f, 30f);

        yield return DlcRegular("enemy.dlc.anemone", "Anemone", "Wanders slowly and creates a circle of projectiles.", 8f, 4f, 100f, 100f, 1f, 0.85f, 0.6f, 2, 0.02f, 0.02f, 1);
        yield return DlcRegular("enemy.dlc.anglerfish", "Anglerfish", "Wanders until the player is close, then charges.", 10f, 10f, 200f, 200f, 1f, 1.1f, 0.2f, 3, 0.03f, 0.03f, 1);
        yield return DlcRegular("enemy.dlc.blobfish", "Blobfish", "Spawns enemies on death.", 10f, 8f, 200f, 200f, 1f, 0.6f, 0.5f, 5, 0.05f, 0.05f, 1);
        yield return DlcRegular("enemy.dlc.clam", "Clam", "Runs away if close and fires a slow projectile.", 2f, 3f, 130f, 130f, 1f, 0.6f, 0.75f, 2, 0.03f, 0.03f, 1);
        yield return DlcRegular("enemy.dlc.colossal_squid", "Colossal Squid", "Closes in and fires crossing spikes.", 30f, 20f, 200f, 200f, 1f, 1f, 0.75f, 3, 0.03f, 0.03f, 1);
        yield return DlcRegular("enemy.dlc.crab", "Crab", "Closes in, fires a spike, then moves backwards.", 4f, 3f, 250f, 250f, 1f, 0.6f, 0.5f, 1, 0.02f, 0.02f, 1);
        yield return DlcRegular("enemy.dlc.diplocaulus", "Diplocaulus", "Wanders and leaves slasher eggs.", 30f, 35f, 200f, 200f, 1f, 0.85f, 0.8f, 5, 0.05f, 0.05f, 1);
        yield return DlcRegular("enemy.dlc.dragonfish", "Dragonfish", "Shoots lines of bullets toward players.", 100f, 50f, 300f, 300f, 1f, 1.15f, 0.9f, 8, 0.05f, 0.05f, 1);
        yield return DlcRegular("enemy.dlc.goblin_shark", "Goblin Shark", "Charges toward where the player is moving.", 12f, 10f, 275f, 275f, 1f, 1.1f, 0.7f, 3, 0.01f, 0.01f, 1);
        yield return DlcRegular("enemy.dlc.hermit", "Hermit", "Wanders and heals enemies.", 5f, 5f, 300f, 300f, 1f, 0.85f, 0.5f, 2, 0.03f, 0.03f, 1, 1f, 0.25f);
        yield return DlcRegular("enemy.dlc.iron_lung", "Iron Lung", "Used by Stargazers to mutate into Dragonfish.", 5f, 4f, 0f, 0f, 1f, 0.25f, 1f, 1, 0f, 0f, 1, 3f, 0.5f);
        yield return DlcRegular("enemy.dlc.lobster", "Lobster", "Chases the player and takes reduced damage.", 1f, 5f, 250f, 250f, 1f, 0.85f, 0.25f, 1, 0.01f, 0.01f, 1, 1f, 0.5f);
        yield return DlcRegular("enemy.dlc.looting_pig", "Looting Pig", "Runs around and drops a crate on death.", 5f, 30f, 350f, 350f, 1f, 0.85f, 0f, 8, 1f, 1f, 1);
        yield return DlcRegular("enemy.dlc.narwhal", "Narwhal", "Closes in and can charge while firing a spike.", 10f, 8f, 250f, 250f, 1f, 1.15f, 0.9f, 2, 0.03f, 0.03f, 1);
        yield return DlcRegular("enemy.dlc.plankton", "Plankton", "Chases the player and sometimes charges.", 1f, 1f, 225f, 225f, 1f, 0.4f, 0f, 1, 0.01f, 0.01f, 1);
        yield return DlcRegular("enemy.dlc.pufferfish", "Pufferfish", "Explodes into projectiles when killed by non-melee damage.", 5f, 2f, 175f, 175f, 1f, 0.85f, 0.7f, 2, 0.03f, 0.01f, 1);
        yield return DlcRegular("enemy.dlc.sea_pig", "Sea Pig", "Wanders and drops materials.", 30f, 15f, 150f, 150f, 1f, 0.85f, 0.6f, 5, 0.01f, 0.01f, 1);
        yield return DlcRegular("enemy.dlc.shrimp", "Shrimp", "Chases the character and deals damage on touch.", 2f, 2f, 300f, 300f, 1f, 0.6f, 0f, 1, 0.01f, 0.01f, 1);
        yield return DlcRegular("enemy.dlc.stargazer", "Stargazer", "Moves toward its Iron Lung and becomes buffed if it dies.", 30f, 15f, 100f, 100f, 1f, 0.85f, 0.5f, 3, 0.02f, 0.02f, 1);
        yield return DlcRegular("enemy.dlc.stonefish", "Stonefish", "Buffs enemies and spawns projectiles around the player.", 20f, 5f, 150f, 150f, 1f, 0.6f, 0.7f, 2, 0.03f, 0.03f, 1);
        yield return DlcRegular("enemy.dlc.vampire_squid", "Vampire Squid", "Chases the player while firing projectiles.", 10f, 5f, 275f, 275f, 1f, 1f, 0f, 1, 0.01f, 0.01f, 1);
        yield return DlcRegular("enemy.dlc.viperfish", "Viperfish", "Starts growing after spawning, then chases.", 1f, 3f, 80f, 80f, 1f, 0.65f, 0f, 1, 0.02f, 0.02f, 1);
        yield return DlcRegular("enemy.dlc.walrus", "Walrus", "Charges toward the player every few seconds.", 40f, 25f, 200f, 200f, 1f, 1.2f, 0.9f, 3, 0.03f, 0.03f, 1);

        yield return DlcElite("enemy.dlc.bat", "Bat", "DLC elite that closes in and creates projectile circles.", 1f, 750f, 300f, 300f, 1f, 1.5f, 10, 11);
        yield return DlcElite("enemy.dlc.giant", "Giant", "DLC elite that chases with orbiting projectile lines.", 1f, 750f, 250f, 250f, 1f, 1.5f, 10, 11);
        yield return DlcElite("enemy.dlc.giant_isopod", "Giant Isopod", "DLC elite that chases and fires spiral projectiles.", 1f, 750f, 200f, 200f, 1f, 1.5f, 10, 11);
        yield return DlcElite("enemy.dlc.impaled_worm", "Impaled Worm", "DLC elite that moves randomly and charges.", 1f, 750f, 250f, 250f, 1f, 1.5f, 10, 11);
        yield return DlcElite("enemy.dlc.jellyfish", "Jellyfish", "DLC elite with orbiting and inward projectile patterns.", 1f, 750f, 225f, 225f, 1f, 1.5f, 10, 11);
        yield return DlcElite("enemy.dlc.megalodon", "Megalodon", "DLC elite that chases and charges with projectile bursts.", 1f, 750f, 350f, 350f, 1f, 1.5f, 10, 11);
        yield return DlcElite("enemy.dlc.prisoner", "Prisoner", "DLC elite that chases and spawns projectile circles.", 1f, 750f, 200f, 200f, 1f, 1.5f, 10, 11);
        yield return DlcElite("enemy.dlc.spider_crab", "Spider Crab", "DLC elite that moves randomly and spawns crabs.", 1f, 750f, 200f, 200f, 1f, 1.5f, 10, 11);
        yield return DlcElite("enemy.dlc.turtle", "Turtle", "DLC elite that chases and fires inward spikes.", 1f, 700f, 200f, 200f, 1f, 1.5f, 10, 11, 3f, 0.5f);

        yield return DlcBoss("enemy.dlc.dead_whale", "Dead Whale", "DLC boss with random movement, charges, and projectile waves.", 31625f, 200f, 30f);
        yield return DlcBoss("enemy.dlc.eel", "Eel", "DLC boss that chases and fires projectile streams.", 31625f, 150f, 30f);
    }

    private static EnemyDefinition Regular(
        string id,
        string name,
        string behavior,
        float health,
        float hpPerWave,
        float minSpeed,
        float maxSpeed,
        float damage,
        float damagePerWave,
        float knockbackResistance,
        int materials,
        float consumableDropChance,
        float lootCrateDropChance,
        int firstWave)
    {
        return new EnemyDefinition(id, name, EnemyCategory.Regular, behavior, health, hpPerWave, minSpeed, maxSpeed, damage, damagePerWave, knockbackResistance, materials, consumableDropChance, lootCrateDropChance, firstWave);
    }

    private static EnemyDefinition Elite(
        string id,
        string name,
        string behavior,
        float health,
        float hpPerWave,
        float minSpeed,
        float maxSpeed,
        float damage,
        float damagePerWave,
        int materials,
        int firstWave)
    {
        return new EnemyDefinition(id, name, EnemyCategory.Elite, behavior, health, hpPerWave, minSpeed, maxSpeed, damage, damagePerWave, 0f, materials, 0f, 0f, firstWave);
    }

    private static EnemyDefinition Boss(
        string id,
        string name,
        string behavior,
        float health,
        float hpPerWave,
        float speed,
        float damage)
    {
        return new EnemyDefinition(id, name, EnemyCategory.Boss, behavior, health, hpPerWave, speed, speed, damage, 0f, 1f, 0, 0f, 0f, 20);
    }

    private static EnemyDefinition DlcRegular(
        string id,
        string name,
        string behavior,
        float health,
        float hpPerWave,
        float minSpeed,
        float maxSpeed,
        float damage,
        float damagePerWave,
        float knockbackResistance,
        int materials,
        float consumableDropChance,
        float lootCrateDropChance,
        int firstWave,
        float armor = 0f,
        float armorPerWave = 0f)
    {
        return new EnemyDefinition(id, name, EnemyCategory.DlcRegular, behavior, health, hpPerWave, minSpeed, maxSpeed, damage, damagePerWave, knockbackResistance, materials, consumableDropChance, lootCrateDropChance, firstWave, armor, armorPerWave);
    }

    private static EnemyDefinition DlcElite(
        string id,
        string name,
        string behavior,
        float health,
        float hpPerWave,
        float minSpeed,
        float maxSpeed,
        float damage,
        float damagePerWave,
        int materials,
        int firstWave,
        float armor = 0f,
        float armorPerWave = 0f)
    {
        return new EnemyDefinition(id, name, EnemyCategory.DlcElite, behavior, health, hpPerWave, minSpeed, maxSpeed, damage, damagePerWave, 0f, materials, 0f, 0f, firstWave, armor, armorPerWave);
    }

    private static EnemyDefinition DlcBoss(
        string id,
        string name,
        string behavior,
        float health,
        float speed,
        float damage)
    {
        return new EnemyDefinition(id, name, EnemyCategory.DlcBoss, behavior, health, 0f, speed, speed, damage, 0f, 1f, 0, 0f, 0f, 20);
    }
}
