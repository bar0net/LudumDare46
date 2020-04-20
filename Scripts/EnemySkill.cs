using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemySkillList
{
    None = 0,
    Tantrum = 1, TantrumLeft = 2, TantrumRight = 3, TantrumAll = 4,
    Singing = 5,
    Cheers = 6,
    Vomit = 7
}


public abstract class EnemySkill
{
    public static EnemySkill GetSkill(EnemySkillList skill)
    {
        switch(skill)
        {
            case EnemySkillList.None:
                break;

            case EnemySkillList.Tantrum:
                return new Tantrum();

            case EnemySkillList.TantrumLeft:
                return new TantrumLeft();

            case EnemySkillList.TantrumRight:
                return new TantrumRight();

            case EnemySkillList.TantrumAll:
                return new TantrumAll();

            case EnemySkillList.Singing:
                return new Singing();

            case EnemySkillList.Cheers:
                return new Cheers();
        }

        return null;
    }

    public EnemySkillList type = EnemySkillList.None;

    public virtual bool CanUse(Enemy self)
    {
        return true;
    }

    public virtual void Use(Manager player, Enemy self)
    {
        return;
    }
}

// Tantrums increase boredom
public class Tantrum : EnemySkill
{
    public Tantrum() { type = EnemySkillList.Tantrum; }

    public override void Use(Manager player, Enemy self)
    {
        self.Bore(); 
    }
}

public class TantrumLeft : EnemySkill
{
    public TantrumLeft() { type = EnemySkillList.TantrumLeft; }

    public override bool CanUse(Enemy self)
    {
        return self.CheckLeft() != null;
    }

    public override void Use(Manager player, Enemy self)
    {
        if (self.CheckLeft() != null) self.CheckLeft().Bore();
    }
}

public class TantrumRight : EnemySkill
{
    public TantrumRight() { type = EnemySkillList.TantrumRight; }

    public override bool CanUse(Enemy self)
    {
        return self.CheckRight() != null;
    }

    public override void Use(Manager player, Enemy self)
    {
        if (self.CheckRight() != null) self.CheckRight().Bore();
    }
}

public class TantrumAll : EnemySkill
{
    public TantrumAll() { type = EnemySkillList.TantrumAll; }

    public override void Use(Manager player, Enemy self)
    {
        self.Bore();
        Enemy e = self.CheckRight(true);
        if (e != null) e.Bore();

        e = self.CheckLeft(true);
        if (e != null) e.Bore();
    }
}

// Singing makes everyone more susceptible to boredom for the next 2 turns
public class Singing : EnemySkill
{
    public Singing() { type = EnemySkillList.Singing; }

    public override void Use(Manager player, Enemy self)
    {
        self.extra_bore_timer = 2;
        if (self.CheckRight() != null) self.CheckRight().extra_bore_timer = 2;
        if (self.CheckLeft() != null) self.CheckLeft().extra_bore_timer = 2;
    }

}

// Cheers makes everyone inmune to hype for a round
public class Cheers : EnemySkill
{
    public Cheers() { type = EnemySkillList.Cheers; }

    public override void Use(Manager player, Enemy self)
    {
        self.hype_resist_timer += 1;
        if (self.CheckRight() != null) self.CheckRight().hype_resist_timer += 1;
        if (self.CheckLeft() != null) self.CheckLeft().hype_resist_timer += 1;
    }
}

// Someone gets their hype back to 0
public class Vomit : EnemySkill
{
    public Vomit() { type = EnemySkillList.Vomit; }

    public override void Use(Manager player, Enemy self)
    {
        int i = Random.Range(0, 3);
        for (int j = 0; j < 3; j++)
        {
            if (player.enemies_in_arena[(i + j) % 3] != null)
            {
                player.enemies_in_arena[(i + j) % 3].hype = 0;
            }
        }
    }
}