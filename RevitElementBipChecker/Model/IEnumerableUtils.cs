using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitElementBipChecker.Model
{
    public static class IEnumerableUtils
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> source)
        {
            ObservableCollection<T> newSource = new ObservableCollection<T>();
            foreach (T t in source)
            {
                newSource.Add(t);
            }

            return newSource;
        }
    }
}
