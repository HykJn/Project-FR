using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CoefficientType
{
    Physical, Magic, True
}

public abstract class Skill
{
    public string skillName;
    public bool isMagic;
    public int tier;
    public float damage;
    public CoefficientType coType;

    public abstract void UseSkill(float status);
}

public class SkillManager
{
}