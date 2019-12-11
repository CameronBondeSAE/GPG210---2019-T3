using System;

namespace Students.Luca.Scripts.GameSave
{
    [Serializable]
    public struct HighScoreEntry : IComparable<HighScoreEntry>
    {
        public string name;
        public int points;

        public HighScoreEntry(string pName, int pPoints)
        {
            name = pName;
            points = pPoints;
        }
        
        public int CompareTo(HighScoreEntry other)
        {
            return (points > other.points ? 1 : (points < other.points ? -1 : 0));
        }
    }
}