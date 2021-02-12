using LorisAngel.Common.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace LorisAngel.Common.Rendering
{
    public class ProfileRenderer
    {

        private LoriUser User { get; set; }
        private ProfileData Data { get; set; }

        public ProfileRenderer(LoriUser user)
        {
            User = user;
            GetProfileData();
        }

        public void Render()
        {

        }

        public void GetProfileData()
        {

        }
    }
}
