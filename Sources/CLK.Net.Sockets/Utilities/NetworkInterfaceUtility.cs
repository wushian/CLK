using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace CLK.Net.Sockets
{
    public static class NetworkInterfaceUtility
    {
        // IPAddress
        public static IPAddress GetIPAddress(AddressFamily addressFamily = AddressFamily.InterNetwork)
        {
            foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {   
                    IPInterfaceProperties ipInterfaceProperties = networkInterface.GetIPProperties();
                    if (ipInterfaceProperties != null)
                    {
                        if (ipInterfaceProperties.UnicastAddresses.Count > 0)
                        {
                            foreach(UnicastIPAddressInformation unicastIPAddressInformation in ipInterfaceProperties.UnicastAddresses)
                            {
                                if (unicastIPAddressInformation.Address.AddressFamily == addressFamily)
                                {
                                    return unicastIPAddressInformation.Address;
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }


        // MacAddress
        public static PhysicalAddress GetMacAddress(IPAddress ipAddress)
        {
            #region Contracts

            if (ipAddress == null) throw new ArgumentNullException();

            #endregion
            foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                IPInterfaceProperties ipProperties = networkInterface.GetIPProperties();
                if (ipProperties != null)
                {
                    foreach (UnicastIPAddressInformation unicastAddress in ipProperties.UnicastAddresses)
                    {
                        if (unicastAddress.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            if (IPAddress.Equals(unicastAddress.Address, ipAddress) == true)
                            {
                                return networkInterface.GetPhysicalAddress();
                            }
                        }
                    }
                }     
            }
            return null;
        }
        

        // OperationalStatus
        public static OperationalStatus GetOperationalStatus(string macAddress)
        {
            #region Contracts

            if (string.IsNullOrEmpty(macAddress) == true) throw new ArgumentNullException();

            #endregion
            return GetOperationalStatus(PhysicalAddress.Parse(macAddress));
        }

        public static OperationalStatus GetOperationalStatus(PhysicalAddress macAddress)
        {
            #region Contracts

            if (macAddress == null) throw new ArgumentNullException();

            #endregion
            foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    if (PhysicalAddress.Equals(macAddress, networkInterface.GetPhysicalAddress()) == true)
                    {
                        return networkInterface.OperationalStatus;
                    }
                }
            }
            return OperationalStatus.Unknown;
        }
    }
}
