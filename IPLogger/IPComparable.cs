using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace IPLogger
{
    public class IPComparable : IComparable<IPComparable>, IEquatable<IPAddress>
    {
        private IPAddress _address;

        public static IPComparable MaxValue => new IPComparable(IPAddress.None);
        public static IPComparable MinValue => new IPComparable(IPAddress.Any);
        public IPAddress Address 
        { 
            get { return _address; }
            set { _address = value; } 
        }

        public IPComparable(IPAddress address)
        {
            _address = address;
        }

        public int CompareTo(IPComparable? other)
        {
            if (other == null) return 1;

            var xBytes = _address.GetAddressBytes();
            var yBytes = other._address.GetAddressBytes();

            int octets = Math.Min(xBytes.Length, yBytes.Length);

            for (int i = 0; i < octets; i++) {
                if (xBytes[i] > yBytes[i])
                    return 1;
                else if (xBytes[i] < yBytes[i])
                    return -1;
            }

            return 0;
        }

        public bool Equals(IPAddress? other)
        {
            return _address == other;
        }
    }
}
