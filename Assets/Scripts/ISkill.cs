public interface ISkill
{
    public SkillStats Stats { get; set; }

    public CharacterBase Source { get; set; }
    SkillSpeed Speed { get; set; }

    public void Use(CharacterBase targets);
}