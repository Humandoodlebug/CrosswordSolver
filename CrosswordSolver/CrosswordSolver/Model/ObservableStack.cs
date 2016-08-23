using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SC.CrosswordSolver.UI.Annotations;

namespace SC.CrosswordSolver.UI.Model
{
    public class ObservableStack<T> : INotifyPropertyChanged
    {
        private readonly List<T> _stackList = new List<T>();
        public int Count => _stackList.Count;


        public void Push(T item)
        {
            _stackList.Add(item);
            OnPropertyChanged(nameof(Count));
        }

        public T Pop()
        {
            var item = _stackList.Last();
            _stackList.RemoveAt(_stackList.Count - 1);
            OnPropertyChanged(nameof(Count));
            return item;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
