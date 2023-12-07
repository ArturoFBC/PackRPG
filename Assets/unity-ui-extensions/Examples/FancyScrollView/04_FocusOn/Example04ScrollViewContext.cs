using System;

namespace UnityEngine.UI.Extensions.Examples
{
    public class Example04ScrollViewContext
    {
        int selectedIndex = -1;

        public int SelectedIndex
        {
            get { return selectedIndex; }
            set
            {
                if (value == selectedIndex)
                {
                    return;
                }

                selectedIndex = value;

                OnSelectedIndexChanged?.Invoke(selectedIndex);
            }
        }

        public Action<Example04ScrollViewCell> OnPressedCell;
        public Action<int> OnSelectedIndexChanged;
    }
}
