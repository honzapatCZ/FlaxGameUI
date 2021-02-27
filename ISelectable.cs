using System;
using System.Collections.Generic;
using FlaxEngine;

namespace FlaxGameUI
{
    public interface ISelectable
    {
        void OnSelect();
        void OnDeSelect();
        void OnSubmit();
        void OnNavigate(NavDir navDir);
    }
}
