﻿namespace ZeroMQ.Monitoring
{
    using System;
    using lib;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Defines extension methods related to monitoring for <see cref="ZmqSocket"/> instances.
    /// </summary>
    public static class ZMonitorSocketExtensions
    {
        /// <summary>
        /// Spawns a <see cref="ZmqSocketType.PAIR"/> socket that publishes all state changes (events) for
        /// the specified socket over the inproc transport at the given endpoint.
        /// </summary>
        /// <remarks>
        /// It is recommended to connect via a <see cref="ZmqSocketType.PAIR"/> socket in another thread
        /// to handle incoming monitoring events. The <see cref="ZmqMonitor"/> class provides an event-driven
        /// abstraction over event processing.
        /// </remarks>
        /// <param name="socket">The <see cref="ZmqSocket"/> instance to monitor for state changes.</param>
        /// <param name="endpoint">The inproc endpoint on which state changes will be published.</param>
        /// <exception cref="ArgumentNullException"><paramref name="socket"/> or <see cref="endpoint"/> is null.</exception>
        /// <exception cref="ArgumentException"><see cref="endpoint"/> is an empty string.</exception>
        /// <exception cref="ZmqSocketException">An error occurred initiating socket monitoring.</exception>
        public static bool Monitor(this ZSocket socket, string endpoint, out ZError error)
        {
            return Monitor(socket, endpoint, ZMonitorEvents.AllEvents, out error);
        }

        /// <summary>
        /// Spawns a <see cref="ZmqSocketType.PAIR"/> socket that publishes the specified state changes (events) for
        /// the specified socket over the inproc transport at the given endpoint.
        /// </summary>
        public static bool Monitor(this ZSocket socket, string endpoint, ZMonitorEvents eventsToMonitor, out ZError error)
        {
            if (socket == null)
            {
                throw new ArgumentNullException("socket");
            }

            if (endpoint == null)
            {
                throw new ArgumentNullException("endpoint");
            }

            if (endpoint == string.Empty)
            {
                throw new ArgumentException("Unable to publish socket events to an empty endpoint.", "endpoint");
            }
            
			// int endpointPtrSize;
            IntPtr endpointPtr = Marshal.StringToHGlobalAnsi(endpoint);
            // using (var endpointPtr = DispoIntPtr.AllocString(endpoint, out endpointPtrSize)) {
            if (-1 == zmq.socket_monitor(socket._socketPtr, endpointPtr, (Int32)eventsToMonitor))
            {
                error = ZError.GetLastErr();
                return false;
            }
            // }

            error = ZError.None;
            return true;
        }
    }
}