using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace LocationTracker.Helpers
{
    /// <summary>
    /// Class to extend UIElementCollection type with ToList method
    /// </summary>
    public static class UIElementCollectionToListExtension
    {
        /// <summary>
        /// Converts UIElementCollection to List of UIElements
        /// </summary>
        public static List<UIElement> ToList(this UIElementCollection collection)
        {
            var resultList = new List<UIElement>();
            foreach (UIElement element in collection)
            {
                resultList.Add(element);
            }

            return resultList;
        } 
    }
}
