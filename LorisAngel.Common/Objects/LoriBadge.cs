using System;

namespace LorisAngel.Common.Objects
{
    public class LoriBadge
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }

        public LoriBadge(int id, string displayName, string code, string description)
        {
            Id = id;
            DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
            Code = code ?? throw new ArgumentNullException(nameof(code));
            Description = description ?? throw new ArgumentNullException(nameof(description));
        }
    }
}
