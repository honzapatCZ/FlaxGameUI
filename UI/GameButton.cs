using System;
using System.Collections.Generic;
using FlaxEngine;
using FlaxEngine.GUI;

namespace FlaxGameUI
{
    public class GameButton : Selectable
    {
        public enum ButtonVerke
        {
            BrushSwap,
            ColorTint
        }
        [EditorDisplay("Game Button"), ExpandGroups]
        public ButtonVerke buttonType;
        bool isColorTint { get => buttonType == ButtonVerke.ColorTint; }
        bool isBrushSwap { get => buttonType == ButtonVerke.BrushSwap; }
        
        [VisibleIf("isColorTint"), EditorDisplay("Game Button"), ExpandGroups]
        public Color NormalColor;
        [VisibleIf("isColorTint"), EditorDisplay("Game Button"), ExpandGroups]
        public Color HoverColor;
        [VisibleIf("isColorTint"), EditorDisplay("Game Button"), ExpandGroups]
        public Color PressedColor;
        [VisibleIf("isColorTint"), EditorDisplay("Game Button"), ExpandGroups]
        public Color DisabledColor;

        [VisibleIf("isBrushSwap"), EditorDisplay("Game Button"), ExpandGroups]
        public IBrush NormalBrush;
        [VisibleIf("isBrushSwap"), EditorDisplay("Game Button"), ExpandGroups]
        public IBrush HoverBrush;
        [VisibleIf("isBrushSwap"), EditorDisplay("Game Button"), ExpandGroups]
        public IBrush PressedBrush;
        [VisibleIf("isBrushSwap"), EditorDisplay("Game Button"), ExpandGroups]
        public IBrush DisabledBrush;

        [Serialize]
        UIControl targetImage;
        Image targetImageControl => targetImage?.Control as Image;
        
        [NoSerialize, EditorDisplay("Game Button"), ExpandGroups]
        public UIControl TargetImage 
        {
            get => targetImage;
            set { if (value != null && !(value.Control is Image)) { Debug.LogError("Thats not an image"); return; } targetImage = value; }
        }

        [EditorDisplay("Game Button"), ExpandGroups]
        public GameEvent OnClick;

        public new bool Enabled
        {
            get => base.Enabled;
            set
            {
                base.Enabled = value;
                if (targetImageControl != null)
                {
                    if (isColorTint)
                    {
                        targetImageControl.Color = !value ? DisabledColor : NormalColor;
                        targetImageControl.MouseOverColor = !value ? DisabledColor : NormalColor;
                    }
                    if (isBrushSwap)
                        targetImageControl.Brush = !value ? DisabledBrush : NormalBrush;
                }
            }
        }

        public override void OnSelect()
        {
            base.OnSelect();

            if(targetImageControl != null)
            {
                if (isColorTint)
                {
                    targetImageControl.Color = HoverColor;
                    targetImageControl.MouseOverColor = HoverColor;
                }                    
                if (isBrushSwap)
                    targetImageControl.Brush = HoverBrush;
            }
        }
        public override void OnDeSelect()
        {
            base.OnDeSelect();

            if (targetImageControl != null)
            {
                if (isColorTint)
                {
                    targetImageControl.Color = NormalColor;
                    targetImageControl.MouseOverColor = NormalColor;
                }                    
                if (isBrushSwap)
                    targetImageControl.Brush = NormalBrush;
            }
        }
        /// <inheritdoc/>
        public override void OnSubmit()
        {
            base.OnSubmit();

            OnClick.Invoke();

            if (targetImageControl != null)
            {
                if (isColorTint)
                {
                    targetImageControl.Color = PressedColor;
                    targetImageControl.MouseOverColor = PressedColor;
                }                    
                if (isBrushSwap)
                    targetImageControl.Brush = PressedBrush;
            }
        }   

        /// <inheritdoc />
        public override void OnMouseLeave()
        {
            OnDeSelect();

            base.OnMouseLeave();
        }
        public override void OnMouseEnter(Vector2 location)
        {
            OnSelect();

            base.OnMouseEnter(location);
        }
        /// <inheritdoc />
        public override bool OnMouseDown(Vector2 location, MouseButton button)
        {
            if (button == MouseButton.Left)
            {
                OnSubmit();
                return true;
            }

            return base.OnMouseDown(location, button);
        }
    }
}
