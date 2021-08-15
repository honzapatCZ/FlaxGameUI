using System;
using System.Collections.Generic;
using FlaxEngine;
using FlaxEngine.GUI;

namespace FlaxGameUI
{
    public class CustomTextBox : TextBox
    {
        [Serialize]
        string text;
        [NoSerialize]
        public new string Text
        {
            get => text;
            set
            {
                text = value;
                Recalculate();
            }
        }
        [Serialize]
        int maxVisibleChars = -1;
        [NoSerialize]
        public int MaxVisibleChars
        {
            get => maxVisibleChars;
            set
            {
                maxVisibleChars = value;
                Recalculate();
            }
        }
        void Recalculate()
        {
            if(MaxVisibleChars < 0)
            {
                base.Text = Text;
            }
            else
            {
                base.Text = Text.Substring(0,Math.Max(Math.Min(MaxVisibleChars, Text.Length), 0));
            }            
        }
    }
}
