using System;

namespace CHD.Client.Marker.Factory
{
    public delegate void MarkerStatusChangedDelegate(
        bool taken,
        Exception exception
        );
}