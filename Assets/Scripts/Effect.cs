
[System.Serializable]
public class Effect
{
    int type;
    string name;
    int value;
    float time;
    int[] antitype;
    float range;
    string InGameEffectName;

    public Effect()
    {
        type = -1;
        name = "";
        value = 0;
        time = 0;
        antitype = new int[4];
        InGameEffectName = "";
    }

    public Effect(int Type, string name, int value, float time, int[] antitype, float range, string InGameEffectName)
    {
        type = Type;
        this.name = name;
        this.value = value;
        this.time = time;
        this.antitype = new int[4];
        antitype.CopyTo(this.antitype, 0);
        this.range = range;
        this.InGameEffectName = InGameEffectName;
    }

    public Effect(Effect effect)
    {
        type = effect.getEffectType();
        name = effect.getName();
        value = effect.getValue();
        time = effect.getTime();
        antitype = new int[4];
        effect.getAntiType().CopyTo(antitype, 0);
        range = effect.getRange();
        InGameEffectName = effect.getInGameEffectName();
    }

    public string getName()
    {
        return name;
    }

    public int getEffectType()
    {
        return type;
    }

    public int getValue()
    {
        return value;
    }

    public float getTime()
    {
        return time;
    }

    public int[] getAntiType()
    {
        return antitype;
    }

    public float getRange()
    {
        return range;
    }

    public string getInGameEffectName()
    {
        return InGameEffectName;
    }
}
