using System;
using System.Globalization;
using System.Threading;

namespace Gosu.Commons.Internationalization
{
    public class TemporaryCulture : IDisposable
    {
        private CultureInfo _previousCulture;

        public TemporaryCulture(string identifier)
        {
            _previousCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = new CultureInfo(identifier);
        }

        public void Dispose()
        {
            Thread.CurrentThread.CurrentCulture = _previousCulture;
        }
    }
}