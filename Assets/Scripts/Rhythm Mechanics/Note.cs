using UnityEngine;

public class Note : ScriptableObject
{
    const int SIXTEENTH = 48;
    const int EIGHTH = 48 * 2;
    const int QUARTER = 48 * 4;
    const int DOTTED_QUARTER = 48 * 6;
    const int HALF = 48 * 8;
    const int DOTTED_HALF = 48 * 12;
    const int WHOLE = 48 * 16;

    public int position;
    public int pitch;
    public int length;

    public Note(int position, int pitch, int length)
    {
        this.position = position;
        this.pitch = pitch;
        this.length = length;
    }
}
