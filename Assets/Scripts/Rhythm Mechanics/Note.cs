[System.Serializable]
public class Note
{
    public const int SIXTEENTH = 48;
    public const int EIGHTH = 48 * 2;
    public const int QUARTER = 48 * 4;
    public const int DOTTED_QUARTER = 48 * 6;
    public const int HALF = 48 * 8;
    public const int DOTTED_HALF = 48 * 12;
    public const int WHOLE = 48 * 16;

    public int moment;
    public int pitch;
    public int length;

    public string Description => $"Note at position {moment} with pitch {pitch} and length {length}";

    public Note(int moment, int pitch, int length)
    {
        this.moment = moment;
        this.pitch = pitch;
        this.length = length;
    }
}
