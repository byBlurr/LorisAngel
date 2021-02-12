﻿namespace LorisAngel.Common.Objects
{
    public class ProfileData
    {
        public string Background { get; set; }
        public char Hair { get; set; }
        public int HairColour { get; set; }
        public char Head { get; set; }
        public int HeadColour { get; set; }
        public char Torso { get; set; }
        public int TorsoColour { get; set; }
        public char Bottoms { get; set; }
        public int BottomsColour { get; set; }
        public char Feet { get; set; }
        public int FeetColour { get; set; }

        public ProfileData()
        {
            Background = "default_purple";
            DissembleAvatarStrings("Z0-Z0-Z0-Z0-Z0"); // Z = No Item, 0 = No Colour
        }

        public string BackgroundImage()
        {
            string r = Background;

            if (!HasAvatar()) r += "_noav";
            r += ".png";

            return r;
        }

        private bool HasAvatar()
        {
            if (Hair == 'Z' && Head == 'Z' && Torso == 'Z' && Bottoms == 'Z' && Feet == 'Z') return false;
            else return true;
        }

        public string AssembleAvatarString() => Hair.ToString() + HairColour + "-" + Head.ToString() + HeadColour + "-" + Torso.ToString() + TorsoColour + "-" + Bottoms.ToString() + BottomsColour + "-" + Feet.ToString() + FeetColour;

        private void DissembleAvatarStrings(string avatar)
        {
            string[] parts = avatar.Split('-');

            Hair = parts[0][0];
            Head = parts[1][0];
            Torso = parts[2][0];
            Bottoms = parts[3][0];
            Feet = parts[4][0];

            HairColour = int.Parse(parts[0][1].ToString());
            HeadColour = int.Parse(parts[1][1].ToString());
            TorsoColour = int.Parse(parts[2][1].ToString());
            BottomsColour = int.Parse(parts[3][1].ToString());
            FeetColour = int.Parse(parts[4][1].ToString());
        }

        public override string ToString()
        {
            return Background + "-" + AssembleAvatarString();
        }
    }
}