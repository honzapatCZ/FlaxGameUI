using System;
using System.Collections.Generic;
using FlaxEngine;

namespace FlaxGameUI
{
    public interface ISelectable
    {
        /// <summary>
        /// Invoked when your item is selected
        /// </summary>
        void OnSelect();
        /// <summary>
        /// Invoked when your item is deselected
        /// </summary>
        void OnDeSelect();
        /// <summary>
        /// Invoked when you should handle something like a press
        /// </summary>
        void OnSubmit();
        /// <summary>
        /// Gives you the ability to handle input if you are for example slider or whatever
        /// </summary>
        /// <returns>Retruns whether you handled it or not, if not auto-navigation will take place, but you can also navigate using <see cref="UIInputSystem.NavigateTo(ISelectable)"/></returns>
        bool OnNavigate(NavDir navDir, UIInputSystem system);
    }
}
