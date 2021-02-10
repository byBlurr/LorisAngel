using System;

namespace LorisAngel.Common.Objects
{
    public class LoriShip
    {
        public ulong User1 { get; set; }
        public ulong User2 { get; set; }
        public string Name1 { get; set; }
        public string Name2 { get; set; }
        public string Shipname { get; set; }
        public int Percentage { get; set; }

        public LoriShip(ulong user1, ulong user2, string name1, string name2, string shipname, int percentage)
        {
            this.User1 = user1;
            this.User2 = user2;
            this.Name1 = name1 ?? throw new ArgumentNullException(nameof(name1));
            this.Name2 = name2 ?? throw new ArgumentNullException(nameof(name2));
            this.Shipname = shipname ?? throw new ArgumentNullException(nameof(shipname));
            this.Percentage = percentage;
        }

        /// <summary>
        /// Returns a readable representation of the LoriShip object
        /// </summary>
        /// <returns>A readable representation as a String</returns>
        public override string ToString() => $"RelationshipObject_{Name1}_{Name2}_{Percentage}%";

        /// <summary>
        /// More detailed ToString() method
        /// </summary>
        /// <returns>A String describing the relationship.</returns>
        public string ToDescriptiveString()
        {
            float ActualPercentage = ((float)Percentage) / 100f;

            if (ActualPercentage <= 10f) return $"A very weak relationship. ({ActualPercentage}%)";
            else if (ActualPercentage <= 25f) return $"This is not going to last long... ({ActualPercentage}%)";
            else if (ActualPercentage <= 50f) return $"I suppose this might last a few months! ({ActualPercentage}%)";
            else if (ActualPercentage <= 75f) return $"This can probably go for a couple years before heart break! ({ActualPercentage}%)";
            else if (ActualPercentage <= 95f) return $"Wow... are you two made for eachother? ({ActualPercentage}%)";
            else return $"A match made in heaven! ({ActualPercentage}%)";
        }

        /// <summary>
        /// Returns the actual percentage as it is stored as an integer.
        /// </summary>
        /// <returns>A float representing the percentage.</returns>
        public float ToActualPercentage() => ((float)Percentage) / 100f;

    }
}
