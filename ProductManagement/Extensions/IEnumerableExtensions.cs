﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ProductManagement.Extensions
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<SelectListItem> ToSelectListItem<T>(this IEnumerable<T> items, int selectedId)
        {
            return from item in items
                select new SelectListItem()
                {
                    Text = item.GetPropertyValue("Name"),
                    Value = item.GetPropertyValue("Id"),
                    Selected = item.GetPropertyValue("Id").Equals(selectedId.ToString())
                };
        }

        public static IEnumerable<SelectListItem> ToSelectListItem<T>(this IEnumerable<T> items, string selectedId)
        {
            selectedId = selectedId ?? "";
            return from item in items
                select new SelectListItem()
                {
                    Text = item.GetPropertyValue("Name"),
                    Value = item.GetPropertyValue("Id"),
                    Selected = item.GetPropertyValue("Id").Equals(selectedId)
                };
        }
    }
}