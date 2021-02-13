using LorisAngel.Common.Objects;

namespace LorisAngel.Common
{
    public class Badges
    {
        public static LoriBadge[] AllBadges = {
            new LoriBadge(0, "founder", "Founder", "This user was here from the start"),
            new LoriBadge(1, "dev", "Developer", "This user helped develop Lori's Angel"),
        };
    
        public static LoriBadge GetBadge(int id)
        {
            if (id >= 0 && id < AllBadges.Length) return AllBadges[id];
            else return null;
        }
    }
}
