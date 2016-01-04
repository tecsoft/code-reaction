using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CodeReaction.Domain.HouseKeeping
{
    /// <summary>
    /// A timer based Task Scheduler for use in calling a function every N seconds
    /// </summary>
    public class TaskScheduler : IDisposable
    {
        /* Default timer period */
        int _delaySeconds = -1;

        System.Threading.Timer _timer;
        TimerCallback _clientCallback;

        /* synchronising objects to prevent timer Reentrancy */
        static readonly object _lock = new object();
        static bool _callbackInProgress = false;

        /// <summary>
        /// Start a timer so that the callback will be run in startDelaySeconds seconds
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="startDelaySeconds"></param>
        public void Start( TimerCallback callback, int startDelaySeconds ) 
        {
            if ( callback == null )
                throw new ArgumentNullException( "callback" );

            _clientCallback = callback;
            
            _delaySeconds = startDelaySeconds;
            if ( _delaySeconds >= 0  )
            {
                Start();
            }
        }

        //--------------------------------------------------------------------------------------------------------------

        void Start()
        {
            if ( _timer != null )
            {
                _timer.Dispose();
                _timer = null;
            }

            // we program the timer to run only once, the callback wrapper will re start it again for us
            if ( _timer == null && _delaySeconds >= 0)
            {
                _timer = new Timer( CallbackReentryWrapper, null, (1000 * _delaySeconds), Timeout.Infinite );
            }
        }

        //--------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// wraps the callback in a lock to avoid reentrancy risk
        /// </summary>
        /// <param name="obj"></param>
        void CallbackReentryWrapper( object obj )
        {
            lock ( _lock )
            {
                try
                {
                    if ( _callbackInProgress == false )
                    {
                        _callbackInProgress = true;
                        _clientCallback( obj );
                    }
                }
                finally
                {
                    _callbackInProgress = false;
                    
                    // restart the timer
                    Start();
                }
            }
        }

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if ( _timer != null )
                    {
                        _timer.Dispose();
                    }
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
