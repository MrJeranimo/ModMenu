using System;
using System.Collections.Generic;
using System.Text;

namespace ModMenu
{
    public class BoolRef
    {
        private readonly Action<bool> _setter;
        private readonly Func<bool> _getter;

        public bool Value
        {
            get => _getter();
            set => _setter(value);
        }

        public BoolRef(Func<bool> getter, Action<bool> setter)
        {
            _getter = getter;
            _setter = setter;
        }
    }
}
